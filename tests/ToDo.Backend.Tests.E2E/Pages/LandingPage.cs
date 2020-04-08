using System.Linq;
using BoDi;
using OpenQA.Selenium;

namespace ToDo.Backend.Tests.E2E.Pages
{
    public sealed class LandingPage : AnyPage
    {
        public LandingPage(IObjectContainer container) 
            : base(container)
        {
        }
        
        public bool IsCorrectLayout()
        {
            return new[] {"login-link", "register-link"}
                .Select(id => FindElement(By.Id(id)) != null)
                .All(_ => _);
        }
    }
}