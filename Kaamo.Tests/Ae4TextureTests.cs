using Kaamo.Texture;
using Kaamo.Texture.Enums;
using System;
using System.IO;
using System.Security.Cryptography;
using Xunit;

namespace Kaamo.Tests
{
    /// <summary>
    /// Tests Abyss Engine 4 texture features.
    /// </summary>
    public class Ae4TextureTests
    {
        /// <summary>
        /// Tests the AE4 texture header reader.
        /// </summary>
        /// <param name="fileName">Input file.</param>
        /// <param name="format">Expected format.</param>
        /// <param name="width">Expected width.</param>
        /// <param name="height">Expected height.</param>
        /// <param name="depth">Expected depth.</param>
        /// <param name="mipmapCount">Expected mipmap count.</param>
        /// <param name="isCubemap">Expected cubemap flag.</param>
        /// <param name="dataLength">Expected data length.</param>
        [Theory]
        [InlineData("ship_nivelian_210_normal.tga.ae4texture", PixelFormat.PvrtcRgb4BppV1,
            1024, 1024, 0, 11, false, 0xac000)]
        [InlineData("risingchristmas_000.tga.ae4texture", PixelFormat.PvrtcRgba4BppV1,
            512, 512, 0, 10, true, 0x104000)]
        [InlineData("performanceWarning.png.ae4texture", PixelFormat.Etc2Rgba8,
            128, 128, 0, 1, false, 0x4000)]
        internal void Read_Ae4Texture_ReadsHeaderCorrectly(
            string fileName, PixelFormat format,
            int width, int height, int depth, int mipmapCount, bool isCubemap, int dataLength)
        {
            using var file = File.OpenRead(Utilities.GetResourcePath(fileName));
            var texture = Ae4TextureReader.Read(file);

            Assert.Equal(format, texture.Format);
            Assert.Equal(width, texture.Width);
            Assert.Equal(height, texture.Height);
            Assert.Equal(depth, texture.Depth);
            Assert.Equal(mipmapCount, texture.MipmapCount);
            Assert.Equal(isCubemap, texture.IsCubemap);
            Assert.Equal(dataLength, texture.Data.Length);
        }

        /// <summary>
        /// Tests the decompressor on textures with cubemaps or multiple mipmap levels.
        /// Checks the hash of the third to last bitmap to test correctness
        /// of both the decompressor and the slicer.
        /// </summary>
        /// <param name="fileName">Input file.</param>
        /// <param name="md5">Expected hash.</param>
        [Theory]
        [InlineData("ship_nivelian_210_normal.tga.ae4texture",
                    "9D27DB99F11507AFCBD5D080B92277F0")]
        [InlineData("risingchristmas_000.tga.ae4texture",
                    "0C3F0B92C3FE3960688CE66A2BA6A86F")]
        internal void Read_Ae4Texture_DecompressesThirdToLastBitmapCorrectly(
            string fileName, string md5)
        {
            const int bitmapIndexLast = 3;

            using var file = File.OpenRead(Utilities.GetResourcePath(fileName));
            var texture = Ae4TextureReader.Read(file);
            var maps = Decompressor.Decompress(texture);
            var secondHash = MD5.HashData(maps[^bitmapIndexLast]);

            Assert.Equal(md5, Convert.ToHexString(secondHash));
        }

        /// <summary>
        /// Tests the decompressor on non-cubemap textures with a single mipmap level.
        /// Checks the hash of the only bitmap to test correctness.
        /// </summary>
        /// <param name="fileName">Input file.</param>
        /// <param name="md5">Expected hash.</param>
        [Theory]
        [InlineData("performanceWarning.png.ae4texture",
                    "1398D28C166D1B96569F5C77C24BC991")]
        internal void Read_Ae4Texture_DecompressesOnlyBitmapCorrectly(
            string fileName, string md5)
        {
            const int bitmapIndex = 0;
            const int bitmapCount = 1;

            using var file = File.OpenRead(Utilities.GetResourcePath(fileName));
            var texture = Ae4TextureReader.Read(file);
            var maps = Decompressor.Decompress(texture);
            var hash = MD5.HashData(maps[bitmapIndex]);

            Assert.Equal(bitmapCount, maps.Count);
            Assert.Equal(md5, Convert.ToHexString(hash));
        }
    }
}
