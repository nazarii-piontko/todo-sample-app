using System;
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

        public WebDriverProvider(TestsEnvSettings settings)
        {
            _settings = settings;
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

            await EnsureSeleniumRunningAsync(driverOptions);

            var webDriver = CreateWebDriver(driverOptions);

            SetDriverDefaults(webDriver);

            return webDriver;
        }

        public async Task CleanUpAsync()
        {
            await Utils.RunCommandAsync(_settings.Selenium.StopCommand,
                _settings.Tests.OperationsTimeout);
        }
        
        private RemoteWebDriver CreateWebDriver(DriverOptions driverOptions)
        {
            return new RemoteWebDriver(_settings.Selenium.Uri, driverOptions);
        }

        private void SetDriverDefaults(IWebDriver webDriver)
        {
            var timeouts = webDriver.Manage().Timeouts();
            timeouts.ImplicitWait = _settings.Selenium.ImplicitWait;
            timeouts.PageLoad = _settings.Selenium.PageLoadWait;
        }

        private async Task EnsureSeleniumRunningAsync(DriverOptions driverOptions)
        {
            if (IsRunning(driverOptions))
                return;

            await Utils.RunCommandAsync(_settings.Selenium.StartCommand,
                _settings.Tests.OperationsTimeout);

            await Utils.ExecuteWithRetryAsync(
                () => Task.FromResult(IsRunning(driverOptions)),
                timeout: _settings.Tests.OperationsTimeout);
        }

        private bool IsRunning(DriverOptions driverOptions)
        {
            try
            {
                // To be 100% sure that everything is running properly we need to create web driver and load a test page
                using var driver = CreateWebDriver(driverOptions);
                SetDriverDefaults(driver);
                driver.Navigate().GoToUrl("https://google.com/");
                return true;
            }
            catch (WebDriverException)
            {
                return false;
            }
        }
    }
}