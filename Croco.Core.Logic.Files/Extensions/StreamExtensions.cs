namespace Croco.Core.Logic.Files.Extensions;

/// <summary>
/// Расширения для класса <see cref="Stream"/>.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Привести поток к массиву байтов
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] ReadStreamAsBytes(this Stream input)
    {
        if (input is MemoryStream memStream)
        {
            return memStream.ToArray();
        }

        using var ms = new MemoryStream();
        input.CopyTo(ms);
        return ms.ToArray();
    }
}