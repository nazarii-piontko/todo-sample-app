using System;

namespace ToDo.Backend.Tests.E2E.Infrastructure.Settings
{
    public sealed class TestsSettings
    {
        public string ArtifactsDirectory { get; set; }
        
        /// <summary>
        /// Timeout for different operational tasks, e.g. start/stop application, start/stop selenium, ect.
        /// </summary>
        public TimeSpan OperationsTimeout { get; set; }
        
        /// <summary>
        /// Timeout for operations during simulation of user activities, e.g. click button, load page, etc.
        /// </summary>
        public TimeSpan RequestsTimeout { get; set; }
    }
}