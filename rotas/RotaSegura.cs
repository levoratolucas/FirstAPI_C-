namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;

    using Microsoft.IdentityModel.Tokens;
    public class RotaSegura
    {

        public void Rota(WebApplication? app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapGet("/rotaSegura", async (HttpContext context) =>
            {
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token invalido ou não fornecido");
                }
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcab");// chave secreta
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                SecurityToken validateToken;
                try
                {
                    // decodifica, verifica e valida token
                    tokenHandler.ValidateToken(token, validationParameters, out validateToken);

                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token invalido ");
                    return;
                }
                // se valido dar andamento na logica do end point
                await context.Response.WriteAsync("Autorizado");

            });
        }
    }
}