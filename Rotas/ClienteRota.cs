namespace firstORM.Rotas
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

        public void Add(WebApplication? app, string rota)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost(rota, async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var nome = json.RootElement.GetProperty("nome").GetString();
                var CPF = json.RootElement.GetProperty("CPF").GetString();
                var email = json.RootElement.GetProperty("email").GetString();

                var cliente = new ClienteModel { nome = nome, CPF = CPF, email = email };

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    dbContext.Cliente.Add(cliente);
                    await dbContext.SaveChangesAsync();
                }

                await context.Response.WriteAsync("Cliente adicionado: " + nome);
            });
        }

        public void List(WebApplication? app, string rota)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapGet(rota, async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    var clientes = await dbContext.Cliente.ToListAsync();
                    await context.Response.WriteAsJsonAsync(clientes);
                }
            });
        }

        public void Search(WebApplication? app, string rota)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost(rota, async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var CPF = json.RootElement.GetProperty("CPF").GetString();

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    var cliente = await dbContext.Cliente.Where(p => EF.Functions.Like(p.CPF, "%" + CPF + "%")).ToListAsync();
                    await context.Response.WriteAsJsonAsync(cliente);
                }
            });
        }

        public void Update(WebApplication? app, string rota)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost(rota, async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();
                var nome = json.RootElement.GetProperty("nome").GetString();
                var CPF = json.RootElement.GetProperty("CPF").GetString();
                var email = json.RootElement.GetProperty("email").GetString();

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    var cliente = await dbContext.Cliente.FindAsync(id);
                    if (cliente != null)
                    {
                        cliente.nome = nome;
                        cliente.CPF = CPF;
                        cliente.email = email;
                        await dbContext.SaveChangesAsync();
                        await context.Response.WriteAsync("cliente atualizado: " + nome);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync("Produto não encontrado");
                    }
                }
            });
        }

        public void Delete(WebApplication? app, string rota)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapDelete(rota, async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                var id = int.Parse(context.Request.Query["id"]);

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    var cliente = await dbContext.Cliente.FindAsync(id);
                    if (cliente != null)
                    {
                        dbContext.Cliente.Remove(cliente);
                        await dbContext.SaveChangesAsync();
                        await context.Response.WriteAsync("Cliente deletado: " + cliente.nome);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync("cliente não encontrado");
                    }
                }
            });
        }
    }
}
