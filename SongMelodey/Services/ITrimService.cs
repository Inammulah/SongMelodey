using Microsoft.AspNetCore.Http;
using SongMelodey.Models;
using SongMelodey.Results;
namespace SongMelodey.Services
{
    public interface ITrimService
    {
        Task<TrimResult> TrimFromSongAsync(int songId, int startMs, int endMs, CancellationToken ct);
       // Task<TrimResult> SaveUploadedClipAsync(IFormFile file, CancellationToken ct);
        Task<string> TrimAudioAsync(string inputPath, string outputPath, int startMs, int endMs);
        Task<int> GetRealDurationAsync(string filePath);
    }
}