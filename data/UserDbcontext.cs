// Arquivo: UserDbContext.cs
using Microsoft.EntityFrameworkCore;
using firstORM.models;

namespace firstORM.data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ProdutoModel> Produto { get; set; }


        public void AddUserModel(String nome, Double valor, String fornecedor)
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

        public void AddUserModel(String nome, String email, String senha)
        {
            var newUser = new UserModel
            {
                nome = nome,
                email = email,
                senha = senha
            };
            Users.Add(newUser);
            SaveChanges();
        }
    }
}
