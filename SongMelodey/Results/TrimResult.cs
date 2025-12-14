namespace SongMelodey.Results
{

    public class TrimResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? FilePath { get; set; }
        public int ClipLengthMs { get; set; }
    }
}
