
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
           // Application settings
            Application.EnableVisualStyles();   // Check OS style parameter to perform best visual experience
            Application.SetCompatibleTextRenderingDefault(false);   // Controls text renderin using GDI+ engine
            
            
            // Define global variables
            GlobalVariables.CurrentProjectDirectory = Environment.CurrentDirectory; // receive OneMarketProject\OneMarket\OneMarket\bin\Debug local path
            GlobalVariables.LogsFolderPath = Path.Combine(GlobalVariables.CurrentProjectDirectory, "Logs"); // Combine path for Logs folder
            GlobalVariables.CurrentPricesLogsPath = Path.Combine(GlobalVariables.LogsFolderPath, "CurrentPrices.txt");  // Combine path for current prices request logs
            GlobalVariables.CurrentPricesJsonLogsPath = Path.Combine(GlobalVariables.LogsFolderPath, "CurrentPricesJson.txt");   // Combine path for current prices Json responses logs
            Directory.CreateDirectory(GlobalVariables.LogsFolderPath);  // Create folder for log files
            
            
            // Check log files expiration date
            // Extranct all log files from relates LogsFolderPath folder
            string[] LogsFolderPathFolderCollectionStartup = Directory.GetFiles(GlobalVariables.LogsFolderPath);
            // Prvent error to occure if there is not log files available in the related directory
            if (LogsFolderPathFolderCollectionStartup.Length > 0)
            {
                DateTime LogLastWriteTime;
                DateTime CurrentTime;
                TimeSpan LastWriteCurretTimeDifference;
                TimeSpan DefineFiveMinutes = TimeSpan.FromMinutes(5);
                // Check all available log files expiration date
                foreach (string SingleLogFileStartup in LogsFolderPathFolderCollectionStartup)
                {
                    LogLastWriteTime = File.GetLastWriteTime(SingleLogFileStartup);
                    CurrentTime = DateTime.Now;
                    LastWriteCurretTimeDifference = (DateTime.Now - LogLastWriteTime);
                    // If log is older then 5 minutes, API limitations expired and same data might be requested once more
                    // Cleare log to save updated data
                    if (LastWriteCurretTimeDifference > DefineFiveMinutes)
                    {
                        try
                        {
                            File.WriteAllText(SingleLogFileStartup, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);    // Message for user
                            Console.WriteLine(ex.ToString());   // Console message for developer
                        }
                    }
                }
            }
            Application.Run(new MainMarketDesigner());  // Run Form window loop
        }
    }
}
