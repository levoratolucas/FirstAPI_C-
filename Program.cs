
public class Program
{
    public static void Main(string[] args)
    {
        var builder = firstORM.AppBuilder.GenerateBuilder(args);
        var app = builder.Build();
        var auth = new firstORM.config.Auth();
        var user = new firstORM.Rotas.UserRota();
        var cliente = new firstORM.Rotas.ClienteRota();
        auth.TokenAuth(app, "/login");
        user.Add(app, "/useradd");
        user.List(app, "/userlist");
        user.Search(app, "/usersearch");
        var produtoRota = new firstORM.Rotas.ProdutoRota();
        produtoRota.Add(app, "/produto/adicionar");
        produtoRota.List(app, "/produto/listar");
        produtoRota.Search(app, "/produto/procurar");
        produtoRota.Update(app, "/produto/atualizar");
        produtoRota.Delete(app, "/produto/deletar");
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

