using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Kaamo.Annotations;
using Microsoft.Win32;

namespace Kaamo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static ICommand OpenFileCommand = new RoutedCommand();

        private string _filePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        private void ExecutedOpenFileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Abyss Engine 4 textures (*.ae4texture)|*.ae4texture";
            if (dialog.ShowDialog() == true)
            {
                _filePath = dialog.FileName;
                LoadTexture();
            }
        }

        private void LoadTexture()
        {
            // Load texture data
            using var file = File.OpenRead(_filePath);
            var tex = Ae4TextureReader.Read(file);
            var bitmaps = Decompressor.Decompress(tex);
            var source = new RgbaBitmapSource(bitmaps[0], tex.Width);
            ImageSource = source;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
