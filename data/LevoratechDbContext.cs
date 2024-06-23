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

        public void AddProdutoModel(string nome, double valor, string fornecedor)
        {
            var newProduto = new ProdutoModel
            {
                nome = nome,
                valor = valor,
                fornecedor = fornecedor
            };
            Produto.Add(newProduto);
            SaveChanges();
        }

        public void AddFornecedorModel(string nome, string cnpj, string email, string telefone)
        {
            var newFornecedor = new FornecedorModel
            {
                nome = nome,
                cnpj = cnpj,
                email = email,
                telefone = telefone
            };
            Fornecedor.Add(newFornecedor);
            SaveChanges();
        }

        public void AddClienteModel(string nome, string cpf, string email)
        {
            var newCliente = new ClienteModel
            {
                nome = nome,
                CPF = cpf,
                email = email
            };
            Cliente.Add(newCliente);
            SaveChanges();
        }
    }
}
