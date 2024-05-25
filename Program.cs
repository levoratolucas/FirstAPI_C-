
using Microsoft.EntityFrameworkCore;
using firstORM.data;
using System.Text.Json;

var builder = firstORM.AppBuilder.GenerateBuilder(args);

var app = builder.Build();

var auth = new firstORM.config.Auth();
auth.TokenAuth(app, "autenticador");
var useradd = new firstORM.Rotas.Useradd();
useradd.Rota(app,"useradd");
var userList = new firstORM.Rotas.UserList();
userList.Rota(app,"/userlist");
var userSearch = new firstORM.Rotas.UserSearch();
userSearch.Rota(app,"/usersearch");



app.Run();
