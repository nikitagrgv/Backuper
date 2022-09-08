using System;
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
                var settings = new Settings("settings.json");

                Console.WriteLine(settings.GetInfo());

                var copyingManager = new CopyingManager(settings);

                copyingManager.FileCopied += OnFileCopied;
                copyingManager.FileNotCopied += OnFileNotCopied;
                copyingManager.DirectoryCreated += OnDirectoryCreated;
                copyingManager.DirectoryNotCreated += OnDirectoryNotCreated;
                copyingManager.DirectoryBackupFailed += OnDirectoryBackupFailed;

                copyingManager.DoBackup();

                Console.WriteLine("Done");
                Console.ReadKey();
            }
            catch (SettingsFileException e)
            {
                Console.WriteLine($"{e.Message} ({e.GetSettingsFilename()})");

                var exampleFile = "example.json";
                Console.WriteLine($"See example in {exampleFile}");
                Settings.CreateExampleJsonFile(exampleFile);

                Console.ReadKey();
            }
            catch (TargetDirectoryNotCreatedException e)
            {
                Console.WriteLine($"{e.Message} ({e.GetTargetDir()})");

                Console.ReadKey();
            }
        }
        
        private static void OnDirectoryNotCreated(string dir)
        {
            Console.WriteLine($"Directory not created: {dir}");
        }

        private static void OnDirectoryCreated(string dir)
        {
            Console.WriteLine($"Directory created: {dir}");
        }

        private static void OnFileNotCopied(string file)
        {
            Console.WriteLine($"File not copied: {file}");
        }

        private static void OnFileCopied(string file)
        {
            Console.WriteLine($"File copied: {file}");
        }

        private static void OnDirectoryBackupFailed(string dir)
        {
            Console.WriteLine($"Directory backup failed: {dir}");
        }

    }
}