using firstORM.models;
using firstORM.data;
using firstORM.models;
using Microsoft.EntityFrameworkCore;

namespace firstORM.services
{
    public class VendaService
    {
        private readonly LevoratechDbContext _dbContext;

        public VendaService(LevoratechDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VendaModel> AddVendaAsync( VendaModel venda)
        {
            // Validação se o cliente e o produto existem
            var cliente = await _dbContext.Cliente.FindAsync(venda.ClienteId);
            var produto = await _dbContext.Produto.FindAsync(venda.ProdutoId);

            if (cliente == null || produto == null)
            {
                throw new ArgumentException("\nCliente ou Produto não encontrado");
            }

            venda.PrecoUnitario = produto.valor;

            if (venda.DataVenda == default(DateTime))
            {
                venda.DataVenda = DateTime.Now;
            }

            if (string.IsNullOrEmpty(venda.NumeroNotaFiscal))
            {
                venda.NumeroNotaFiscal = Guid.NewGuid().ToString();
            }

            _dbContext.Venda.Add(venda);
            await _dbContext.SaveChangesAsync();
            return venda;
        }

        public async Task<IEnumerable<object>> GetVendasByProdutoDetalhadaAsync(int produtoId)
        {
            return await _dbContext.Venda
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Where(v => v.ProdutoId == produtoId)
                .Select(v => new 
                {
                    v.Id,
                    v.DataVenda,
                    ProdutoNome = v.Produto.nome,
                    ClienteNome = v.Cliente.nome,
                    v.QuantidadeVendida,
                    v.PrecoUnitario
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetVendasByProdutoAgregadaAsync(int produtoId)
        {
            return await _dbContext.Venda
                .Include(v => v.Produto)
                .Where(v => v.ProdutoId == produtoId)
                .GroupBy(v => new { v.ProdutoId, v.Produto.nome })
                .Select(g => new 
                {
                    ProdutoNome = g.Key.nome,
                    QuantidadeTotalVendida = g.Sum(v => v.QuantidadeVendida),
                    PrecoTotal = g.Sum(v => v.PrecoUnitario * v.QuantidadeVendida)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetVendasByClienteDetalhadaAsync(int clienteId)
        {
            return await _dbContext.Venda
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Where(v => v.ClienteId == clienteId)
                .Select(v => new 
                {
                    v.Id,
                    v.DataVenda,
                    ProdutoNome = v.Produto.nome,
                    v.QuantidadeVendida,
                    v.PrecoUnitario
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetVendasByClienteAgregadaAsync(int clienteId)
        {
            return await _dbContext.Venda
                .Include(v => v.Cliente)
                .Where(v => v.ClienteId == clienteId)
                .GroupBy(v => v.ClienteId)
                .Select(g => new 
                {
                    ClienteNome = g.FirstOrDefault().Cliente.nome,
                    QuantidadeTotalVendida = g.Sum(v => v.QuantidadeVendida),
                    PrecoTotal = g.Sum(v => v.PrecoUnitario * v.QuantidadeVendida)
                })
                .ToListAsync();
        }
    }
}