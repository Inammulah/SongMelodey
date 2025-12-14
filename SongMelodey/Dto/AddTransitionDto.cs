namespace SongMelodey.Dto
{
    public class AddTransitionDto
    {
        public int MedleyClipId { get; set; }
        public string TransitionName { get; set; }
        public string FilePath { get; set; }
        public int DurationMs { get; set; }
    }

}
