using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CompressionLib;

namespace AEIEditor
{
	public static class Compression
    {
        public static bool IsFormatCompressed(AeFormat format)
        {
            var name = format.ToString();
            return !(format == AeFormat.Unknown || name.Contains("Raw"));
        }

        public static bool IsFormatCompressed(Ae2Format format)
            => IsFormatCompressed(format.ToAeFormat());

        public static bool IsFormatMipMappable(AeFormat format)
            => IsFormatCompressed(format);

        public static bool IsFormatMipMappable(Ae2Format format)
            => IsFormatCompressed(format.ToAeFormat());

		public static string[] GetFormatQualityName(Ae2Format compressionAe2Format)
		{
			if (compressionAe2Format <= Ae2Format.Pvrtci4A)
			{
				if (compressionAe2Format == Ae2Format.Pvrtci2A || compressionAe2Format == Ae2Format.Pvrtci4A)
				{
					return _pvrtcQualityNames;
				}
			}
			else
			{
				switch (compressionAe2Format)
				{
				case Ae2Format.Dxt1:
				case Ae2Format.Dxt3:
				case Ae2Format.Dxt5:
					return _dxtQualityNames;
				case (Ae2Format)34:
				case (Ae2Format)35:
					break;
				default:
					if (compressionAe2Format == Ae2Format.Etc1)
					{
						return _etcQualityNames;
					}
					break;
				}
			}
			return _defaultQualityNames;
		}

		public static string GetFormatName(Ae2Format compressionAe2Format)
		{
			if (compressionAe2Format <= Ae2Format.Dxt5)
			{
				switch (compressionAe2Format)
				{
				case Ae2Format.RawUi:
					return "None, interface";
				case (Ae2Format)2:
					break;
				case Ae2Format.Raw:
					return "None";
				default:
					switch (compressionAe2Format)
					{
					case Ae2Format.Pvrtci2A:
						return "PVRTC1 2bpp";
					case (Ae2Format)14:
					case (Ae2Format)15:
						break;
					case Ae2Format.Pvrtci4A:
						return "PVRTC1 4bpp";
					case Ae2Format.Atc:
						return "ATC 4bpp";
					default:
						switch (compressionAe2Format)
						{
						case Ae2Format.Dxt1:
							return "DXT1";
						case Ae2Format.Dxt3:
							return "DXT3";
						case Ae2Format.Dxt5:
							return "DXT5";
						}
						break;
					}
					break;
				}
			}
			else
			{
				if (compressionAe2Format == Ae2Format.Etc1)
				{
					return "ETC1 no Alpha";
				}
				if (compressionAe2Format == Ae2Format.RawCubeMapPc)
				{
					return "None, CubeMap(PC)";
				}
				if (compressionAe2Format == Ae2Format.RawCubeMap)
				{
					return "None, CubeMap";
				}
			}
			return "Unknown";
		}

		public static Ae2Format FormatFromArgument(string name)
		{
			switch (name)
			{
			case "RAW":
				return Ae2Format.Raw;
			case "RAW_UI":
				return Ae2Format.RawUi;
			case "RAW_CubeMap_PC":
				return Ae2Format.RawCubeMapPc;
			case "RAW_CubeMap":
				return Ae2Format.RawCubeMap;
			case "DXT1":
				return Ae2Format.Dxt1;
			case "DXT3":
				return Ae2Format.Dxt3;
			case "DXT5":
				return Ae2Format.Dxt5;
			case "ETC1":
				return Ae2Format.Etc1;
			case "PVRTCI2A":
				return Ae2Format.Pvrtci2A;
			case "PVRTCI4A":
				return Ae2Format.Pvrtci4A;
			case "ATC":
				return Ae2Format.Atc;
			}
			return Ae2Format.Unknown;
		}

		/// <summary>
		/// Returns whether the specified format is compressed with PVRTexLib,
		/// i.e. PVR or ETC.
		/// </summary>
		/// <param name="format">The format to check.</param>
		/// <returns>Whether the specified format is compressed with PVRTexLib.</returns>
        private static bool IsFormatCompressedWithPvrTexLib(PixelFormat format) => format switch
        {
            PixelFormat.Pvrtci2BppRgba or PixelFormat.Pvrtci4BppRgba
                or PixelFormat.Etc1 or PixelFormat.Etc2Rgb => true,
            _ => false
        };

		private static unsafe byte[] CompressAtc(byte[] data, int width, int height, int quality, Ae2Format _)
        {
            uint compSize;
			var ret = (AtcError)ATC.GetCompressedSize(width, height, &compSize);
			if (ret != AtcError.Success)
			{
				throw new Exception("Failed to determine data size. " + ret);
			}

			var output = new byte[compSize];
            using var pIn = new PinnedObject(data);
            using var pOut = new PinnedObject(output);

            var err = (AtcError)ATC.Compress(pOut.GetPointer(), (byte*)pIn.GetPointer(), width, height, &compSize);
            if (err != AtcError.Success)
            {
                throw new Exception("Failed to compress data. " + err);
            }

            return output;
		}

		private static unsafe byte[] CompressDxt(byte[] data, int width, int height, int quality, Ae2Format compressionAe2Format)
		{
            var flags = compressionAe2Format switch
            {
                Ae2Format.Dxt1 => 1,
                Ae2Format.Dxt3 => 2,
                Ae2Format.Dxt5 => 4,
				_ => throw new Exception(
                    "Unknown compression format " + compressionAe2Format + "\nContact the developer.")
            };
            var blockSize = compressionAe2Format switch
            {
                Ae2Format.Dxt1 => 8,
                Ae2Format.Dxt3 or Ae2Format.Dxt5 => 16,
				_ => throw new Exception(
                    "Unknown compression format " + compressionAe2Format + "\nContact the developer.")
            };
            flags |= quality switch
            {
                0 => 16 | 64,
                1 => 16 | 32,
                2 => 8 | 32,
                3 => 256 | 32,
                _ => throw new Exception(
                    "Unknown compression format " + compressionAe2Format + "\nContact the developer.")
            };

			int blockResolutionX;
			int blockResolutionY;
			var blockCountAndFixResolution = GetBlockCountAndFixResolution(width, height, out blockResolutionX, out blockResolutionY);
			var channelCount = 4;
			var blockMask = 0;
			var blockMap = new byte[blockResolutionX, blockResolutionY];
			byte b = 0;
			for (var i = 0; i < blockResolutionY; i++)
			{
				for (var j = 0; j < blockResolutionX; j++)
				{
					blockMask |= 1 << b;
					blockMap[j, i] = b;
					b += 1;
				}
			}
			var output = new byte[blockCountAndFixResolution * blockSize];
            using var pOut = new PinnedObject(output);
            var pResult = (byte*)pOut.GetPointer();
            Parallel.For(0, blockCountAndFixResolution, delegate(int blockId)
            {
                using (var pIn = new PinnedObject(GetPixelBlock(blockSize, data, blockId, blockResolutionX, blockResolutionY, channelCount, width, blockMap)))
                {
                    DXT.CompressBlock(pResult + blockId * (long)blockSize, (byte*)pIn.GetPointer(), blockMask, flags);
                }
                ProgressManager.Update();
            });
            return output;
		}

		private static unsafe byte[] PvrTexLibCompressor(byte[] data, int width, int height, int quality, Ae2Format format)
        {
            var pxFmt = format.ToAeFormat().ToPixelFormat();
            var textureDataSize = PVRTexLib.GetTextureDataSize(width, height, (int)pxFmt, false);
			var ret = 0U;
			var array = new byte[textureDataSize];
			using (var pinnedObject = new PinnedObject(data))
			{
				using (var pinnedObject2 = new PinnedObject(array))
				{
					ret = PVRTexLib.Compress(
                        (byte*)pinnedObject.GetPointer(),
                        pinnedObject2.GetPointer(),
                        width, height, (int)pxFmt, quality, false);
				}
			}
			if (ret != textureDataSize)
			{
				throw new Exception("Failed to compress data. " + format);
			}
			return array;
		}

		private static unsafe void GetMinDimensions(int width, int height, PixelFormat format, bool mipMapped, out int minWidth, out int minHeight)
        {
            var minW = 1u;
            var minH = 1u;
			if (format != PixelFormat.Unknown && IsFormatCompressedWithPvrTexLib(format))
			{ 
                PVRTexLib.GetMinDimensions(width, height, (int) format, mipMapped, &minW, &minH);
			}
			minWidth = (int)minW;
			minHeight = (int)minH;
		}

		public static int GetMipLevelCount(int width, int height, AeFormat format, bool mipMapped = false)
		{
			if (!IsFormatMipMappable(format))
			{
				return 1;
			}
			var pxFmt = format.ToPixelFormat();
			if (pxFmt != PixelFormat.Unknown)
			{
				return PVRTexLib.GetMipLevelCount(width, height, (int) pxFmt, mipMapped);
			}
			return GetMipLevelCount(width, height, 1, 1, mipMapped);
		}

		public static int GetMipLevelCount(int width, int height, int minWidth = 1, int minHeight = 1, bool mipMapped = false)
		{
			return PVRTexLib.GetMipLevelCount(width, height, minWidth, minHeight, mipMapped);
		}

		private static int GetBlockCountAndFixResolution(int width, int height, out int blockResolutionX, out int blockResolutionY, int minResX = 1, int minResY = 1, int maxResX = 4, int maxResY = 4)
		{
			var actualWidth = width < minResX ? minResX : width;
			var actualHeight = height < minResY ? minResY : height;
			blockResolutionX = actualWidth < maxResX ? actualWidth : maxResX;
			blockResolutionY = actualHeight < maxResY ? actualHeight : maxResY;
			return width * height / (blockResolutionX * blockResolutionY);
		}

		private static int GetBlockCount(int width, int height, int minResX = 1, int minResY = 1, int maxResX = 4, int maxResY = 4)
		{
			int num;
			int num2;
			return GetBlockCountAndFixResolution(width, height, out num, out num2, minResX, minResY, maxResX, maxResY);
		}

		private static byte[] GetBitmapPixels(Bitmap bmp)
		{
			var format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
			var width = bmp.Width;
			var height = bmp.Height;
			var bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
			var num = width * height * 4;
			var array = new byte[num];
			Marshal.Copy(bitmapData.Scan0, array, 0, num);
			bmp.UnlockBits(bitmapData);
			Program.Switch_ARGB_ABGR(array);
			return array;
		}

		private static byte[] GetPixelBlock(int blockSize, byte[] pixelData, int blockId, int blockResolutionX, int blockResolutionY, int channelCount, int width, byte[,] blockMap)
		{
			var array = new byte[blockResolutionX * blockResolutionY * channelCount];
			var num = width / blockResolutionX;
			var num2 = blockId / num;
			var num3 = (blockId - num2 * num) * blockResolutionX;
			var num4 = num2 * blockResolutionY;
			for (var i = 0; i < blockResolutionX; i++)
			{
				for (var j = 0; j < blockResolutionY; j++)
				{
					var sourceIndex = channelCount * ((num4 + j) * width + num3 + i);
					var destinationIndex = blockMap[i, j] * channelCount;
					Array.Copy(pixelData, sourceIndex, array, destinationIndex, channelCount);
				}
			}
			return array;
		}

		private static byte[][] GetBitmapMipData(Bitmap bmp, bool mipMapped, int quality, CompressorDelegate compressor, Ae2Format format, bool trackBlockProgress)
		{
			var num = bmp.Width;
			var num2 = bmp.Height;
			int num3;
			int num4;
            var pxFmt = format.ToAeFormat().ToPixelFormat();
			GetMinDimensions(num, num2, pxFmt, mipMapped, out num3, out num4);
			var flag = num < num3;
			var flag2 = num2 < num4;
			if (flag || flag2)
			{
				if (flag && flag2)
				{
					num = num3;
					num2 = num4;
				}
				else if (flag)
				{
					num = num3;
				}
				else
				{
					num2 = num4;
				}
				bmp = Program.ReRenderBitmap(bmp, num, num2);
			}
			var mipLevelCount = GetMipLevelCount(num, num2, num3, num4, mipMapped);
			var mipLevelCount2 = GetMipLevelCount(num3 / 2, num4 / 2, 1, 1, mipMapped);
			var num5 = mipLevelCount + mipLevelCount2;
			ProgressManager.SetMax(num5);
			var array = new Bitmap[num5];
			var levelWidths = new int[num5];
			var levelHeights = new int[num5];
			var bitmapLevelDatas = new byte[num5][];
			var num6 = 0;
			for (var i = 0; i < num5; i++)
			{
				levelWidths[i] = num;
				levelHeights[i] = num2;
				array[i] = bmp;
				bitmapLevelDatas[i] = GetBitmapPixels(bmp);
				if (trackBlockProgress)
				{
					num6 += GetBlockCount(num, num2, num3, num4) + 1;
				}
				else
				{
					num6++;
				}
				var num7 = i + 1;
				if (num7 < mipLevelCount && num7 < mipLevelCount)
				{
					num /= 2;
					num2 /= 2;
					bmp = Program.ReRenderBitmap(bmp, num, num2);
				}
				ProgressManager.Update();
			}
			var result = new byte[num5][];
			ProgressManager.Update("Texture conversion");
			ProgressManager.SetMax(num6);
			Parallel.For(0, num5, delegate(int mipLevelId)
			{
				result[mipLevelId] = compressor(bitmapLevelDatas[mipLevelId], levelWidths[mipLevelId], levelHeights[mipLevelId], quality, format);
				ProgressManager.Update();
			});
			foreach (var image in array)
			{
				image.Dispose();
			}
			return result;
		}

		private static byte[] GetBitmapData(Bitmap bmp, bool mipMapped, int quality, CompressorDelegate compressor, Ae2Format compressionAe2Format, bool trackBlockProgress = true)
		{
			var bitmapMipData = GetBitmapMipData(bmp, mipMapped, quality, compressor, compressionAe2Format, trackBlockProgress);
			var num = 0;
			foreach (var array2 in bitmapMipData)
			{
				num += array2.Length;
			}
			var array3 = new byte[num];
			var num2 = 0;
			foreach (var array5 in bitmapMipData)
			{
				var num3 = array5.Length;
				Array.Copy(array5, 0, array3, num2, num3);
				num2 += num3;
			}
			return array3;
		}

		public static byte[] Compress(Bitmap bmp, bool mipMapped, int quality, Ae2Format compressionAe2Format)
		{
			if (compressionAe2Format <= Ae2Format.Dxt5)
			{
				switch (compressionAe2Format)
				{
				case Ae2Format.RawUi:
				case Ae2Format.Raw:
					break;
				case (Ae2Format)2:
					goto IL_EE;
				default:
					switch (compressionAe2Format)
					{
					case Ae2Format.Pvrtci2A:
					case Ae2Format.Pvrtci4A:
						goto IL_D7;
					case (Ae2Format)14:
					case (Ae2Format)15:
						goto IL_EE;
					case Ae2Format.Atc:
						return GetBitmapData(bmp, mipMapped, quality, CompressAtc, compressionAe2Format, false);
					default:
						switch (compressionAe2Format)
						{
						case Ae2Format.Dxt1:
							return GetBitmapData(bmp, mipMapped, quality, CompressDxt, compressionAe2Format);
						case Ae2Format.Dxt3:
							return GetBitmapData(bmp, mipMapped, quality, CompressDxt, compressionAe2Format);
						case (Ae2Format)34:
						case (Ae2Format)35:
							goto IL_EE;
						case Ae2Format.Dxt5:
							return GetBitmapData(bmp, mipMapped, quality, CompressDxt, compressionAe2Format);
						default:
							goto IL_EE;
						}
					}
				}
			}
			else
			{
				if (compressionAe2Format == Ae2Format.Etc1)
				{
					goto IL_D7;
				}
				if (compressionAe2Format != Ae2Format.RawCubeMapPc && compressionAe2Format != Ae2Format.RawCubeMap)
				{
					goto IL_EE;
				}
			}
			return GetBitmapPixels(bmp);
			IL_D7:
			return GetBitmapData(bmp, mipMapped, quality, PvrTexLibCompressor, compressionAe2Format, false);
			IL_EE:
			throw new Exception("Unknown compression format " + compressionAe2Format + "\nContact the developer.");
		}

		public static unsafe Bitmap Deсompress(byte[] data, int width, int height, Ae2Format format, bool mipMapped)
		{
			byte[] output;
            var aeFmt = format.ToAeFormat();
			if (IsFormatCompressed(aeFmt))
            {
                var pxFmt = aeFmt.ToPixelFormat();
				output = new byte[width * height * 4];
                using var pIn = new PinnedObject(data);
                using var pOut = new PinnedObject(output);
                if (format == Ae2Format.Atc)
                {
                    var ret = (AtcError) ATC.Decompress(
                        pIn.GetPointer(), (byte*)pOut.GetPointer(), width, height);
                    if (ret != AtcError.Success)
                    {
                        throw new Exception("Failed to decompress data " + GetFormatName(format) + ". " + ret);
                    }
                }
                else if (!PVRTexLib.Decompress(pIn.GetPointer(), (byte*)pOut.GetPointer(), width, height, (int)pxFmt))
                {
                    throw new Exception("Failed to decompress data " + GetFormatName(format));
                }
            }
            else
			{
				output = data;
			}
			Program.Switch_ARGB_ABGR(output);
			var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
			Marshal.Copy(output, 0, bitmapData.Scan0, output.Length);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		internal static readonly Ae2Format[] Formats = {
			Ae2Format.Raw,
			Ae2Format.RawUi,
			Ae2Format.RawCubeMapPc,
			Ae2Format.RawCubeMap,
			Ae2Format.Dxt1,
			Ae2Format.Dxt3,
			Ae2Format.Dxt5,
			Ae2Format.Etc1,
			Ae2Format.Pvrtci2A,
			Ae2Format.Pvrtci4A,
			Ae2Format.Atc
		};

		private static readonly string[] _defaultQualityNames = {
			"None",
			"None",
			"None",
			"None"
		};

		private static readonly string[] _dxtQualityNames = {
			"Low without color correction",
			"Low",
			"Medium",
			"High"
		};

		private static readonly string[] _etcQualityNames = {
			"Low without color correction",
			"Low",
			"High without color correction",
			"High"
		};

		private static readonly string[] _pvrtcQualityNames = {
			"Low",
			"Medium",
			"High",
			"Best"
		};

		private enum DxtVersion
		{
			Dxt1 = 1,
			Dxt3,
			Dxt5 = 4
		}

		private enum DxtQuality
		{
			ColourClusterFit = 8,
			ColourRangeFit = 16,
			ColourIterativeClusterFit = 256
		}

		private enum DxtMetric
		{
			ColourMetricPerceptual = 32,
			ColourMetricUniform = 64
		}

		/// <summary>
		/// PVRTexLib pixel formats.
		/// </summary>
		public enum PixelFormat
		{
			Pvrtci2BppRgb,
			Pvrtci2BppRgba,
			Pvrtci4BppRgb,
			Pvrtci4BppRgba,
			Pvrtcii2Bpp,
			Pvrtcii4Bpp,
			Etc1,
			Dxt1,
			Dxt2,
			Dxt3,
			Dxt4,
			Dxt5,
			Uyvy = 16,
			Yuy2,
			Bw1Bpp,
			SharedExponentR9G9B9E5,
			Rgbg8888,
			Grgb8888,
			Etc2Rgb,
			Etc2Rgba,
			Etc2RgbA1,
			EacR11,
			EacRg11,
			Unknown
		}

		/// <summary>
		/// Abyss Engine 2 (Galaxy on Fire 2) formats.
		/// </summary>
		public enum Ae2Format : byte
		{
            Unknown,
            RawUi,
            Raw = 3,
            Pvrtci2A = 13,
            Pvrtci4A = 16,
            Atc,
            Dxt1 = 32,
            Dxt3,
            Dxt5 = 36,
            Etc1 = 64,
            RawCubeMapPc = 129,
            RawCubeMap = 194
        }

		/// <summary>
		/// Abyss Engine 4 (Galaxy on Fire: Manticore) formats.
		/// </summary>
        public enum Ae4Format : byte
        {
			Pvrtci4 = 0x0a,
			Pvrtci4A = 0x0c,
			Etc2Rgb = 0x10
        }

		/// <summary>
		/// A summary of Abyss Engine formats.
		/// </summary>
        public enum AeFormat
		{
			Unknown,
            RawUi,
            Raw,
            Pvrtci2A,
            Pvrtci4A,
            Atc,
            Dxt1,
            Dxt3,
            Dxt5,
            Etc1,
			Etc2Rgb,
            RawCubeMapPc,
            RawCubeMap
		}

		/// <summary>
		/// Converts an <see cref="Ae2Format"/> to a <see cref="AeFormat"/>.
		/// </summary>
		/// <param name="format">The format to convert.</param>
		/// <returns>The converted format,
		/// or <see cref="AeFormat.Unknown"/> if no valid mapping is available.</returns>
		public static AeFormat ToAeFormat(this Ae2Format format)
        {
            var name = format.ToString();
            if (Enum.TryParse<AeFormat>(name, out var aeFormat))
            {
                return aeFormat;
            }

            return AeFormat.Unknown;
        }

		/// <summary>
		/// Converts an <see cref="Ae4Format"/> to a <see cref="AeFormat"/>.
		/// </summary>
		/// <param name="format">The format to convert.</param>
		/// <returns>The converted format,
		/// or <see cref="AeFormat.Unknown"/> if no valid mapping is available.</returns>
		public static AeFormat ToAeFormat(this Ae4Format format)
        {
            var name = format.ToString();
            if (Enum.TryParse<AeFormat>(name, out var aeFormat))
            {
                return aeFormat;
            }

            return AeFormat.Unknown;
		}

        /// <summary>
        /// Converts an <see cref="AeFormat"/> to a <see cref="PixelFormat"/>.
        /// </summary>
        /// <param name="format">The format to convert.</param>
        /// <returns>The converted format,
        /// or <see cref="PixelFormat.Unknown"/> if no valid mapping is available.</returns>
		public static PixelFormat ToPixelFormat(this AeFormat format) => format switch
        {
            AeFormat.Pvrtci2A => PixelFormat.Pvrtci2BppRgba,
            AeFormat.Pvrtci4A => PixelFormat.Pvrtci4BppRgba,
            AeFormat.Dxt1 => PixelFormat.Dxt1,
            AeFormat.Dxt3 => PixelFormat.Dxt3,
            AeFormat.Dxt5 => PixelFormat.Dxt5,
            AeFormat.Etc1 => PixelFormat.Etc1,
            AeFormat.Etc2Rgb => PixelFormat.Etc2Rgb,
            _ => PixelFormat.Unknown
        };

        private enum AtcError : uint
		{
			Success,
			BadDimensions,
			BufferTooSmall,
			BadFlags,
			BadSignature,
			BadColorChannelOrder,
			LowQualityResult,
			BadArguments,
			CannotOpenFile,
			MemoryAllocationFailed
		}

		private delegate byte[] CompressorDelegate(byte[] data, int width, int height, int quality, Ae2Format format);
	}
}
