using System;
using System.Diagnostics;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.AudioConverter;

public class FFmpegAudioConverterService(IConfiguration configuration) : IAudioConverter
{
    private readonly string _ffmpegPath = configuration["FfmpegPath"] ?? @"C:\home\site\ffmpeg\ffmpeg.exe";
    public async Task<Stream> ConvertWebMToOggAsync(Stream input, CancellationToken ct = default)
    {
        var tempInput = Path.GetTempFileName() + ".webm";
        var tempOutput = Path.GetTempFileName() + ".ogg";

        using var fs = new FileStream(tempInput, FileMode.Create);
            await input.CopyToAsync(fs, ct);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = $"-i \"{tempInput}\" -c:a libopus \"{tempOutput}\" -y",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        await process.WaitForExitAsync(ct);

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new Exception($"FFmpeg failed: {error}");
        }

        var outputStream = new MemoryStream(await File.ReadAllBytesAsync(tempOutput, ct));

        File.Delete(tempInput);
        File.Delete(tempOutput);

        outputStream.Position = 0;
        return outputStream;
    }
}
