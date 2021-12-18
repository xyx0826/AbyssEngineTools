using System;
using System.Collections.Generic;
using System.Reflection;
using Kaamo.Texture.Enums;

namespace Kaamo.Texture.Utils
{
    /// <summary>
    /// Utilities for pixel format types and computations.
    /// </summary>
    internal static class PixelFormatUtils
    {
        private static readonly Dictionary<PixelFormat, int> BitsPerPixel = new()
        {
            { PixelFormat.Rgb8, 24 },
            { PixelFormat.Rgba4, 16 },
            { PixelFormat.Rgba8, 32 },
            { PixelFormat.Rgb5A1, 16 },
            { PixelFormat.Rg11B10F, 32 },
            { PixelFormat.Bgra8, 32 },
            { PixelFormat.Alpha8, 8 },
            { PixelFormat.Stencil8, 8 },
            { PixelFormat.Luminance8, 8 },
            { PixelFormat.LuminanceAlpha8, 16 },
            { PixelFormat.Depth32F, 32 },
            { PixelFormat.Depth24Stencil8, 32 },
            { PixelFormat.Rgb16Bpp, 16 },
            { PixelFormat.Rgb40Bpp, 40 },
            //{ PixelFormat.Bgra8Ui, 32 },
            //{ PixelFormat.Rgba8CubemapPc, 32 },
            //{ PixelFormat.Rgba8Cubemap, 32 }
        };

        private static readonly Dictionary<PixelFormat, int> BytesPerBlock = new()
        {
            { PixelFormat.PvrtcRgb2BppV1, 8 },
            { PixelFormat.PvrtcRgb4BppV1, 8 },
            { PixelFormat.PvrtcRgba2BppV1, 8 },
            { PixelFormat.PvrtcRgba4BppV1, 8 },
            { PixelFormat.PvrtcRgba2BppV2, 8 },
            { PixelFormat.PvrtcRgba4BppV2, 8 },
            { PixelFormat.AtcRgb, 8 },
            { PixelFormat.AtcRgbaExplicitAlpha, 16 },
            { PixelFormat.AtcRgbaInterpolatedAlpha, 16 },
            { PixelFormat.Etc1Rgb8, 8 },
            { PixelFormat.Etc2Rgb8, 8 },
            { PixelFormat.Etc2Rgba8, 16 },
            { PixelFormat.Etc2Rgb8A1, 8 },
            { PixelFormat.Dxt1Rgba, 8 },
            { PixelFormat.Dxt3Rgba, 16 },
            { PixelFormat.Dxt5Rgba, 16 }
        };

        private static Dictionary<Engine, Dictionary<byte, PixelFormat>> _mappings;

        /// <summary>
        /// Builds a lookup table of engine-specific pixel format enum values
        /// to corresponding enum fields.
        /// </summary>
        private static void LoadMappings()
        {
            // Populate mappings with engines
            _mappings = new Dictionary<Engine, Dictionary<byte, PixelFormat>>();
            foreach (var eng in Enum.GetValues<Engine>())
            {
                _mappings[eng] = new Dictionary<byte, PixelFormat>();
            }

            // Scan through all pixel format enum fields
            var pfInfos = typeof(PixelFormat).GetFields();
            foreach (var pfInfo in pfInfos)
            {
                foreach (var attr in pfInfo.GetCustomAttributes<PixelFormatAttribute>())
                {
                    _mappings[attr.EngineVersion][attr.Value] =
                        (PixelFormat)(pfInfo.GetRawConstantValue() ?? 0);
                }
            }
        }

        /// <summary>
        /// Computes the size of a pixel format with the specified dimensions.
        /// </summary>
        /// <param name="format">Pixel format.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="depth">Depth.</param>
        /// <returns>Computed size in bytes.</returns>
        public static int GetFormatSize(PixelFormat format, int width, int height, int depth)
        {
            if (BitsPerPixel.TryGetValue(format, out var bpp))
            {
                // Raw formats
                return height * width * depth * bpp / 8;
            }

            if (BytesPerBlock.TryGetValue(format, out var bpb))
            {
                switch (format)
                {
                    case PixelFormat.Rgb16Bpb1:
                    case PixelFormat.Rgb16Bpb2:
                        // Short circuit, we don't know these formats
                        return 0;
                    case PixelFormat.PvrtcRgb2BppV1:
                    case PixelFormat.PvrtcRgba2BppV1:
                        width = width >= 16 ? width : 16;
                        height = height >= 8 ? height : 8;
                        return (2 * width * height + 7) / bpb;
                    case PixelFormat.PvrtcRgb4BppV1:
                    case PixelFormat.PvrtcRgba4BppV1:
                        width = width >= 8 ? width : 8;
                        height = height >= 8 ? height : 8;
                        return (4 * width * height + 7) / bpb;
                }

                return ((width + 3)) / 4 * ((height + 3) / 4) * bpb;
            }

            throw new Exception($"Format {format} does not have an associated " +
                                "bits-per-pixel or bytes-per-block count.");
        }

        /// <summary>
        /// Computes the size of a face with the specified mipmap count.
        /// </summary>
        /// <param name="format">Pixel format.</param>
        /// <param name="mipmapCount">Count of mipmaps including the first layer.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="depth">Depth.</param>
        /// <returns></returns>
        public static int GetFaceSize(
            PixelFormat format, int mipmapCount, int width, int height, int depth)
        {
            var w = width;
            var h = height;
            var d = depth;
            var size = 0;
            for (var _ = 0; _ < mipmapCount; _++)
            {
                w = w > 0 ? w : 1;
                h = h > 0 ? h : 1;
                d = d > 0 ? d : 1;
                size += GetFormatSize(format, w, h, d);
                w /= 2;
                h /= 2;
                d /= 2;
            }

            return size;
        }

        /// <summary>
        /// Returns whether the specified pixel format is compressed.
        /// </summary>
        /// <param name="format">The format to check.</param>
        /// <returns>Whether the specified pixel format is compressed.</returns>
        public static bool IsCompressed(this PixelFormat format)
            => format is >= PixelFormat.PvrtcRgb2BppV1 and <= PixelFormat.Dxt5Rgba;

        public static int GetSize(
            PixelFormat format, int mipmapCount, int width, int height, int depth, bool isCubemap)
        {
            var size = GetFaceSize(format, mipmapCount, width, height, depth);
            return isCubemap ? size * 6 : size;
        }

        public static int GetLoadedSize(
            PixelFormat format, int mipmapCount, int width, int height, int depth, int lod)
        {
            var l = lod > -1 ? lod : 0;
            var w = width / Math.Pow(2, l);
            var h = width / Math.Pow(2, l);
            var d = width / Math.Pow(2, l);
            return GetFaceSize(format, mipmapCount - l, (int)w, (int)h, (int)d);
        }

        public static void GetMipmapDimensions(int width, int height, int depth, int level,
            out int mipmapWidth, out int mipmapHeight, out int mipmapDepth)
        {
            mipmapWidth = (int)(width / Math.Pow(2, level));
            mipmapHeight = (int)(height / Math.Pow(2, level));
            mipmapDepth = (int)(depth / Math.Pow(2, level));
        }

        public static int GetMipmapLevelSize(
            PixelFormat format, int width, int height, int depth, int level)
        {
            GetMipmapDimensions(width, height, depth, level,
                out var mw, out var mh, out var md);
            return GetFormatSize(format, mw, mh, md);
        }

        public static int GetOffset(
            PixelFormat format, int mipmapCount,
            int width, int height, int depth, int face, int level, bool isCubemap)
        {
            var offset = isCubemap
                ? GetSize(format, mipmapCount, width, height, depth, true) / 6 * face
                : 0;
            for (var i = 0; i < level; i++)
            {
                offset += GetMipmapLevelSize(format, width, height, depth, i);
            }

            return offset;
        }

        public static PixelFormat GetPixelFormat(Engine engine, byte value)
        {
            if (_mappings == null)
            {
                LoadMappings();
            }

            if (_mappings.TryGetValue(engine, out var formats))
            {
                if (formats.TryGetValue(value, out var format))
                {
                    return format;
                }
            }

            throw new Exception("Unknown engine version or format value.");
        }
    }
}
