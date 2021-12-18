using System;
using System.IO;

namespace Kaamo.Texture.Utils
{
    /// <summary>
    /// High-performance utility methods for reading integers from a <see cref="Stream"/>.
    /// Whether we wanted it or not, we've ran into a war with the Cabal on Mars.
    /// </summary>
    public static class StreamUtilities
    {
        /// <summary>
        /// Reads a 16-bit unsigned integer. Uses <see cref="Span{T}"/> for performance.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The integer.</returns>
        public static ushort ReadUShort(this Stream stream)
        {
            Span<byte> uint16 = stackalloc byte[2];
            stream.Read(uint16);
            return (ushort)(uint16[0] | uint16[1] << 8);
        }

        /// <summary>
        /// Reads several 16-bit unsigned integers. Uses <see cref="Span{T}"/> for performance.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="count">The count of integers to read.</param>
        /// <returns>An array of integers.</returns>
        public static ushort[] ReadUShorts(this Stream stream, int count)
        {
            var values = new ushort[count];
            Span<byte> bytes = stackalloc byte[count * sizeof(ushort)];
            stream.Read(bytes);
            for (var i = 0; i < count; i++)
            {
                values[i] = (ushort)(bytes[i * 2] | bytes[i * 2 + 1] << 8);
            }
            return values;
        }

        /// <summary>
        /// Reads a 32-bit signed integer. Uses <see cref="Span{T}"/> for performance.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The integer.</returns>
        public static int ReadInt(this Stream stream)
        {
            Span<byte> int32 = stackalloc byte[4];
            stream.Read(int32);
            return int32[0] | int32[1] << 8 |
                   int32[2] << 16 | int32[3] << 24;
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer. Uses <see cref="Span{T}"/> for performance.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The integer.</returns>
        public static uint ReadUInt(this Stream stream)
        {
            Span<byte> uint32 = stackalloc byte[4];
            stream.Read(uint32);
            return (uint)(uint32[0] | uint32[1] << 8 |
                uint32[2] << 16 | uint32[3] << 24);
        }

        /// <summary>
        /// Reads a specified length of bytes from the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="count">The count of bytes to read.</param>
        /// <returns>The bytes.</returns>
        public static byte[] ReadBytes(this Stream stream, int count)
        {
            var result = new byte[count];
            stream.Read(result, 0, count);
            return result;
        }
    }
}
