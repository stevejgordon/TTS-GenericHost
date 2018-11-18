using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

/*
 * IMPORTANT:
 * This sample requires C# 7.1 or later.
 * Requires the following Nuget Packages: 
 *      Microsoft.Extensions.Hosting, Version 2.1.1
 *      Microsoft.Extensions.Hosting.Abstractions, Version 2.1.1
 *      Microsoft.Extensions.Http, Version 2.1.1
 */

namespace AudioSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((ctx, cfg) => cfg.AddJsonFile("appsettings.json"))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<TtsAuthorizationHandler>();

                    services.AddHttpClient("TextToSpeech", client =>
                    {
                        client.BaseAddress = new Uri("https://westus.tts.speech.microsoft.com");
                        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                        client.DefaultRequestHeaders.Add("User-Agent", "erhopf-speech-test");
                    })
                    .AddHttpMessageHandler<TtsAuthorizationHandler>();

                    services.AddHttpClient("CognitiveServicesAuthorization", client =>
                    {
                        client.BaseAddress = new Uri("https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken");
                    });

                    services.AddSingleton<IHostedService, AudioService>();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}