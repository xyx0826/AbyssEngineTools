using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AEIEditor.Properties;

namespace AEIEditor
{
	public partial class MainForm : Form
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.compressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.QLow = new System.Windows.Forms.ToolStripMenuItem();
            this.QMid = new System.Windows.Forms.ToolStripMenuItem();
            this.QHigh = new System.Windows.Forms.ToolStripMenuItem();
            this.QHighest = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gradientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.OpacityView = new System.Windows.Forms.ToolStripMenuItem();
            this.Status = new System.Windows.Forms.StatusStrip();
            this.StatusState = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.NameField = new System.Windows.Forms.ToolStripStatusLabel();
            this.SizeField = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusScale = new System.Windows.Forms.ToolStripStatusLabel();
            this.CompressionList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.RegionListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.texturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RegionAddGrp = new System.Windows.Forms.ToolStripMenuItem();
            this.RegionAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.RegionSymbolAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.RegionDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RegionImport = new System.Windows.Forms.ToolStripMenuItem();
            this.RegionExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.NewBox = new System.Windows.Forms.ToolStripButton();
            this.OpenBox = new System.Windows.Forms.ToolStripButton();
            this.SaveBox = new System.Windows.Forms.ToolStripButton();
            this.SaveAsBox = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ImportBox = new System.Windows.Forms.ToolStripButton();
            this.ExportBox = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.OpacityViewBox = new System.Windows.Forms.ToolStripButton();
            this.RegionSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.RegionTip = new System.Windows.Forms.ToolStripLabel();
            this.ReginSymbolEditor = new System.Windows.Forms.ToolStripTextBox();
            this.ReginSymbolGroupTip = new System.Windows.Forms.ToolStripLabel();
            this.RegionUp = new System.Windows.Forms.ToolStripButton();
            this.RegionDown = new System.Windows.Forms.ToolStripButton();
            this.RegionEditSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.RegionPosTip = new System.Windows.Forms.ToolStripLabel();
            this.RegionPosX = new System.Windows.Forms.ToolStripTextBox();
            this.RegionPosSplitter = new System.Windows.Forms.ToolStripLabel();
            this.RegionPosY = new System.Windows.Forms.ToolStripTextBox();
            this.RegionSizeTip = new System.Windows.Forms.ToolStripLabel();
            this.RegionSizeX = new System.Windows.Forms.ToolStripTextBox();
            this.RegionSizeSplitter = new System.Windows.Forms.ToolStripLabel();
            this.RegionSizeY = new System.Windows.Forms.ToolStripTextBox();
            this.RegionIOSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.RegionImportBox = new System.Windows.Forms.ToolStripButton();
            this.RegionExportBox = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.MipMapCheck = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.Status.SuspendLayout();
            this.RegionListContextMenu.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(792, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(126, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolStripSeparator6,
            this.compressionToolStripMenuItem});
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(57, 21);
            this.imageToolStripMenuItem.Text = "Image";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(238, 6);
            // 
            // compressionToolStripMenuItem
            // 
            this.compressionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.QLow,
            this.QMid,
            this.QHigh,
            this.QHighest});
            this.compressionToolStripMenuItem.Name = "compressionToolStripMenuItem";
            this.compressionToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.compressionToolStripMenuItem.Text = "Compressed Texture Quality";
            // 
            // QLow
            // 
            this.QLow.Enabled = false;
            this.QLow.Name = "QLow";
            this.QLow.Size = new System.Drawing.Size(108, 22);
            this.QLow.Text = "None";
            this.QLow.Click += new System.EventHandler(this.lowToolStripMenuItem_Click_1);
            // 
            // QMid
            // 
            this.QMid.Enabled = false;
            this.QMid.Name = "QMid";
            this.QMid.Size = new System.Drawing.Size(108, 22);
            this.QMid.Text = "None";
            this.QMid.Click += new System.EventHandler(this.mediumToolStripMenuItem_Click_1);
            // 
            // QHigh
            // 
            this.QHigh.Enabled = false;
            this.QHigh.Name = "QHigh";
            this.QHigh.Size = new System.Drawing.Size(108, 22);
            this.QHigh.Text = "None";
            this.QHigh.Click += new System.EventHandler(this.highToolStripMenuItem_Click_1);
            // 
            // QHighest
            // 
            this.QHighest.Checked = true;
            this.QHighest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.QHighest.Enabled = false;
            this.QHighest.Name = "QHighest";
            this.QHighest.Size = new System.Drawing.Size(108, 22);
            this.QHighest.Text = "None";
            this.QHighest.Click += new System.EventHandler(this.bestToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backgroundToolStripMenuItem,
            this.toolStripSeparator3,
            this.OpacityView});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // backgroundToolStripMenuItem
            // 
            this.backgroundToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gradientToolStripMenuItem,
            this.checkerToolStripMenuItem,
            this.whiteToolStripMenuItem,
            this.blackToolStripMenuItem});
            this.backgroundToolStripMenuItem.Name = "backgroundToolStripMenuItem";
            this.backgroundToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.backgroundToolStripMenuItem.Text = "Background";
            // 
            // gradientToolStripMenuItem
            // 
            this.gradientToolStripMenuItem.Checked = true;
            this.gradientToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gradientToolStripMenuItem.Name = "gradientToolStripMenuItem";
            this.gradientToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.gradientToolStripMenuItem.Text = "Gradient";
            this.gradientToolStripMenuItem.Click += new System.EventHandler(this.gradientToolStripMenuItem_Click);
            // 
            // checkerToolStripMenuItem
            // 
            this.checkerToolStripMenuItem.Name = "checkerToolStripMenuItem";
            this.checkerToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.checkerToolStripMenuItem.Text = "Pixel checker";
            this.checkerToolStripMenuItem.Click += new System.EventHandler(this.checkerToolStripMenuItem_Click);
            // 
            // whiteToolStripMenuItem
            // 
            this.whiteToolStripMenuItem.Name = "whiteToolStripMenuItem";
            this.whiteToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.whiteToolStripMenuItem.Text = "White";
            this.whiteToolStripMenuItem.Click += new System.EventHandler(this.whiteToolStripMenuItem_Click);
            // 
            // blackToolStripMenuItem
            // 
            this.blackToolStripMenuItem.Name = "blackToolStripMenuItem";
            this.blackToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.blackToolStripMenuItem.Text = "Black";
            this.blackToolStripMenuItem.Click += new System.EventHandler(this.blackToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(144, 6);
            // 
            // OpacityView
            // 
            this.OpacityView.Checked = true;
            this.OpacityView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.OpacityView.Name = "OpacityView";
            this.OpacityView.Size = new System.Drawing.Size(147, 22);
            this.OpacityView.Text = "Opacity";
            this.OpacityView.Click += new System.EventHandler(this.transparencyToolStripMenuItem_Click);
            // 
            // Status
            // 
            this.Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusState,
            this.StatusProgress,
            this.NameField,
            this.SizeField,
            this.StatusScale});
            this.Status.Location = new System.Drawing.Point(0, 503);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(792, 22);
            this.Status.TabIndex = 3;
            this.Status.Text = "statusStrip1";
            // 
            // StatusState
            // 
            this.StatusState.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StatusState.Name = "StatusState";
            this.StatusState.Size = new System.Drawing.Size(39, 17);
            this.StatusState.Text = "Done";
            // 
            // StatusProgress
            // 
            this.StatusProgress.Name = "StatusProgress";
            this.StatusProgress.Size = new System.Drawing.Size(256, 20);
            this.StatusProgress.Visible = false;
            // 
            // NameField
            // 
            this.NameField.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.NameField.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.NameField.Name = "NameField";
            this.NameField.Size = new System.Drawing.Size(34, 21);
            this.NameField.Text = "File:";
            this.NameField.Visible = false;
            // 
            // SizeField
            // 
            this.SizeField.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.SizeField.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SizeField.Name = "SizeField";
            this.SizeField.Size = new System.Drawing.Size(38, 21);
            this.SizeField.Text = "Size:";
            this.SizeField.Visible = false;
            // 
            // StatusScale
            // 
            this.StatusScale.Name = "StatusScale";
            this.StatusScale.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StatusScale.Size = new System.Drawing.Size(738, 17);
            this.StatusScale.Spring = true;
            this.StatusScale.Text = "Scale: 100%";
            this.StatusScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CompressionList
            // 
            this.CompressionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CompressionList.FormattingEnabled = true;
            this.CompressionList.Location = new System.Drawing.Point(80, 48);
            this.CompressionList.Name = "CompressionList";
            this.CompressionList.Size = new System.Drawing.Size(151, 20);
            this.CompressionList.TabIndex = 6;
            this.CompressionList.SelectionChangeCommitted += new System.EventHandler(this.CompressionList_SelectionChange);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "Compression";
            // 
            // RegionListContextMenu
            // 
            this.RegionListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.texturesToolStripMenuItem,
            this.RegionAddGrp,
            this.RegionDelete,
            this.toolStripSeparator1,
            this.selectAllToolStripMenuItem,
            this.allToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.RegionImport,
            this.RegionExport});
            this.RegionListContextMenu.Name = "RegionListContextMenu";
            this.RegionListContextMenu.ShowItemToolTips = false;
            this.RegionListContextMenu.Size = new System.Drawing.Size(132, 208);
            this.RegionListContextMenu.Text = "Textures";
            this.RegionListContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.RegionListContextMenu_Opening);
            // 
            // texturesToolStripMenuItem
            // 
            this.texturesToolStripMenuItem.Enabled = false;
            this.texturesToolStripMenuItem.Name = "texturesToolStripMenuItem";
            this.texturesToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.texturesToolStripMenuItem.Text = "Textures:";
            this.texturesToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.texturesToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // RegionAddGrp
            // 
            this.RegionAddGrp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RegionAdd,
            this.RegionSymbolAdd});
            this.RegionAddGrp.Name = "RegionAddGrp";
            this.RegionAddGrp.Size = new System.Drawing.Size(131, 22);
            this.RegionAddGrp.Text = "Create";
            // 
            // RegionAdd
            // 
            this.RegionAdd.Enabled = false;
            this.RegionAdd.Name = "RegionAdd";
            this.RegionAdd.Size = new System.Drawing.Size(119, 22);
            this.RegionAdd.Text = "Texture";
            this.RegionAdd.Click += new System.EventHandler(this.RegionAdd_Click);
            // 
            // RegionSymbolAdd
            // 
            this.RegionSymbolAdd.Enabled = false;
            this.RegionSymbolAdd.Name = "RegionSymbolAdd";
            this.RegionSymbolAdd.Size = new System.Drawing.Size(119, 22);
            this.RegionSymbolAdd.Text = "Symbol";
            this.RegionSymbolAdd.Click += new System.EventHandler(this.RegionSymbolAdd_Click);
            // 
            // RegionDelete
            // 
            this.RegionDelete.Enabled = false;
            this.RegionDelete.Name = "RegionDelete";
            this.RegionDelete.Size = new System.Drawing.Size(131, 22);
            this.RegionDelete.Text = "Delete";
            this.RegionDelete.Click += new System.EventHandler(this.RegionDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(128, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Enabled = false;
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.selectAllToolStripMenuItem.Text = "Selection:";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Enabled = false;
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.allToolStripMenuItem.Text = "Select all";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Enabled = false;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // RegionImport
            // 
            this.RegionImport.Enabled = false;
            this.RegionImport.Name = "RegionImport";
            this.RegionImport.Size = new System.Drawing.Size(131, 22);
            this.RegionImport.Text = "Import";
            this.RegionImport.Click += new System.EventHandler(this.RegionImport_Click);
            // 
            // RegionExport
            // 
            this.RegionExport.Enabled = false;
            this.RegionExport.Name = "RegionExport";
            this.RegionExport.Size = new System.Drawing.Size(131, 22);
            this.RegionExport.Text = "Export";
            this.RegionExport.Click += new System.EventHandler(this.RegionExport_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewBox,
            this.OpenBox,
            this.SaveBox,
            this.SaveAsBox,
            this.toolStripSeparator4,
            this.ImportBox,
            this.ExportBox,
            this.toolStripSeparator5,
            this.OpacityViewBox,
            this.RegionSeparator,
            this.RegionTip,
            this.ReginSymbolEditor,
            this.ReginSymbolGroupTip,
            this.RegionUp,
            this.RegionDown,
            this.RegionEditSeparator,
            this.RegionPosTip,
            this.RegionPosX,
            this.RegionPosSplitter,
            this.RegionPosY,
            this.RegionSizeTip,
            this.RegionSizeX,
            this.RegionSizeSplitter,
            this.RegionSizeY,
            this.RegionIOSeparator,
            this.RegionImportBox,
            this.RegionExportBox,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(792, 25);
            this.toolStrip1.TabIndex = 12;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // NewBox
            // 
            this.NewBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NewBox.Image = global::AEIEditor.Properties.Resources.NewBox_Image;
            this.NewBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewBox.Name = "NewBox";
            this.NewBox.Size = new System.Drawing.Size(23, 22);
            this.NewBox.Text = "New";
            this.NewBox.Click += new System.EventHandler(this.NewBox_Click);
            // 
            // OpenBox
            // 
            this.OpenBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenBox.Image = global::AEIEditor.Properties.Resources.OpenBox_Image;
            this.OpenBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpenBox.Name = "OpenBox";
            this.OpenBox.Size = new System.Drawing.Size(23, 22);
            this.OpenBox.Text = "Open";
            this.OpenBox.Click += new System.EventHandler(this.OpenBox_Click);
            // 
            // SaveBox
            // 
            this.SaveBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveBox.Image = global::AEIEditor.Properties.Resources.SaveBox_Image;
            this.SaveBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveBox.Name = "SaveBox";
            this.SaveBox.Size = new System.Drawing.Size(23, 22);
            this.SaveBox.Text = "Save";
            this.SaveBox.Click += new System.EventHandler(this.SaveBox_Click);
            // 
            // SaveAsBox
            // 
            this.SaveAsBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveAsBox.Image = global::AEIEditor.Properties.Resources.SaveAsBox_Image;
            this.SaveAsBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveAsBox.Name = "SaveAsBox";
            this.SaveAsBox.Size = new System.Drawing.Size(23, 22);
            this.SaveAsBox.Text = "Save as...";
            this.SaveAsBox.Click += new System.EventHandler(this.SaveAsBox_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // ImportBox
            // 
            this.ImportBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ImportBox.Image = global::AEIEditor.Properties.Resources.ImportBox_Image;
            this.ImportBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ImportBox.Name = "ImportBox";
            this.ImportBox.Size = new System.Drawing.Size(23, 22);
            this.ImportBox.Text = "Import";
            this.ImportBox.Click += new System.EventHandler(this.ImportBox_Click);
            // 
            // ExportBox
            // 
            this.ExportBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ExportBox.Image = global::AEIEditor.Properties.Resources.ExportBox_Image;
            this.ExportBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExportBox.Name = "ExportBox";
            this.ExportBox.Size = new System.Drawing.Size(23, 22);
            this.ExportBox.Text = "Export";
            this.ExportBox.Click += new System.EventHandler(this.ExportBox_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // OpacityViewBox
            // 
            this.OpacityViewBox.Checked = true;
            this.OpacityViewBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.OpacityViewBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpacityViewBox.Image = global::AEIEditor.Properties.Resources.OpacityViewBox_Image;
            this.OpacityViewBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpacityViewBox.Name = "OpacityViewBox";
            this.OpacityViewBox.Size = new System.Drawing.Size(23, 22);
            this.OpacityViewBox.Text = "Display transparency";
            this.OpacityViewBox.Click += new System.EventHandler(this.OpacityViewBox_Click);
            // 
            // RegionSeparator
            // 
            this.RegionSeparator.Name = "RegionSeparator";
            this.RegionSeparator.Size = new System.Drawing.Size(6, 25);
            this.RegionSeparator.Visible = false;
            // 
            // RegionTip
            // 
            this.RegionTip.Name = "RegionTip";
            this.RegionTip.Size = new System.Drawing.Size(70, 22);
            this.RegionTip.Text = "Texture №";
            this.RegionTip.Visible = false;
            // 
            // ReginSymbolEditor
            // 
            this.ReginSymbolEditor.AutoSize = false;
            this.ReginSymbolEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ReginSymbolEditor.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.ReginSymbolEditor.MaxLength = 1;
            this.ReginSymbolEditor.Name = "ReginSymbolEditor";
            this.ReginSymbolEditor.Size = new System.Drawing.Size(25, 23);
            this.ReginSymbolEditor.Text = " ";
            this.ReginSymbolEditor.Visible = false;
            this.ReginSymbolEditor.TextChanged += new System.EventHandler(this.RegionSymbol_Edit);
            // 
            // ReginSymbolGroupTip
            // 
            this.ReginSymbolGroupTip.Name = "ReginSymbolGroupTip";
            this.ReginSymbolGroupTip.Size = new System.Drawing.Size(55, 22);
            this.ReginSymbolGroupTip.Text = "group 1";
            this.ReginSymbolGroupTip.Visible = false;
            // 
            // RegionUp
            // 
            this.RegionUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RegionUp.Image = global::AEIEditor.Properties.Resources.RegionUp_Image;
            this.RegionUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RegionUp.Name = "RegionUp";
            this.RegionUp.Size = new System.Drawing.Size(23, 22);
            this.RegionUp.Text = "Up";
            this.RegionUp.Visible = false;
            this.RegionUp.Click += new System.EventHandler(this.RegionUp_Click);
            // 
            // RegionDown
            // 
            this.RegionDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RegionDown.Image = global::AEIEditor.Properties.Resources.RegionDown_Image;
            this.RegionDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RegionDown.Name = "RegionDown";
            this.RegionDown.Size = new System.Drawing.Size(23, 22);
            this.RegionDown.Text = "Down";
            this.RegionDown.Visible = false;
            this.RegionDown.Click += new System.EventHandler(this.RegionDown_Click);
            // 
            // RegionEditSeparator
            // 
            this.RegionEditSeparator.Name = "RegionEditSeparator";
            this.RegionEditSeparator.Size = new System.Drawing.Size(6, 25);
            this.RegionEditSeparator.Visible = false;
            // 
            // RegionPosTip
            // 
            this.RegionPosTip.Name = "RegionPosTip";
            this.RegionPosTip.Size = new System.Drawing.Size(57, 22);
            this.RegionPosTip.Text = "Position:";
            this.RegionPosTip.Visible = false;
            // 
            // RegionPosX
            // 
            this.RegionPosX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RegionPosX.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.RegionPosX.MaxLength = 4;
            this.RegionPosX.Name = "RegionPosX";
            this.RegionPosX.Size = new System.Drawing.Size(32, 25);
            this.RegionPosX.Tag = "";
            this.RegionPosX.Text = "0";
            this.RegionPosX.Visible = false;
            this.RegionPosX.TextChanged += new System.EventHandler(this.RegionPosX_Edit);
            // 
            // RegionPosSplitter
            // 
            this.RegionPosSplitter.Name = "RegionPosSplitter";
            this.RegionPosSplitter.Size = new System.Drawing.Size(11, 22);
            this.RegionPosSplitter.Text = ",";
            this.RegionPosSplitter.Visible = false;
            // 
            // RegionPosY
            // 
            this.RegionPosY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RegionPosY.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.RegionPosY.MaxLength = 4;
            this.RegionPosY.Name = "RegionPosY";
            this.RegionPosY.Size = new System.Drawing.Size(32, 25);
            this.RegionPosY.Tag = "";
            this.RegionPosY.Text = "0";
            this.RegionPosY.Visible = false;
            this.RegionPosY.TextChanged += new System.EventHandler(this.RegionPosY_Edit);
            // 
            // RegionSizeTip
            // 
            this.RegionSizeTip.Name = "RegionSizeTip";
            this.RegionSizeTip.Size = new System.Drawing.Size(34, 22);
            this.RegionSizeTip.Text = "Size:";
            this.RegionSizeTip.Visible = false;
            // 
            // RegionSizeX
            // 
            this.RegionSizeX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RegionSizeX.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.RegionSizeX.MaxLength = 4;
            this.RegionSizeX.Name = "RegionSizeX";
            this.RegionSizeX.Size = new System.Drawing.Size(32, 25);
            this.RegionSizeX.Text = "0";
            this.RegionSizeX.Visible = false;
            this.RegionSizeX.TextChanged += new System.EventHandler(this.RegionSizeX_Edit);
            // 
            // RegionSizeSplitter
            // 
            this.RegionSizeSplitter.Name = "RegionSizeSplitter";
            this.RegionSizeSplitter.Size = new System.Drawing.Size(13, 22);
            this.RegionSizeSplitter.Text = "*";
            this.RegionSizeSplitter.Visible = false;
            // 
            // RegionSizeY
            // 
            this.RegionSizeY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RegionSizeY.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.RegionSizeY.MaxLength = 4;
            this.RegionSizeY.Name = "RegionSizeY";
            this.RegionSizeY.Size = new System.Drawing.Size(32, 25);
            this.RegionSizeY.Text = "0";
            this.RegionSizeY.Visible = false;
            this.RegionSizeY.TextChanged += new System.EventHandler(this.RegionSizeY_Edit);
            // 
            // RegionIOSeparator
            // 
            this.RegionIOSeparator.Name = "RegionIOSeparator";
            this.RegionIOSeparator.Size = new System.Drawing.Size(6, 25);
            this.RegionIOSeparator.Visible = false;
            // 
            // RegionImportBox
            // 
            this.RegionImportBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RegionImportBox.Image = global::AEIEditor.Properties.Resources.RegionImportBox_Image;
            this.RegionImportBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RegionImportBox.Name = "RegionImportBox";
            this.RegionImportBox.Size = new System.Drawing.Size(23, 22);
            this.RegionImportBox.Text = "Import Fragments";
            this.RegionImportBox.Visible = false;
            this.RegionImportBox.Click += new System.EventHandler(this.RegionImportBox_Click);
            // 
            // RegionExportBox
            // 
            this.RegionExportBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RegionExportBox.Image = global::AEIEditor.Properties.Resources.RegionExportBox_Image;
            this.RegionExportBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RegionExportBox.Name = "RegionExportBox";
            this.RegionExportBox.Size = new System.Drawing.Size(23, 22);
            this.RegionExportBox.Text = "Export Fragments";
            this.RegionExportBox.Visible = false;
            this.RegionExportBox.Click += new System.EventHandler(this.RegionExportBox_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // MipMapCheck
            // 
            this.MipMapCheck.AutoCheck = false;
            this.MipMapCheck.AutoSize = true;
            this.MipMapCheck.Enabled = false;
            this.MipMapCheck.Location = new System.Drawing.Point(237, 50);
            this.MipMapCheck.Name = "MipMapCheck";
            this.MipMapCheck.Size = new System.Drawing.Size(90, 16);
            this.MipMapCheck.TabIndex = 14;
            this.MipMapCheck.Text = "Mip-Mapping";
            this.MipMapCheck.UseVisualStyleBackColor = true;
            this.MipMapCheck.Click += new System.EventHandler(this.MipMapCheck_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(792, 525);
            this.Controls.Add(this.MipMapCheck);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CompressionList);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "AEI editor by Catlabs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_Closing);
            this.Shown += new System.EventHandler(this.Form1_PostLoad);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.Status.ResumeLayout(false);
            this.Status.PerformLayout();
            this.RegionListContextMenu.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private IContainer components;

		private MenuStrip menuStrip1;

		public StatusStrip Status;

		private ToolStripMenuItem fileToolStripMenuItem;

		private ToolStripMenuItem openToolStripMenuItem;

		public ToolStripStatusLabel StatusState;

		public ToolStripProgressBar StatusProgress;

		private ToolStripMenuItem saveToolStripMenuItem;

		public ComboBox CompressionList;

		private Label label1;

		private ToolStripMenuItem imageToolStripMenuItem;

		private ToolStripMenuItem importToolStripMenuItem;

		private ToolStripMenuItem exportToolStripMenuItem;

		private ToolStripMenuItem viewToolStripMenuItem;

		private ToolStripMenuItem backgroundToolStripMenuItem;

		private ToolStripMenuItem checkerToolStripMenuItem;

		private ToolStripMenuItem whiteToolStripMenuItem;

		private ToolStripMenuItem blackToolStripMenuItem;

		private ToolStripMenuItem gradientToolStripMenuItem;

		public ToolStripStatusLabel NameField;

		public ToolStripStatusLabel SizeField;

		private ToolStripMenuItem newToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem saveAsToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator3;

		public ToolStripMenuItem OpacityView;

		private ToolStripMenuItem RegionAddGrp;

		private ToolStripMenuItem RegionDelete;

		private ToolStrip toolStrip1;

		private ToolStripButton NewBox;

		private ToolStripButton OpenBox;

		private ToolStripButton SaveBox;

		private ToolStripButton SaveAsBox;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripButton ImportBox;

		private ToolStripButton ExportBox;

		private ToolStripSeparator toolStripSeparator5;

		public ToolStripButton OpacityViewBox;

		public CheckBox MipMapCheck;

		private ToolStripSeparator toolStripSeparator6;

		private ToolStripMenuItem compressionToolStripMenuItem;

		private ToolStripMenuItem copyToolStripMenuItem;

		private ToolStripMenuItem pasteToolStripMenuItem;

		private ToolStripSeparator RegionSeparator;

		private ToolStripLabel RegionPosTip;

		private ToolStripLabel RegionSizeTip;

		private ToolStripLabel RegionPosSplitter;

		private ToolStripLabel RegionSizeSplitter;

		private ToolStripMenuItem selectAllToolStripMenuItem;

		private ToolStripMenuItem allToolStripMenuItem;

		public ToolStripTextBox RegionPosX;

		public ToolStripTextBox RegionPosY;

		public ToolStripTextBox RegionSizeX;

		public ToolStripTextBox RegionSizeY;

		public ToolStripStatusLabel StatusScale;

		public ContextMenuStrip RegionListContextMenu;

		public ToolStripLabel RegionTip;

		private ToolStripButton RegionUp;

		private ToolStripButton RegionDown;

		private ToolStripMenuItem texturesToolStripMenuItem;

		private ToolStripMenuItem RegionImport;

		private ToolStripMenuItem RegionExport;

		private ToolStripButton RegionImportBox;

		private ToolStripButton RegionExportBox;

		private ToolStripMenuItem QLow;

		private ToolStripMenuItem QMid;

		private ToolStripMenuItem QHigh;

		private ToolStripMenuItem QHighest;

		private ToolStripSeparator toolStripSeparator1;

		public ToolStripSeparator RegionIOSeparator;

		public ToolStripSeparator RegionEditSeparator;

		public ToolStripTextBox ReginSymbolEditor;

		private ToolStripMenuItem RegionAdd;

		private ToolStripMenuItem RegionSymbolAdd;

		public ToolStripLabel ReginSymbolGroupTip;
        private ToolStripButton toolStripButton1;
    }
}
