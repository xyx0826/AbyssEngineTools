using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace AEIEditor
{
    public class AeScene : Panel
    {
        /// <summary>
        /// Constructs a scene.
        /// </summary>
        /// <param name="location">The location of the scene.</param>
        /// <param name="size">The size of the scene.</param>
        public AeScene(Point location, Size size)
        {
            // Set parent properties
            Location = location;
            Size = size;
            BorderStyle = BorderStyle.FixedSingle;
            BackgroundImageLayout = ImageLayout.Tile;
            AutoScroll = true;
            DoubleBuffered = true;

            // Add listeners
            MouseEnter += HandleMouseEnter;
            MouseClick += Viewport_MouseClick;
            Resize += SetBg;
            Disposed += UnLoad;

            // Create inner viewport
            _viewport = new PictureBox();
            _viewport.Location = new Point(0, 0);
            _viewport.Size = new Size(0, 0);
            _viewport.SizeMode = PictureBoxSizeMode.AutoSize;
            _viewport.Dock = DockStyle.None;
            _viewport.BackColor = Color.Transparent;
            _viewport.BorderStyle = BorderStyle.None;
            _viewport.Padding = new Padding(5);
            _viewport.MouseEnter += HandleMouseEnter;
            _viewport.MouseWheel += Viewport_MouseWheel;
            _viewport.MouseDown += Viewport_MouseDown;
            _viewport.MouseMove += Viewport_MouseMove;
            _viewport.MouseClick += Viewport_MouseClick;
            _viewport.MouseLeave += Viewport_MouseLeave;
            _viewport.Paint += RedrawTextures;
            Controls.Add(_viewport);
            SetBg(true);
            WorkEnd();
        }

        /// <summary>
        /// Constructs a headless scene for console mode.
        /// </summary>
        public AeScene()
        {
            UiManager.ShowUi = false;
            WorkEnd();
        }

        protected override void OnMouseWheel(MouseEventArgs e) { }


        protected override Point ScrollToControl(Control activeControl)
        {
            return AutoScrollPosition;
        }

        private void SetBg(bool changedImage)
        {
            if (!changedImage && _backGroundMode > 2)
            {
                return;
            }

            Bitmap bitmap = null;
            switch (_backGroundMode)
            {
                case 1:
                    if (Height > 0)
                    {
                        var num = 255.0 / Height;
                        bitmap = new Bitmap(1, Height);
                        for (var i = 0; i < Height; i++)
                        {
                            bitmap.SetPixel(0, i, Color.FromArgb((int) (num * i), Color.SlateGray));
                        }
                    }

                    break;
                case 2:
                {
                    var num2 = 10;
                    var num3 = 1;
                    if (_aeRes != null)
                    {
                        var transform = GetTransform();
                        num2 = transform.InitialScale;
                        num3 = transform.CurrentScale;
                    }

                    var num4 = num3 - num2;
                    var num5 = num4 >= 0 ? (int) Math.Pow(2.0, num4) : 1;
                    var num6 = num5 * 2;
                    bitmap = new Bitmap(num6, num6);
                    for (var j = 0; j < num5; j++)
                    {
                        for (var k = 0; k < num5; k++)
                        {
                            bitmap.SetPixel(j, k, Color.White);
                        }
                    }

                    for (var l = num5; l < num6; l++)
                    {
                        for (var m = 0; m < num5; m++)
                        {
                            bitmap.SetPixel(l, m, Color.LightGray);
                        }
                    }

                    for (var n = 0; n < num5; n++)
                    {
                        for (var num7 = num5; num7 < num6; num7++)
                        {
                            bitmap.SetPixel(n, num7, Color.LightGray);
                        }
                    }

                    for (var num8 = num5; num8 < num6; num8++)
                    {
                        for (var num9 = num5; num9 < num6; num9++)
                        {
                            bitmap.SetPixel(num8, num9, Color.White);
                        }
                    }

                    break;
                }
                case 3:
                    bitmap = new Bitmap(1, 1);
                    bitmap.SetPixel(0, 0, Color.White);
                    break;
                case 4:
                    bitmap = new Bitmap(1, 1);
                    bitmap.SetPixel(0, 0, Color.Black);
                    break;
                default:
                    throw new Exception("Unknown background mode: " + _backGroundMode);
            }

            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
            }

            BackgroundImage = bitmap;
        }

        private void SetBg(object sender, EventArgs e)
        {
            SetBg(false);
        }

        public void SetBackGround(int mode)
        {
            _backGroundMode = mode;
            SetBg(true);
        }

        public void Load(AeImage nAeRes)
        {
            UnLoad();
            _aeRes = nAeRes;
            var resourceBitmap = GetResourceBitmap();
            SetInitialBitmap(resourceBitmap);
            if (resourceBitmap != null)
            {
                SetBitmap(new Bitmap(resourceBitmap));
                _loaded = true;
            }

        }

        public void LoadResource(string path)
        {
            if (path == null)
            {
                return;
            }

            Load(Program.DoAsync(f => new AeImage((string) f), "Reading data", path));
        }

        public void UnLoad(bool resetSelection = true)
        {
            if (UiManager.ShowUi)
            {
                _viewport.InitialImage?.Dispose();
                _viewport.Image?.Dispose();
                _viewport.Image = _viewport.InitialImage = null;
            }

            if (_loaded)
            {
                if (resetSelection)
                {
                    SetTexturesSelected(false);
                }

                _aeRes.Dispose();
            }

            _aeRes = null;
            _loaded = false;
        }

        public void UnLoad(object sender, EventArgs e)
        {
            UnLoad(false);
        }

        public void WorkStart()
        {
            if (_creatingRegion)
            {
                return;
            }

            _ready = false;
        }

        public void WorkEnd(bool updateViewport = false)
        {
            if (!_creatingRegion)
            {
                _ready = true;
            }

            if (!updateViewport)
            {
                return;
            }

            ReDraw();
        }

        public bool IsReady()
        {
            return _ready && !_creatingRegion && !_draggingRegion || _doDrawRegion;
        }

        public bool IsLoaded()
        {
            return _loaded;
        }

        public bool ImportTexture(string path, AeImageTexture texture, bool showWarning = true)
        {
            var result = true;
            path = Program.ValidateFilePath(path, Program.DefaultTextureExtension);
            if (path != null)
            {
                using (var fileStream = File.OpenRead(path))
                {
                    var bitmap = new Bitmap(fileStream);
                    var width = texture.Size.Width;
                    var height = texture.Size.Height;
                    if (width != bitmap.Width || height != bitmap.Height)
                    {
                        result = false;
                        if (showWarning)
                        {
                            UiManager.ShowMessage(
                                "The texture fragment had the wrong resolution and was automatically scaled",
                                "Importing a texture fragment");
                        }

                        bitmap = Program.ReRenderBitmap(bitmap, width, height);
                    }

                    var graphics = Graphics.FromImage(GetResourceBitmap());
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.DrawImageUnscaled(bitmap, texture.Location);
                }
            }

            return result;
        }

        public void ImportTextures(string thePath, AeImageTexture[] textures)
        {
            Program.DoAsync(delegate
            {
                thePath = Program.GetValidatedPath(thePath);
                if (thePath == null)
                {
                    return;
                }

                var files =
                    Directory.GetFiles(thePath, "*.png", SearchOption.TopDirectoryOnly);
                var num = files.Length;
                if (num == 0)
                {
                    UiManager.ShowMessage("Could not find any PNG files",
                        "Importing texture fragments");
                    return;
                }

                var num2 = textures.Length;
                var num3 = num < num2 ? num : num2;
                ProgressManager.Update("Importing texture fragments", num3);
                var flag = false;
                for (var i = 0; i < num3; i++)
                {
                    if (!ImportTexture(files[i], textures[i], false))
                    {
                        flag = true;
                    }

                    ProgressManager.Update();
                }

                if (!flag)
                {
                    return;
                }

                UiManager.ShowMessage(
                    "Some texture fragments had the wrong resolution and were automatically scaled",
                    "Importing texture fragments");
            }, "Searching for files");
        }

        public void ImportTextures(string thePath, KeyValuePair<int, string>[] textureMap)
        {
            ProgressManager.Start("Importing texture fragments");
            thePath = Program.GetValidatedPath(thePath);
            if (thePath != null)
            {
                ProgressManager.SetMax(textureMap.Length);
                foreach (var keyValuePair in textureMap)
                {
                    ImportTexture(Path.Combine(thePath, keyValuePair.Value),
                        GetTexture(keyValuePair.Key));
                    ProgressManager.Update();
                }
            }

            ProgressManager.End();
        }

        public void ImportTextures(string thePath)
        {
            ImportTextures(thePath, GetTextures());
        }

        public void ExportTexture(string fileName, AeImageTexture texture)
        {
            using (var bitmap = GetResourceBitmap()
                .Clone(new Rectangle(texture.Location, texture.Size), PixelFormat.Format32bppArgb))
            {
                bitmap.Save(fileName);
            }
        }

        public void ExportTextures(string thePath, AeImageTexture[] textures)
        {
            Program.DoAsync(delegate
            {
                thePath = Program.GetValidatedPath(thePath, true);
                if (thePath == null)
                {
                    return;
                }

                ProgressManager.SetMax(textures.Length);
                var fileNameWithoutExtension =
                    Path.GetFileNameWithoutExtension(GetResource().Path);
                var text = ".png";
                var str = string.Format(".{0}{1}", fileNameWithoutExtension, text);
                foreach (var aeimageTexture in textures)
                {
                    var symbolData = aeimageTexture.SymbolData;
                    if (symbolData != null)
                    {
                        ExportTexture(
                            Path.Combine(thePath,
                                string.Format("{0:000}.{1}.Symbol_{2}_{3}{4}",
                                    aeimageTexture.Id + 1, fileNameWithoutExtension,
                                    symbolData.GroupId, symbolData.Symbol, text)), aeimageTexture);
                    }
                    else
                    {
                        ExportTexture(
                            Path.Combine(thePath, (aeimageTexture.Id + 1).ToString("000") + str),
                            aeimageTexture);
                    }

                    ProgressManager.Update();
                }
            }, "Exporting texture fragments");
        }

        public void ExportTextures(string thePath, KeyValuePair<int, string>[] textureMap)
        {
            ProgressManager.Start("Exporting texture fragments");
            thePath = Program.GetValidatedPath(thePath, true);
            if (thePath != null)
            {
                ProgressManager.SetMax(textureMap.Length);
                foreach (var keyValuePair in textureMap)
                {
                    ExportTexture(Path.Combine(thePath, keyValuePair.Value),
                        GetTexture(keyValuePair.Key));
                    ProgressManager.Update();
                }
            }

            ProgressManager.End();
        }

        public void ExportTextures(string thePath)
        {
            ExportTextures(thePath, GetTextures());
        }

        public void BeginDrawingRegion(bool asSymbol)
        {
            _drawingSymbolRegion = asSymbol;
            _doDrawRegion = true;
        }

        public void ReDraw()
        {
            if (!UiManager.ShowUi)
            {
                return;
            }

            _viewport.Refresh();
        }

        public Bitmap GetInitialBitmap()
        {
            return (Bitmap) _viewport.InitialImage;
        }

        public Bitmap GetBitmap()
        {
            return (Bitmap) _viewport.Image;
        }

        public void SetInitialBitmap(Bitmap a)
        {
            if (!UiManager.ShowUi)
            {
                return;
            }

            if (_viewport.InitialImage != null)
            {
                _viewport.InitialImage.Dispose();
            }

            _viewport.InitialImage = a;
        }

        public void SetBitmap(Bitmap a, double zoom = 1.0)
        {
            if (!UiManager.ShowUi)
            {
                return;
            }

            if (_viewport.Image != null)
            {
                _viewport.Image.Dispose();
            }

            _viewport.Image = a;
            SetBg(false);
            UiManager.SetStatusScale(100.0 * zoom);
        }

        public void ImportBitmap(string fileName)
        {
            if (fileName == null)
            {
                return;
            }

            ProgressManager.Start("Reading texture");
            var flag = _aeRes == null;
            var thePath = !flag
                ? _aeRes.Path
                : Path.Combine(Path.GetDirectoryName(fileName),
                    Path.GetFileNameWithoutExtension(fileName) + ".aei");
            Bitmap bitmap;
            using (var fileStream = File.OpenRead(fileName))
            {
                bitmap = new Bitmap(fileStream);
            }

            var nearestPot = Program.GetNearestPot((ushort) bitmap.Width);
            var nearestPot2 = Program.GetNearestPot((ushort) bitmap.Height);
            if (nearestPot != bitmap.Width || nearestPot2 != bitmap.Height)
            {
                UiManager.ShowMessage(
                    "The texture had the wrong resolution and was automatically scaled",
                    "Importing a texture");
                bitmap = Program.ReRenderBitmap(bitmap, nearestPot, nearestPot2);
            }

            var aeimage = new AeImage(_aeRes, bitmap, thePath);
            if (!flag)
            {
                aeimage.Textures = _aeRes.Textures;
            }

            ProgressManager.Update("Filling in information");
            Load(aeimage);
            ProgressManager.End();
        }

        public void ExportBitmap(string fileName)
        {
            if (fileName == null)
            {
                return;
            }

            ProgressManager.Start("Saving texture");
            GetResourceBitmap().Save(fileName);
            ProgressManager.End();
        }

        public AeImage GetResource()
        {
            return _aeRes;
        }

        public Bitmap GetResourceBitmap()
        {
            return _aeRes.Bmp;
        }

        public TransformContainer GetTransform()
        {
            return _aeRes.Transform;
        }

        public bool SetMipMapped(bool flag)
        {
            var resource = GetResource();
            if (flag && !Compression.IsFormatMipMappable(resource.CompressionAe2Format))
            {
                return false;
            }

            resource.MipMapped = flag;
            return true;
        }

        public AeImageTexture GetTexture(int index)
        {
            return _aeRes.Textures[index];
        }

        public void SetTexture(int index, AeImageTexture tex)
        {
            _aeRes.Textures[index] = tex;
        }

        public AeImageTexture[] GetTextures()
        {
            return _aeRes.Textures;
        }

        public AeImageTexture[] GetSelectedTextures()
        {
            return Array.FindAll(GetTextures(), tex => tex.IsSelected);
        }

        public AeImageTexture GetSelectedTexture()
        {
            var selectedTextures = GetSelectedTextures();
            if (selectedTextures.Length != 1)
            {
                return null;
            }

            return selectedTextures[0];
        }

        public bool IsTextureSelected()
        {
            return GetSelectedTextures().Length != 0;
        }

        public int GetTextureCount()
        {
            return _aeRes.Textures.Length;
        }

        public void RecalculateTextureIndices()
        {
            var textureCount = GetTextureCount();
            for (var i = 0; i < textureCount; i++)
            {
                _aeRes.Textures[i].Id = i;
            }
        }

        public void AddTexture(
            AeImageTexture texture, int startIndex, bool recalculateIndices = true)
        {
            Program.Array_Insert(texture, ref _aeRes.Textures, startIndex);
            if (!recalculateIndices)
            {
                return;
            }

            RecalculateTextureIndices();
        }

        public void AddTextures(AeImageTexture[] textures, int startIndex)
        {
            for (var i = 0; i < textures.Length; i++)
            {
                AddTexture(textures[i], startIndex + i, false);
            }

            if (startIndex == GetTextureCount())
            {
                return;
            }

            RecalculateTextureIndices();
        }

        public void DeleteSelectedTextures()
        {
            var textureCount = GetTextureCount();
            if (textureCount == 1)
            {
                var aeimageTexture = _aeRes.Textures[0];
                aeimageTexture.Location.X = aeimageTexture.Location.Y = 0;
                aeimageTexture.Size = _aeRes.Size;
            }
            else
            {
                var selectedTextures = GetSelectedTextures();
                if (textureCount == selectedTextures.Length)
                {
                    Array.Resize(ref _aeRes.Textures, 1);
                    SetTexture(0, new AeImageTexture(_aeRes.Size, 0));
                }
                else
                {
                    Array.Sort(selectedTextures, (a, b) => b.Id - a.Id);
                    foreach (var aeimageTexture2 in selectedTextures)
                    {
                        Program.Array_Delete(ref _aeRes.Textures, aeimageTexture2.Id);
                    }

                    RecalculateTextureIndices();
                }
            }

            HandleSelectionChanged();
        }

        public int GetNewTextureIndex()
        {
            var textures = GetTextures();
            var selectedTextures = GetSelectedTextures();
            var num = Array.FindIndex(textures, tex => tex.SymbolData != null);
            if (selectedTextures.Length == 0)
            {
                if (num != -1)
                {
                    return num;
                }

                return GetTextureCount();
            }

            var num2 = -1;
            var flag = false;
            foreach (var aeimageTexture in selectedTextures)
            {
                if (aeimageTexture.SymbolData == null)
                {
                    var id = aeimageTexture.Id;
                    if (id > num2)
                    {
                        num2 = id;
                    }

                    flag = true;
                }
            }

            if (flag)
            {
                return num2 + 1;
            }

            return num;
        }

        public void SetTexturesSelected(bool flag, bool handleUi = true)
        {
            foreach (var tex in GetTextures())
            {
                tex.IsSelected = flag;
            }

            if (handleUi)
            {
                HandleSelectionChanged();
            }
        }

        public void MoveTexture(int offset)
        {
            var texture = GetSelectedTexture();
            var textures = _aeRes.Textures;
            var symbolData = texture.SymbolData;
            if (symbolData != null)
            {
                var groupId = symbolData.GroupId;
                if (offset < 0 && groupId == 0 || offset > 0 && Array.FindIndex(textures,
                    tex => tex != texture && tex.SymbolData != null &&
                           tex.SymbolData.GroupId == groupId) < 0)
                {
                    return;
                }

                symbolData.GroupId += offset;
                UiManager.UpdateEditor(texture);
            }
            else
            {
                var id = texture.Id;
                var num = id + offset;
                if (offset < 0 && id == 0 || offset > 0 && num == GetTextureCount() ||
                    textures[num].SymbolData != null)
                {
                    return;
                }

                var aeimageTexture = textures[num];
                var id2 = aeimageTexture.Id;
                textures[num] = textures[id];
                aeimageTexture.Id = texture.Id;
                textures[id] = aeimageTexture;
                texture.Id = id2;
                HandleSelectionChanged();
            }
        }

        public void RedrawTextures(object sender, PaintEventArgs e)
        {
            if (!_loaded)
            {
                return;
            }

            foreach (var aeimageTexture in GetTextures())
            {
                var transform = GetTransform();
                var location = aeimageTexture.Location;
                var size = aeimageTexture.Size;
                var num = Math.Pow(2.0, transform.CurrentScale - transform.InitialScale);
                var num2 = (int) (location.X * num) + 5;
                var num3 = (int) (location.Y * num) + 5;
                var num4 = (int) (size.Width * num);
                var num5 = (int) (size.Height * num);
                var isSelected = aeimageTexture.IsSelected;
                var graphics = e.Graphics;
                var symbolData = aeimageTexture.SymbolData;
                var flag = symbolData != null;
                var pen = flag
                    ? isSelected ? Pens.Blue : Pens.LightSkyBlue
                    : isSelected ? Pens.Red : Pens.LightGreen;
                graphics.DrawRectangle(pen, num2, num3, num4, num5);
                if (isSelected)
                {
                    graphics.DrawLine(pen, num2, num3, num2 + num4, num3 + num5);
                    graphics.DrawLine(pen, num2 + num4, num3, num2, num3 + num5);
                }

                var num6 = 5;
                var num7 = 2;
                var num8 = num6 + num7;
                string text;
                if (!flag)
                {
                    text = (aeimageTexture.Id + 1).ToString();
                }
                else
                {
                    var symbol = (char) symbolData.Symbol;
                    text = symbol.ToString();
                }

                var text2 = text;
                var font = new Font(SystemFonts.IconTitleFont, FontStyle.Bold);
                var sizeF = graphics.MeasureString(text2, font);
                using (Brush brush = new SolidBrush(Color.FromArgb(128, pen.Color)))
                {
                    graphics.FillRectangle(brush,
                        new Rectangle(num2 + num6, num3 + num6, num6 + (int) sizeF.Width,
                            num6 + (int) sizeF.Height));
                }

                graphics.DrawString(text2, font, SystemBrushes.HighlightText, num2 + num8,
                    num3 + num8);
            }
        }

        private void HandleMouseEnter(object sender, EventArgs e) => _viewport.Focus();

        public void RenderImageScaled(int scale = 0, int maxImageSize = 8192)
        {
            var transform = GetTransform();
            if (scale == 0)
            {
                scale = transform.CurrentScale;
            }

            var num = Math.Pow(2.0, scale - transform.InitialScale);
            Image initialBitmap = GetInitialBitmap();
            var num2 = (int) (initialBitmap.Width * num);
            var num3 = (int) (initialBitmap.Height * num);
            var num4 = num2;
            if (num3 > num4)
            {
                num4 = num3;
            }

            if (num4 > maxImageSize)
            {
                return;
            }

            var bitmap = GetInitialBitmap();
            var flag = true;
            try
            {
                bitmap = Program.ReRenderBitmap(bitmap, num2, num3, Program.PxFmtCurrent, false);
            }
            catch
            {
                flag = false;
            }

            if (!flag)
            {
                return;
            }

            transform.CurrentScale = scale;
            SetBitmap(bitmap, num);
        }

        private void Viewport_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!_ready || e.Delta == 0)
            {
                return;
            }

            WorkStart();
            if (_loaded)
            {
                Image initialBitmap = GetInitialBitmap();
                var width = initialBitmap.Width;
                var height = initialBitmap.Height;
                var num = height > width ? height : width;
                var maxImageSize = 8192;
                if (num > 8192)
                {
                    maxImageSize = num;
                }

                var flag = true;
                var num2 = GetTransform().CurrentScale;
                if (e.Delta > 0)
                {
                    num2++;
                }
                else if (num2 != 1)
                {
                    num2--;
                }
                else
                {
                    flag = false;
                }

                if (flag)
                {
                    RenderImageScaled(num2, maxImageSize);
                }
            }

            WorkEnd();
        }

        private void Viewport_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            _mouseDragStartPos = e.Location;
            var transform = GetTransform();
            var num = Math.Pow(2.0, transform.CurrentScale - transform.InitialScale);
            var num2 = (int) Math.Round((_mouseDragStartPos.X - 5) / num);
            var num3 = (int) Math.Round((_mouseDragStartPos.Y - 5) / num);
            if (_doDrawRegion)
            {
                var newTextureIndex = GetNewTextureIndex();
                var aeimageTexture = new AeImageTexture(new Point(num2, num3),
                    newTextureIndex, _drawingSymbolRegion);
                AddTexture(aeimageTexture, newTextureIndex);
                SetTexturesSelected(false);
                aeimageTexture.IsSelected = true;
                _doDrawRegion = false;
                _creatingRegion = true;
                HandleSelectionChanged();
                return;
            }

            var selectedTexture = GetSelectedTexture();
            if (selectedTexture == null)
            {
                return;
            }

            var location = selectedTexture.Location;
            var size = selectedTexture.Size;
            var x = location.X;
            var y = location.Y;
            var num4 = x + size.Width;
            var num5 = y + size.Height;
            _draggingRegion = num2 >= x && num3 >= y && num2 <= num4 && num3 <= num5;
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_creatingRegion && !_draggingRegion)
            {
                return;
            }

            var selectedTexture = GetSelectedTexture();
            var size = GetResource().Size;
            var size2 = selectedTexture.Size;
            var location = selectedTexture.Location;
            var location2 = e.Location;
            var x = location.X;
            var y = location.Y;
            var width = size2.Width;
            var height = size2.Height;
            var width2 = size.Width;
            var height2 = size.Height;
            var transform = GetTransform();
            var num = Math.Pow(2.0, transform.CurrentScale - transform.InitialScale);
            var num2 = (int) Math.Round((location2.X - _mouseDragStartPos.X) / num);
            var num3 = (int) Math.Round((location2.Y - _mouseDragStartPos.Y) / num);
            if (_creatingRegion)
            {
                if (x + num2 > width2)
                {
                    num2 = width2 - x;
                }

                if (y + num3 > height2)
                {
                    num3 = height2 - y;
                }

                selectedTexture.Size = new Size(num2, num3);
            }
            else
            {
                var num4 = location.X + num2;
                var num5 = location.Y + num3;
                if (num4 >= 0 && num4 + width <= width2)
                {
                    var aeimageTexture = selectedTexture;
                    aeimageTexture.Location.X = aeimageTexture.Location.X + num2;
                }

                if (num5 >= 0 && num5 + height <= height2)
                {
                    var aeimageTexture2 = selectedTexture;
                    aeimageTexture2.Location.Y = aeimageTexture2.Location.Y + num3;
                }

                _mouseDragStartPos = location2;
            }

            UiManager.UpdateEditor(selectedTexture);
            ReDraw();
        }

        private void Viewport_MouseClick(object sender, MouseEventArgs e)
        {
            if (_doDrawRegion || _creatingRegion || _draggingRegion)
            {
                UiManager.SetState("Done");
                _ready = !(_doDrawRegion = _creatingRegion = _draggingRegion = false);
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                UiManager.ShowViewportContextMenu(MousePosition);
                return;
            }

            if (e.Button != MouseButtons.Left || sender == this && !_loaded)
            {
                return;
            }

            var transform = GetTransform();
            var num = Math.Pow(2.0, transform.CurrentScale - transform.InitialScale);
            var location = e.Location;
            var num2 = (int) ((location.X - 5) / num);
            var num3 = (int) ((location.Y - 5) / num);
            var textureCount = GetTextureCount();
            var textures = GetTextures();
            var flag = !ModifierKeys.HasFlag(Keys.Control);
            var flag2 = !ModifierKeys.HasFlag(Keys.Shift);
            if (flag)
            {
                SetTexturesSelected(false, false);
            }

            for (var i = textureCount - 1; i > -1; i--)
            {
                var aeimageTexture = textures[i];
                var location2 = aeimageTexture.Location;
                var size = aeimageTexture.Size;
                var x = location2.X;
                var y = location2.Y;
                var num4 = x + size.Width;
                var num5 = y + size.Height;
                if (num2 >= x && num3 >= y && num2 <= num4 && num3 <= num5)
                {
                    aeimageTexture.IsSelected = !aeimageTexture.IsSelected;
                    if (flag && flag2)
                    {
                        break;
                    }
                }
            }

            HandleSelectionChanged();
        }

        private void Viewport_MouseLeave(object sender, EventArgs e)
        {
            if (!_doDrawRegion && !_creatingRegion && !_draggingRegion)
            {
                return;
            }

            UiManager.SetState("Done");
            _ready = !(_doDrawRegion = _creatingRegion = _draggingRegion = false);
        }

        private void HandleSelectionChanged()
        {
            UiManager.ShowRegionControls(GetSelectedTextures());
            ReDraw();
        }

        private const int ViewportPadding = 5;

        private const int MaxScaledImageSize = 8192;

        private int _backGroundMode = 1;

        public AeImageTexture[] SelectedRegions = new AeImageTexture[0];

        private bool _ready;

        private bool _drawingSymbolRegion;

        private bool _doDrawRegion;

        private bool _creatingRegion;

        private bool _draggingRegion;

        private bool _loaded;

        private Point _mouseDragStartPos;

        private AeImage _aeRes;

        private PictureBox _viewport;
    }
}