using System;

namespace MSSQL_Database_Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            string exportLocation = $@"C:\Temp\DBBackup\{DateTime.Now.ToString("yyyy-MM-dd")}";
            var databases = new string[] { "AviciiDev", "WaitingForLoveDev", "GuessNoChurchSundayDev", "ToComeAroundDev" };

            foreach(var db in databases)
            {
                string dbPath = exportLocation + "\\" + db;

                Console.WriteLine($"Backing up {db} to {dbPath}...");

                var result = Core.Services.DatabaseService.DownloadDatabase(db, dbPath);

                if (result)
                {
                    Console.WriteLine("Backup complete!");
                }
                else
                {
                    Console.WriteLine("An error occured!");
                }
            }

            Console.WriteLine("Program complete! Exiting...");
        }
    }
}
