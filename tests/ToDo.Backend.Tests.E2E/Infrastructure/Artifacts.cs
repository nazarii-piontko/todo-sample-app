using System.IO;
using ToDo.Backend.Tests.E2E.Infrastructure.Settings;

namespace ToDo.Backend.Tests.E2E.Infrastructure
{
    public sealed class Artifacts
    {
        private readonly TestsEnvSettings _settings;

        public Artifacts(TestsEnvSettings settings)
        {
            _settings = settings;
        }

        public string Directory => _settings.Tests.ArtifactsDirectory;

        public void Init()
        {
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);
        }

        public string GetPath(string name)
        {
            return Path.Combine(Directory, name);
        }
    }
}