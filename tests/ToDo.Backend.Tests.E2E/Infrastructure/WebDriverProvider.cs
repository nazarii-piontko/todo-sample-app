using System;
using System.Net.Http;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using ToDo.Backend.Tests.E2E.Infrastructure.Settings;

namespace ToDo.Backend.Tests.E2E.Infrastructure
{
    public sealed class WebDriverProvider
    {
        private readonly TestsEnvSettings _settings;
        private readonly HttpClient _httpClient;

        public WebDriverProvider(TestsEnvSettings settings, 
            HttpClient httpClient)
        {
            _settings = settings;
            _httpClient = httpClient;
        }

        public async Task<IWebDriver> CreateAsync(SeleniumDriver type = SeleniumDriver.Chrome)
        {
            DriverOptions driverOptions;

            switch (type)
            {
                case SeleniumDriver.Chrome:
                    driverOptions = new ChromeOptions();
                    break;
                case SeleniumDriver.Firefox:
                    driverOptions = new FirefoxOptions();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await EnsureSeleniumRunningAsync();

            var webDriver = new RemoteWebDriver(_settings.Selenium.Uri, driverOptions);

            SetDriverDefaults(webDriver);

            return webDriver;
        }

        public async Task CleanUpAsync()
        {
            await Utils.RunCommandAsync(_settings.Selenium.StopCommand,
                _settings.Tests.OperationsTimeout);
        }

        private void SetDriverDefaults(IWebDriver webDriver)
        {
            webDriver.Manage().Timeouts().ImplicitWait = _settings.Selenium.ImplicitWait;
        }

        private async Task EnsureSeleniumRunningAsync()
        {
            Task<bool> IsRunningAsync() => _httpClient.IsUriAccessibleAsync(new Uri(_settings.Selenium.Uri, "status"));

            if (await IsRunningAsync())
                return;

            await Utils.RunCommandAsync(_settings.Selenium.StartCommand,
                _settings.Tests.OperationsTimeout);

            await Utils.ExecuteWithRetryAsync(
                IsRunningAsync,
                timeout: _settings.Tests.OperationsTimeout);
        }
    }
}