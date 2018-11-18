using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSample
{
    public class TtsAuthorizationHandler : DelegatingHandler
    {
        private readonly HttpClient _authClient;

        public TtsAuthorizationHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            var subscriptionKey = configuration["TTS:SubscriptionKey"];

            _authClient = httpClientFactory.CreateClient("CognitiveServicesAuthorization");
            _authClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await FetchTokenAsync();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
        
        private async Task<string> FetchTokenAsync()
        {
            var result = await _authClient.PostAsync("", null);

            result.EnsureSuccessStatusCode();

            return await result.Content.ReadAsStringAsync();
        }
    }
}