using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Widgets.Firework.Domain.Api;
using Nop.Services.Logging;

namespace Nop.Plugin.Widgets.Firework.Services
{
    /// <summary>
    /// Represents HTTP client to request third-party services
    /// </summary>
    public class FireworkHttpClient
    {
        #region Fields

        private readonly FireworkSettings _fireworkSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public FireworkHttpClient(FireworkSettings fireworkSettings, HttpClient httpClient, ILogger logger)
        {
            httpClient.Timeout = TimeSpan.FromSeconds(fireworkSettings.RequestTimeout ?? FireworkDefaults.RequestTimeout);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, FireworkDefaults.UserAgent);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

            _fireworkSettings = fireworkSettings;
            _httpClient = httpClient;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Request services
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">Request</param>
        /// <returns>The asynchronous task whose result contains response details</returns>
        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) where TRequest : IApiRequest where TResponse : IApiResponse
        {
            //prepare request parameters
            var requestString = request is IBodiedRequest bodiedRequest ? bodiedRequest.Body : JsonConvert.SerializeObject(request);

            if (_fireworkSettings.LogDebugInfo)
                await _logger.InsertLogAsync(LogLevel.Debug, "Firework request " + request.Path, requestString);

            var requestContent = new StringContent(requestString,
                Encoding.UTF8,
                request is IBodiedRequest ? MimeTypes.TextPlain : MimeTypes.ApplicationJson);
            var baseUrl = new Uri(_fireworkSettings.UseSandbox ? FireworkDefaults.SandboxApiUrl : FireworkDefaults.ApiUrl); 
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(baseUrl, request.Path))
            {
                Content = requestContent
            };

            //add authorization
            if (request is IAuthorizedRequest authorized)
                requestMessage.Headers.Add(HeaderNames.Authorization, $"Bearer {authorized.Token}");

            //add headers for webhook requests
            if (request is IWebhookRequest webhook)
            {
                requestMessage.Headers.Add(FireworkDefaults.HeaderContentSignature, webhook.ContentSignature);
                requestMessage.Headers.Add(FireworkDefaults.HeaderWebhookEvent, webhook.EventType);
                requestMessage.Headers.Add(FireworkDefaults.HeaderTimestamp, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString());
            }

            //execute request and get result
            var httpResponse = await _httpClient.SendAsync(requestMessage);
            var responseString = await httpResponse.Content.ReadAsStringAsync();

            if (_fireworkSettings.LogDebugInfo)
                await _logger.InsertLogAsync(LogLevel.Debug, "Firework response", responseString);

            try
            {
                var result = JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty);
                if (!string.IsNullOrEmpty(result?.Error))
                    throw new NopException($"Request error: {result.Error}");

                return result;
            }
            catch
            {
                throw new NopException($"Request error: {responseString}");
            }
        }

        #endregion
    }
}