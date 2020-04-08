using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToDo.Backend.Persistence;

namespace ToDo.Backend.Tests.Integration
{
    public sealed class BackendApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("db");
                });
                
                InitializeDatabase(services);
            });
        }

        private static void InitializeDatabase(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            
            using var scope = provider.CreateScope();
            
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            context.Database.EnsureCreated();
        }
    }
}