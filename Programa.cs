
public class Programa
{
    public static void Main(string[] args)
    {
        var builder = firstORM.AppBuilder.GenerateBuilder(args);
        var app = builder.Build();
        var auth = new firstORM.config.Auth();
        var user = new firstORM.service.UserRota();
        var clienteRota = new firstORM.service.ClienteRota();
        auth.TokenAuth(app, "/login");
        user.Add(app, "/useradd");
        user.List(app, "/userlist");
        user.Search(app, "/usersearch");
        var produtoRota = new firstORM.service.ProdutoRota();
        produtoRota.Rotas(app);
        clienteRota.Rotas(app);
        app.Run();
    }
}

