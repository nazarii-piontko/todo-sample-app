using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Frontend.Services;
using ToDo.Frontend.Services.Abstractions;
using ToDo.Frontend.ViewModels;

namespace ToDo.Frontend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            RegisterServices(builder.Services);

            builder.RootComponents.Add<Views.App>("#app");

            await builder.Build().RunAsync();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

            services.AddBlazoredLocalStorage();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IAuthDataStorage, AuthDataStorage>();
            services.AddSingleton<RestClient>();

            services.AddSingleton<IToDoListsService, ToDoListsService>();
            services.AddSingleton<IToDoItemsService, ToDoItemsService>();

            services.AddTransient<IndexViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<AppViewModel>();
            services.AddTransient<ListsViewModel>();
            services.AddTransient<ItemsViewModel>();
        }
    }
}
