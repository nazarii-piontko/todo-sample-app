using FluentAssertions;
using TechTalk.SpecFlow;
using ToDo.Backend.Tests.E2E.Pages;

namespace ToDo.Backend.Tests.E2E.Bindings.Steps
{
    [Binding]
    public sealed class RegisterSteps
    {
        private readonly RegisterPage _registerPage;

        public RegisterSteps(RegisterPage registerPage)
        {
            _registerPage = registerPage;
        }

        [Then(@"(?:I should )?see correct register page")]
        public void SeeCorrectLandingPage()
        {
            _registerPage.IsCorrectLayout().Should().BeTrue();
        }
    }
}