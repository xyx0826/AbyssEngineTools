using System.Drawing;
using System.IO;

namespace AEIEditor
{
	public class AeImageTexture
	{
		public AeImageTexture(Point l, Size s, int i)
		{
			Location = l;
			Size = s;
			Id = i;
		}

		public AeImageTexture(Point l, int i, bool isSymbolRegion)
		{
			Location = l;
			Size = default(Size);
			Id = i;
			if (!isSymbolRegion)
			{
				return;
			}
			int newSymbolGroupId;
			ushort newSymbol;
			UiManager.GetNewSymbolData(out newSymbolGroupId, out newSymbol);
			SymbolData = new AeImageTextureSymbolData(newSymbolGroupId, newSymbol);
		}

		public AeImageTexture(Size s, int i)
		{
			Location = default;
			Size = s;
			Id = i;
		}

		public AeImageTexture(BinaryReader br, int i, bool isSymbol = false, int newSymbolGroupId = 0, ushort newSymbol = 0)
		{
			var x = br.ReadUInt16();
			var y = br.ReadUInt16();
			var w = br.ReadUInt16();
			var h = br.ReadUInt16();
			if (isSymbol)
			{
				SymbolData = new AeImageTextureSymbolData(newSymbolGroupId, newSymbol);
			}
			Location = new Point(x, y);
			Size = new Size(w, h);
			Id = i;
		}

		public AeImageTexture(AeImageTexture src, int i)
		{
			SymbolData = src.SymbolData;
			Location = src.Location;
			Size = src.Size;
			Id = i;
		}

		public void Write(BinaryWriter bw)
		{
			bw.Write((ushort)Location.X);
			bw.Write((ushort)Location.Y);
			bw.Write((ushort)Size.Width);
			bw.Write((ushort)Size.Height);
		}

		public AeImageTextureSymbolData SymbolData;

		public Point Location;

		public Size Size;

		public int Id;

		public bool IsSelected;
	}
}
