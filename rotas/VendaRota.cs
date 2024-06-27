using firstORM.services;
using firstORM.data;
using firstORM.models;
using firstORM.config;


namespace firstORM.rota
{
    public class VendaRota
    {
        private readonly VendaService VendaService;

        public VendaRota(LevoratechDbContext dbContext)
        {
            VendaService = new VendaService(dbContext);
        }


        public void Rotas(WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/vendas", async (HttpContext context, VendaModel venda) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return Results.Unauthorized();
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
                if (!ValToken.ValidateToken(context, out _)) return Results.Unauthorized();
                var produtoId = int.Parse(context.Request.RouteValues["produtoId"].ToString());
                var vendas = await VendaService.GetVendasByProdutoAgregadaAsync(produtoId);
                return Results.Ok(vendas);
            });

            app.MapGet("/vendas/cliente/detalhada/{clienteId}", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return Results.Unauthorized();
                var clienteId = int.Parse(context.Request.RouteValues["clienteId"].ToString());
                var vendas = await VendaService.GetVendasByClienteDetalhadaAsync(clienteId);
                return Results.Ok(vendas);
            });

            app.MapGet("/vendas/cliente/sumarizada/{clienteId}", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return Results.Unauthorized();
                var clienteId = int.Parse(context.Request.RouteValues["clienteId"].ToString());
                var vendas = await VendaService.GetVendasByClienteAgregadaAsync(clienteId);
                return Results.Ok(vendas);
            });
        }
    }
}
