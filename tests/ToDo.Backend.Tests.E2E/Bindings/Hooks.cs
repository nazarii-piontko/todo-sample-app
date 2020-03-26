using System;
using System.Drawing;
using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using TechTalk.SpecFlow;

namespace ToDo.Backend.Tests.E2E.Bindings
{
    [Binding]
    public sealed class Hooks
    {
        private readonly IObjectContainer _objectContainer;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var webDriver = new RemoteWebDriver(new ChromeOptions());
            webDriver.Manage().Window.Size = new Size(800, 600);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5d);
            _objectContainer.RegisterInstanceAs<IWebDriver>(webDriver);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _objectContainer.Resolve<IWebDriver>().Dispose();
        }
    }
}