using FluentAssertions;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace ToDo.Backend.Tests.E2E.Bindings.Steps
{
    [Binding]
    public sealed class LandingSteps
    {
        private readonly IWebDriver _webDriver;

        public LandingSteps(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        [Then(@"(?:I should )?see correct landing page")]
        public void SeeCorrectLandingPage()
        {
            FindElement(By.ClassName("navbar")).Should().NotBeNull();
        }

        private IWebElement FindElement(By by)
        {
            try
            {
                return _webDriver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }
    }
}