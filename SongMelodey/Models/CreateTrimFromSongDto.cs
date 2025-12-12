namespace SongMelodey.Models
{
    public class CreateTrimFromSongDto
    {
        public int SongId { get; set; }
        public int StartMs { get; set; }  // in milliseconds
        public int EndMs { get; set; }    // in milliseconds
    }
}
