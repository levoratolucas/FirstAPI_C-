namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using firstORM.models;
    using System.Text.Json;

    public class FornecedorRota
    {
        private FornecedorService FornecedorService;

        public FornecedorRota(LevoratechDbContext dbContext){
            FornecedorService = new FornecedorService(dbContext);
        }
        private TokenValidationParameters GetValidationParameters()
        {
            var key = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcab");
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }

        private bool ValidateToken(HttpContext context, out SecurityToken validatedToken)
        {
            validatedToken = null;
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.WriteAsync("Adicione um token").Wait();
                return false;
            }

            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, GetValidationParameters(), out validatedToken);
                return true;
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.WriteAsync("Token inválido").Wait();
                return false;
            }
        }

        public void Rotas(WebApplication? app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/fornecedor/adicionar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var nome = json.RootElement.GetProperty("nome").GetString();
                var cnpj = json.RootElement.GetProperty("cnpj").GetString();
                var telefone = json.RootElement.GetProperty("telefone").GetString();
                var email = json.RootElement.GetProperty("email").GetString();

                var fornecedor = new FornecedorModel { nome = nome, cnpj = cnpj, email = email , telefone = telefone};
                
                    await FornecedorService.AddfornecedorAsync(fornecedor);                    
                
                await context.Response.WriteAsync("fornecedor adicionado: " + nome);
            });
        

            app.MapGet("/fornecedor/listar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                
                    var fornecedors = await FornecedorService.GetAllfornecedorsAsync();
                    await context.Response.WriteAsJsonAsync(fornecedors);

                
            });
       

            app.MapPost("/fornecedor/procurar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt16();

                    var fornecedors = await FornecedorService.GetfornecedorByIdAsync(id);
                    await context.Response.WriteAsJsonAsync(fornecedors);
                
            });
       

            app.MapPost("/fornecedor/atualizar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();
                var nome = json.RootElement.GetProperty("nome").GetString();
                var cnpj = json.RootElement.GetProperty("cnpj").GetString();
                var telefone = json.RootElement.GetProperty("telefone").GetString();
                var email = json.RootElement.GetProperty("email").GetString();

                await FornecedorService.update(context,app,nome,cnpj,email,telefone,id);
            });
        

            app.MapPost("/fornecedor/deletar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;
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
