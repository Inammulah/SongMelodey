namespace SongMelodey.Services
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using SongMelodey.Models;
    using System.Diagnostics;
    using System.Globalization;

    using SongMedleyAPI.Services;

    public class TrimService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly FFprobeService _ffprobe;

        public TrimService(IWebHostEnvironment env, IConfiguration config, FFprobeService ffprobe)
        {
            _env = env;
            _config = config;
            _ffprobe = ffprobe;
        }

        public async Task<string> TrimAudioAsync(string inputPath, string outputPath, int startMs, int endMs)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            var ffmpegPath = _config["FFmpeg:FFmpegPath"] ?? "ffmpeg";

            var startSec = startMs / 1000.0;
            var durationSec = (endMs - startMs) / 1000.0;

            var args = $"-y -i \"{inputPath}\" -ss {startSec} -t {durationSec} -acodec copy \"{outputPath}\"";

            var process = new Process
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

            process.Start();
            await process.WaitForExitAsync();

            return outputPath;
        }

        public async Task<int> GetRealDurationAsync(string filePath)
        {
            var durationMs = await _ffprobe.GetAudioDurationMsAsync(filePath);
            return (int)Math.Round(durationMs);
        }
    }


}
