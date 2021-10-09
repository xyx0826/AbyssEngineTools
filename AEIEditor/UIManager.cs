using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AEIEditor
{
	public static class UiManager
	{
		public static void UpdateProgressUi(long waitTime = 0L)
		{
			var flag = waitTime != 0L;
			var flag2 = !flag;
			if (flag)
			{
				var timestamp = Stopwatch.GetTimestamp();
				if (timestamp - _lastUpdateTime >= waitTime)
				{
					_lastUpdateTime = timestamp;
					flag2 = true;
				}
			}
			if (flag2)
			{
				if (ShowUi)
				{
					_tForm.StatusProgress.Visible = ProgressManager.Active;
					var maxProgress = ProgressManager.MaxProgress;
					if (maxProgress < 0)
					{
						_tForm.StatusProgress.Style = ProgressBarStyle.Marquee;
					}
					else
					{
						_tForm.StatusProgress.Style = ProgressBarStyle.Continuous;
						_tForm.StatusProgress.Maximum = maxProgress;
					}
					_tForm.StatusProgress.Value = ProgressManager.CurrentProgress;
					_tForm.StatusState.Text = ProgressManager.State;
				}
				else
				{
					ProgressManager.DrawConsoleProgress();
				}
			}
			Application.DoEvents();
		}

		public static void Init(MainForm f, AeScene s)
		{
			_tForm = f;
			_scene = s;
		}

		public static void SetState(string txt)
		{
			_tForm.StatusState.Text = txt;
		}

		public static void SetStatusScale(double val)
		{
			_tForm.StatusScale.Text = "Scale: " + val + "%";
		}

		public static void UpdateEditor(AeImageTexture texture)
		{
			var symbolData = texture.SymbolData;
			if (symbolData != null)
			{
				_tForm.RegionTip.Text = "Symbol texture";
				_tForm.ReginSymbolGroupTip.Text = "group №" + (symbolData.GroupId + 1);
				ToolStripItem reginSymbolEditor = _tForm.ReginSymbolEditor;
				var symbol = (char)symbolData.Symbol;
				reginSymbolEditor.Text = symbol.ToString();
			}
			else
			{
				_tForm.RegionTip.Text = "Texture fragment №" + (texture.Id + 1);
			}
			var location = texture.Location;
			var size = texture.Size;
			_tForm.RegionPosX.Text = location.X.ToString();
			_tForm.RegionPosY.Text = location.Y.ToString();
			_tForm.RegionSizeX.Text = size.Width.ToString();
			_tForm.RegionSizeY.Text = size.Height.ToString();
		}

		public static void ShowViewportContextMenu(Point pos)
		{
			_tForm.RegionListContextMenu.Show(pos);
		}

		private static void SetControlsAvailability(ToolStripItem[] controls, bool flag)
		{
			if (flag)
			{
				foreach (var toolStripItem in controls)
				{
					toolStripItem.Enabled = toolStripItem.Visible = true;
				}
				return;
			}
			for (var j = controls.Length - 1; j >= 0; j--)
			{
				var toolStripItem2 = controls[j];
				toolStripItem2.Enabled = toolStripItem2.Visible = false;
			}
		}

		public static void ShowRegionControls(AeImageTexture[] selectedTextures)
		{
			if (!ShowUi)
			{
				return;
			}
			var num = selectedTextures.Length;
			var flag = num != 0;
			var flag2 = num == 1;
			AeImageTextureSymbolData aeimageTextureSymbolData = null;
			if (flag)
			{
				var aeimageTexture = selectedTextures[0];
				if (flag2)
				{
					UpdateEditor(aeimageTexture);
				}
				else
				{
					_tForm.RegionTip.Text = "Selected texture fragments: " + num;
				}
				aeimageTextureSymbolData = aeimageTexture.SymbolData;
			}
			SetControlsAvailability(_tForm.BaseRegionControls, flag);
			SetControlsAvailability(_tForm.SymbolRegionControls, aeimageTextureSymbolData != null && flag2);
			SetControlsAvailability(_tForm.RegionEditControls, flag2);
			SetControlsAvailability(_tForm.RegionIoControls, flag);
		}

		/// <summary>
		/// Shows a message on the console and as a message box.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="caption">The caption.</param>
		public static void ShowMessage(string message, string caption)
        {
            Console.WriteLine($"<{caption}>\t{message}");
            if (ShowUi)
			{
				MessageBox.Show(message, caption);
			}
		}

		public static void GetNewSymbolData(out int newSymbolGroupId, out ushort newSymbol)
		{
			newSymbolGroupId = 0;
			newSymbol = 0;
			foreach (var aeimageTexture in _scene.GetTextures())
			{
				var symbolData = aeimageTexture.SymbolData;
				if (symbolData != null)
				{
					var groupId = symbolData.GroupId;
					var symbol = symbolData.Symbol;
					if (groupId > newSymbolGroupId)
					{
						newSymbolGroupId = groupId;
					}
					if (newSymbolGroupId == groupId && symbol > newSymbol)
					{
						newSymbol = symbol;
					}
				}
			}
			if (newSymbol > 65535)
			{
				return;
			}
			newSymbol += 1;
		}

		private static long _lastUpdateTime;

		public static bool ShowUi = true;

		private static MainForm _tForm;

		private static AeScene _scene;
	}
}
