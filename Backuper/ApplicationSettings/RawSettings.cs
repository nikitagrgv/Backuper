namespace Backuper.ApplicationSettings;

public struct RawSettings
{
    public string TargetDir { get; set; }
    public string[] SourceDirs { get; set; }

    public static RawSettings GetExample()
    {
        return new RawSettings
        {
            TargetDir = "D:\\TEST\\TO",
            SourceDirs = new[] {"D:\\TEST\\FROM1", "D:\\TEST\\FROM2"}
        };
    }
}