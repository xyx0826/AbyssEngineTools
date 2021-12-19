using Kaamo.Texture;
using Kaamo.Texture.Enums;
using System;
using System.IO;
using System.Security.Cryptography;
using Xunit;

namespace Kaamo.Tests
{
    /// <summary>
    /// Tests Abyss Engine 2 texture features.
    /// </summary>
    public class AeImageTests
    {
        [Theory]
        [InlineData("ship_019_midorian_diffuse.aei", PixelFormat.PvrtcRgba4BppV1, 1024, 1024, 1, 0)]
        [InlineData("skybox_007.aei", PixelFormat.Dxt5Rgba, 2048, 2048, 1, 0)]
        [InlineData("gof2_items_2.aei", PixelFormat.Bgra8, 256, 256, 20, 0)]
        [InlineData("gof2_font_langselect_iphone4.aei", PixelFormat.Bgra8, 256, 256, 0, 1)]
        internal void Read_AeImage_ReadsHeaderCorrectly(
            string filePath, PixelFormat format,
            int width, int height, int regionCount, int glyphGroupCount)
        {
            using var file = File.OpenRead(Utilities.GetResourcePath(filePath));
            var texture = AeImageReader.Read(file);

            Assert.Equal(format, texture.Format);
            Assert.Equal(width, texture.Width);
            Assert.Equal(height, texture.Height);
            Assert.Equal(regionCount, texture.Regions.Count);
            Assert.Equal(glyphGroupCount, texture.GlyphGroups.Count);
        }

        /// <summary>
        /// Tests the decompressor on textures.
        /// Checks the hash of the texture to test correctness.
        /// </summary>
        /// <param name="fileName">Input file.</param>
        /// <param name="md5">Expected hash.</param>
        [Theory]
        [InlineData("ship_019_midorian_diffuse.aei",
            "E1AC4F869DE1B0F586A7AB133A178D5E")]
        internal void Read_AeImage_DecompressesCorrectly(
            string fileName, string md5)
        {
            const int bitmapIndex = 0;
            const int bitmapCount = 11;

            using var file = File.OpenRead(Utilities.GetResourcePath(fileName));
            var texture = AeImageReader.Read(file);
            var hash = MD5.HashData(texture.Surfaces[bitmapIndex]);

            using var dds = DirectDrawSurface.CreateWriteStream(fileName + ".dds", texture);
            foreach (var map in texture.Surfaces)
            {
                dds.Write(map);
            }

            Assert.Equal(bitmapCount, texture.Surfaces.Count);
            Assert.Equal(md5, Convert.ToHexString(hash));
        }
    }
}
