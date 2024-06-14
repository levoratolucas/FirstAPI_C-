using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace firstORM.data
{
    public class ProdutoDbContextFactory : IDesignTimeDbContextFactory<ProdutoDbContext>
    {
        public ProdutoDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ProdutoDbContext>();
            var connectionString = configuration.GetConnectionString("ProdutoConnection");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new ProdutoDbContext(optionsBuilder.Options);
        }
    }
}
