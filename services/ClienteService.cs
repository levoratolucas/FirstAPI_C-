namespace firstORM.service
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
        

        // Método para consultar todos os clientes
        public async Task<List<firstORM.models.ClienteModel>> GetAllclientesAsync(LevoratechDbContext _dbContext)
        {
            return await _dbContext.Cliente.ToListAsync();
        }

        // Método para consultar um cliente a partir do seu Id
        public async Task<firstORM.models.ClienteModel> GetclienteByIdAsync(LevoratechDbContext _dbContext,int id)
        {
            return await _dbContext.Cliente.FindAsync(id);
        }
        
        // Método para  gravar um novo cliente
        public async Task AddClienteAsync(LevoratechDbContext _dbContext,firstORM.models.ClienteModel cliente)
        {
            _dbContext.Cliente.Add(cliente);
            await _dbContext.SaveChangesAsync();
        }
        public async Task update(HttpContext context, WebApplication? app, string nome, string CPF, string email, int id ){
            using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                    var cliente = await dbContext.Cliente.FindAsync(id);
                    if (cliente != null)
                    {
                        cliente.nome = nome;
                        cliente.CPF = CPF;
                        cliente.email = email;
                        await dbContext.SaveChangesAsync();
                        await context.Response.WriteAsync("cliente atualizado: " + nome);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync("cliente não encontrado");
                    }
                }
        }
        // Método para atualizar os dados de um cliente
        public async Task UpdateclienteAsync(LevoratechDbContext _dbContext,int id, firstORM.models.ClienteModel cliente)
        {
            _dbContext.Entry(cliente).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // Método para excluir um cliente
        public async Task DeleteclienteAsync(LevoratechDbContext _dbContext,int id)
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
