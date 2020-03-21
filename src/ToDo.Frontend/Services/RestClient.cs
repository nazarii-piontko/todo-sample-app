using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Components;
using ToDo.Backend.DTO;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.Services
{
    public sealed class RestClient
    {   
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
        
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IAuthDataStorage _authDataStorage;

        public RestClient(HttpClient httpClient,
            NavigationManager navigationManager,
            IAuthDataStorage authDataStorage)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _navigationManager = navigationManager;
            _authDataStorage = authDataStorage;
        }

        public Task<TResponse> GetAsync<TResponse>(string path,
            IEnumerable<RequestParam> queryParameters = null,
            IEnumerable<RequestParam> urlSegmentParameters = null,
            IEnumerable<RequestParam> headers = null,
            CancellationToken cancellationToken = default)
            where TResponse : class
        {
            return RequestAsync<object, TResponse>(HttpMethod.Get,
                path,
                queryParameters: queryParameters,
                urlSegmentParameters: urlSegmentParameters,
                headers: headers,
                request: null,
                cancellationToken: cancellationToken);
        }

        public Task<TResponse> PostAsync<TRequest, TResponse>(string path,
            IEnumerable<RequestParam> queryParameters = null,
            IEnumerable<RequestParam> urlSegmentParameters = null,
            IEnumerable<RequestParam> headers = null,
            TRequest request = null,
            CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : class
        {
            return RequestAsync<TRequest, TResponse>(HttpMethod.Post,
                path,
                queryParameters: queryParameters,
                urlSegmentParameters: urlSegmentParameters,
                headers: headers,
                request: request,
                cancellationToken: cancellationToken);
        }
        
        public Task<TResponse> PutAsync<TRequest, TResponse>(string path,
            IEnumerable<RequestParam> queryParameters = null,
            IEnumerable<RequestParam> urlSegmentParameters = null,
            IEnumerable<RequestParam> headers = null,
            TRequest request = null,
            CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : class
        {
            return RequestAsync<TRequest, TResponse>(HttpMethod.Put,
                path,
                queryParameters: queryParameters,
                urlSegmentParameters: urlSegmentParameters,
                headers: headers,
                request: request,
                cancellationToken: cancellationToken);
        }
        
        public Task<TResponse> PatchAsync<TRequest, TResponse>(string path,
            IEnumerable<RequestParam> queryParameters = null,
            IEnumerable<RequestParam> urlSegmentParameters = null,
            IEnumerable<RequestParam> headers = null,
            TRequest request = null,
            CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : class
        {
            return RequestAsync<TRequest, TResponse>(HttpMethod.Patch,
                path,
                queryParameters: queryParameters,
                urlSegmentParameters: urlSegmentParameters,
                headers: headers,
                request: request,
                cancellationToken: cancellationToken);
        }
        
        public Task<TResponse> DeleteAsync<TResponse>(string path,
            IEnumerable<RequestParam> queryParameters = null,
            IEnumerable<RequestParam> urlSegmentParameters = null,
            IEnumerable<RequestParam> headers = null,
            CancellationToken cancellationToken = default)
            where TResponse : class
        {
            return RequestAsync<object, TResponse>(HttpMethod.Delete,
                path,
                queryParameters: queryParameters,
                urlSegmentParameters: urlSegmentParameters,
                headers: headers,
                request: null,
                cancellationToken: cancellationToken);
        }
        
        public Task<TResponse> DeleteAsync<TRequest, TResponse>(string path,
            IEnumerable<RequestParam> queryParameters = null,
            IEnumerable<RequestParam> urlSegmentParameters = null,
            IEnumerable<RequestParam> headers = null,
            TRequest request = null,
            CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : class
        {
            return RequestAsync<TRequest, TResponse>(HttpMethod.Delete,
                path,
                queryParameters: queryParameters,
                urlSegmentParameters: urlSegmentParameters,
                headers: headers,
                request: request,
                cancellationToken: cancellationToken);
        }

        private async Task<TResponse> RequestAsync<TRequest, TResponse>(HttpMethod method,
            string path,
            IEnumerable<RequestParam> queryParameters = null,
            IEnumerable<RequestParam> urlSegmentParameters = null,
            IEnumerable<RequestParam> headers = null,
            TRequest request = null,
            CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : class
        {
            var uri = BuildFinalUri(path, queryParameters, urlSegmentParameters);

            using var reqMessage = await CreateRequestMessageAsync(method, uri, headers)
                .ConfigureAwait(false);

            if (request != null)
                AddRequestBody(reqMessage, request);
            
            var respMessage = await _httpClient.SendAsync(reqMessage, cancellationToken)
                .ConfigureAwait(false);

            await HandleFailIfNeedAsync(respMessage)
                .ConfigureAwait(false);

            var response = await ReadResponseAsObjectAsync<TResponse>(respMessage)
                .ConfigureAwait(false);

            return response;
        }

        private static void AddRequestBody<TRequest>(HttpRequestMessage reqMessage, TRequest requestBody) 
            where TRequest : class
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(requestBody);

            reqMessage.Content = new ByteArrayContent(json);
            reqMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        private static async Task<TResponse> ReadResponseAsObjectAsync<TResponse>(HttpResponseMessage respMessage)
            where TResponse : class
        {
            if (respMessage.StatusCode == HttpStatusCode.NoContent)
                return default;
            
            var json = await respMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            
            var response = JsonSerializer.Deserialize<TResponse>(json, Options);
            
            return response;
        }

        private async Task HandleFailIfNeedAsync(HttpResponseMessage respMessage)
        {
            if (respMessage.IsSuccessStatusCode)
                return;
            
            switch (respMessage.StatusCode)
            {
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.TooManyRequests:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.InsufficientStorage:
                    respMessage.EnsureSuccessStatusCode();
                    break;
                default:
                {
                    var mediaType = respMessage.Content?.Headers?.ContentType?.MediaType;
                    if (mediaType?.Equals("application/json", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        var error = await ReadResponseAsObjectAsync<ErrorResponse>(respMessage)
                            .ConfigureAwait(false);
                        
                        throw new Exception($"Service request failed: {string.Join(',', error.Errors)}");
                    }

                    if (respMessage.Content != null)
                    {
                        var content = await respMessage.Content.ReadAsStringAsync()
                            .ConfigureAwait(false);
                        
                        if (!string.IsNullOrEmpty(content))
                            throw new Exception($"Service request failed: {content}");
                    }

                    throw new Exception($"Service request failed: {respMessage.StatusCode}");
                }
            }
        }

        private Uri BuildFinalUri(string path,
            IEnumerable<RequestParam> queryParameters,
            IEnumerable<RequestParam> urlSegmentParameters)
        {
            var relativeUriBuilder = new StringBuilder(path);
            
            if (urlSegmentParameters != null)
            {
                foreach (var parameter in urlSegmentParameters)
                {
                    relativeUriBuilder.Replace("{" + parameter.Name + "}",
                        HttpUtility.UrlEncode(parameter.Value?.ToString() ?? string.Empty));
                }
            }

            if (queryParameters != null)
            {
                var isFirstParam = true;
                foreach (var parameter in queryParameters)
                {
                    relativeUriBuilder.Append(isFirstParam ? '?' : '&');
                    isFirstParam = false;
                    
                    relativeUriBuilder.Append(HttpUtility.UrlEncode(parameter.Name));
                    relativeUriBuilder.Append('=');
                    relativeUriBuilder.Append(HttpUtility.UrlEncode(parameter.Value?.ToString() ?? string.Empty));
                }
            }
            
            if (!Uri.TryCreate(new Uri(_navigationManager.BaseUri), relativeUriBuilder.ToString(), out var uri))
                throw new Exception($@"Unable to builder service request URI, BaseURI: '{_navigationManager.BaseUri}', RelativeURI: '{relativeUriBuilder}'");

            return uri;
        }

        private async Task<HttpRequestMessage> CreateRequestMessageAsync(HttpMethod method, 
            Uri uri,
            IEnumerable<RequestParam> headers)
        {
            var reqMessage = new HttpRequestMessage(method, uri);

            await AddAuthorizationDataAsync(reqMessage)
                .ConfigureAwait(false);

            if (headers != null)
            {
                foreach (var param in headers)
                    reqMessage.Headers.TryAddWithoutValidation(param.Name, param.Value?.ToString() ?? string.Empty);
            }
            
            return reqMessage;
        }

        private async Task AddAuthorizationDataAsync(HttpRequestMessage reqMessage)
        {
            var token = await _authDataStorage.GetTokenAsync()
                .ConfigureAwait(false);

            if (!string.IsNullOrEmpty(token))
            {
                var bearer = "Bearer " + token;

                reqMessage.Headers.TryAddWithoutValidation("Authorization", bearer);
            }
        }
    }
}