namespace firstORM.Rotas
{
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using System.Text.Json;


    public class UserSearch
    {

        public void Rota(WebApplication? app, String rota)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost(rota, async (HttpContext context) =>
                {
                    using var reader = new System.IO.StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    var json = JsonDocument.Parse(body);
                    var nome = json.RootElement.GetProperty("nome").ToString();
                    using (var scope = app.Services.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                        var users = await dbContext.Users
                            .Where(u => EF.Functions.Like(u.nome, "%" + nome + "%"))
                            .ToListAsync();
                        return Results.Json(users);
                    }
                });
        }
    }
}