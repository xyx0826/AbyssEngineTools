namespace AEIEditor
{
	public class AeImageTextureSymbolData
	{
		public AeImageTextureSymbolData(int newSymbolGroupId, ushort newSymbol)
		{
			GroupId = newSymbolGroupId;
			Symbol = newSymbol;
		}

		public int GroupId;

		public ushort Symbol;
	}
}
