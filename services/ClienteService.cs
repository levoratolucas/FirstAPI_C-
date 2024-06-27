namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using firstORM.models;
    using System.Text.Json;

    public class ClienteService
    {


        private LevoratechDbContext _dbContext;

        public ClienteService(LevoratechDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<firstORM.models.ClienteModel>> GetAllclientesAsync()
        {
            return await _dbContext.Cliente.ToListAsync();
        }

        // Método para consultar um cliente a partir do seu Id
        public async Task<firstORM.models.ClienteModel> GetclienteByIdAsync(int id)
        {
            return await _dbContext.Cliente.FindAsync(id);
        }

        // Método para  gravar um novo cliente
        public async Task AddClienteAsync(firstORM.models.ClienteModel cliente)
        {
            _dbContext.Cliente.Add(cliente);
            await _dbContext.SaveChangesAsync();
        }
        public async Task update(HttpContext context, WebApplication? app, string nome, string CPF, string email, int id)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                var cliente = await dbContext.Cliente.FindAsync(id);
                
                if (cliente != null)
                {
                    
                    cliente.nome = !string.IsNullOrEmpty(nome) ? nome : cliente.nome;
                    cliente.CPF = !string.IsNullOrEmpty(CPF) ? CPF : cliente.CPF;
                    cliente.email = !string.IsNullOrEmpty(email) ? email : cliente.email;
                    await dbContext.SaveChangesAsync();
                    var clienteJson = JsonSerializer.Serialize(cliente);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(clienteJson);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("cliente não encontrado");
                }
            }
        }
        // Método para atualizar os dados de um cliente
        public async Task UpdateclienteAsync(int id, firstORM.models.ClienteModel cliente)
        {
            _dbContext.Entry(cliente).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // Método para excluir um cliente
        public async Task DeleteclienteAsync(int id)
        {
            var cliente = await _dbContext.Cliente.FindAsync(id);
            if (cliente != null)
            {
                _dbContext.Cliente.Remove(cliente);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
