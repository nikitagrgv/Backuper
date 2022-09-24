using System.IO;
using System.Text;
using System.Text.Json;

namespace Backuper.ApplicationSettings
{
    internal class SettingsManager
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() {WriteIndented = true};

        private readonly string _settingsFilename;
        private Settings _settings;

        public SettingsManager(string settingsFilename)
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
            _settings = JsonSerializer.Deserialize<Settings>(jsonString);
        }

        private void ValidateSettings()
        {
            if (_settings.TargetDir == null ||
                _settings.SourceDirs == null)
                throw new BadSettingsFileException(_settingsFilename);
        }

        public string[] GetSourceDirs()
        {
            return _settings.SourceDirs;
        }

        public string GetTargetDir()
        {
            return _settings.TargetDir;
        }

        public string GetSettingsInfo()
        {
            var info = new StringBuilder();

            info.AppendLine($"Target directory: {_settings.TargetDir}");
            info.AppendLine($"Source directories [{_settings.SourceDirs.Length.ToString()}]:");

            foreach (var sourceDir in _settings.SourceDirs)
                info.AppendLine($"  {sourceDir}");

            return info.ToString();
        }

        public static void CreateExampleJsonFile(string exampleJsonFilename)
        {
            var exampleJsonString = JsonSerializer.Serialize(Settings.GetExample(), JsonSerializerOptions);
            File.WriteAllText(exampleJsonFilename, exampleJsonString);
        }
    }
}