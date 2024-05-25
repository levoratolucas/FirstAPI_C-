namespace firstORM.Rotas
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;

    public class UserList
    {

        public void Rota(WebApplication? app, String rota)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapGet(rota, async (HttpContext context) =>
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
                        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                        var users = await dbContext.Users.OrderByDescending(u => u.id).ToListAsync();
                          await context.Response.WriteAsJsonAsync(users);
                          ;
                    }

                }catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token invalido ");
                
                
                }

                    
                });
        }
    }
}