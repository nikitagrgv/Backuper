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
            Directory.CreateDirectory(_settings.GetTargetDir());
        }
        catch
        {
            throw new TargetDirNotCreatedException(_settings.GetTargetDir());
        }
    }

    public event CopyEventHandler FileCopied;
    public event CopyEventHandler FileNotCopied;
    public event CopyEventHandler DirCreated;
    public event CopyEventHandler DirNotCreated;
    public event CopyEventHandler DirBackupFailed;

    public void DoBackup()
    {
        foreach (var sourceDir in _settings.GetSourceDirs())
        {
            try
            {
                DoBackupDir(sourceDir, _settings.GetTargetDir());
            }
            catch
            {
                OnDirBackupFailed(sourceDir);
            }
        }
    }

    private void DoBackupDir(string sourcePath, string backupPath)
    {
        var targetPath = GetTargetPath(sourcePath, backupPath);

        CreateAllBackupSubdirs(sourcePath, targetPath);
        CopyAllFiles(sourcePath, targetPath);
    }

    private string GetTargetPath(string sourcePath, string backupPath)
    {
        var sourceDirName = new DirectoryInfo(sourcePath).Name;
        var targetPath = $"{backupPath}\\{_datestamp}\\{sourceDirName}";
        return targetPath;
    }

    private void CreateAllBackupSubdirs(string sourcePath, string targetPath)
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