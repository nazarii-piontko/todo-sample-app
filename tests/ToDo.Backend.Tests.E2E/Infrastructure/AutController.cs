using System.Net.Http;
using System.Threading.Tasks;
using ToDo.Backend.Tests.E2E.Infrastructure.Settings;

namespace ToDo.Backend.Tests.E2E.Infrastructure
{
    public sealed class AutController
    {
        private readonly TestsEnvSettings _settings;
        private readonly HttpClient _httpClient;

        public AutController(TestsEnvSettings settings,
            HttpClient httpClient)
        {
            _settings = settings;
            _httpClient = httpClient;
        }

        public async Task StartAsync()
        {
            var isRunning = await IsRunningAsync();
            if (isRunning)
                return;
                
            await Utils.RunCommandAsync(_settings.AutDeploy.StartCommand,
                _settings.Tests.OperationsTimeout);
            
            await Utils.ExecuteWithRetryAsync(
                IsRunningAsync,
                timeout: _settings.Tests.OperationsTimeout);
        }

        public async Task StopAsync()
        {
            var isRunning = await IsRunningAsync();
            if (!isRunning)
                return;

            await Utils.RunCommandAsync(_settings.AutDeploy.StopCommand,
                _settings.Tests.OperationsTimeout);

            await Utils.ExecuteWithRetryAsync(
                async () => !await IsRunningAsync(),
                _settings.Tests.OperationsTimeout);
        }

        public async Task<bool> IsRunningAsync()
        {
            var isRunning = await _httpClient.IsUriAccessibleAsync(_settings.Aut.ProbeUri);
            return isRunning;
        }
    }
}