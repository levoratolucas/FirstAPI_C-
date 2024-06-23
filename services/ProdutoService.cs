using firstORM.data;
using firstORM.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace firstORM.services
{
    public class ProductService
    {
        // private readonly LevoratechDbContext _context;

        // public ProductService(LevoratechDbContext context)
        // {
        //     _context = context;
        // }

        // public async Task<List<ProdutoModel>> GetAllProductsAsync()
        // {
        //     return await _context.Produto.ToListAsync();
        // }

        // public async Task<ProdutoModel> GetProductByIdAsync(int id)
        // {
        //     return await _context.Produto.FindAsync(id);
        // }

        public async Task AddProduto(firstORM.models.ProdutoModel produto, WebApplication? app)
        {
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    dbContext.Produto.Add(produto);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        public async Task ListProduto(HttpContext context, WebApplication? app)
        {
            {
                 using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    var produtos = await dbContext.Produto.ToListAsync();
                    await context.Response.WriteAsJsonAsync(produtos);
                }
            }
        }
        public async Task SearchProduto(String nome,HttpContext context, WebApplication? app)
        {
            {
                 using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    var produtos = await dbContext.Produto.Where(p => EF.Functions.Like(p.nome, "%" + nome + "%")).ToListAsync();
                    await context.Response.WriteAsJsonAsync(produtos);
                }
            }
        }
        public async Task updateProduto(int id,String nome,Double valor, String fornecedor,HttpContext context, WebApplication? app)
        {
            {
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
            }
        }


            // public async Task UpdateProductAsync(ProdutoModel produto)
            // {
            //     _context.Produto.Update(produto);
            //     await _context.SaveChangesAsync();
            // }

            // public async Task DeleteProductAsync(int id)
            // {
            //     var produto = await _context.Produto.FindAsync(id);
            //     if (produto != null)
            //     {
            //         _context.Produto.Remove(produto);
            //         await _context.SaveChangesAsync();
            //     }
            // }
        
    }
}
