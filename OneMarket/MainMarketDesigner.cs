using System;
using System.Windows.Forms;
using System.IO;
using AlbionMarketCurrentPrices;    // Market current prices component dll
using AlbionDataProjectHttpRequest; // Server request managing dll

namespace OneMarket
{
    // partial - class is defined in multiple files
    public partial class MainMarketDesigner : Form
    {
        // Initialize Form application
        public MainMarketDesigner()
        {
            InitializeComponent();  // Form application config setup
            ClearAppHistory.Enabled = true; // Enable timer event with preset value 5 min
        }

        // Define AP for AlbionMarketCurrentPrices component
        private IMarketCurrentPrices LocalInstanceOfMarketCurrentPricesComponent;
        private IDataProjectHttpRequest LocalInstanceOfDataProjectHttpRequestComponent;

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
        private async void button1_Click(object sender, EventArgs e)
        {
            string RepeatedHttpRequestId = "";  // Extracted Http request unique ID from log file
            string ResponseBody;   // Received Http request answer from Albion Project Data database

            // Provide Http request URL <-- West server, prices, json format
            string TestFullUrl = (string)(GlobalVariables.AlbionOnlineWestServerHost +
                GlobalVariables.ApiRequestEntryPoint +
                "/stats/prices/T3_BAG,T7_BAG?locations=Caerleon,Bridgewatch&qualities=2");
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

                    // Use AlbionMarketCurrentPrices dll to extract and parse Json data
                    LocalInstanceOfMarketCurrentPricesComponent = new MarketCurrentPrices();
                    LocalInstanceOfMarketCurrentPricesComponent.ExtractRepeatedJsonAnswerForCurrentPrices(RepeatedHttpRequestId, GlobalVariables.CurrentPricesJsonLogsPath);
                    GlobalVariables.RepeatedHttpRequestFlag = 1;
                    Console.WriteLine("LOG");
                    break;

                // Repeated request was not detected, new query
                case 1:

                    // Use AlbionDataProjectHttpRequest to perform Http request
                    LocalInstanceOfDataProjectHttpRequestComponent = new DataProjectHttpRequest();
                    ResponseBody = await LocalInstanceOfDataProjectHttpRequestComponent.PerformAlbionDataProjectHttpRequest(TestFullUrl, GlobalVariables.CurrentPricesLogsPath, GlobalVariables.CurrentPricesJsonLogsPath);
                    if (ResponseBody == null)
                    {
                        Console.WriteLine("Response body is null");
                        break;
                    }
                    // Use AlbionMarketCurrentPrices dll to parse Json data
                    LocalInstanceOfMarketCurrentPricesComponent = new MarketCurrentPrices();
                    LocalInstanceOfMarketCurrentPricesComponent.ParseMarketCurrentPrices(ResponseBody);
                    Console.WriteLine("HTTP");
                    break;
            }
        }
    }
}
