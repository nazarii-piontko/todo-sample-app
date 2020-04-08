using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using ToDo.Backend.Persistence;

namespace ToDo.Backend
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await MigrateDatabaseAsync(host);

            await host.RunAsync().ConfigureAwait(false);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

        private static async Task MigrateDatabaseAsync(IHost host)
        {
            var policy = Policy
                .Handle<Npgsql.NpgsqlException>()
                .WaitAndRetryAsync(120, _ => TimeSpan.FromSeconds(1));

            await policy.ExecuteAsync(async () =>
            {
                using var scope = host.Services.CreateScope();
                await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }
}
