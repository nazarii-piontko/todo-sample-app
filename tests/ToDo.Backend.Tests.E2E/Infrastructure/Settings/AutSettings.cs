using System;

namespace ToDo.Backend.Tests.E2E.Infrastructure.Settings
{
    /// <summary>
    /// Application under test (AUT) settings.
    /// </summary>
    public sealed class AutSettings
    {
        public Uri BaseUri { get; set; }
        
        public Uri ProbeUri { get; set; }
    }
}