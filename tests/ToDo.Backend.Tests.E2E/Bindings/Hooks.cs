using System;
using System.IO;
using System.Net.Http;
using System.Text;
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
        public static async Task BeforeAll(IObjectContainer container)
        {
            Console.WriteLine("### BeforeAll");
            
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
            
            // Ensure nothing is running from previous session
            await container.Resolve<AutController>().StopAsync();
            await container.Resolve<WebDriverProvider>().CleanUpAsync();
        }

        [BeforeScenario]
        public static async Task BeforeScenario(IObjectContainer container, ScenarioInfo info)
        {
            Console.WriteLine($"### BeforeScenario: {info.Title}");

            var webDriverFactory = container.Resolve<WebDriverProvider>();
            var driver = await webDriverFactory.CreateAsync();
            container.RegisterInstanceAs(driver, dispose: true);
            
            container.RegisterTypeAs<AnyPage, AnyPage>();
            container.RegisterTypeAs<LandingPage, LandingPage>();
        }
        
        [AfterScenario]
        public void AfterScenario(IObjectContainer container, ScenarioContext context, ScenarioInfo info)
        {
            Console.WriteLine($"### AfterScenario: {info.Title}");
            
            if (context.TestError != null)
            {
                Console.WriteLine($"### Scenario Error: {context.TestError.Message}");

                var screenshotName = new StringBuilder(info.Title);
                foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
                    screenshotName.Replace(invalidFileNameChar, ' ');

                container.Resolve<AnyPage>().MakeScreenshot(screenshotName.ToString());
            }
        }

        [AfterTestRun]
        public static async Task AfterAll(IObjectContainer container)
        {
            Console.WriteLine("### AfterAll");
            
            if (container.IsRegistered<AutController>())
                await container.Resolve<AutController>().StopAsync();
            
            if (container.IsRegistered<WebDriverProvider>())
                await container.Resolve<WebDriverProvider>().CleanUpAsync();
        }
    }
}