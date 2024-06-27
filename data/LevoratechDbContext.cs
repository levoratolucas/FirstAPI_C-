using Microsoft.EntityFrameworkCore;
using firstORM.models;

namespace firstORM.data
{
    public class LevoratechDbContext : DbContext
    {
        public LevoratechDbContext(DbContextOptions<LevoratechDbContext> options) : base(options) { }

        public DbSet<ProdutoModel> Produto { get; set; }
        public DbSet<FornecedorModel> Fornecedor { get; set; }
        public DbSet<ClienteModel> Cliente { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<VendaModel> Venda { get; set; }
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
