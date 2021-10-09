using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace AEIEditor
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void checkerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			gradientToolStripMenuItem.Checked = false;
			checkerToolStripMenuItem.Checked = true;
			whiteToolStripMenuItem.Checked = false;
			blackToolStripMenuItem.Checked = false;
			_scene.SetBackGround(2);
			_scene.WorkEnd();
		}

		private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			gradientToolStripMenuItem.Checked = false;
			checkerToolStripMenuItem.Checked = false;
			whiteToolStripMenuItem.Checked = true;
			blackToolStripMenuItem.Checked = false;
			_scene.SetBackGround(3);
			_scene.WorkEnd();
		}

		private void blackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			gradientToolStripMenuItem.Checked = false;
			checkerToolStripMenuItem.Checked = false;
			whiteToolStripMenuItem.Checked = false;
			blackToolStripMenuItem.Checked = true;
			_scene.SetBackGround(4);
			_scene.WorkEnd();
		}

		private void gradientToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			gradientToolStripMenuItem.Checked = true;
			checkerToolStripMenuItem.Checked = false;
			whiteToolStripMenuItem.Checked = false;
			blackToolStripMenuItem.Checked = false;
			_scene.SetBackGround(1);
			_scene.WorkEnd();
		}

		private void Form1_PostLoad(object sender, EventArgs e)
        {
            var xOffset = 12;
            var yOffset = 79;
			_scene = new AeScene(new Point(xOffset, yOffset), new Size(Width - xOffset - 28, Status.Location.Y - yOffset - 3));
			_scene.WorkStart();
			UiManager.Init(this, _scene);
			Controls.Add(_scene);
			BaseRegionControls = new ToolStripItem[]
			{
				RegionSeparator,
				RegionTip
			};
			SymbolRegionControls = new ToolStripItem[]
			{
				ReginSymbolEditor,
				ReginSymbolGroupTip
			};
			RegionEditControls = new ToolStripItem[]
			{
				RegionUp,
				RegionDown,
				RegionEditSeparator,
				RegionPosTip,
				RegionPosY,
				RegionPosSplitter,
				RegionPosX,
				RegionSizeTip,
				RegionSizeY,
				RegionSizeSplitter,
				RegionSizeX
			};
			RegionIoControls = new ToolStripItem[]
			{
				RegionIOSeparator,
				RegionImportBox,
				RegionExportBox
			};
			CompressionList.BeginUpdate();
			foreach (var compressionFormat in Compression.Formats)
			{
				CompressionList.Items.Add(Compression.GetFormatName(compressionFormat));
			}
			CompressionList.SelectedIndex = 0;
			CompressionList.EndUpdate();
			_scene.WorkEnd();
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
            if (_scene != null)
			{
				// Hold off until after PostLoad
				_scene.Width = Width - _scene.Location.X - 28;
                _scene.Height = Status.Location.Y - _scene.Location.Y - 3;
			}
		}

		private void MainForm_Closing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !_scene.IsReady();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			SizeField.Visible = false;
			NameField.Visible = false;
			CompressionList.SelectedIndex = 0;
			MipMapCheck.Checked = false;
			_scene.UnLoad();
			_scene.WorkEnd();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "AEI files (*.aei)|*.aei";
				openFileDialog.RestoreDirectory = true;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					_scene.LoadResource(openFileDialog.FileName);
					UpdateUi();
				}
			}
			_scene.WorkEnd();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			if (_scene.IsLoaded())
			{
				_scene.GetResource().Write();
			}
			else
			{
				UiManager.ShowMessage("File not loaded", "Error");
			}
			_scene.WorkEnd();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			if (_scene.IsLoaded())
			{
				var resource = _scene.GetResource();
				using (var saveFileDialog = new SaveFileDialog())
				{
					saveFileDialog.Filter = "AEI files (*.aei)|*.aei";
					var filePath = resource.Path;
					saveFileDialog.FileName = "Copy " + Path.GetFileNameWithoutExtension(filePath);
					saveFileDialog.RestoreDirectory = true;
					if (saveFileDialog.ShowDialog() == DialogResult.OK)
					{
						resource.Path = saveFileDialog.FileName;
						Program.DoAsync(resource.Write, "Writing data");
						UpdateUi();
					}
					goto IL_B3;
				}
			}
			UiManager.ShowMessage("File not loaded", "Error");
			IL_B3:
			_scene.WorkEnd();
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "Portable Network Graphics (*.png)|*.png";
				openFileDialog.RestoreDirectory = true;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					_scene.ImportBitmap(openFileDialog.FileName);
					UpdateUi();
				}
			}
			_scene.WorkEnd();
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			if (_scene.IsLoaded())
			{
				using (var saveFileDialog = new SaveFileDialog())
				{
					saveFileDialog.Filter = "Portable Network Graphics (*.png)|*.png";
					saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_scene.GetResource().Path);
					saveFileDialog.RestoreDirectory = true;
					if (saveFileDialog.ShowDialog() == DialogResult.OK)
					{
						_scene.ExportBitmap(saveFileDialog.FileName);
					}
					goto IL_8E;
				}
			}
			UiManager.ShowMessage("File not loaded", "Error");
			IL_8E:
			_scene.WorkEnd();
		}

		private void transparencyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			var @checked = OpacityView.Checked;
			OpacityView.Checked = OpacityViewBox.Checked = !@checked;
			Program.PxFmtCurrent = !@checked ? Program.PxFmtFull : Program.PxFmtColor;
			if (_scene.IsLoaded())
			{
				var bitmap = _scene.GetBitmap();
				_scene.SetBitmap(Program.ReRenderBitmap(_scene.GetInitialBitmap(), bitmap.Width, bitmap.Height, Program.PxFmtCurrent));
				bitmap.Dispose();
			}
			_scene.WorkEnd();
		}

		private void NewBox_Click(object sender, EventArgs e)
		{
			newToolStripMenuItem_Click(sender, e);
		}

		private void OpenBox_Click(object sender, EventArgs e)
		{
			openToolStripMenuItem_Click(sender, e);
		}

		private void SaveBox_Click(object sender, EventArgs e)
		{
			saveToolStripMenuItem_Click(sender, e);
		}

		private void SaveAsBox_Click(object sender, EventArgs e)
		{
			saveAsToolStripMenuItem_Click(sender, e);
		}

		private void ImportBox_Click(object sender, EventArgs e)
		{
			importToolStripMenuItem_Click(sender, e);
		}

		private void ExportBox_Click(object sender, EventArgs e)
		{
			exportToolStripMenuItem_Click(sender, e);
		}

		private void OpacityViewBox_Click(object sender, EventArgs e)
		{
			transparencyToolStripMenuItem_Click(sender, e);
		}

		public void UpdateQualityControls(Compression.Ae2Format compressionAe2Format)
		{
			var formatQualityName = Compression.GetFormatQualityName(compressionAe2Format);
			QLow.Text = formatQualityName[0];
			QMid.Text = formatQualityName[1];
			QHigh.Text = formatQualityName[2];
			QHighest.Text = formatQualityName[3];
			QLow.Enabled = QMid.Enabled = QHigh.Enabled = QHighest.Enabled = Compression.IsFormatCompressed(compressionAe2Format);
		}

		public void UpdateQualityControls(byte quality)
		{
			switch (quality)
			{
			case 0:
				QLow.Checked = true;
				QMid.Checked = false;
				QHigh.Checked = false;
				QHighest.Checked = false;
				return;
			case 1:
				QLow.Checked = false;
				QMid.Checked = true;
				QHigh.Checked = false;
				QHighest.Checked = false;
				return;
			case 2:
				QLow.Checked = false;
				QMid.Checked = false;
				QHigh.Checked = true;
				QHighest.Checked = false;
				return;
			case 3:
				QLow.Checked = false;
				QMid.Checked = false;
				QHigh.Checked = false;
				QHighest.Checked = true;
				return;
			default:
				return;
			}
		}

		public void UpdateQualityControls(byte quality, Compression.Ae2Format compressionAe2Format)
		{
			UpdateQualityControls(quality);
			UpdateQualityControls(compressionAe2Format);
		}

		public void UpdateUi()
		{
			var resource = _scene.GetResource();
			if (resource.CompressionAe2Format == Compression.Ae2Format.Unknown)
			{
				UiManager.ShowMessage("This file cannot be opened, it is not supported.", "Operation not possible");
				return;
			}
			OpacityView.Checked = OpacityViewBox.Checked = true;
			Program.PxFmtCurrent = PixelFormat.Format32bppArgb;
			NameField.Text = "File: " + Path.GetFileNameWithoutExtension(resource.Path);
			SizeField.Text = "Size: " + resource.Size.Width + " x " + resource.Size.Height;
			SizeField.Visible = true;
			NameField.Visible = true;
			UpdateQualityControls(resource.Quality, resource.CompressionAe2Format);
			MipMapCheck.Enabled = Compression.IsFormatMipMappable(resource.CompressionAe2Format);
			MipMapCheck.Checked = resource.MipMapped;
			CompressionList.SelectedIndex = Array.IndexOf(Compression.Formats, resource.CompressionAe2Format);
		}

		private void MipMapCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (!_scene.IsReady() || !_scene.IsLoaded())
			{
				return;
			}
			_scene.WorkStart();
			var flag = !MipMapCheck.Checked;
			if (!_scene.SetMipMapped(flag))
			{
				throw new Exception("Cannot enable mip-mapping");
			}
			MipMapCheck.Checked = flag;
			_scene.WorkEnd();
		}

		private void CompressionList_SelectionChange(object sender, EventArgs e)
		{
			_scene.WorkStart();
			var resource = _scene.GetResource();
			var selectedIndex = CompressionList.SelectedIndex;
			var compressionFormat = Compression.Formats[selectedIndex];
			if (_scene.IsReady())
			{
				var flag = Compression.IsFormatMipMappable(compressionFormat);
				if (MipMapCheck.Checked && !flag)
				{
					MipMapCheck.Checked = false;
				}
				if (_scene.IsLoaded())
				{
					resource.MipMapped = MipMapCheck.Checked;
				}
			}
			if (_scene.IsLoaded())
			{
				resource.CompressionAe2Format = compressionFormat;
			}
			UpdateQualityControls(compressionFormat);
			MipMapCheck.Enabled = Compression.IsFormatMipMappable(compressionFormat);
			_scene.WorkEnd();
		}

		private void RegionListContextMenu_Opening(object sender, CancelEventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			if (_scene.IsLoaded())
			{
				RegionSymbolAdd.Enabled = RegionAdd.Enabled = allToolStripMenuItem.Enabled = true;
				RegionDelete.Enabled = copyToolStripMenuItem.Enabled = _scene.IsTextureSelected();
				pasteToolStripMenuItem.Enabled = _copiedTextures.Length != 0;
				RegionImport.Enabled = RegionExport.Enabled = _scene.GetSelectedTextures().Length != 0;
			}
			else
			{
				e.Cancel = true;
			}
			_scene.WorkEnd();
		}

		private void RegionAdd_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			StatusState.Text = "Stretch the area on the texture with the left mouse button, or click another to cancel.";
			_scene.BeginDrawingRegion(false);
		}

		private void RegionSymbolAdd_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			StatusState.Text = "Stretch the area on the texture with the left mouse button, or click another to cancel.";
			_scene.BeginDrawingRegion(true);
		}

		private void RegionDelete_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			_scene.DeleteSelectedTextures();
			_scene.WorkEnd();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			var selectedTextures = _scene.GetSelectedTextures();
			var num = selectedTextures.Length;
			Array.Resize(ref _copiedTextures, num);
			for (var i = 0; i < num; i++)
			{
				var aeimageTexture = selectedTextures[i];
				_copiedTextures[i] = new AeImageTexture(aeimageTexture, aeimageTexture.Id);
			}
			_scene.WorkEnd();
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			var num = _copiedTextures.Length;
			var array = new AeImageTexture[num];
			var newTextureIndex = _scene.GetNewTextureIndex();
			for (var i = 0; i < num; i++)
			{
				array[i] = new AeImageTexture(_copiedTextures[i], newTextureIndex + i);
			}
			_scene.AddTextures(array, newTextureIndex);
			_scene.WorkEnd(true);
		}

		private void allToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			_scene.SetTexturesSelected(true);
			_scene.WorkEnd(true);
		}

		private void RegionSymbol_Edit(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			var toolStripTextBox = (ToolStripTextBox)sender;
			var flag = true;
			var symbol = '\0';
			try
			{
				symbol = char.Parse(((ToolStripItem)sender).Text);
			}
			catch
			{
				flag = false;
			}
			var selectedTexture = _scene.GetSelectedTexture();
			if (flag)
			{
				selectedTexture.SymbolData.Symbol = symbol;
				_scene.ReDraw();
			}
			else
			{
				toolStripTextBox.Text = selectedTexture.SymbolData.Symbol.ToString();
			}
			_scene.WorkEnd();
		}

		private void RegionPosX_Edit(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			var toolStripTextBox = (ToolStripTextBox)sender;
			var flag = true;
			ushort num = 0;
			try
			{
				num = ushort.Parse(((ToolStripItem)sender).Text);
			}
			catch
			{
				flag = false;
			}
			var selectedTexture = _scene.GetSelectedTexture();
			var width = _scene.GetResource().Size.Width;
			var width2 = selectedTexture.Size.Width;
			if (flag && num >= 0)
			{
				if (num + width2 > width)
				{
					num = (ushort)(width - width2);
					toolStripTextBox.Text = num.ToString();
				}
				selectedTexture.Location.X = num;
				_scene.ReDraw();
			}
			else
			{
				toolStripTextBox.Text = selectedTexture.Location.X.ToString();
			}
			_scene.WorkEnd();
		}

		private void RegionPosY_Edit(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			var toolStripTextBox = (ToolStripTextBox)sender;
			var flag = true;
			ushort num = 0;
			try
			{
				num = ushort.Parse(((ToolStripItem)sender).Text);
			}
			catch
			{
				flag = false;
			}
			var selectedTexture = _scene.GetSelectedTexture();
			var height = _scene.GetResource().Size.Height;
			var height2 = selectedTexture.Size.Height;
			if (flag && num >= 0)
			{
				if (num + height2 > height)
				{
					num = (ushort)(height - height2);
					toolStripTextBox.Text = num.ToString();
				}
				selectedTexture.Location.Y = num;
				_scene.ReDraw();
			}
			else
			{
				toolStripTextBox.Text = selectedTexture.Location.Y.ToString();
			}
			_scene.WorkEnd();
		}

		private void RegionSizeX_Edit(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			var toolStripTextBox = (ToolStripTextBox)sender;
			var flag = true;
			ushort num = 0;
			try
			{
				num = ushort.Parse(((ToolStripItem)sender).Text);
			}
			catch
			{
				flag = false;
			}
			var selectedTexture = _scene.GetSelectedTexture();
			var width = _scene.GetResource().Size.Width;
			var x = selectedTexture.Location.X;
			if (flag && num >= 0)
			{
				if (x + num > width)
				{
					num = (ushort)(width - x);
					toolStripTextBox.Text = num.ToString();
				}
				selectedTexture.Size.Width = num;
				_scene.ReDraw();
			}
			else
			{
				toolStripTextBox.Text = selectedTexture.Size.Width.ToString();
			}
			_scene.WorkEnd();
		}

		private void RegionSizeY_Edit(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			var toolStripTextBox = (ToolStripTextBox)sender;
			var flag = true;
			ushort num = 0;
			try
			{
				num = ushort.Parse(((ToolStripItem)sender).Text);
			}
			catch
			{
				flag = false;
			}
			var selectedTexture = _scene.GetSelectedTexture();
			var height = _scene.GetResource().Size.Height;
			var y = selectedTexture.Location.Y;
			if (flag && num >= 0)
			{
				if (y + num > height)
				{
					num = (ushort)(height - y);
					toolStripTextBox.Text = num.ToString();
				}
				selectedTexture.Size.Height = num;
				_scene.ReDraw();
			}
			else
			{
				toolStripTextBox.Text = selectedTexture.Size.Height.ToString();
			}
			_scene.WorkEnd();
		}

		private void RegionUp_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			_scene.MoveTexture(1);
			_scene.WorkEnd();
		}

		private void RegionDown_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			_scene.MoveTexture(-1);
			_scene.WorkEnd();
		}

		private void RegionImport_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			if (_scene.GetTextureCount() == 1)
			{
				importToolStripMenuItem_Click(sender, e);
				return;
			}
			_scene.WorkStart();
			var selectedTextures = _scene.GetSelectedTextures();
			var filePath = _scene.GetResource().Path;
			if (selectedTextures.Length == 1)
			{
				var aeimageTexture = selectedTextures[0];
				using (var openFileDialog = new OpenFileDialog())
				{
					openFileDialog.Filter = "Portable Network Graphics (*.png)|*.png";
					var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
					var symbolData = aeimageTexture.SymbolData;
					if (symbolData != null)
					{
						openFileDialog.FileName = string.Format("{0:000}.{1}.Symbol_{2}_{3}", aeimageTexture.Id + 1, fileNameWithoutExtension, symbolData.GroupId, symbolData.Symbol);
					}
					else
					{
						openFileDialog.FileName = string.Format("{0:000}.{1}", aeimageTexture.Id + 1, fileNameWithoutExtension);
					}
					openFileDialog.RestoreDirectory = true;
					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						_scene.ImportTexture(openFileDialog.FileName, aeimageTexture);
					}
					goto IL_174;
				}
			}
			using (var folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.Description = "Choose a path for importing selected fragments of textures; PNG files are selected in alphabetical order";
				folderBrowserDialog.SelectedPath = Path.GetDirectoryName(filePath);
				folderBrowserDialog.ShowNewFolderButton = false;
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					_scene.ImportTextures(folderBrowserDialog.SelectedPath, selectedTextures);
				}
			}
			IL_174:
			_scene.RenderImageScaled();
			_scene.WorkEnd();
		}

		private void RegionExport_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			if (_scene.GetTextureCount() == 1)
			{
				exportToolStripMenuItem_Click(sender, e);
				return;
			}
			_scene.WorkStart();
			var selectedTextures = _scene.GetSelectedTextures();
			var filePath = _scene.GetResource().Path;
			if (selectedTextures.Length == 1)
			{
				var aeimageTexture = selectedTextures[0];
				using (var saveFileDialog = new SaveFileDialog())
				{
					saveFileDialog.Filter = "Portable Network Graphics (*.png)|*.png";
					var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
					var symbolData = aeimageTexture.SymbolData;
					if (symbolData != null)
					{
						saveFileDialog.FileName = string.Format("{0:000}.{1}.Symbol_{2}_{3}", aeimageTexture.Id + 1, fileNameWithoutExtension, symbolData.GroupId, symbolData.Symbol);
					}
					else
					{
						saveFileDialog.FileName = string.Format("{0:000}.{1}", aeimageTexture.Id + 1, fileNameWithoutExtension);
					}
					saveFileDialog.RestoreDirectory = true;
					if (saveFileDialog.ShowDialog() == DialogResult.OK)
					{
						_scene.ExportTexture(saveFileDialog.FileName, aeimageTexture);
					}
					goto IL_172;
				}
			}
			using (var folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.Description = "Choose a path to export selected texture fragments";
				folderBrowserDialog.SelectedPath = Path.GetDirectoryName(filePath);
				folderBrowserDialog.ShowNewFolderButton = true;
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					_scene.ExportTextures(folderBrowserDialog.SelectedPath, selectedTextures);
				}
			}
			IL_172:
			_scene.WorkEnd();
		}

		private void RegionImportBox_Click(object sender, EventArgs e)
		{
			RegionImport_Click(sender, e);
		}

		private void RegionExportBox_Click(object sender, EventArgs e)
		{
			RegionExport_Click(sender, e);
		}

		private void SetQuality(byte quality)
		{
			if (_scene.IsLoaded())
			{
				_scene.GetResource().Quality = quality;
			}
			UpdateQualityControls(quality);
		}

		private void lowToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			SetQuality(0);
			_scene.WorkEnd();
		}

		private void mediumToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			SetQuality(1);
			_scene.WorkEnd();
		}

		private void highToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			SetQuality(2);
			_scene.WorkEnd();
		}

		private void bestToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_scene.IsReady())
			{
				return;
			}
			_scene.WorkStart();
			SetQuality(3);
			_scene.WorkEnd();
		}

		private AeImageTexture[] _copiedTextures = new AeImageTexture[0];

		public ToolStripItem[] BaseRegionControls;

		public ToolStripItem[] SymbolRegionControls;

		public ToolStripItem[] RegionEditControls;

		public ToolStripItem[] RegionIoControls;

		private AeScene _scene;
	}
}
