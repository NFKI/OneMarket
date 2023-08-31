using System;
using System.Windows.Forms; // Contains classes for creating Windows-based applications
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
