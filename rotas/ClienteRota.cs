namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using firstORM.config;
    using firstORM.data;
    using firstORM.models;
    using System.Text.Json;

    public class ClienteRota
    {
        private ClienteService ClienteService;

        public ClienteRota(LevoratechDbContext dbContext)
        {
            ClienteService = new ClienteService(dbContext);
        }


        public void Rotas(WebApplication? app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/cliente/adicionar", async (HttpContext context) =>
            {

                if (!ValToken.ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var nome = json.RootElement.GetProperty("nome").GetString();
                var CPF = json.RootElement.GetProperty("CPF").GetString();
                var email = json.RootElement.GetProperty("email").GetString();
                if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(CPF) && !string.IsNullOrEmpty(email))
                {
                    var cliente = new ClienteModel { nome = nome, CPF = CPF, email = email };

                    await ClienteService.AddClienteAsync(cliente);

                    var clienteJson = JsonSerializer.Serialize(cliente);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(clienteJson);
                }
                else
                {
                    var dadosJson = json.RootElement.ToString();
                    await context.Response.WriteAsync(dadosJson);
                }
            });






            app.MapGet("/cliente/listar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;


                var clientes = await ClienteService.GetAllclientesAsync();
                await context.Response.WriteAsJsonAsync(clientes);


            });


            app.MapPost("/cliente/procurar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt16();

                var clientes = await ClienteService.GetclienteByIdAsync(id);
                await context.Response.WriteAsJsonAsync(clientes);

            });


            app.MapPost("/cliente/atualizar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();

                var nome = json.RootElement.TryGetProperty("nome", out var nomeProperty) ? nomeProperty.GetString() : string.Empty;
                var CPF = json.RootElement.TryGetProperty("CPF", out var cpfProperty) ? cpfProperty.GetString() : string.Empty;
                var email = json.RootElement.TryGetProperty("email", out var emailProperty) ? emailProperty.GetString() : string.Empty;

                await ClienteService.update(context, app, nome, CPF, email, id);
            });


            app.MapPost("/cliente/deletar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;
                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();


                var clientes = await ClienteService.GetclienteByIdAsync(id);
                await ClienteService.DeleteclienteAsync(id);

                await context.Response.WriteAsync("executado");


            });
        }
    }
}
