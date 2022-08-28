using System.Text.Json.Serialization;

namespace Backuper.ApplicationSettings
{
    public class RawSettings
    {
        public string TargetDir { get; set; }
        public string[] SourceDirs { get; set; }

        // [JsonConverter(typeof(JsonStringEnumConverter))]
    }

}