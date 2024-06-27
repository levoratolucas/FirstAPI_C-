namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using firstORM.models;
    using System.Text.Json;

    public class FornecedorService
    {

        private LevoratechDbContext _dbContext;
        public FornecedorService(LevoratechDbContext db)
        {
            _dbContext = db;
        }
        // Método para consultar todos os fornecedors
        public async Task<List<firstORM.models.FornecedorModel>> GetAllfornecedorsAsync()
        {
            return await _dbContext.Fornecedor.ToListAsync();
        }

        // Método para consultar um fornecedor a partir do seu Id
        public async Task<firstORM.models.FornecedorModel> GetfornecedorByIdAsync(int id)
        {
            return await _dbContext.Fornecedor.FindAsync(id);
        }

        // Método para  gravar um novo fornecedor
        public async Task AddfornecedorAsync(firstORM.models.FornecedorModel fornecedor)
        {
            _dbContext.Fornecedor.Add(fornecedor);
            await _dbContext.SaveChangesAsync();
        }
        public async Task Update(HttpContext context, WebApplication? app, string nome, string cnpj, string email, string telefone, int id)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                var fornecedor = await dbContext.Fornecedor.FindAsync(id);

                if (fornecedor != null)
                {
                    fornecedor.nome = !string.IsNullOrEmpty(nome) ? nome : fornecedor.nome;
                    fornecedor.cnpj = !string.IsNullOrEmpty(cnpj) ? cnpj : fornecedor.cnpj;
                    fornecedor.email = !string.IsNullOrEmpty(email) ? email : fornecedor.email;
                    fornecedor.telefone = !string.IsNullOrEmpty(telefone) ? telefone : fornecedor.telefone;

                    await dbContext.SaveChangesAsync();

                    var fornecedorJson = JsonSerializer.Serialize(fornecedor);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(fornecedorJson);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("fornecedor não encontrado");
                }
            }
        }
        // Método para atualizar os dados de um fornecedor
        public async Task UpdatefornecedorAsync(int id, firstORM.models.FornecedorModel fornecedor)
        {
            _dbContext.Entry(fornecedor).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // Método para excluir um fornecedor
        public async Task DeletefornecedorAsync(int id)
        {
            var fornecedor = await _dbContext.Fornecedor.FindAsync(id);
            if (fornecedor != null)
            {
                _dbContext.Fornecedor.Remove(fornecedor);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
