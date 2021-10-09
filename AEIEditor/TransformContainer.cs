using System.Drawing;

namespace AEIEditor
{
	public class TransformContainer
	{
		public void CalculateScale(Size size)
		{
			var num = 16;
			var num2 = 1;
			var width = size.Width;
			var height = size.Height;
			var i = width < height ? width : height;
			while (i > num)
			{
				i /= 2;
				num2++;
			}
			InitialScale = CurrentScale = num2;
		}

		public int InitialScale;

		public int CurrentScale;
	}
}
