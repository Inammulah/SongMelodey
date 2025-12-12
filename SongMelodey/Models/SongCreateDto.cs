namespace SongMelodey.Models
{
    public class SongCreateDto
    {

        public int? ThemeId { get; set; }
        public string SongTitle { get; set; }
        public string ArtistName { get; set; }
        public int DurationSec { get; set; }
        public IFormFile File { get; set; }
    }
}
