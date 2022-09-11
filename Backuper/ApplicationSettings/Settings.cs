namespace Backuper.ApplicationSettings
{
    public class Settings
    {
        public string TargetDir { get; set; }
        public string[] SourceDirs { get; set; }

        public static Settings GetExample()
        {
            return new Settings
            {
                TargetDir = "D:\\TEST\\TO",
                SourceDirs = new[] {"D:\\TEST\\FROM1", "D:\\TEST\\FROM2"}
            };
        }
    }
}