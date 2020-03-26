using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace ToDo.Backend.Tests.E2E.Bindings.Steps
{
    [Binding]
    public sealed class CommonSteps
    {
        private readonly IWebDriver _webDriver;

        public CommonSteps(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        [Given(@"(?:I )?open page at (?<path>[\/\w\d-]+)")]
        [When(@"(?:I )?open page at (?<path>[\/\w\d-]+)")]
        public void OpenPageAtPath(string path)
        {
            _webDriver.Navigate().GoToUrl("http://todo-app" + path);
        }
    }
}