using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net.Http;  // Makes posible to perform Http requests
using System.Net.Http.Headers;  // Provides Http related headers

namespace OneMarket
{
    // partial - class is defined in multiple files
    public partial class MainMarketDesigner : Form
    {
        // Initialize Form application
        public MainMarketDesigner()
        {
            InitializeComponent();  // Form application config setup
        }

        // API Endpoint Rate Limits:
        // 180 per 1 minute
        // 300 per 5 minutes - timer default value
        private void ClearAppHistory_Tick(object sender, EventArgs e)
        {
            // Extranct all log files from relates LogsFolderPath folder
            string[] LogsFolderPathFolderCollection = Directory.GetFiles(GlobalVariables.LogsFolderPath);
            // Prvent error to occure if there is not log files available in the related directory
            if (LogsFolderPathFolderCollection.Length > 0){

                // Clear all available Log files
                foreach (string SingleLogFile in LogsFolderPathFolderCollection)
                {
                    try
                    {
                        File.WriteAllText(SingleLogFile, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error clearing file '{SingleLogFile}': {ex.Message}");
                    }

                }

            }

        }

        // Init customer data request procedure
        private void button1_Click(object sender, EventArgs e)
        {
            string RequestId = "";  // Unique ID generated with each log save, is required to identify a sertain log in a .txt file
            string RepeatedHttpRequestId = "";  // Extracted Http request unique ID from log file
            string RepeatedJsonAnswer = ""; // Storage for repeated Json answer from log file

            // No need to waste timer resources, init timer by first request
            if (!ClearAppHistory.Enabled)
            {
                // Enable timer event with preset value 5 min
                ClearAppHistory.Enabled = true;
            }

            Thread AskHttpClient = new Thread(async () =>
            {
                // Provide Http request URL <-- West server, prices, json format
                string TestFullUrl = (string)(GlobalVariables.AlbionOnlineWestServerHost +
                                              GlobalVariables.ApiRequestEntryPoint +
                                              "/stats/prices/T3_BAG,T5_BAG?locations=Caerleon,Bridgewatch&qualities=2");
                // Check existance of the sertain API request subject Log file
                // Some logs were already saved, check for repeated requests not to overflow server
                if (File.Exists(GlobalVariables.CurrentPricesLogsPath))
                {
                    // Enable stream reading to parse current prices logs file
                    using (StreamReader CurrentPricesLogsRead = new StreamReader(GlobalVariables.CurrentPricesLogsPath))
                    {
                        string CurrentPricesLogsReadLine;   // variable to store each line of log in the while loop
                        while ((CurrentPricesLogsReadLine = CurrentPricesLogsRead.ReadLine()) != null)
                        {
                            // Used Regex to identify right URL in the logs file
                            if (CurrentPricesLogsReadLine.EndsWith($": {TestFullUrl}"))
                            {
                                int ColonIndex = CurrentPricesLogsReadLine.IndexOf(':');    // define index of symbol after URL ID in log file
                                RepeatedHttpRequestId = CurrentPricesLogsReadLine.Substring(0, ColonIndex); // Extract portion of data before ':'
                                GlobalVariables.RepeatedHttpRequestFlag = 0;    // Http request was already made once in a five minutes
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // If it is a first request and first log app should create related folders and file to store them
                    Directory.CreateDirectory(GlobalVariables.LogsFolderPath);
                    File.Create(GlobalVariables.CurrentPricesLogsPath).Dispose();   // Release resources related to File stream by .Dispose()
                    File.Create(GlobalVariables.CurrentPricesJsonLogsPath).Dispose();
                }
                switch (GlobalVariables.RepeatedHttpRequestFlag)
                {
                    // Repeated request detected in Log files
                    case 0:
                        // Search for Json answer in log by unique ID
                        using (StreamReader CurrentPricesJsonLogsRead = new StreamReader(GlobalVariables.CurrentPricesJsonLogsPath))
                        {
                            string CurrentPricesJsonLogsReadLine;
                            while ((CurrentPricesJsonLogsReadLine = CurrentPricesJsonLogsRead.ReadLine()) != null)
                            {
                                // If unique ID was detected
                                if (CurrentPricesJsonLogsReadLine.StartsWith($"{RepeatedHttpRequestId}: "))
                                {
                                    int ColonIndex = CurrentPricesJsonLogsReadLine.IndexOf(':');    // Détect separating symbol
                                    RepeatedJsonAnswer = CurrentPricesJsonLogsReadLine.Substring(ColonIndex + 1);   // Extract data after free space
                                    break;
                                }

                                // TODO - same ID or mistake detected

                            }
                        }
                        MarketCurrentPirces.ParseMarketCurrentPrices(RepeatedJsonAnswer, RequestId);    // Start parsing log data, save resources of server
                        GlobalVariables.RepeatedHttpRequestFlag = 1;
                        Console.WriteLine("Test");
                        break;
                    // Repeated request was not detected, new query
                    case 1:

                        using (StreamWriter CurrentPricesLogsWrite = new StreamWriter(GlobalVariables.CurrentPricesLogsPath, true))
                        {
                            RequestId = Guid.NewGuid().ToString();  // Generate unique ID to identify certain Http request
                            CurrentPricesLogsWrite.WriteLine($"{RequestId}: {TestFullUrl}");
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
                                // Set Client request parameters, eneblse server Gzip compression
                                Client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                                try
                                {
                                    // Receive Http response from server
                                    HttpResponseMessage response = await Client.GetAsync(TestFullUrl);
                                    // Throw Http exeption in case of server error
                                    response.EnsureSuccessStatusCode();
                                    // Extract response content
                                    string ResponseBody = await response.Content.ReadAsStringAsync();
                                    // Parse received Json data
                                    MarketCurrentPirces.ParseMarketCurrentPrices(ResponseBody, RequestId);
                                }
                                catch (HttpRequestException ex)
                                {
                                    // Define outputing an Http exeption message
                                    Console.WriteLine($"An exception occured: {ex.Message}");
                                }

                            }   // Client dispose here
                        }   // Handler dispose here
                        break;
                }
            });
            AskHttpClient.Start();  // Separate MainMarket.cs Thread from MainMarketDesigner.cs Thread
        }
    }
}
