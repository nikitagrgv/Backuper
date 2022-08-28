using System.IO;
using System.Text;
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

                if (_rawSettings.SourceDirs == null || _rawSettings.TargetDir == null)
                    throw new BadJsonFileException(settingsFileName);
            }
            catch (JsonException)
            {
                throw new BadJsonFileException(settingsFileName);
            }
            catch (FileNotFoundException)
            {
                throw new NotFoundSettingsFileException(settingsFileName);
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

        public string GetInfo()
        {
            var info = new StringBuilder();

            info.Append($"Target directory: {_rawSettings.TargetDir}\n");
            info.Append($"Source directories [{_rawSettings.SourceDirs.Length}]: \n");

            foreach (var sourceDir in _rawSettings.SourceDirs)
                info.Append($"  {sourceDir}\n");

            return info.ToString();
        }

        public static void GenerateExampleJsonFile(string exampleJsonFile)
        {
            var exampleJsonString = JsonSerializer.Serialize(new RawSettings
            {
                TargetDir = "D:\\TEST\\TO",
                SourceDirs = new[] {"D:\\TEST\\FROM1", "D:\\TEST\\FROM2"}
            }, new JsonSerializerOptions {WriteIndented = true});

            File.WriteAllText(exampleJsonFile, exampleJsonString);
        }
    }
}