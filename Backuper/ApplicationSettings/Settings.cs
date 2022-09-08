using System.IO;
using System.Text;
using System.Text.Json;

namespace Backuper.ApplicationSettings;

internal class Settings
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() {WriteIndented = true};

    private readonly string _settingsFilename;
    private RawSettings _rawSettings;

    public Settings(string settingsFilename)
    {
        try
        {
            _settingsFilename = settingsFilename;
            DeserializeSettings();
            ValidateSettings();
        }
        catch (JsonException)
        {
            throw new BadSettingsFileException(settingsFilename);
        }
        catch (FileNotFoundException)
        {
            throw new NotFoundSettingsFileException(settingsFilename);
        }
    }

    private void DeserializeSettings()
    {
        var jsonString = File.ReadAllText(_settingsFilename);
        _rawSettings = JsonSerializer.Deserialize<RawSettings>(jsonString);
    }

    private void ValidateSettings()
    {
        if (_rawSettings.TargetDir == null ||
            _rawSettings.SourceDirs == null)
            throw new BadSettingsFileException(_settingsFilename);
    }

    public string[] GetSourceDirs()
    {
        return _rawSettings.SourceDirs;
    }

    public string GetTargetDir()
    {
        return _rawSettings.TargetDir;
    }

    public string GetInfo()
    {
        var info = new StringBuilder();

        info.AppendLine($"Target directory: {_rawSettings.TargetDir}");
        info.AppendLine($"Source directories [{_rawSettings.SourceDirs.Length.ToString()}]:");

        foreach (var sourceDir in _rawSettings.SourceDirs)
            info.AppendLine($"  {sourceDir}");

        return info.ToString();
    }

    public static void CreateExampleJsonFile(string exampleJsonFilename)
    {
        var exampleJsonString = JsonSerializer.Serialize(GetExampleRawSettings(), JsonSerializerOptions);
        File.WriteAllText(exampleJsonFilename, exampleJsonString);
    }

    private static RawSettings GetExampleRawSettings()
    {
        return new RawSettings
        {
            TargetDir = "D:\\TEST\\TO",
            SourceDirs = new[] {"D:\\TEST\\FROM1", "D:\\TEST\\FROM2"}
        };
    }
}