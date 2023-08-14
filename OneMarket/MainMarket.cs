using System;
using System.Collections.Generic;   // Provides interfaces and classes for working with collections like dictionary, list and others
using System.Windows.Forms; // Contains classes for creating Windows-based applications
using Newtonsoft.Json;  // Parse Json
using System.IO;    // Files operations

namespace OneMarket
{
    public static class GlobalVariables
    {
        public static string AlbionOnlineWestServerHost = "https://west.albion-online-data.com";   // West server Host URL
        public static string AlbionOnlineEastServerHost = "https://east.albion-online-data.com";   // East server Host URL
        public static string ApiRequestEntryPoint = "/api/v2"; // API enty point plus curent version
        public static string CurrentProjectDirectory;
        public static string LogsFolderPath;
        public static string CurrentPricesLogsPath;
        public static string CurrentPricesJsonLogsPath;
        public static int RepeatedHttpRequestFlag = 1;
    }
    public class MarketCurrentPirces
    {
        // Current Prices property list
        public string item_id { get; set; }
        public string city { get; set; }
        public int quality { get; set; }
        public int sell_price_min { get; set; }
        public DateTime sell_price_min_date { get; set; }
        public int sell_price_max { get; set; }
        public DateTime sell_price_max_date { get; set; }
        public int buy_price_min { get; set; }
        public DateTime buy_price_min_date { get; set; }
        public int buy_price_max { get; set; }
        public DateTime buy_price_max_date { get; set; }

        // Perform Json parising
        public static void ParseMarketCurrentPrices(string ResponseBody, string RequestId)
        {
            // Store Log of Json responses
            if (GlobalVariables.RepeatedHttpRequestFlag == 1) {
                // User StreamWriter in true mode to append data
                using (StreamWriter CurrentPricesJsonLogsWrite = new StreamWriter(GlobalVariables.CurrentPricesJsonLogsPath, true))
                {
                    CurrentPricesJsonLogsWrite.WriteLine($"{RequestId}: {ResponseBody}");
                }
            }
            // Declare a list to store multiple instances of MarketCurrentPrices class
            // Number of instances is defined by the size of given JSON array []
            // Console.WriteLine(ResponseBody);
            List<MarketCurrentPirces> MarketCurrentPircesJsonList = JsonConvert.DeserializeObject<List<MarketCurrentPirces>>(ResponseBody);
            // Print out every instance of the class stored in the created list
            foreach(MarketCurrentPirces RequestedObjectDetails in MarketCurrentPircesJsonList)
            {
                /* 
                 * Required request parameters (Prices):
                 * itemList -> define desired items ID
                 * locations -> define desired market location
                 * qualities -> define game item quality
                */
                Console.WriteLine($"Item ID: {RequestedObjectDetails.item_id}\n" +
                    $"City: {RequestedObjectDetails.city}\n" +
                    $"Quality: {RequestedObjectDetails.quality}\n" +
                    $"Sell price (min): {RequestedObjectDetails.sell_price_min}\n" +
                    $"Transaction date: {RequestedObjectDetails.sell_price_min_date}\n" +
                    $"Sell price (max): {RequestedObjectDetails.sell_price_max}\n" +
                    $"Transaction date: {RequestedObjectDetails.sell_price_max_date}\n" +
                    $"Buy price (min): {RequestedObjectDetails.buy_price_min}\n" +
                    $"Transaction date: {RequestedObjectDetails.buy_price_min_date}\n" +
                    $"Buy price (max): {RequestedObjectDetails.buy_price_max}\n" +
                    $"Transaction date: {RequestedObjectDetails.buy_price_max_date}");
            }
        }

    }

    internal static class MainMarket
    {
        //Please note that Windows Form UI flow is operating in STA mode by default
        // Set Main() to work in "one in time" mode using [STAThread] method
        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();   // Check OS style parameter to perform best visual experience
            Application.SetCompatibleTextRenderingDefault(false);   // Controls text renderin using GDI+ engine
            GlobalVariables.CurrentProjectDirectory = Environment.CurrentDirectory; // receive OneMarketProject\OneMarket\OneMarket\bin\Debug local path
            GlobalVariables.LogsFolderPath = Path.Combine(GlobalVariables.CurrentProjectDirectory, "Logs"); // Combine path for Logs folder
            GlobalVariables.CurrentPricesLogsPath = Path.Combine(GlobalVariables.LogsFolderPath, "CurrentPrices.txt");  // Combine path for current prices request logs
            GlobalVariables.CurrentPricesJsonLogsPath = Path.Combine(GlobalVariables.LogsFolderPath, "CurrentPricesJson.txt");   // Combine path for current prices Json responses logs
            Application.Run(new MainMarketDesigner());  // Run Form window loop
        }
    }
}
