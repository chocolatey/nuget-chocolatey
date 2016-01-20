namespace NuGet
{
    public class DownloadCache
    {
        public string OriginalUrl { get; set; }
        public string FileName { get; set; }
        public string Checksum { get; set; }
    }
}