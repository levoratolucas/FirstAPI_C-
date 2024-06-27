namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
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

            app.MapPost("/cliente/adicionar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var nome = json.RootElement.GetProperty("nome").GetString();
                var CPF = json.RootElement.GetProperty("CPF").GetString();
                var email = json.RootElement.GetProperty("email").GetString();

                var cliente = new ClienteModel { nome = nome, CPF = CPF, email = email };

                await ClienteService.AddClienteAsync(cliente);

                await context.Response.WriteAsync("cliente adicionado: " + nome);
            });


            app.MapGet("/cliente/listar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;


                var clientes = await ClienteService.GetAllclientesAsync();
                await context.Response.WriteAsJsonAsync(clientes);


            });


            app.MapPost("/cliente/procurar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt16();

                var clientes = await ClienteService.GetclienteByIdAsync(id);
                await context.Response.WriteAsJsonAsync(clientes);

            });


            app.MapPost("/cliente/atualizar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();
                var nome = json.RootElement.GetProperty("nome").GetString();
                var CPF = json.RootElement.GetProperty("CPF").GetString();
                var email = json.RootElement.GetProperty("email").GetString();

                await ClienteService.update(context, app, nome, CPF, email, id);
            });


            app.MapPost("/cliente/deletar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;
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
