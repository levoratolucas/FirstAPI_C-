namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using firstORM.models;
    using System.Text.Json;

    public class ProdutoService
    {

        LevoratechDbContext _dbContext;
        public ProdutoService(LevoratechDbContext db)
        {
            _dbContext = db;
        }
        // Método para consultar todos os produtos
        public async Task<List<ProdutoModel>> GetAllProdutosAsync()
        {
            return await _dbContext.Produto.ToListAsync();
        }

        // Método para consultar um produto a partir do seu Id
        public async Task<ProdutoModel> GetProdutoByIdAsync(int id)
        {
            return await _dbContext.Produto.FindAsync(id);
        }

        // Método para  gravar um novo produto
        public async Task AddProdutoAsync(ProdutoModel produto)
        {
            _dbContext.Produto.Add(produto);
            await _dbContext.SaveChangesAsync();
        }
        public async Task update(HttpContext context, WebApplication? app, string nome, decimal valor, string fornecedor, int id)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                var produto = await dbContext.Produto.FindAsync(id);

                if (produto != null)
                {
                    produto.nome = !string.IsNullOrEmpty(nome) ? nome : produto.nome;
                    produto.valor = valor != default ? valor : produto.valor;
                    produto.fornecedor = !string.IsNullOrEmpty(fornecedor) ? fornecedor : produto.fornecedor;

                    await dbContext.SaveChangesAsync();

                    var produtoJson = JsonSerializer.Serialize(produto);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(produtoJson);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Produto não encontrado");
                }
            }
        }
        // Método para atualizar os dados de um produto
        public async Task UpdateProdutoAsync(int id, ProdutoModel produto)
        {
            _dbContext.Entry(produto).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // Método para excluir um produto
        public async Task DeleteProdutoAsync(int id)
        {
            var produto = await _dbContext.Produto.FindAsync(id);
            if (produto != null)
            {
                _dbContext.Produto.Remove(produto);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
