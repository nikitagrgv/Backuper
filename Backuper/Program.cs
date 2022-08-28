using System;
using System.IO;
using Backuper.ApplicationSettings;
using Backuper.FilesCopying;

namespace Backuper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Settings settings = new Settings("settings.json");

                Console.WriteLine(settings.GetInfo());

                var copyingManager = new CopyingManager(settings);

                copyingManager.FileCopied += (file) => Console.WriteLine($"File copied: {file}");
                copyingManager.FileNotCopied += (file) => Console.WriteLine($"File not copied: {file}");
                copyingManager.DirectoryCreated += (dir) => Console.WriteLine($"Directory created: {dir}");
                copyingManager.DirectoryNotCreated += (dir) => Console.WriteLine($"Directory not created: {dir}");

                copyingManager.DoBackup();

                Console.WriteLine("Done");

            }
            catch (SettingsFileException e)
            {
                Console.WriteLine($"{e.Message} ({e.GetSettingsFileName()})");

                var exampleFile = "example.json";
                Console.WriteLine($"See example in {exampleFile}");
                Settings.GenerateExampleJsonFile(exampleFile);

                Console.ReadKey();
                return;
            }
            catch (TargetDirectoryCreatingException e)
            {
                Console.WriteLine($"{e.Message} ({e.getTargetDir()})");

                Console.ReadKey();
                return;
            }
            
        }
    }
}