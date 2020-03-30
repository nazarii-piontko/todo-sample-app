namespace ToDo.Backend.Tests.E2E.Infrastructure.Settings
{
    /// <summary>
    /// Setting root.
    /// </summary>
    public sealed class TestsEnvSettings
    {
        public AutSettings Aut { get; set; }
        
        public AutDeploySettings AutDeploy { get; set; }
        
        public SeleniumSettings Selenium { get; set; }
        
        public TestsSettings Tests { get; set; }
    }
}