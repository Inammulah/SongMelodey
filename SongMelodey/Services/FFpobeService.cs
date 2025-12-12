namespace SongMelodey.Services
{
    using System.Diagnostics;

    namespace SongMedleyAPI.Services
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
                var tcs = new TaskCompletionSource<double>();

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _ffprobePath,
                        Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{filePath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.EnableRaisingEvents = true;

                process.OutputDataReceived += (s, e) =>
                {
                    if (double.TryParse(e.Data, out var seconds))
                        tcs.TrySetResult(seconds * 1000); // convert to ms
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    // If ffprobe prints a warning, ignore it
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                return await tcs.Task;
            }
        }
    }

}
