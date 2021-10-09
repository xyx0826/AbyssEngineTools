using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AEIEditor
{
	public static class Program
	{
		/// <summary>
		/// Validates the specified path.
		/// </summary>
		/// <param name="path">The path to validate.</param>
		/// <param name="createIfNotExists">Whether to create the path if it does not exist.</param>
		/// <returns>The validated path, or null if the path is invalid.</returns>
		public static string GetValidatedPath(string path, bool createIfNotExists = false)
        {
			if (Path.GetInvalidPathChars().Any(path.Contains))
			{
				UiManager.ShowMessage("The path contains invalid characters: " + path, "Checking the path");
				return null;
			}
			if (!Path.IsPathRooted(path))
			{
				// Convert relative to absolute
				path = Path.Combine(Application.StartupPath, path);
			}
			if (!Directory.Exists(path))
			{
				var pathRoot = Path.GetPathRoot(path);
				if (pathRoot != null && !Array.Exists(Directory.GetLogicalDrives(), a => a.Equals(pathRoot)))
				{
					UiManager.ShowMessage("The path is unreachable: " + path, "Checking the path");
					return null;
				}
				if (!createIfNotExists)
				{
					if (pathRoot != null)
					{
						if (Array.Exists(Directory.GetLogicalDrives(), a => a.Equals(pathRoot)) && Directory.Exists(path))
						{
							return path;
						}
					}
					UiManager.ShowMessage("Path not found: " + path, "Checking the path");
					return null;
				}
				Directory.CreateDirectory(path);
			}
			return path;
		}

		public static string ValidateFilePath(string path, string reservedExtension, bool ensureExists = true, bool createPathIfNotExists = false)
		{
			if (!Path.HasExtension(path))
			{
				path = Path.ChangeExtension(path, reservedExtension);
			}
			if (!File.Exists(path))
			{
				if (ensureExists)
				{
					UiManager.ShowMessage("File not found: " + path, "Checking file path");
					return null;
				}
				if (createPathIfNotExists && GetValidatedPath(Path.GetDirectoryName(path), createPathIfNotExists) == null)
				{
					return null;
				}
			}
			return path;
		}

		public static void Array_Insert<T>(T obj, ref T[] arr, int startIndex)
		{
			Array.Resize(ref arr, arr.Length + 1);
			for (var i = arr.Length - 1; i > startIndex; i--)
			{
				arr[i] = arr[i - 1];
			}
			arr[startIndex] = obj;
		}

		public static void Array_Delete<T>(ref T[] arr, int index)
		{
			var num = arr.Length - 1;
			for (var i = index; i < num; i++)
			{
				arr[i] = arr[i + 1];
			}
			Array.Resize(ref arr, num);
		}

		public static void DoAsync(Action act, string caption, int max)
		{
			ProgressManager.Start(caption, max);
			using (var task = Task.Factory.StartNew(act))
			{
				while (!task.IsCompleted)
				{
					UiManager.UpdateProgressUi(50L);
				}
				if (task.Exception != null)
				{
					throw task.Exception.InnerException;
				}
			}
			ProgressManager.End();
		}

		public static void DoAsync(Action act, string caption)
		{
			DoAsync(act, caption, -1);
		}

		public static T DoAsync<T>(Func<object, T> act, string caption, int max, object param)
		{
			ProgressManager.Start(caption, max);
			T result;
			using (var task = Task<T>.Factory.StartNew(act, param))
			{
				while (!task.IsCompleted)
				{
					UiManager.UpdateProgressUi(50L);
				}
				if (task.Exception != null)
				{
					throw task.Exception.InnerException;
				}
				result = task.Result;
			}
			ProgressManager.End();
			return result;
		}

		public static T DoAsync<T>(Func<object, T> act, string caption, object param)
		{
			return DoAsync(act, caption, -1, param);
		}

		public static ushort GetNearestPot(ushort x)
		{
			x -= 1;
			x |= (ushort)((uint)x >> 1);
			x |= (ushort)((uint)x >> 2);
			x |= (ushort)((uint)x >> 4);
			x |= (ushort)((uint)x >> 8);
			return (ushort) (x + 1);
		}

		public static void Switch_ARGB_ABGR(byte[] pixelData)
		{
			for (var i = 0; i < pixelData.Length; i += 4)
			{
				var b = pixelData[i];
				pixelData[i] = pixelData[i + 2];
				pixelData[i + 2] = b;
			}
		}

		public static Bitmap ReRenderBitmap(Bitmap bmp, int width, int height, PixelFormat pxFmt, bool hq = true)
		{
			Bitmap result;
			using (var bitmap = ReRenderBitmap(bmp, width, height, hq))
			{
				result = ReRenderBitmap(bitmap, pxFmt);
			}
			return result;
		}

		public static Bitmap ReRenderBitmap(Bitmap bmp, int width, int height, bool hq = true)
		{
			var bitmap = new Bitmap(width, height);
			using (var graphics = Graphics.FromImage(bitmap))
			{
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.DrawImage(bmp, new Rectangle(0, 0, width, height));
			}
			return bitmap;
		}

		public static Bitmap ReRenderBitmap(Bitmap bmp, PixelFormat pxFmt)
		{
			return bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), pxFmt);
		}

		[STAThread]
		private static int Main(string[] args)
		{
			var result = ConsoleResult.Ok;
			if (args != null && args.Length != 0)
			{
				if (!NativeMethods.ConsoleAttach())
				{
					return 1;
				}
				result = ConsoleManager.DoWork(args);
				NativeMethods.ConsoleFree();
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
			}
			return (int)result;
		}

		public const PixelFormat PxFmtFull = PixelFormat.Format32bppArgb;

		public const PixelFormat PxFmtColor = PixelFormat.Format32bppRgb;

		public const string DefaultTextureExtension = ".png";

		public static PixelFormat PxFmtCurrent = PixelFormat.Format32bppArgb;
	}
}
