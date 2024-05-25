namespace firstORM.Rotas
{
    using System.Text.Json;
    using firstORM.data;
    public class Useradd
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
                    var email = json.RootElement.GetProperty("email").ToString();
                    var senha = json.RootElement.GetProperty("senha").ToString();
                    using (var scope = app.Services.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                        dbContext.AddUserModel(nome, email, senha);
                    }
                    return "adicionado " + nome;
                });
        }
    }
}