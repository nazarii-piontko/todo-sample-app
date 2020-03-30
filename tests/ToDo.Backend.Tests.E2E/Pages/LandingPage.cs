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
            return FindElement(By.ClassName("navbar")) != null;
        }
    }
}