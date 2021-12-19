using System.Runtime.InteropServices;
using Kaamo.Texture.Enums;

namespace Kaamo.Texture;

/// <summary>
/// P/Invoked native methods.
/// </summary>
internal static class Native
{
    private const string DllName = "Kaamo.Texture.dll";

    /// <summary>
    /// Decompresses the specified texture data.
    /// </summary>
    /// <param name="data">The data to decompress.</param>
    /// <param name="offset">The offset to decompress data from.</param>
    /// <param name="format">The data format.</param>
    /// <param name="x">Texture pixel width.</param>
    /// <param name="y">Texture pixel height.</param>
    /// <param name="output">Array to store output into.</param>
    /// <returns>The length of consumed compressed data.</returns>
    [DllImport(DllName)]
    public static extern uint Decompress(
        [MarshalAs(UnmanagedType.LPArray)] byte[] data, int offset,
        NativePixelFormat format, int x, int y,
        [MarshalAs(UnmanagedType.LPArray)] byte[] output);
}