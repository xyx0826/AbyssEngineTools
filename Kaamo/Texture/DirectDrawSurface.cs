using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Kaamo.Texture
{
    internal static class DirectDrawSurface
    {
        private static readonly byte[] DdsMagic = { 0x44, 0x44, 0x53, 0x20 };

        #region DDSHeader
        /// <summary>
        /// Describes a DDS file header.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DdsHeader
        {
            /// <summary>
            /// Size of structure.This member must be set to 124.
            /// </summary>
            public uint Size;

            /// <summary>
            /// Flags to indicate which members contain valid data.
            /// </summary>
            public DdsHeaderFlags Flags;

            /// <summary>
            /// Surface height (in pixels).
            /// </summary>
            public uint Height;

            /// <summary>
            /// Surface width (in pixels).
            /// </summary>
            public uint Width;

            /// <summary>
            /// The pitch or number of bytes per scan line in an uncompressed texture;
            /// the total number of bytes in the top level texture for a compressed texture.
            /// For information about how to compute the pitch, see
            /// the DDS File Layout section of the Programming Guide for DDS.
            /// https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide
            /// </summary>
            public uint PitchOrLinearSize;

            /// <summary>
            /// Depth of a volume texture(in pixels), otherwise unused.
            /// </summary>
            public uint Depth;

            /// <summary>
            /// Number of mipmap levels, otherwise unused.
            /// </summary>
            public uint MipMapCount;

            /// <summary>
            /// Unused.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public uint[] Reserved1;

            /// <summary>
            /// The pixel format.
            /// </summary>
            public DdsPixelFormat PixelFormat;

            /// <summary>
            /// Specifies the complexity of the surfaces stored.
            /// </summary>
            public DdsHeaderCaps Caps;

            /// <summary>
            /// Additional detail about the surfaces stored.
            /// </summary>
            public DdsHeaderCaps2 Caps2;

            /// <summary>
            /// Unused.
            /// </summary>
            public uint Caps3;

            /// <summary>
            /// Unused.
            /// </summary>
            public uint Caps4;

            /// <summary>
            /// Unused.
            /// </summary>
            public uint Reserved2;
        }

        [Flags]
        public enum DdsHeaderFlags : uint
        {
            /// <summary>
            /// Required in every .dds file.
            /// </summary>
            Caps = 0x1,
            /// <summary>
            /// Required in every .dds file.
            /// </summary>
            Height = 0x2,
            /// <summary>
            /// Required in every .dds file.
            /// </summary>
            Width = 0x4,
            /// <summary>
            /// Required when pitch is provided for an uncompressed texture.
            /// </summary>
            Pitch = 0x8,
            /// <summary>
            /// Required in every .dds file.
            /// </summary>
            PixelFormat = 0x1000,
            /// <summary>
            /// Required in a mipmapped texture.
            /// </summary>
            MipMapCount = 0x20000,
            /// <summary>
            /// Required when pitch is provided for a compressed texture.
            /// </summary>
            LinearSize = 0x80000,
            /// <summary>
            /// Required in a depth texture.
            /// </summary>
            Depth = 0x800000
        }

        [Flags]
        public enum DdsHeaderCaps : uint
        {
            /// <summary>
            /// Optional; must be used on any file that contains more than one surface
            /// (a mipmap, a cubic environment map, or mipmapped volume texture).
            /// </summary>
            Complex = 0x8,
            /// <summary>
            /// Optional; should be used for a mipmap.
            /// </summary>
            MipMap = 0x400000,
            /// <summary>
            /// Required.
            /// </summary>
            Texture = 0x1000
        }

        [Flags]
        public enum DdsHeaderCaps2 : uint
        {
            /// <summary>
            /// Required for a cube map.
            /// </summary>
            Cubemap = 0x200,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            CubemapPositiveX = 0x400,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            CubemapNegativeX = 0x800,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            CubemapPositiveY = 0x1000,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            CubemapNegativeY = 0x2000,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            CubemapPositiveZ = 0x4000,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            CubemapNegativeZ = 0x8000,
            /// <summary>
            /// Required for a volume texture.
            /// </summary>
            Volume = 0x200000
        }
        #endregion

        #region DDSPF
        /// <summary>
        /// Surface pixel format.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DdsPixelFormat
        {
            /// <summary>
            /// Structure size; set to 32 (bytes).
            /// </summary>
            public uint Size;

            /// <summary>
            /// Values which indicate what type of data is in the surface.
            /// </summary>
            public DdsPixelFormatFlags Flags;

            /// <summary>
            /// Four-character codes for specifying compressed or custom formats.
            /// A FourCC of <see cref="DdsPixelFormatFourCc.DX10"/> indicates the
            /// presence of the <see cref="DdsHeaderDxt10"/> extended header,
            /// and <see cref="DdsHeaderDxt10.Format"/> indicates the true format.
            /// When using a four-character code, <see cref="Flags"/> must include
            /// <see cref="DdsPixelFormatFlags.FourCC"/>.
            /// </summary>
            public DdsPixelFormatFourCc FourCC;

            /// <summary>
            /// Number of bits in an RGB (possibly including alpha) format.
            /// Valid when <see cref="Flags"/> includes <see cref="DdsPixelFormatFlags.RGB"/>,
            /// <see cref="DdsPixelFormatFlags.Luminance"/>,or <see cref="DdsPixelFormatFlags.YUV"/>.
            /// </summary>
            public uint RGBBitCount;

            /// <summary>
            /// Red(or lumiannce or Y) mask for reading color data. For instance,
            /// given the A8R8G8B8 format, the red mask would be 0x00ff0000.
            /// </summary>
            public uint RBitMask;

            /// <summary>
            /// Green(or U) mask for reading color data. For instance,
            /// given the A8R8G8B8 format, the green mask would be 0x0000ff00.
            /// </summary>
            public uint GBitMask;

            /// <summary>
            /// Blue(or V) mask for reading color data. For instance,
            /// given the A8R8G8B8 format, the blue mask would be 0x000000ff.
            /// </summary>
            public uint BBitMask;

            /// <summary>
            /// Alpha mask for reading alpha data. <see cref="Flags"/> must include
            /// <see cref="DdsPixelFormatFlags.AlphaPixels"/> or <see cref="DdsPixelFormatFlags.Alpha"/>.
            /// For instance, given the A8R8G8B8 format, the alpha mask would be 0xff000000.
            /// </summary>
            public uint ABitMask;
        }

        [Flags]
        public enum DdsPixelFormatFlags : uint
        {
            /// <summary>
            /// Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
            /// </summary>
            AlphaPixels = 0x1,
            /// <summary>
            /// Used in some older DDS files for alpha channel only uncompressed data
            /// (<see cref="DdsPixelFormat.RGBBitCount"/> contains the alpha channel bitcount;
            /// <see cref="DdsPixelFormat.ABitMask"/> contains valid data)
            /// </summary>
            Alpha = 0x2,
            /// <summary>
            /// Texture contains compressed RGB data; <see cref="DdsPixelFormat.FourCC"/> contains valid data.
            /// </summary>
            FourCC = 0x4,
            /// <summary>
            /// Texture contains uncompressed RGB data;
            /// <see cref="DdsPixelFormat.RGBBitCount"/> and the RGB masks
            /// (<see cref="DdsPixelFormat.RBitMask"/>,
            /// <see cref="DdsPixelFormat.GBitMask"/>,
            /// <see cref="DdsPixelFormat.BBitMask"/>) contain valid data.
            /// </summary>
            RGB = 0x40,
            /// <summary>
            /// Used in some older DDS files for YUV uncompressed data
            /// (<see cref="DdsPixelFormat.RGBBitCount"/> contains the YUV bit count;
            /// <see cref="DdsPixelFormat.RBitMask"/> contains the Y mask,
            /// <see cref="DdsPixelFormat.GBitMask"/> contains the U mask,
            /// <see cref="DdsPixelFormat.BBitMask"/> contains the V mask)
            /// </summary>
            YUV = 0x200,
            /// <summary>
            /// Used in some older DDS files for single channel color uncompressed data
            /// (<see cref="DdsPixelFormat.RGBBitCount"/> contains the luminance channel bit count;
            /// <see cref="DdsPixelFormat.RBitMask"/> contains the channel mask).
            /// Can be combined with <see cref="AlphaPixels"/> for a two channel DDS file.
            /// </summary>
            Luminance = 0x20000
        }

        public enum DdsPixelFormatFourCc : uint
        {
            DXT1 = 0x31545844,
            DXT2 = 0x32545844,
            DXT3 = 0x33545844,
            DXT4 = 0x34545844,
            DXT5 = 0x35545844,
            DX10 = 0x30315844
        }
        #endregion

        private static Stream CreateWriteStream(string filePath, int width, int height, int depth, int mipmapCount, bool isCubemap)
        {
            DdsHeader header = default;
            header.Size = 124;
            header.Flags = DdsHeaderFlags.Caps | DdsHeaderFlags.Height
                                               | DdsHeaderFlags.MipMapCount | DdsHeaderFlags.PixelFormat
                                               | DdsHeaderFlags.Width | DdsHeaderFlags.Pitch;
            header.Height = (uint)height;
            header.Width = (uint)width;
            header.PitchOrLinearSize = header.Width * 32u;
            header.MipMapCount = (uint)mipmapCount;
            header.Depth = (uint)depth;
            header.Reserved1 = new uint[11];

            // DDSPF
            DdsPixelFormat pixelFormat = default;
            pixelFormat.Size = 32;
            pixelFormat.Flags = DdsPixelFormatFlags.AlphaPixels | DdsPixelFormatFlags.RGB;
            pixelFormat.RGBBitCount = 32u;
            pixelFormat.RBitMask = 0xffu;
            pixelFormat.GBitMask = 0xff00u;
            pixelFormat.BBitMask = 0xff0000u;
            pixelFormat.ABitMask = 0xff000000u;
            header.PixelFormat = pixelFormat;

            // DdsHeader second part, unset fields are zero
            header.Caps = DdsHeaderCaps.Complex | DdsHeaderCaps.MipMap | DdsHeaderCaps.Texture;
            header.Caps2 = DdsHeaderCaps2.Volume;

            if (isCubemap)
            {
                header.Caps2 = DdsHeaderCaps2.Cubemap | DdsHeaderCaps2.CubemapNegativeX |
                               DdsHeaderCaps2.CubemapNegativeY | DdsHeaderCaps2.CubemapNegativeZ |
                               DdsHeaderCaps2.CubemapPositiveX | DdsHeaderCaps2.CubemapPositiveY |
                               DdsHeaderCaps2.CubemapPositiveZ;
            }

            // Output DDS file
            var file = File.OpenWrite(filePath);
            file.Write(DdsMagic);

            // Write DDS header
            var headerSize = Marshal.SizeOf(header);
            var headerData = new byte[headerSize];
            var hHeaderData = GCHandle.Alloc(headerData, GCHandleType.Pinned);
            var pHeaderData = hHeaderData.AddrOfPinnedObject();
            Marshal.StructureToPtr(header, pHeaderData, true);
            hHeaderData.Free();
            file.Write(headerData);

            return file;
        }

        public static Stream CreateWriteStream(string filePath, AeImage image)
            => CreateWriteStream(filePath, image.Width, image.Height, 0, image.MipmapCount, false);

        public static Stream CreateWriteStream(string filePath, Ae4Texture texture)
            => CreateWriteStream(filePath, texture.Width, texture.Height, texture.Depth,
                texture.MipmapCount, texture.IsCubemap);
    }
}
