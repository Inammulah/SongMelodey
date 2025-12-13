using System.Diagnostics;

namespace SongMelodey.Services
{
    public class FFprobeService
    {
        private readonly string _ffprobePath;

        public FFprobeService(IConfiguration config)
        {
            _ffprobePath = config["FFmpeg:FFprobePath"] ?? "ffprobe";
        }

        public async Task<double> GetAudioDurationMsAsync(string filePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffprobePath,
                    Arguments = $"-v error -show_entries format=duration -of csv=p=0 \"{filePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string? output = await process.StandardOutput.ReadLineAsync();
            await process.WaitForExitAsync();

            if (double.TryParse(output, out double seconds))
                return seconds * 1000;

            return 0;
        }
    }
}
