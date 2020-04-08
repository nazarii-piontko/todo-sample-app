using System.Drawing;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using ToDo.Backend.Tests.E2E.Infrastructure;
using ToDo.Backend.Tests.E2E.Pages;

namespace ToDo.Backend.Tests.E2E.Bindings.Steps
{
    [Binding]
    public sealed class CommonSteps
    {
        private readonly AutController _controller;
        private readonly AnyPage _page;

        public CommonSteps(AutController controller, AnyPage page)
        {
            _controller = controller;
            _page = page;
        }

        [Given(@"service is running")]
        public async Task ServiceIsRunning()
        {
            await _controller.StartAsync();

            _page.WaitUntilPageIsAvailable("/");
        }

        [Given(@"web browser window with size (?<width>\d+)x(?<height>\d+)")]
        public void WebBrowserWindowWithSize(int width, int height)
        {
            _page.WebDriver.Manage().Window.Size = new Size(width, height);
        }

        [Given(@"opened page at (?<path>[\/\w-]+)")]
        [When(@"(?:I )?open page at (?<path>[\/\w-]+)")]
        public void OpenPageAtPath(string path)
        {
            _page.Navigate(path); 
            _page.WaitUntilLoaded();
        }

        [Then(@"make screenshot with name (?<name>[ -_\w\s]+)")]
        public void MakeScreenshot(string name)
        {
            _page.MakeScreenshot(name);
        }

        [When(@"(?:I )?input '(?<value>[\w@.]+)' into '(?<field>[\w@.]+)'")]
        public void EnterValueIntoInput(string value, string field)
        {
            _page.EnterValueIntoInput(field, value);
        }
        
        [When(@"(?:I )?click button '(?<button>[\w@.]+)'")]
        public void ClickButton(string button)
        {
            _page.ClickButton(button);
        }

        [Then(@"page should be redirected to (?<path>[\/\w-]+) within (?<timeout>\d+) sec")]
        public void PageShouldBeRedirected(string path, int timeout)
        {
            _page.WaitForPath(path, timeout);
        }
    }
}