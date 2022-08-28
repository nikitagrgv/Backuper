using System;
using System.IO;
using Backuper.ApplicationSettings;

namespace Backuper.FilesCopying
{
    internal class CopyingManager
    {
        public delegate void CopyEventHandler(string name);

        private readonly Settings _settings;

        public CopyingManager(Settings settings)
        {
            _settings = settings;

            try
            {
                Directory.CreateDirectory(_settings.getTargetDir());
            }
            catch
            {
                throw new TargetDirectoryCreatingException(_settings.getTargetDir());
            }
        }

        public event CopyEventHandler FileCopied;
        public event CopyEventHandler FileNotCopied;
        public event CopyEventHandler DirectoryCreated;
        public event CopyEventHandler DirectoryNotCreated;

        public void DoBackup()
        {
            foreach (var sourceDir in _settings.getSourceDirs())
                CopyAllFromDirectory(sourceDir, _settings.getTargetDir());
        }

        private void CopyAllFromDirectory(string sourcePath, string backupPath)
        {
            var targetPath = GetTargetPath(sourcePath, backupPath);

            CopyAllSubdirectories(sourcePath, targetPath);
            CopyAllFiles(sourcePath, targetPath);
        }

        private string GetTargetPath(string sourcePath, string backupPath)
        {
            var sourceDirName = new DirectoryInfo(sourcePath).Name;
            var targetPath = backupPath + "\\" + sourceDirName;
            return targetPath;
        }

        private void CopyAllSubdirectories(string sourcePath, string targetPath)
        {
            var subdirectories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);

            foreach (var directoryPath in subdirectories)
            {
                var newDirectory = directoryPath.Replace(sourcePath, targetPath);
                try
                {
                    if (!Directory.Exists(newDirectory))
                    {
                        Directory.CreateDirectory(newDirectory);
                        OnDirectoryCreated(newDirectory);
                    }
                }
                catch
                {
                    OnDirectoryNotCreated(newDirectory);
                }
            }
        }

        private void CopyAllFiles(string sourcePath, string targetPath)
        {
            var allFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);

            foreach (var filePath in allFiles)
            {
                var newFile = filePath.Replace(sourcePath, targetPath);
                try
                {
                    File.Copy(filePath, newFile, true);
                    OnFileCopied(filePath);
                }
                catch
                {
                    OnFileNotCopied(filePath);
                }
            }
        }

        private void OnFileCopied(string filename)
        {
            FileCopied?.Invoke(filename);
        }

        private void OnFileNotCopied(string filename)
        {
            FileNotCopied?.Invoke(filename);
        }

        private void OnDirectoryCreated(string dirName)
        {
            DirectoryCreated?.Invoke(dirName);
        }

        private void OnDirectoryNotCreated(string dirName)
        {
            DirectoryNotCreated?.Invoke(dirName);
        }
    }

    public class TargetDirectoryCreatingException : ApplicationException
    {
        private readonly string _targetDir;

        public TargetDirectoryCreatingException(string targetDir)
            : base("Failed to create target directory")
        {
            _targetDir = targetDir;
        }

        public string getTargetDir()
        {
            return _targetDir;
        }
    }
}