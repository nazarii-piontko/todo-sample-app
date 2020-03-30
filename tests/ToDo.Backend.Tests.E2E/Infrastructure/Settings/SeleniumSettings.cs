using System;

namespace ToDo.Backend.Tests.E2E.Infrastructure.Settings
{
    public sealed class SeleniumSettings
    {
        public string[] StartCommand { get; set; }
        
        public string[] StopCommand { get; set; }
        
        public Uri Uri { get; set; }
        
        public TimeSpan ImplicitWait { get; set; }
    }
}