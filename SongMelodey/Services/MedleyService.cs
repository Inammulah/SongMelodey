namespace SongMelodey.Services
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using SongMelodey.Models;
    using System.Diagnostics;
    using System.Text;
    using SongMelodey.Results;
    using System.Diagnostics;
    using Microsoft.EntityFrameworkCore;

    public class MedleyService : IMedleyService
    {
        private readonly SongsMedleyMakerAndDjContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly FFprobeService _ffprobe;

        public MedleyService(
            SongsMedleyMakerAndDjContext db,
            IWebHostEnvironment env,
            IConfiguration config,
            FFprobeService ffprobe)
        {
            _db = db;
            _env = env;
            _config = config;
            _ffprobe = ffprobe;
        }

        public async Task<MedleyResult> GenerateMedleyAsync(int medleyId, CancellationToken ct)
        {
            var medley = await _db.Medleys
                .Include(m => m.MedleyClips)
                    .ThenInclude(mc => mc.TrimClip)
                .Include(m => m.MedleyClips)
                    .ThenInclude(mc => mc.TransitionClip)
                .FirstOrDefaultAsync(m => m.MedleyId == medleyId, ct);

            if (medley == null)
                return new MedleyResult { Success = false, Error = "Medley not found" };

            var root = _env.WebRootPath ?? "wwwroot";
            var outputDir = Path.Combine(root, "medleys");
            Directory.CreateDirectory(outputDir);

            var outputFile = $"medley_{medleyId}_{Guid.NewGuid()}.mp3";
            var outputPath = Path.Combine(outputDir, outputFile);

            // 1️⃣ Create concat list
            var concatFile = Path.GetTempFileName();

            using (var sw = new StreamWriter(concatFile))
            {
                foreach (var mc in medley.MedleyClips.OrderBy(x => x.SequenceNumber))
                {
                    sw.WriteLine($"file '{Path.Combine(root, mc.TrimClip.FilePath.TrimStart('/'))}'");

                    if (mc.TransitionClip != null)
                    {
                        sw.WriteLine($"file '{Path.Combine(root, mc.TransitionClip.FilePath.TrimStart('/'))}'");
                    }
                }
            }

            // 2️⃣ FFmpeg merge
            var ffmpegPath = _config["FFmpeg:FFmpegPath"] ?? "ffmpeg";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = $"-y -f concat -safe 0 -i \"{concatFile}\" -acodec copy \"{outputPath}\"",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync(ct);

            // 3️⃣ Duration
            var duration = await _ffprobe.GetAudioDurationMsAsync(outputPath);

            // 4️⃣ Update DB
            medley.OutputFilePath = $"/medleys/{outputFile}";
            medley.TotalDurationMs = (int)duration;
            medley.ClipCount = medley.MedleyClips.Count;
            medley.IsFinal = true;

            await _db.SaveChangesAsync(ct);

            return new MedleyResult
            {
                Success = true,
                FilePath = medley.OutputFilePath,
                TotalDurationMs = (int)duration
            };
        }
    }


}
