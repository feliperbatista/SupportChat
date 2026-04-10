using System;
using System.Diagnostics;
using Application.Interfaces;
using FFMpegCore.Pipes;
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
        if (input.CanSeek)
            input.Position = 0;

        var output = new MemoryStream();

        await FFMpegCore.FFMpegArguments
            .FromPipeInput(new StreamPipeSource(input))
            .OutputToPipe(new StreamPipeSink(output), options => options
                .WithAudioCodec("libopus")
                .ForceFormat("ogg"))
            .ProcessAsynchronously();

        output.Position = 0;

        return output;
    }
}
