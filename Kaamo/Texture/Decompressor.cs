using System;
using System.Collections.Generic;
using Kaamo.Texture.Enums;
using Kaamo.Texture.Utils;

namespace Kaamo.Texture
{
    internal static class Decompressor
    {
        /// <summary>
        /// Internally decompresses texture data with specified parameters.
        /// </summary>
        /// <param name="format">Pixel format.</param>
        /// <param name="width">Texture width.</param>
        /// <param name="height">Texture height.</param>
        /// <param name="depth">Texture depth. It'd better be zero!</param>
        /// <param name="mipmapCount">Texture mipmap count, including base level.</param>
        /// <param name="isCubemap">Whether the texture is a cubemap.</param>
        /// <param name="data">Pixel data.</param>
        /// <returns>
        /// An array of RGBA 8888 bitmaps containing faces and their mipmap levels.
        /// </returns>
        private static IReadOnlyList<byte[]> Decompress(
            PixelFormat format, int width, int height, int depth, int mipmapCount, bool isCubemap,
            byte[] data)
        {
            if (depth > 0)
            {
                throw new Exception("Unsupported non-zero texture depth.");
            }

            var pvrFormat = format switch
            {
                PixelFormat.PvrtcRgb2BppV1 or PixelFormat.PvrtcRgba2BppV1
                    or PixelFormat.PvrtcRgba2BppV2 => NativePixelFormat.Pvrtc2,
                PixelFormat.PvrtcRgb4BppV1 or PixelFormat.PvrtcRgba4BppV1
                    or PixelFormat.PvrtcRgba4BppV2 => NativePixelFormat.Pvrtc4,
                PixelFormat.AtcRgb => throw new Exception("Damn"),
                PixelFormat.Etc1Rgb8 => NativePixelFormat.Etc1,
                PixelFormat.Etc2Rgb8 => NativePixelFormat.Etc2,
                PixelFormat.Etc2Rgba8 => NativePixelFormat.Etc2Eac,
                PixelFormat.Etc2Rgb8A1 => NativePixelFormat.Etc2Punchthrough,
                PixelFormat.Dxt1Rgba => NativePixelFormat.Bc1A,
                PixelFormat.Dxt3Rgba => NativePixelFormat.Bc3,
                PixelFormat.Dxt5Rgba => NativePixelFormat.Bc5,
                _ => throw new Exception("Uncompressed or unexpected pixel format.")
            };

            var totalRead = 0;
            var faceCount = isCubemap ? 6 : 1;
            var mipmaps = new byte[mipmapCount * faceCount][];
            for (var face = 0; face < faceCount; face++)
            {
                for (var level = 0; level < mipmapCount; level++)
                {
                    // Get dimensions of the current level
                    PixelFormatUtils.GetMipmapDimensions(width, height, depth,
                        level, out var w, out var h, out _);

                    // Create output buffer, each pixel is RGBA8
                    var rgba = new byte[w * h * 4];

                    // Compute starting position for current face and level
                    var offset = PixelFormatUtils.GetOffset(format, mipmapCount,
                        width, height, depth, face, level, isCubemap);

                    // Starting position sanity check
                    if (totalRead != offset)
                    {
                        throw new Exception("Decompression read offset mismatch: " +
                                            $"Expected {totalRead} bytes, computed {offset} bytes.");
                    }

                    // Compute compressed size for current face and level
                    var inSize = PixelFormatUtils.GetMipmapLevelSize(format, width,
                        height, depth, level);

                    // Call into library to decompress pixels
                    var readSize = Compression.Decompress(data, offset, pvrFormat, w, h,
                        rgba);

                    // Compressed size sanity check
                    if (inSize != readSize)
                    {
                        throw new Exception("Decompression read size mismatch: " +
                                            $"Expected {inSize} bytes, read {readSize} bytes.");
                    }

                    totalRead += (int)readSize;
                    mipmaps[level + face * mipmapCount] = rgba;
                }
            }

            return mipmaps;
        }

        public static IReadOnlyList<byte[]> Decompress(AeImage image) => Decompress(image.Format,
            image.Width, image.Height, 0, image.MipmapCount, false, image.Data);

        /// <summary>
        /// Decompresses the specified texture.
        /// Each face and mipmap within the texture is decompressed separately.
        /// </summary>
        /// <param name="texture">The texture to decompress.</param>
        /// <returns>
        /// An array of RGBA 8888 bitmaps containing faces and their mipmap levels.
        /// </returns>
        public static IReadOnlyList<byte[]> Decompress(Ae4Texture texture)
            => Decompress(texture.Format, texture.Width, texture.Height, texture.Depth,
                texture.MipmapCount, texture.IsCubemap, texture.Data);
    }
}
