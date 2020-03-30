using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ToDo.Backend.Tests.E2E.Infrastructure
{
    public static class Utils
    {
        public static Task ExecuteWithRetryAsync(Func<bool> action,
            TimeSpan? timeout = null,
            TimeSpan? retry = null)
        {
            return ExecuteWithRetryAsync(() =>
            {
                var success = action();
                return Task.FromResult(success);
            }, timeout, retry);
        }
        
        public static async Task ExecuteWithRetryAsync(Func<Task<bool>> action,
            TimeSpan? timeout = null,
            TimeSpan? retry = null)
        {
            if (retry == null)
                retry = TimeSpan.Zero;
            if (timeout == null)
                timeout = TimeSpan.Zero;
            
            var sw = new Stopwatch();
            sw.Start();

            try
            {
                while (true)
                {
                    var success = await action();

                    if (success)
                        break;

                    if (sw.Elapsed >= timeout.Value - retry.Value)
                        throw new TimeoutException();

                    await Task.Delay(retry.Value);
                }
            }
            finally
            {
                sw.Stop();
            }
        }

        public static async Task<bool> IsUriAccessibleAsync(this HttpClient client, Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            try
            {
                var response = await client.SendAsync(request);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        
        public static async Task RunCommandAsync(ICollection<string> command, TimeSpan timeout)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command.First(),
                UseShellExecute = true
            };
            foreach (var arg in command.Skip(1))
                startInfo.ArgumentList.Add(arg);
            
            var process = Process.Start(startInfo);
            
            if (process == null)
                throw new ArgumentException($"Unable to start '{string.Join(' ', command)}'");

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                
                var retry = TimeSpan.FromMilliseconds(100);
                
                while (true)
                {
                    process.Refresh();
                    
                    if (process.HasExited)
                    {
                        var exitCode = process.ExitCode;
                        
                        if (exitCode != 0) 
                            throw new Exception($"No-zero exit code of '{string.Join(' ', command)}': {exitCode}");

                        break;
                    }
                    else if (sw.Elapsed < timeout - retry)
                    {
                        await Task.Delay(retry);
                    }
                    else
                        throw new Exception($"Command run timeout: '{string.Join(' ', command)}'");
                }
            }
            finally
            {
                process.Dispose();
            }
        }
    }
}