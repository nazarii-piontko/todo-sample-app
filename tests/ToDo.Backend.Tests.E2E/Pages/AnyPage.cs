using System;
using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
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

        public void WaitUntilPageIsAvailable(string path)
        {
            var waitPageAvailable = new WebDriverWait(WebDriver,
                _container.Resolve<TestsEnvSettings>().Tests.OperationsTimeout);

            waitPageAvailable.Until(d =>
            {
                try
                {
                    var url = BuildAbsUrl(path);
                    d.Navigate().GoToUrl(url);

                    var waitPageLoaded = new WebDriverWait(d,
                        _container.Resolve<TestsEnvSettings>().Tests.RequestsTimeout);

                    waitPageLoaded.Until(IsPageLoaded);

                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public void Navigate(string path)
        {
            var url = BuildAbsUrl(path);
            WebDriver.Navigate().GoToUrl(url);
        }

        public void WaitUntilLoaded()
        {
            var wait = new WebDriverWait(WebDriver, 
                _container.Resolve<TestsEnvSettings>().Tests.RequestsTimeout);

            wait.Until(IsPageLoaded);
        }

        public void EnterValueIntoInput(string field, string value)
        {
            field = field.ToLowerInvariant().Replace(' ', '-');
            
            foreach (var idCandidate in new[] {field, field + "-input"})
            {
                var input = FindElement(By.Id(idCandidate));

                if (input != null)
                {
                    input.Clear();
                    input.SendKeys(value);
                    return;
                }
            }
            
            throw new ArgumentException($"Input '{field}' doesn't found");
        }

        public void ClickButton(string button)
        {
            button = button.ToLowerInvariant().Replace(' ', '-');

            foreach (var idCandidate in new[] {button, button + "-button", button + "-link"})
            {
                var input = FindElement(By.Id(idCandidate));

                if (input != null)
                {
                    input.Click();
                    return;
                }
            }
            
            throw new ArgumentException($"Button '{button}' doesn't found");
        }

        public void WaitForPath(string path, int timeout)
        {
            var url = new Uri(_container.Resolve<TestsEnvSettings>().Aut.BaseUri, path).AbsoluteUri;
            
            var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(timeout));

            wait.Until(d => d.Url.Equals(url, StringComparison.OrdinalIgnoreCase));
        }
        
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
        
        private static IWebElement FindElement(ISearchContext driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }
        
        private static bool IsPageLoaded(IWebDriver driver)
        {
            return driver.ExecuteJavaScript<string>("return document.readyState").Equals("complete")
                   && driver.Title.StartsWith("ToDo Application", StringComparison.Ordinal)
                   && FindElement(driver, By.Id("blazor-loading-ui")) == null;
        }
        
        private Uri BuildAbsUrl(string path)
        {
            return new Uri(_container.Resolve<TestsEnvSettings>().Aut.BaseUri, path);
        }
    }
}