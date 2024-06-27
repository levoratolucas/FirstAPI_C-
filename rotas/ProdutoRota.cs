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

    public class ProdutoRota
    {



        private ProdutoService produtoService;
        public ProdutoRota(LevoratechDbContext db)
        {
            produtoService = new ProdutoService(db);
        }

        public void Rotas(WebApplication? app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/produto/adicionar", async (HttpContext context) =>
                {
                    if (!ValToken.ValidateToken(context, out _)) return;

                    using var reader = new System.IO.StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    var json = JsonDocument.Parse(body);
                    var nome = json.RootElement.GetProperty("nome").GetString();
                    var valor = json.RootElement.GetProperty("valor").GetDecimal();
                    var fornecedor = json.RootElement.GetProperty("fornecedor").GetString();

                    if (!string.IsNullOrEmpty(nome) && valor > 0 && !string.IsNullOrEmpty(fornecedor))
                    {
                        var produto = new ProdutoModel { nome = nome, valor = valor, fornecedor = fornecedor };
                        await produtoService.AddProdutoAsync(produto);

                        var produtoJson = JsonSerializer.Serialize(produto);
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(produtoJson);
                    }
                    else
                    {
                        var dadosJson = json.RootElement.ToString();
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(dadosJson);
                    }
                });


            app.MapGet("/produto/listar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;


                var produtos = await produtoService.GetAllProdutosAsync();
                await context.Response.WriteAsJsonAsync(produtos);


            });


            app.MapPost("/produto/procurar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt16();


                var produtos = await produtoService.GetProdutoByIdAsync(id);
                await context.Response.WriteAsJsonAsync(produtos);

            });


           app.MapPost("/produto/atualizar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();

                var nome = json.RootElement.TryGetProperty("nome", out var nomeProperty) ? nomeProperty.GetString() : string.Empty;
                var valor = json.RootElement.TryGetProperty("valor", out var valorProperty) ? valorProperty.GetDecimal() : default;
                var fornecedor = json.RootElement.TryGetProperty("fornecedor", out var fornecedorProperty) ? fornecedorProperty.GetString() : string.Empty;

                await produtoService.update(context, app, nome, valor, fornecedor, id);
            });


            app.MapPost("/produto/deletar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;
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
