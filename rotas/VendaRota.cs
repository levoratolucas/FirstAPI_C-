using firstORM.services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using firstORM.data;
using firstORM.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions; // Adicione essa linha se necessário

namespace firstORM.rota
{
    public class VendaRota
    {
        private readonly VendaService VendaService;

        public VendaRota(LevoratechDbContext dbContext)
        {
            VendaService = new VendaService(dbContext);
        }

        private TokenValidationParameters GetValidationParameters()
        {
            var key = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcab");
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }

        private bool ValidateToken(HttpContext context, out SecurityToken validatedToken)
        {
            validatedToken = null;
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.WriteAsync("Adicione um token").Wait();
                return false;
            }

            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, GetValidationParameters(), out validatedToken);
                return true;
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.WriteAsync("Token inválido").Wait();
                return false;
            }
        }

        public void Rotas(WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/vendas", async (HttpContext context, VendaModel venda) =>
            {
                if (!ValidateToken(context, out _)) return Results.Unauthorized();
                try
                {
                    var novaVenda = await VendaService.AddVendaAsync(venda);
                    return Results.Created($"/vendas/{novaVenda.Id}", novaVenda);
                }
                catch (ArgumentException e)
                {
                    return Results.BadRequest(e.Message);
                }
            });

            app.MapGet("/vendas/produto/sumarizada/{produtoId}", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return Results.Unauthorized();
                var produtoId = int.Parse(context.Request.RouteValues["produtoId"].ToString());
                var vendas = await VendaService.GetVendasByProdutoAgregadaAsync(produtoId);
                return Results.Ok(vendas);
            });

            app.MapGet("/vendas/cliente/detalhada/{clienteId}", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return Results.Unauthorized();
                var clienteId = int.Parse(context.Request.RouteValues["clienteId"].ToString());
                var vendas = await VendaService.GetVendasByClienteDetalhadaAsync(clienteId);
                return Results.Ok(vendas);
            });

            app.MapGet("/vendas/cliente/sumarizada/{clienteId}", async (HttpContext context) =>
            {
                if (!ValidateToken(context, out _)) return Results.Unauthorized();
                var clienteId = int.Parse(context.Request.RouteValues["clienteId"].ToString());
                var vendas = await VendaService.GetVendasByClienteAgregadaAsync(clienteId);
                return Results.Ok(vendas);
            });
        }
    }
}
