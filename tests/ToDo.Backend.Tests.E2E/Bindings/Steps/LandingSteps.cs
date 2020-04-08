using FluentAssertions;
using TechTalk.SpecFlow;
using ToDo.Backend.Tests.E2E.Pages;

namespace ToDo.Backend.Tests.E2E.Bindings.Steps
{
    [Binding]
    public sealed class LandingSteps
    {
        private readonly LandingPage _landingPage;

        public LandingSteps(LandingPage landingPage)
        {
            _landingPage = landingPage;
        }

        [Then(@"(?:I should )?see correct landing page")]
        public void SeeCorrectLandingPage()
        {
            _landingPage.IsCorrectLayout().Should().BeTrue();
        }
    }
}