using System;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;
using ToDo.Backend.Tests.E2E.Infrastructure;
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

        [Then(@"(?:I should )?see correct landing page within (?<seconds>\d+) sec")]
        public async Task SeeCorrectLandingPage(int seconds)
        {
            Func<Task> act = () =>
                Utils.ExecuteWithRetryAsync(() => _landingPage.IsCorrectLayout(),
                    TimeSpan.FromSeconds(seconds));
            await act.Should().NotThrowAsync();
        }
    }
}