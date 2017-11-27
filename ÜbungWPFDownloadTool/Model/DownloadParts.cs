namespace ÜbungWPFDownloadTool.Model
{
    public class DownloadParts
    {
        public long Offset { get; set; }
        public long Bytes { get; set; }
        public bool Finished { get; set; }
    }
}