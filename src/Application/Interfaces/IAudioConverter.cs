using System;

namespace Application.Interfaces;

public interface IAudioConverter
{
    Task<Stream> ConvertWebMToOggAsync(Stream input, CancellationToken ct = default);
}
