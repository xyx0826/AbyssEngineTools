using System;
using System.Drawing;
using System.IO;

namespace AEIEditor
{
    public struct AeImageHeader
    {

    }

	/// <summary>
	/// An Abyss Engine image file which may contain multiple textures.
	/// </summary>
	public class AeImage : IDisposable
	{
		public AeImage(AeImage aeRes, Bitmap tBmp, string thePath)
		{
			Path = Program.ValidateFilePath(thePath, ".aei", false);
			if (Path == null)
			{
				return;
			}
			Size = new Size(tBmp.Width, tBmp.Height);
			Bmp = tBmp;
			Transform.CalculateScale(Size);
			if (aeRes != null)
			{
				Quality = aeRes.Quality;
				MipMapped = aeRes.MipMapped;
				CompressionAe2Format = aeRes.CompressionAe2Format;
				Textures = aeRes.Textures;
				aeRes.Dispose();
				return;
			}
			CompressionAe2Format = Compression.Ae2Format.Raw;
			Textures = new AeImageTexture[1];
			Textures[0] = new AeImageTexture(Size, 0);
		}

		/// <summary>
		/// Creates a new image from the specified path.
		/// </summary>
		/// <param name="path">Path to image file.</param>
		public AeImage(string path)
		{
			Path = path;
			using (var reader = new BinaryReader(File.OpenRead(Path)))
			{
				// Check magic
				var text = new string(reader.ReadChars(8));
				if (text != "AEimage\0")
				{
					throw new Exception("Header's unknown: " + text);
				}

				// Check format
				var b = reader.ReadByte();
                if (Enum.IsDefined(typeof(Compression.Ae2Format), b))
                {
					// Format valid
					CompressionAe2Format = (Compression.Ae2Format)b;
                    MipMapped = false;
                }
                else
                {
					// Try again with first two bits removed (isMipmapped)
                    b -= 2;
                    if (Enum.IsDefined(typeof(Compression.Ae2Format), b))
                    {
						CompressionAe2Format = (Compression.Ae2Format)b;
                        MipMapped = Compression.IsFormatMipMappable(CompressionAe2Format);
                    }
                    else
                    {
                        return;
                    }
                }

				var w = reader.ReadUInt16();
				var h = reader.ReadUInt16();
                var n = reader.ReadUInt16();
				Size = new Size(w, h);
				Transform.CalculateScale(Size);
				Textures = new AeImageTexture[n];
				for (var i = 0; i < n; i++)
				{
					Textures[i] = new AeImageTexture(reader, i);
				}
				var len = Compression.IsFormatCompressed(CompressionAe2Format) ? reader.ReadInt32() : h * w * 4;
				ProgressManager.Update("Reading texture");
				var rawimageData = reader.ReadBytes(len);
				ProgressManager.Update("Texture conversion");
				Bmp = Compression.Deсompress(rawimageData, w, h, CompressionAe2Format, MipMapped);
				var num4 = reader.ReadUInt16();
				for (var j = 0; j < num4; j++)
				{
					var nSymbols = reader.ReadUInt16();
					var symbols = new ushort[nSymbols];
					for (var k = 0; k < nSymbols; k++)
					{
						symbols[k] = reader.ReadUInt16();
					}
					var num6 = Textures.Length;
					Array.Resize(ref Textures, num6 + nSymbols);
					for (var l = 0; l < nSymbols; l++)
					{
						var i = num6 + l;
						Textures[i] = new AeImageTexture(reader, i, true, j, symbols[l]);
					}
				}
				if (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					Quality = reader.ReadByte();
				}
				else
				{
					Quality = 3;
				}
			}
		}

		public void Write()
		{
			var num = Textures.Length;
			var array = new AeImageTexture[num];
			var num2 = 0;
			var num3 = 0;
			foreach (var aeimageTexture in Textures)
			{
				var symbolData = aeimageTexture.SymbolData;
				if (symbolData != null)
				{
					var num4 = symbolData.GroupId + 1;
					if (num4 > num3)
					{
						num3 = num4;
					}
				}
			}
			var array2 = new AeImageTexture[num3][];
			var array3 = new int[num3];
			for (var j = 0; j < num3; j++)
			{
				array2[j] = new AeImageTexture[num];
				array3[j] = 0;
			}
			foreach (var aeimageTexture2 in Textures)
			{
				var symbolData2 = aeimageTexture2.SymbolData;
				if (symbolData2 != null)
				{
					var groupId = symbolData2.GroupId;
					array2[groupId][array3[groupId]] = aeimageTexture2;
					array3[groupId]++;
				}
				else
				{
					array[num2] = aeimageTexture2;
					num2++;
				}
			}
			for (var l = 0; l < num3; l++)
			{
				Array.Resize(ref array2[l], array3[l]);
			}
			Array.Resize(ref array, num2);
			using (var binaryWriter = new BinaryWriter(File.OpenWrite(Path)))
			{
				binaryWriter.Write("AEimage\0".ToCharArray());
				binaryWriter.Write((byte)(MipMapped ? CompressionAe2Format + 2 : CompressionAe2Format));
				binaryWriter.Write((ushort)Size.Width);
				binaryWriter.Write((ushort)Size.Height);
				binaryWriter.Write((ushort)num2);
				foreach (var aeimageTexture3 in array)
				{
					aeimageTexture3.Write(binaryWriter);
				}
				ProgressManager.Update("Preparing texture");
				var array5 = Compression.Compress(new Bitmap(Bmp), MipMapped, Quality, CompressionAe2Format);
				ProgressManager.SetMax(-1);
				ProgressManager.Update("Writing texture");
				if (Compression.IsFormatCompressed(CompressionAe2Format))
				{
					binaryWriter.Write(array5.Length);
				}
				binaryWriter.Write(array5);
				binaryWriter.Write((ushort)num3);
				foreach (var array7 in array2)
				{
					binaryWriter.Write((ushort)array7.Length);
					foreach (var aeimageTexture4 in array7)
					{
						binaryWriter.Write(aeimageTexture4.SymbolData.Symbol);
					}
					foreach (var aeimageTexture5 in array7)
					{
						aeimageTexture5.Write(binaryWriter);
					}
				}
				binaryWriter.Write(Quality);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || Bmp == null)
			{
				return;
			}
			Bmp.Dispose();
		}

		public byte Quality = 3;

		public TransformContainer Transform = new TransformContainer();

		public string Path;

		public Compression.Ae2Format CompressionAe2Format;

		public bool MipMapped;

		public Size Size;

		public AeImageTexture[] Textures;

		public Bitmap Bmp;
	}
}
