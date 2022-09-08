using System;
using System.IO;
using Backuper.ApplicationSettings;

namespace Backuper.FilesCopying;

internal class CopyingManager
{
    public delegate void CopyEventHandler(string name);

    private readonly Settings _settings;
    private readonly string _datestamp = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss");


    public CopyingManager(Settings settings)
    {
        _settings = settings;

        try
        {
            Directory.CreateDirectory(_settings.GetTargetDirectory());
        }
        catch
        {
            throw new TargetDirectoryNotCreatedException(_settings.GetTargetDirectory());
        }
    }

    public event CopyEventHandler FileCopied;
    public event CopyEventHandler FileNotCopied;
    public event CopyEventHandler DirectoryCreated;
    public event CopyEventHandler DirectoryNotCreated;
    public event CopyEventHandler DirectoryBackupFailed;

    public void DoBackup()
    {
        foreach (var sourceDir in _settings.GetSourceDirectories())
        {
            try
            {
                DoBackupDirectory(sourceDir, _settings.GetTargetDirectory());
            }
            catch
            {
                OnDirectoryBackupFailed(sourceDir);
            }
        }
    }

    private void DoBackupDirectory(string sourcePath, string backupPath)
    {
        var targetPath = GetTargetPath(sourcePath, backupPath);

        CreateAllBackupSubdirectories(sourcePath, targetPath);
        CopyAllFiles(sourcePath, targetPath);
    }

    private string GetTargetPath(string sourcePath, string backupPath)
    {
        var sourceDirName = new DirectoryInfo(sourcePath).Name;
        var targetPath = $"{backupPath}\\{_datestamp}\\{sourceDirName}";
        return targetPath;
    }

    private void CreateAllBackupSubdirectories(string sourcePath, string targetPath)
    {
        var subdirectories = GetAllSubdirectories(sourcePath);

        foreach (var subdirectoryPath in subdirectories)
        {
            var newSubdirectoryPath = subdirectoryPath.Replace(sourcePath, targetPath);

            try
            {
                CreateDirectory(newSubdirectoryPath);
            }
            catch
            {
                OnDirectoryNotCreated(newSubdirectoryPath);
            }
        }
    }

    private static string[] GetAllSubdirectories(string sourcePath)
    {
        return Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
    }

    private void CreateDirectory(string newSubdirectoryPath)
    {
        if (Directory.Exists(newSubdirectoryPath)) return;

        Directory.CreateDirectory(newSubdirectoryPath);
        OnDirectoryCreated(newSubdirectoryPath);
    }

    private void CopyAllFiles(string sourcePath, string targetPath)
    {
        var allFiles = GetAllFilesInDirectory(sourcePath);

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

    private static string[] GetAllFilesInDirectory(string sourcePath)
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

    private void OnDirectoryCreated(string dirName)
    {
        DirectoryCreated?.Invoke(dirName);
    }

    private void OnDirectoryNotCreated(string dirName)
    {
        DirectoryNotCreated?.Invoke(dirName);
    }

    private void OnDirectoryBackupFailed(string dirName)
    {
        DirectoryBackupFailed?.Invoke(dirName);
    }
}

public class TargetDirectoryNotCreatedException : ApplicationException
{
    private readonly string _targetDirectory;

    public TargetDirectoryNotCreatedException(string targetDirectory)
        : base("Failed to create target directory")
    {
        _targetDirectory = targetDirectory;
    }

    public string GetTargetDir()
    {
        return _targetDirectory;
    }
}