using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Kaamo.Texture.Enums;
using Kaamo.Texture.Utils;

namespace Kaamo.Texture
{
    /// <summary>
    /// A region in an <see cref="AeImage"/>.
    /// </summary>
    [DebuggerDisplay("Region #{Index}, {Region}")]
    public struct AeImageRegion
    {
        public AeImageRegion(int index, Rect region)
        {
            Index = index;
            Region = region;
        }

        /// <summary>
        /// Region index.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Region area.
        /// </summary>
        public readonly Rect Region;
    }

    /// <summary>
    /// A glyph in an <see cref="AeImage"/>.
    /// </summary>
    [DebuggerDisplay("Glyph #{Index} {Glyph}, {Region}")]
    public struct AeImageGlyph
    {
        public AeImageGlyph(int index, char glyph)
        {
            Index = index;
            Glyph = glyph;
            _regionSet = false;
        }

        /// <summary>
        /// Sets the region of the glyph.
        /// Can only be used once.
        /// </summary>
        /// <param name="region">Glyph region.</param>
        /// <exception cref="Exception">
        /// Thrown if the glyph already has a region set.
        /// </exception>
        public void SetRegion(Rect region)
        {
            if (_regionSet)
            {
                throw new Exception("The glyph already has a region set.");
            }

            _regionSet = true;
            Region = region;
        }

        /// <summary>
        /// Glyph index within the glyph group.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// The glyph.
        /// </summary>
        public readonly char Glyph;

        private bool _regionSet;

        /// <summary>
        /// Glyph region area.
        /// </summary>
        public Rect Region { get; private set; }
    }

    /// <summary>
    /// A Galaxy on Fire 2 image.
    /// </summary>
    [DebuggerDisplay("Image {Width}x{Height} in {Format}, " +
                     "{Data.Length} bytes, {Regions.Count} regions, " +
                     "{GlyphGroups.Count} glyph groups")]
    public class AeImage
    {
        public AeImage(
            PixelFormat format, ushort width, ushort height, int mipmapCount,
            bool isCubemap, IList<AeImageRegion> regions, byte[] data,
            IList<IList<AeImageGlyph>> glyphGroups)
        {
            Format = format;
            Width = width;
            Height = height;
            MipmapCount = mipmapCount;
            IsCubemap = isCubemap;
            Regions = regions;
            Data = data;
            GlyphGroups = glyphGroups;

            CreateSurfaces();
        }

        /// <summary>
        /// Creates surfaces from original texture data.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the image already have surfaces.
        /// </exception>
        private void CreateSurfaces()
        {
            if (Surfaces is { Count: > 0 })
            {
                throw new Exception("The image already have surfaces.");
            }

            if (Format.IsCompressed())
            {
                // Directly decompress into surfaces
                Surfaces = Decompressor.Decompress(this, true);
            }
            else
            {
                if (!Enum.TryParse(Format.ToString(),
                        out NativePixelFormat nativeFormat))
                {
                    throw new Exception("Could not create surfaces for this texture because" +
                                        $"the library could not convert {Format}.");
                }

                // Slice up data
                var surfaces = new List<byte[]>(MipmapCount);
                for (var level = 0; level < MipmapCount; level++)
                {
                    // The utilities multiply input by 6 if IsCubemap
                    var actualHeight = IsCubemap ? Height / 6 : Height;

                    // Get size of the current level
                    var size = PixelFormatUtils.GetSize(
                        Format, level + 1, Width, actualHeight, 1, IsCubemap);

                    // Compute starting position for current face and level
                    var offset = PixelFormatUtils.GetOffset(Format, MipmapCount,
                        Width, actualHeight, 1, 0, level, IsCubemap);

                    // Convert to BGRA8
                    var surface = new ArraySegment<byte>(Data, offset, size).ToArray();
                    if (!Native.ConvertInPlace(surface, actualHeight * Width, nativeFormat,
                            NativePixelFormat.Bgra8))
                    {
                        throw new Exception("Could not create surfaces for this texture because" +
                                            $"the library could not convert {Format}.");
                    }
                    surfaces.Add(surface);
                }

                Surfaces = surfaces;
            }
        }

        /// <summary>
        /// Texture data pixel format.
        /// </summary>
        public readonly PixelFormat Format;

        /// <summary>
        /// Texture data pixel width.
        /// </summary>
        public readonly ushort Width;

        /// <summary>
        /// Texture data pixel height.
        /// </summary>
        public readonly ushort Height;

        /// <summary>
        /// Computed texture mipmap count.
        /// </summary>
        public readonly int MipmapCount;

        /// <summary>
        /// Whether the texture is a cubemap.
        /// </summary>
        public readonly bool IsCubemap;

        /// <summary>
        /// Texture regions.
        /// </summary>
        public readonly IList<AeImageRegion> Regions;

        /// <summary>
        /// Original texture data.
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// Glyph groups in the image.
        /// </summary>
        public IList<IList<AeImageGlyph>> GlyphGroups;

        /// <summary>
        /// Mipmaps and faces of the image in <see cref="PixelFormat.Bgra8"/>.
        /// </summary>
        public IReadOnlyList<byte[]> Surfaces;
    }

    public static class AeImageReader
    {
        /// <summary>
        /// Magic bytes at the beginning of each <see cref="AeImage"/> file.
        /// </summary>
        private static readonly byte[] Magic = Encoding.UTF8.GetBytes("AEimage\0");

        /// <summary>
        /// Reads a 8-bit magic from the reader and compares it with the expected magic.
        /// If there is not enough bytes left, nothing is read.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Whether the magic value is correct.</returns>
        private static bool CheckMagic(Stream stream)
        {
            if (stream.Position + 8 <= stream.Length)
            {
                // Length OK
                Span<byte> buf = stackalloc byte[8];
                stream.Read(buf);
                return buf.SequenceEqual(new ReadOnlySpan<byte>(Magic));
            }

            throw new Exception("Not enough length.");
        }

        private static Rect ReadRect(Stream stream)
        {
            if (stream.Position + 8 <= stream.Length)
            {
                Span<byte> buf = stackalloc byte[8];
                stream.Read(buf);
                var x = buf[0] | buf[1] << 8;
                var y = buf[2] | buf[3] << 8;
                var w = buf[4] | buf[5] << 8;
                var h = buf[6] | buf[7] << 8;
                return new Rect(x, y, w, h);
            }

            throw new Exception("Not enough length.");
        }

        /// <summary>
        /// Reads the specified number of regions from the image stream.
        /// </summary>
        /// <param name="stream">The image stream.</param>
        /// <param name="nRegions">Number of regions to read.</param>
        /// <returns>Regions.</returns>
        private static IList<AeImageRegion> ReadRegions(Stream stream, int nRegions)
        {
            var regions = new AeImageRegion[nRegions];

            for (var i = 0; i < nRegions; i++)
            {
                regions[i] = new AeImageRegion(i, ReadRect(stream));
            }

            return regions;
        }

        /// <summary>
        /// Reads the specified number of glyph groups and all their glyphs
        /// from the image stream.
        /// </summary>
        /// <param name="stream">The image stream.</param>
        /// <param name="nGlyphGroups">Number of glyph groups to read.</param>
        /// <returns>Glyph groups.</returns>
        private static IList<IList<AeImageGlyph>> ReadGlyphGroups(
            Stream stream, int nGlyphGroups)
        {
            Span<byte> buf = stackalloc byte[2];
            var groups = new AeImageGlyph[nGlyphGroups][];

            for (var i = 0; i < nGlyphGroups; i++)
            {
                var nGlyphs = stream.ReadUShort();
                var glyphs = new AeImageGlyph[nGlyphs];
                for (var j = 0; j < nGlyphs; j++)
                {
                    stream.Read(buf);
                    var glyphChar = Encoding.Unicode.GetString(buf)[0];
                    glyphs[j] = new AeImageGlyph(i, glyphChar);
                }
                for (var j = 0; j < nGlyphs; j++)
                {
                    glyphs[j].SetRegion(ReadRect(stream));
                }

                groups[i] = glyphs;
            }

            return groups;
        }

        /// <summary>
        /// Reads an <see cref="AeImage"/> from the specified reader.
        /// </summary>
        /// <param name="stream">The stream to read file from.</param>
        /// <returns>The read image.</returns>
        public static AeImage Read(Stream stream)
        {
            if (!CheckMagic(stream))
            {
                throw new Exception("Magic value does not match.");
            }

            var formatByte = stream.ReadByte();
            var isMipmapped = (formatByte & 0b10) != 0;
            var isCubemap = (formatByte & 0b10000000) != 0;
            var format = PixelFormatUtils.GetPixelFormat(Engine.AbyssEngine2, (byte)formatByte);
            var _ = stream.ReadUShorts(3);
            var w = _[0];
            var h = _[1];
            var regions = ReadRegions(stream, _[2]);
            var length = 4 * w * h;
            if (format.IsCompressed())
            {
                length = stream.ReadInt();
            }

            var data = stream.ReadBytes(length);
            var nGlyphGroups = stream.ReadUShort();
            var glyphGroups = ReadGlyphGroups(stream, nGlyphGroups);

            // Compute number of mipmaps
            var mipmapCount = 1;
            if (isMipmapped)
            {
                mipmapCount = 0;
                var myDataLen = data.Length;
                while (myDataLen != 0)
                {
                    myDataLen -= PixelFormatUtils.GetMipmapLevelSize(
                        format, w, h, 1, mipmapCount);
                    mipmapCount++;
                }
            }

            return new AeImage(format, w, h, mipmapCount, isCubemap, regions, data, glyphGroups);
        }
    }
}
