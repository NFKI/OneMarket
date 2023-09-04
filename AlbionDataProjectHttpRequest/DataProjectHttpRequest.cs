using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlbionDataProjectHttpRequest
{
    public class DataProjectHttpRequest : IDataProjectHttpRequest
    {
        public async Task<string> PerformAlbionDataProjectHttpRequest(string FormedHttpRequestFullUrl, string CurrentPricesLogsPath, string CurrentPricesJsonLogsPath)
        {
            string RequestId = "";  // Unique ID generated with each log save, is required to identify a sertain log in a .txt file
            string HttpRequestResponseBody = "";   // Value used for extracted content of Http request

            using (StreamWriter CurrentPricesLogsWrite = new StreamWriter(CurrentPricesLogsPath, true))
            {
                RequestId = Guid.NewGuid().ToString();  // Generate unique ID to identify certain Http request
                CurrentPricesLogsWrite.WriteLine($"{RequestId}: {FormedHttpRequestFullUrl}");
            }
            // Create instance of Http handler and configure automatic decompression to use GZip
            using (HttpClientHandler Handler = new HttpClientHandler
            {
                // Define Automatic decompression as Gzip
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
            })
            {
                // Create a new instance of Http client and pass the handler
                using (HttpClient Client = new HttpClient(Handler))
                {
                    // Set Client request parameters, eneble server Gzip compression
                    Client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    try
                    {
                        // Receive Http response from server
                        HttpResponseMessage response = await Client.GetAsync(FormedHttpRequestFullUrl);
                        // Throw Http exeption in case of server error
                        response.EnsureSuccessStatusCode();
                        // Extract response content
                        HttpRequestResponseBody = await response.Content.ReadAsStringAsync();
                        // Store Log of Json responses
                        using (StreamWriter CurrentPricesJsonLogsWrite = new StreamWriter(CurrentPricesJsonLogsPath, true))
                        {
                            CurrentPricesJsonLogsWrite.WriteLine($"{RequestId}: {HttpRequestResponseBody}");  // User StreamWriter in true mode to append data
                        }

                    }
                    catch (HttpRequestException ex)
                    {
                        // Define outputing an Http exeption message
                        Console.WriteLine($"An exception occured: {ex.Message}");
                    }

                }   // Client dispose here
            }   // Handler dispose here
            return HttpRequestResponseBody;

        }
        public void FormatAlbionDataProjectHttpRequest()
        {

        }

    }
}
