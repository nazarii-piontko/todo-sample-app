using System.Linq;
using BoDi;
using OpenQA.Selenium;

namespace ToDo.Backend.Tests.E2E.Pages
{
    public sealed class RegisterPage : AnyPage
    {
        public RegisterPage(IObjectContainer container) 
            : base(container)
        {
        }
        
        public bool IsCorrectLayout()
        {
            return new[] {"email-input", "password-input", "register-button"}
                .Select(id => FindElement(By.Id(id)) != null)
                .All(_ => _);
        }
    }
}