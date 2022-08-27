using System.IO;
using System.Text.Json;

namespace Backuper.ApplicationSettings
{
    internal class Settings
    {
        private readonly RawSettings _rawSettings;

        public Settings(string settingsFileName)
        {
            try
            {
                var jsonString = File.ReadAllText(settingsFileName);
                _rawSettings = JsonSerializer.Deserialize<RawSettings>(jsonString);
            }
            catch (JsonException)
            {
                throw new BadJsonFileException(settingsFileName);
            }
            catch (FileNotFoundException)
            {
                throw new SettingsFileNotFoundException(settingsFileName);
            }
        }

        public string[] getSourceDirs()
        {
            return _rawSettings.SourceDirs;
        }

        public string getTargetDir()
        {
            return _rawSettings.TargetDir;
        }

        public static void GenerateExampleJsonFile(string exampleJsonFile)
        {
            var exampleJsonString = JsonSerializer.Serialize(new RawSettings
            {
                TargetDir = "target",
                SourceDirs = new[] {"source", "another source"}
            }, new JsonSerializerOptions {WriteIndented = true});

            File.WriteAllText(exampleJsonFile, exampleJsonString);
        }
    }
}