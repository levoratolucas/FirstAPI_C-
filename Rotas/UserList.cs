namespace firstORM.Rotas
{
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;

    public class UserList
    {

        public void Rota(WebApplication? app, String rota)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapGet(rota, async (HttpContext context) =>
                {
                    using (var scope = app.Services.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                        var users = await dbContext.Users.OrderByDescending(u => u.id).ToListAsync();
                        return Results.Json(users);
                    }
                });
        }
    }
}