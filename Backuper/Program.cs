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
                var settingsManager = new SettingsManager("settings.json");

                Console.WriteLine(settingsManager.GetSettingsInfo());

                var backupManager = new BackupManager(settingsManager);

                backupManager.FileCopied += OnFileCopied;
                backupManager.FileNotCopied += OnFileNotCopied;
                backupManager.DirCreated += OnDirCreated;
                backupManager.DirNotCreated += OnDirNotCreated;
                backupManager.DirBackupFailed += OnDirBackupFailed;

                backupManager.DoBackup();

                Console.WriteLine("Done");
                Console.ReadKey();
            }
            catch (SettingsFileException e)
            {
                Console.WriteLine($"{e.Message} ({e.GetSettingsFilename()})");

                var exampleFile = "example.json";
                Console.WriteLine($"See example in {exampleFile}");
                SettingsManager.CreateExampleJsonFile(exampleFile);

                Console.ReadKey();
            }
            catch (TargetDirNotCreatedException e)
            {
                Console.WriteLine($"{e.Message} ({e.GetTargetDir()})");

                Console.ReadKey();
            }
        }

        private static void OnDirNotCreated(string dir)
        {
            Console.WriteLine($"Directory not created: {dir}");
        }

        private static void OnDirCreated(string dir)
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

        private static void OnDirBackupFailed(string dir)
        {
            Console.WriteLine($"Directory backup failed: {dir}");
        }
    }
}