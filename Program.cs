


var builder = firstORM.AppBuilder.GenerateBuilder(args);
var app = builder.Build();
var auth = new firstORM.config.Auth();
auth.TokenAuth(app, "/login");
var user = new firstORM.Rotas.UserRota();
var produto = new firstORM.Rotas.ProdutoRota();
user.Add(app,"/useradd");
user.List(app,"/userlist");
user.Search(app,"/usersearch");
produto.List(app,"/List");
produto.Add(app,"/add");
app.Run();
