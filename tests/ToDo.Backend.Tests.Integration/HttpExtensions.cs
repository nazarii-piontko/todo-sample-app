using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ToDo.Backend.Tests.Integration
{
    public static class HttpExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        
        public static HttpContent AsJsonContent(this object data)
        {
            var json = JsonSerializer.Serialize(data, JsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public static async Task<T> AsTypeAsync<T>(this HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<T>(json, JsonOptions);
            return data;
        }
    }
}