using System;
using System.IO;
using Backuper.ApplicationSettings;

namespace Backuper.FilesCopying
{
    internal class BackupManager
    {
        public delegate void CopyEventHandler(string name);

        private readonly string _currDateString;
        private readonly SettingsManager _settingsManager;

        public BackupManager(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _currDateString = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss");

            try
            {
                Directory.CreateDirectory(_settingsManager.GetTargetDir());
            }
            catch
            {
                throw new TargetDirNotCreatedException(_settingsManager.GetTargetDir());
            }
        }

        public event CopyEventHandler FileCopied;
        public event CopyEventHandler FileNotCopied;
        public event CopyEventHandler DirCreated;
        public event CopyEventHandler DirNotCreated;
        public event CopyEventHandler DirBackupFailed;

        public void DoBackup()
        {
            foreach (var sourceDir in _settingsManager.GetSourceDirs())
                try
                {
                    DoBackupDir(sourceDir, _settingsManager.GetTargetDir());
                }
                catch
                {
                    OnDirBackupFailed(sourceDir);
                }
        }

        private void DoBackupDir(string sourcePath, string backupPath)
        {
            var targetPath = GetTargetPath(sourcePath, backupPath);

            CreateAllSubdirs(sourcePath, targetPath);
            CopyAllFiles(sourcePath, targetPath);
        }

        private string GetTargetPath(string sourcePath, string backupPath)
        {
            var sourceDirName = new DirectoryInfo(sourcePath).Name;
            return $"{backupPath}\\{_currDateString}\\{sourceDirName}";
        }

        private void CreateAllSubdirs(string sourcePath, string targetPath)
        {
            var subdirs = GetAllSubdirs(sourcePath);

            foreach (var subdirPath in subdirs)
            {
                var newSubdirPath = subdirPath.Replace(sourcePath, targetPath);

                try
                {
                    CreateDir(newSubdirPath);
                }
                catch
                {
                    OnDirNotCreated(newSubdirPath);
                }
            }
        }

        private static string[] GetAllSubdirs(string sourcePath)
        {
            return Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
        }

        private void CreateDir(string newSubdirPath)
        {
            if (Directory.Exists(newSubdirPath)) return;

            Directory.CreateDirectory(newSubdirPath);
            OnDirCreated(newSubdirPath);
        }

        private void CopyAllFiles(string sourcePath, string targetPath)
        {
            var allFiles = GetAllFilesInDir(sourcePath);

            foreach (var filePath in allFiles)
            {
                var newFilePath = filePath.Replace(sourcePath, targetPath);
                try
                {
                    CopyFile(filePath, newFilePath);
                }
                catch
                {
                    OnFileNotCopied(filePath);
                }
            }
        }

        private static string[] GetAllFilesInDir(string sourcePath)
        {
            return Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
        }

        private void CopyFile(string filePath, string newFilePath)
        {
            File.Copy(filePath, newFilePath, true);
            OnFileCopied(filePath);
        }

        private void OnFileCopied(string filename)
        {
            FileCopied?.Invoke(filename);
        }

        private void OnFileNotCopied(string filename)
        {
            FileNotCopied?.Invoke(filename);
        }

        private void OnDirCreated(string dirName)
        {
            DirCreated?.Invoke(dirName);
        }

        private void OnDirNotCreated(string dirName)
        {
            DirNotCreated?.Invoke(dirName);
        }

        private void OnDirBackupFailed(string dirName)
        {
            DirBackupFailed?.Invoke(dirName);
        }
    }

    public class TargetDirNotCreatedException : ApplicationException
    {
        private readonly string _targetDir;

        public TargetDirNotCreatedException(string targetDir)
            : base("Failed to create target directory")
        {
            _targetDir = targetDir;
        }

        public string GetTargetDir()
        {
            return _targetDir;
        }
    }
}