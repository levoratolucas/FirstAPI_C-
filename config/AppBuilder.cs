
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
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 26)))
                    
            );
             builder.Services.AddDbContext<ProdutoDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),                
                 new MySqlServerVersion(new Version(8, 0, 26))
            ));

            return builder;
        }
    }
}

