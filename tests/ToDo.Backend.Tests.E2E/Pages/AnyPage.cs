using System;
using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using ToDo.Backend.Tests.E2E.Infrastructure;
using ToDo.Backend.Tests.E2E.Infrastructure.Settings;

namespace ToDo.Backend.Tests.E2E.Pages
{
    public class AnyPage
    {
        private readonly IObjectContainer _container;

        public AnyPage(IObjectContainer container)
        {
            _container = container;
        }

        public void MakeScreenshot(string name)
        {
            var path = _container.Resolve<Artifacts>().GetPath(name + ".png");
            var screenshot = WebDriver.TakeScreenshot();
            screenshot.SaveAsFile(path);
        }

        public IWebDriver WebDriver => _container.Resolve<IWebDriver>();
        
        protected IWebElement FindElement(By by)
        {
            try
            {
                return WebDriver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public void Navigate(string path)
        {
            var url = new Uri(_container.Resolve<TestsEnvSettings>().Aut.BaseUri, path);
            WebDriver.Navigate().GoToUrl(url);
        }
    }
}