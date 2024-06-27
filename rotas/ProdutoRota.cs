namespace firstORM.rota
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
        

        
        private ProdutoService produtoService;
        public ProdutoRota(LevoratechDbContext db){
            produtoService = new ProdutoService(db);
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

            app.MapPost("/produto/adicionar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var nome = json.RootElement.GetProperty("nome").GetString();
                var valor = json.RootElement.GetProperty("valor").GetDecimal();
                var fornecedor = json.RootElement.GetProperty("fornecedor").GetString();

                var produto = new ProdutoModel { nome = nome, valor = valor, fornecedor = fornecedor };
                
                    await produtoService.AddProdutoAsync(produto);                    
                
                await context.Response.WriteAsync("Produto adicionado: " + nome);
            });
        

            app.MapGet("/produto/listar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                
                    var produtos = await produtoService.GetAllProdutosAsync();
                    await context.Response.WriteAsJsonAsync(produtos);

                
            });
       

            app.MapPost("/produto/procurar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt16();

                
                    var produtos = await produtoService.GetProdutoByIdAsync(id);
                    await context.Response.WriteAsJsonAsync(produtos);
                
            });
       

            app.MapPost("/produto/atualizar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();
                var nome = json.RootElement.GetProperty("nome").GetString();
                var valor = json.RootElement.GetProperty("valor").GetDecimal();
                var fornecedor = json.RootElement.GetProperty("fornecedor").GetString();

                await produtoService.update(context,app,nome,valor,fornecedor,id);
            });
        

            app.MapPost("/produto/deletar", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return;
                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();

                
                    var produtos = await produtoService.GetProdutoByIdAsync(id);
                    await produtoService.DeleteProdutoAsync(id);

                    await context.Response.WriteAsync("executado");
                    
                
            });
        }
    }
}
