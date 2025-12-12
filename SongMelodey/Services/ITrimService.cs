using SongMelodey.Models;



namespace SongMelodey.Services

{
    public interface ITrimService
    {
        /// <summary>
        /// Trim an existing song file using FFmpeg and return saved file path and duration (ms).
        /// </summary>
        //Task<(bool Success, string FilePath, long ClipLengthMs, string Error)> TrimFromSongAsync(
        //    int songId, int startMs, int endMs, CancellationToken ct = default);

        ///// <summary>
        ///// Save an uploaded clip file (client-provided trimmed file).
        ///// </summary>
        //Task<(bool Success, string FilePath, long ClipLengthMs, string Error)> SaveUploadedClipAsync(
        //    IFormFile file, CancellationToken ct = default);

        Task<string> TrimAudioAsync(string inputPath, string outputPath, int startMs, int endMs);

        Task<TrimResult> TrimFromSongAsync(int songId, int startMs, int endMs, CancellationToken ct);
        Task<TrimResult> SaveUploadedClipAsync(IFormFile file, CancellationToken ct);

        // NEW FUNCTION
        Task<int> GetRealDurationAsync(string filePath);
    }
}
