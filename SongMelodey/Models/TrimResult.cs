namespace SongMelodey.Models
{
    public class TrimResult
    {
        public bool Success { get; set; }
        public string FilePath { get; set; }
        public long ClipLengthMs { get; set; }
        public string Error { get; set; }
    }

}
