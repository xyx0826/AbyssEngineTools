using System;

namespace AEIEditor
{
	public static class ProgressManager
	{
		public static void SetMax(int max)
		{
			Update(0);
			MaxProgress = max < 2 ? -1 : max;
		}

		public static void Update(string caption, int max)
		{
			State = caption;
			SetMax(max);
		}

		public static void Update(string caption)
		{
			State = caption;
		}

		public static void Update(int current)
		{
			CurrentProgress = current;
		}

		public static void Update()
		{
			CurrentProgress++;
		}

		public static void Start(string caption, int max)
		{
			CurrentProgress = 0;
			SetMax(max);
			Update(caption);
			Active = true;
			UiManager.UpdateProgressUi();
		}

		public static void Start(string caption)
		{
			Start(caption, -1);
		}

		public static void End()
		{
			Active = false;
			Update("Done");
			UiManager.UpdateProgressUi();
		}

		public static void DrawConsoleProgress()
		{
			if (_consoleSpinnerState == 3)
			{
				_consoleSpinnerState = 1;
			}
			else
			{
				_consoleSpinnerState++;
			}
			Console.SetCursorPosition(0, Console.CursorTop);
			switch (_consoleSpinnerState)
			{
			case 0:
				Console.Write("-");
				break;
			case 1:
				Console.Write("\\");
				break;
			case 2:
				Console.Write("|");
				break;
			case 3:
				Console.Write("/");
				break;
			}
			if (MaxProgress != -1)
			{
				Console.CursorLeft++;
				Console.Write((100.0 * (CurrentProgress / (double)MaxProgress)).ToString("F") + " %            ");
				return;
			}
			Console.Write("            ");
		}

		private static int _consoleSpinnerState = 1;

		public static bool Active;

		public static int MaxProgress;

		public static int CurrentProgress;

		public static string State;
	}
}
