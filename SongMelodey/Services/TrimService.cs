using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SongMelodey.Models;
using System.Diagnostics;

namespace SongMelodey.Services
{
    public class TrimService : ITrimService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly FFprobeService _ffprobe;
        private readonly SongsMedleyMakerAndDjContext _db; // Injected DbContext

        public TrimService(
            IWebHostEnvironment env,
            IConfiguration config,
            FFprobeService ffprobe,
            SongsMedleyMakerAndDjContext db) // Add DbContext parameter
        {
            _env = env;
            _config = config;
            _ffprobe = ffprobe;
            _db = db; // Store reference
        }

        public async Task <TrimResult> TrimFromSongAsync(int songId, int startMs, int endMs, CancellationToken ct)
        {
            try
            {
                var root = _env.WebRootPath ?? "wwwroot";

                // Load song from injected DbContext
                var song = await _db.Songs.FindAsync(songId);
                if (song == null)
                    return new TrimResult { Success = false, Error = "Song not found" };

                // FIX: Correct input path
                var inputPath = Path.Combine(root, song.FilePath.TrimStart('/', '\\'));
                inputPath = Path.GetFullPath(inputPath); // Get absolute path

                if (!File.Exists(inputPath))
                    return new TrimResult { Success = false, Error = $"Song file missing on server. Path: {inputPath}" };

                var clipDir = Path.Combine(root, "clips");
                Directory.CreateDirectory(clipDir);

                var fileName = $"clip_{Guid.NewGuid()}.mp3";
                var outputPath = Path.Combine(clipDir, fileName);

                // Perform trim
                await TrimAudioAsync(inputPath, outputPath, startMs, endMs);

                // Get duration
                var duration = await GetRealDurationAsync(outputPath);

                // Return relative path for web access
                var webPath = outputPath.Replace(root, "").Replace("\\", "/");
                if (!webPath.StartsWith("/"))
                    webPath = "/" + webPath;

                return new TrimResult
                {
                    Success = true,
                    FilePath = webPath,
                    ClipLengthMs = duration
                };
            }
            catch (Exception ex)
            {
                return new TrimResult { Success = false, Error = $"Trim failed: {ex.Message}" };
            }
        }

        public async Task<TrimResult> SaveUploadedClipAsync(IFormFile file, CancellationToken ct)
        {
            try
            {
                var root = _env.WebRootPath ?? "wwwroot";
                var clipDir = Path.Combine(root, "clips");
                Directory.CreateDirectory(clipDir);

                // Sanitize filename
                var safeFileName = Path.GetFileNameWithoutExtension(file.FileName)
                    .Replace(" ", "_")
                    .Replace("(", "")
                    .Replace(")", "");
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"upload_{Guid.NewGuid()}_{safeFileName}{extension}";
                var outputPath = Path.Combine(clipDir, fileName);

                using (var stream = new FileStream(outputPath, FileMode.Create))
                    await file.CopyToAsync(stream, ct);

                var duration = await GetRealDurationAsync(outputPath);

                // Return relative path for web access
                var webPath = outputPath.Replace(root, "").Replace("\\", "/");
                if (!webPath.StartsWith("/"))
                    webPath = "/" + webPath;

                return new TrimResult
                {
                    Success = true,
                    FilePath = webPath,
                    ClipLengthMs = duration
                };
            }
            catch (Exception ex)
            {
                return new TrimResult { Success = false, Error = $"Upload failed: {ex.Message}" };
            }
        }

        public async Task<string> TrimAudioAsync(string inputPath, string outputPath, int startMs, int endMs)
        {
            var ffmpegPath = _config["FFmpeg:ExecutablePath"] ?? "ffmpeg";

            var startSec = startMs / 1000.0;
            var durationSec = (endMs - startMs) / 1000.0;

            // Ensure output directory exists
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir!);

            // FIX: MP3 trim with re-encoding
            var args = $"-y -ss {startSec:F3} -i \"{inputPath}\" -t {durationSec:F3} -acodec libmp3lame -b:a 192k \"{outputPath}\"";

            using var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            await proc.WaitForExitAsync();

            if (proc.ExitCode != 0)
            {
                var error = await proc.StandardError.ReadToEndAsync();
                throw new Exception($"FFmpeg error: {error}");
            }

            return outputPath;
        }

        public async Task<int> GetRealDurationAsync(string filePath)
        {
            try
            {
                var ms = await _ffprobe.GetAudioDurationMsAsync(filePath);
                return (int)ms;
            }
            catch
            {
                // Fallback: calculate from file if ffprobe fails
                return 0;
            }
        }
    }

    public class TrimResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? FilePath { get; set; }
        public int ClipLengthMs { get; set; }
    }
}