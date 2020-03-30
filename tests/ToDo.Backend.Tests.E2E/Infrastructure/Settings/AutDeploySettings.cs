namespace ToDo.Backend.Tests.E2E.Infrastructure.Settings
{
    /// <summary>
    /// Application under test (AUT) deploy settings.
    /// Start/stop scripts.
    /// </summary>
    public sealed class AutDeploySettings
    {
        public string[] StartCommand { get; set; }
        
        public string[] StopCommand { get; set; }
    }
}