namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using firstORM.config;
    using firstORM.models;
    using System.Text.Json;

    public class FornecedorRota
    {
        private FornecedorService FornecedorService;

        public FornecedorRota(LevoratechDbContext dbContext)
        {
            FornecedorService = new FornecedorService(dbContext);
        }


        public void Rotas(WebApplication? app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/fornecedor/adicionar", async (HttpContext context) =>
                {
                    if (!ValToken.ValidateToken(context, out _)) return;

                    using var reader = new System.IO.StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    var json = JsonDocument.Parse(body);
                    var nome = json.RootElement.GetProperty("nome").GetString();
                    var cnpj = json.RootElement.GetProperty("cnpj").GetString();
                    var telefone = json.RootElement.GetProperty("telefone").GetString();
                    var email = json.RootElement.GetProperty("email").GetString();

                    if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(cnpj) && !string.IsNullOrEmpty(telefone) && !string.IsNullOrEmpty(email))
                    {
                        var fornecedor = new FornecedorModel { nome = nome, cnpj = cnpj, email = email, telefone = telefone };
                        await FornecedorService.AddfornecedorAsync(fornecedor);

                        var fornecedorJson = JsonSerializer.Serialize(fornecedor);
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(fornecedorJson);
                    }
                    else
                    {
                        var dadosJson = json.RootElement.ToString();
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(dadosJson);
                    }
                });


            app.MapGet("/fornecedor/listar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;


                var fornecedors = await FornecedorService.GetAllfornecedorsAsync();
                await context.Response.WriteAsJsonAsync(fornecedors);


            });


            app.MapPost("/fornecedor/procurar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt16();

                var fornecedors = await FornecedorService.GetfornecedorByIdAsync(id);
                await context.Response.WriteAsJsonAsync(fornecedors);

            });


            app.MapPost("/fornecedor/atualizar", async (HttpContext context) =>
                {
                    if (!ValToken.ValidateToken(context, out _)) return;

                    using var reader = new System.IO.StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    var json = JsonDocument.Parse(body);
                    var id = json.RootElement.GetProperty("id").GetInt32();

                    var nome = json.RootElement.TryGetProperty("nome", out var nomeProperty) ? nomeProperty.GetString() : string.Empty;
                    var cnpj = json.RootElement.TryGetProperty("cnpj", out var cnpjProperty) ? cnpjProperty.GetString() : string.Empty;
                    var telefone = json.RootElement.TryGetProperty("telefone", out var telefoneProperty) ? telefoneProperty.GetString() : string.Empty;
                    var email = json.RootElement.TryGetProperty("email", out var emailProperty) ? emailProperty.GetString() : string.Empty;

                    await FornecedorService.Update(context, app, nome, cnpj, email, telefone, id);
                });


            app.MapPost("/fornecedor/deletar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;
                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();


                var fornecedors = await FornecedorService.GetfornecedorByIdAsync(id);
                await FornecedorService.DeletefornecedorAsync(id);

                await context.Response.WriteAsync("executado");


            });
        }
    }
}
