namespace firstORM.service
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using firstORM.models;
    using System.Text.Json;

    public class ProdutoRota
    {
        private firstORM.services.ProductService productService = new services.ProductService();
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
                var valor = json.RootElement.GetProperty("valor").GetDouble();
                var fornecedor = json.RootElement.GetProperty("fornecedor").GetString();

                var produto = new ProdutoModel { nome = nome, valor = valor, fornecedor = fornecedor };
               using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    dbContext.Produto.Add(produto);
                    await dbContext.SaveChangesAsync();
                }
                await context.Response.WriteAsync("Produto adicionado: " + nome);
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
                    var produtos = await dbContext.Produto.ToListAsync();
                    await context.Response.WriteAsJsonAsync(produtos);
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
                var nome = json.RootElement.GetProperty("nome").GetString();

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    var produtos = await dbContext.Produto.Where(p => EF.Functions.Like(p.nome, "%" + nome + "%")).ToListAsync();
                    await context.Response.WriteAsJsonAsync(produtos);
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
                var valor = json.RootElement.GetProperty("valor").GetDouble();
                var fornecedor = json.RootElement.GetProperty("fornecedor").GetString();

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    var produto = await dbContext.Produto.FindAsync(id);
                    if (produto != null)
                    {
                        produto.nome = nome;
                        produto.valor = valor;
                        produto.fornecedor = fornecedor;
                        await dbContext.SaveChangesAsync();
                        await context.Response.WriteAsync("Produto atualizado: " + nome);
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
                    var produto = await dbContext.Produto.FindAsync(id);
                    if (produto != null)
                    {
                        dbContext.Produto.Remove(produto);
                        await dbContext.SaveChangesAsync();
                        await context.Response.WriteAsync("Produto deletado: " + produto.nome);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync("Produto não encontrado");
                    }
                }
            });
        }
    }
}
