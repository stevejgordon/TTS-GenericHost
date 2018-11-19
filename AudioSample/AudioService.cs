using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AudioSample
{
    public class AudioService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AudioService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Provide the text to be spoken and saved to an audio file.");
            Console.Write("> ");

            var textToSpeak = Console.ReadLine();

            using (var textToSpeechClient = _httpClientFactory.CreateClient("TextToSpeech"))
            {
                const string path = "cognitiveservices/v1";

                // SSML + XML body for the request
                string body = $@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
                              <voice name='Microsoft Server Speech Text to Speech Voice (en-US, Jessa24kRUS)'>
                              {textToSpeak}
                              </voice></speak>";

                // Instantiate the request
                using (var request = new HttpRequestMessage(HttpMethod.Post, path))
                {
                    // Set the request content and media type
                    request.Content = new StringContent(body, Encoding.UTF8, "application/ssml+xml");

                    // Set output header
                    request.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");

                    try
                    {
                        // Create a request                   
                        using (var response = await textToSpeechClient.SendAsync(request))
                        {
                            Console.WriteLine($"Status code: {response.StatusCode}");

                            response.EnsureSuccessStatusCode();

                            // Asynchronously read the response
                            using (var dataStream = await response.Content.ReadAsStreamAsync())
                            {
                                /* Write the response to a file. In this sample,
                                 * it's an audio file. Then close the stream. */
                                using (var fileStream = new FileStream("sample.wav", FileMode.Create, FileAccess.Write, FileShare.Write))
                                {
                                    await dataStream.CopyToAsync(fileStream);
                                    fileStream.Close();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                if (new FileInfo("sample.wav").Length == 0)
                {
                    Console.WriteLine("The response is empty. Please check your request. Press any key to exit.");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Your speech file is ready for playback. Press any key to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}