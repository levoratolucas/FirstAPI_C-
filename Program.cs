
public class Program
{
    public static void Main(string[] args)
    {
        var builder = firstORM.AppBuilder.GenerateBuilder(args);
        var app = builder.Build();
        var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<firstORM.data.LevoratechDbContext>();


        var auth = new firstORM.config.Auth();
        var user = new firstORM.rota.UserRota();
        var clienteRota = new firstORM.rota.ClienteRota(dbContext);
        var produtoRota = new firstORM.rota.ProdutoRota(dbContext);
        var vendaRota = new firstORM.rota.VendaRota(dbContext);


        auth.TokenAuth(app);
        user.Rotas(app);
        produtoRota.Rotas(app);
        clienteRota.Rotas(app);        
        vendaRota.Rotas(app);

        
        app.Run();
    }
}

