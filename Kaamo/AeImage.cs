using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Kaamo.Enums;
using Kaamo.Utils;

namespace Kaamo
{
    /// <summary>
    /// A region in an <see cref="AeImage"/>.
    /// </summary>
    [DebuggerDisplay("Region #{Index}, {Region}")]
    internal struct AeImageRegion
    {
        /// <summary>
        /// Region index.
        /// </summary>
        public int Index;

        /// <summary>
        /// Region area.
        /// </summary>
        public Rect Region;
    }

    /// <summary>
    /// A glyph in an <see cref="AeImage"/>.
    /// </summary>
    [DebuggerDisplay("Glyph #{Index} {Glyph}, {Region}")]
    internal struct AeImageGlyph
    {
        /// <summary>
        /// Glyph index within the glyph group.
        /// </summary>
        public int Index;

        /// <summary>
        /// The glyph.
        /// </summary>
        public char Glyph;

        /// <summary>
        /// Glyph region area.
        /// </summary>
        public Rect Region;
    }

    /// <summary>
    /// A Galaxy on Fire 2 image.
    /// </summary>
    [DebuggerDisplay("Image {Width}x{Height} in {Format}, " +
                     "{Data.Length} bytes, {Regions.Count} regions, " +
                     "{GlyphGroups.Count} glyph groups")]
    internal class AeImage
    {
        /// <summary>
        /// Texture data pixel format.
        /// </summary>
        public PixelFormat Format;

        /// <summary>
        /// Texture data pixel width.
        /// </summary>
        public ushort Width;

        /// <summary>
        /// Texture data pixel height.
        /// </summary>
        public ushort Height;

        /// <summary>
        /// Computed texture mipmap count.
        /// </summary>
        public int MipmapCount;

        /// <summary>
        /// Whether the texture is a cubemap.
        /// </summary>
        public bool IsCubemap;

        /// <summary>
        /// Texture regions.
        /// </summary>
        public ICollection<AeImageRegion> Regions;

        /// <summary>
        /// Texture data.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Glyph groups in the image.
        /// </summary>
        public ICollection<ICollection<AeImageGlyph>> GlyphGroups;
    }

    internal static class AeImageReader
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

        private static ICollection<AeImageRegion> ReadRegions(Stream stream, int nRegions)
        {
            var regions = new AeImageRegion[nRegions];

            for (var i = 0; i < nRegions; i++)
            {
                regions[i] = new AeImageRegion
                {
                    Index = i,
                    Region = ReadRect(stream)
                };
            }

            return regions;
        }

        private static ICollection<ICollection<AeImageGlyph>> ReadGlyphGroups(
            Stream stream, int nGlyphGroups)
        {
            Span<byte> buf = stackalloc byte[2];
            ReadOnlySpan<byte> bufView = buf;
            var groups = new AeImageGlyph[nGlyphGroups][];

            for (var i = 0; i < nGlyphGroups; i++)
            {
                var nGlyphs = stream.ReadUShort();
                var glyphs = new AeImageGlyph[nGlyphs];
                Array.Fill(glyphs, new AeImageGlyph());
                for (var j = 0; j < nGlyphs; j++)
                {
                    stream.Read(buf);
                    glyphs[j].Index = j;
                    glyphs[j].Glyph = Encoding.Unicode.GetString(bufView)[0];
                }
                for (var j = 0; j < nGlyphs; j++)
                {
                    glyphs[j].Region = ReadRect(stream);
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

            return new AeImage
            {
                Format = format,
                Width = w,
                Height = h,
                MipmapCount = mipmapCount,
                IsCubemap = isCubemap,
                Regions = regions,
                Data = data,
                GlyphGroups = glyphGroups
            };
        }
    }
}
