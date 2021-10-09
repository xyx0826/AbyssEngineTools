using System;
using System.Runtime.InteropServices;

namespace AEIEditor
{
	public static class NativeMethods
	{
		[DllImport("kernel32.dll")]
		private static extern bool AttachConsole(uint dwProcessId);

		[DllImport("kernel32.dll")]
		private static extern bool FreeConsole();

		public static bool ConsoleFree()
		{
			return FreeConsole();
		}

		public static bool ConsoleAttach()
		{
			var flag = AttachConsole(uint.MaxValue);
			if (flag)
			{
				Console.WriteLine();
			}
			return flag;
		}

		private const uint AttachParentProcess = 4294967295U;
	}
}
