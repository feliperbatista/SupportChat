using System;
using System.Diagnostics;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.AudioConverter;

public class FFmpegAudioConverterService(IConfiguration configuration) : IAudioConverter
{
    private readonly string _ffmpegPath = configuration["FfmpegPath"] 
    ?? Path.Combine(
        Directory.GetCurrentDirectory(),
        "ffmpeg",
        "ffmpeg.exe"
    );
    public async Task<Stream> ConvertWebMToOggAsync(Stream input, CancellationToken ct = default)
    {
        var tempInput = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.webm");
        var tempOutput = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.ogg");

        try
        {
            if (input.CanSeek)
                input.Position = 0;

            using (var fs = new FileStream(tempInput, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await input.CopyToAsync(fs, ct);
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"-i \"{tempInput}\" -c:a libopus \"{tempOutput}\" -y -loglevel error",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var error = await process.StandardError.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            if (process.ExitCode != 0)
                throw new Exception($"FFmpeg failed: {error}");

            var outputStream = new MemoryStream(await File.ReadAllBytesAsync(tempOutput, ct));
            return outputStream;
        }
        finally
        {
            try { if (File.Exists(tempInput)) File.Delete(tempInput); } catch {}
            try { if (File.Exists(tempOutput)) File.Delete(tempOutput); } catch {}
        }
    }
}
