using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using firstORM.data;
using System;

namespace firstORM
{
    public class AppBuilder
    {
        public static WebApplicationBuilder GenerateBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var mysqlVersion = new MySqlServerVersion(new Version(8, 0, 26));

            void AddMySql<TContext>(IServiceCollection services, MySqlServerVersion mysqlVersion) where TContext : DbContext
            {
                services.AddDbContext<TContext>(options =>
                    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), mysqlVersion));
            }
            AddMySql<LevoratechDbContext>(builder.Services, mysqlVersion);

            return builder;
        }
    }
}
