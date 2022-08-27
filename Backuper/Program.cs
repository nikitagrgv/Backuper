using System;
using Backuper.ApplicationSettings;

namespace Backuper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Settings settings;
            try
            {
                settings = new Settings("settings.json");
            }
            catch (SettingsException e)
            {
                Console.WriteLine($"{e.Message} ({e.GetFileName()})");

                string exampleFile = "example.json";
                Console.WriteLine($"See example in {exampleFile}");
                Settings.GenerateExampleJsonFile(exampleFile);

                Console.ReadKey();
                return;
            }

        }
    }
}