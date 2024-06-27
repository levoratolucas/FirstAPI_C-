namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using System.Text.Json;
    public class UserRota
    {

        public void Rotas(WebApplication? app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/user/add", async (HttpContext context) =>
                {
                    if (!context.Request.Headers.ContainsKey("Authorization"))// verifica se veio um token
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Adicione um token");
                    }
                    //recebe token e tira o Bearer a baixo
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    //using System.IdentityModel.Tokens.Jwt;
                    var tokenHandler = new JwtSecurityTokenHandler(); // cria um objeto JWT responsaveu por token


                    // using System.Text;
                    var key = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcab");// palavra secreta

                    // using Microsoft.IdentityModel.Tokens;
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };// gera um objeto construido com a chave secreta

                    SecurityToken validateToken; // gera um objeto pra validar token que tem a função responsavel

                    try
                    {
                        // decodifica, verifica e valida token
                        tokenHandler.ValidateToken(token, validationParameters, out validateToken);
                        using var reader = new System.IO.StreamReader(context.Request.Body);
                        var body = await reader.ReadToEndAsync();

                        // using System.Text.Json;
                        var json = JsonDocument.Parse(body);
                        var nome = json.RootElement.GetProperty("username").ToString();
                        var email = json.RootElement.GetProperty("email").ToString();
                        var senha = json.RootElement.GetProperty("senha").ToString();
                        using (var scope = app.Services.CreateScope())
                        {
                            // using firstORM.data;
                            var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                            dbContext.AddUserModel(nome, email, senha);
                        }

                        await context.Response.WriteAsync("adicionado " + nome);

                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token invalido ");


                    }

                });
        

            app.MapGet("/user/list", async (HttpContext context) =>
                {
                    if (!context.Request.Headers.ContainsKey("Authorization"))// verifica se veio um token
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Adicione um token");
                    }
                    //recebe token e tira o Bearer a baixo
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    var tokenHandler = new JwtSecurityTokenHandler(); // cria um objeto JWT responsaveu por token

                    var key = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcab");// palavra secreta

                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };// gera um objeto construido com a chave secreta

                    SecurityToken validateToken; // gera um objeto pra validar token que tem a função responsavel

                    try
                    {
                        // decodifica, verifica e valida token
                        tokenHandler.ValidateToken(token, validationParameters, out validateToken);
                        using (var scope = app.Services.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                            var users = await dbContext.Users.OrderByDescending(u => u.id).ToListAsync();
                            await context.Response.WriteAsJsonAsync(users);
                            ;
                        }

                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token invalido ");


                    }


                });
        
        

            app.MapPost("/user/search", async (HttpContext context) =>
                {

                    if (!context.Request.Headers.ContainsKey("Authorization"))// verifica se veio um token
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Adicione um token");
                    }
                    //recebe token e tira o Bearer a baixo
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    var tokenHandler = new JwtSecurityTokenHandler(); // cria um objeto JWT responsaveu por token

                    var key = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcab");// palavra secreta

                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };// gera um objeto construido com a chave secreta

                    SecurityToken validateToken; // gera um objeto pra validar token que tem a função responsavel

                    try
                    {
                        // decodifica, verifica e valida token
                        tokenHandler.ValidateToken(token, validationParameters, out validateToken);
                        using var reader = new System.IO.StreamReader(context.Request.Body);
                        var body = await reader.ReadToEndAsync();
                        var json = JsonDocument.Parse(body);
                        var nome = json.RootElement.GetProperty("nome").ToString();
                        using (var scope = app.Services.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                            var users = await dbContext.Users.Where(u => EF.Functions.Like(u.nome, "%" + nome + "%")).ToListAsync();
                            await context.Response.WriteAsJsonAsync(users);

                        }

                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token invalido ");


                    }



                });
        }
    
    
    }
}