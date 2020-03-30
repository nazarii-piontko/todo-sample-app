using System.Net.Http;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;
using ToDo.Backend.Tests.E2E.Infrastructure;
using ToDo.Backend.Tests.E2E.Infrastructure.Settings;
using ToDo.Backend.Tests.E2E.Pages;

namespace ToDo.Backend.Tests.E2E.Bindings
{
    [Binding]
    public sealed class Hooks
    {
        [BeforeTestRun]
        public static void BeforeAll(IObjectContainer container)
        {
            container.RegisterInstanceAs(new HttpClient());
            
            var settings = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .AddEnvironmentVariables()
                .Build()
                .Get<TestsEnvSettings>();

            container.RegisterInstanceAs(settings);
            
            container.RegisterTypeAs<Artifacts, Artifacts>();
            container.RegisterTypeAs<AutController, AutController>();
            container.RegisterTypeAs<WebDriverProvider, WebDriverProvider>();
            
            container.Resolve<Artifacts>().Init();
        }

        [BeforeScenario]
        public static void BeforeScenario(IObjectContainer container)
        {
            container.RegisterFactoryAs(c =>
            {
                var webDriverFactory = c.Resolve<WebDriverProvider>();
                var driver = webDriverFactory.CreateAsync().Result;
                return driver;
            });
            
            container.RegisterTypeAs<AnyPage, AnyPage>();
            container.RegisterTypeAs<LandingPage, LandingPage>();
        }
        
        [AfterTestRun]
        public static async Task AfterAll(IObjectContainer container)
        {
            if (container.IsRegistered<AutController>())
                await container.Resolve<AutController>().StopAsync();
            
            if (container.IsRegistered<WebDriverProvider>())
                await container.Resolve<WebDriverProvider>().CleanUpAsync();
        }
    }
}