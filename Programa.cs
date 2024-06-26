
public class Programa
{
    public static void Main(string[] args)
    {
        var builder = firstORM.AppBuilder.GenerateBuilder(args);
        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
        var auth = new firstORM.config.Auth();
        var user = new firstORM.service.UserRota();
        var cliente = new firstORM.service.ClienteRota();
        auth.TokenAuth(app, "/login");
        user.Add(app, "/useradd");
        user.List(app, "/userlist");
        user.Search(app, "/usersearch");
        var produtoRota = new firstORM.service.ProdutoRota();
        produtoRota.Rotas(app);
        cliente.List(app, "/clienteList");
        cliente.Add(app, "/clienteadd");
        cliente.Delete(app, "/clientedelete");
        cliente.Search(app, "/clientesearch");
        cliente.Update(app, "/clienteupdate");
        app.Run();


        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // var produtoRota2 = new ProdutoRota2(app);
        // produtoRota2.ConfigureRoutes();
        

        app.Run();
    }
}

