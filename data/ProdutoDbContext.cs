using Microsoft.EntityFrameworkCore;
using firstORM.models;

namespace firstORM.data
{
    public class ProdutoDbContext : DbContext
    {
        public ProdutoDbContext(DbContextOptions<ProdutoDbContext> options) : base(options) { }
        
        public DbSet<ProdutoModel> Produto { get; set; }

        
         public void AddUserModel(String nome,Double valor, String fornecedor )
        {
            var newUser = new ProdutoModel
            {
               nome = nome,
               valor = valor,
               fornecedor = fornecedor

            };
            Produto.Add(newUser);
            SaveChanges();
        }

    }
}
