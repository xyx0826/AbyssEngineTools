using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kaamo
{
    /// <summary>
    /// https://stackoverflow.com/a/21565089/5755216
    /// </summary>
    internal class RgbaBitmapSource : BitmapSource
    {
        private readonly byte[] rgbaBuffer;
        private readonly int pixelWidth;
        private readonly int pixelHeight;

        public RgbaBitmapSource(byte[] rgbaBuffer, int pixelWidth)
        {
            this.rgbaBuffer = rgbaBuffer;
            this.pixelWidth = pixelWidth;
            this.pixelHeight = rgbaBuffer.Length / (4 * pixelWidth);
        }

        public override unsafe void CopyPixels(
            Int32Rect sourceRect, Array pixels, int stride, int offset)
        {
            fixed (byte* source = rgbaBuffer, destination = (byte[])pixels)
            {
                byte* dstPtr = destination + offset;

                for (int y = sourceRect.Y; y < sourceRect.Y + sourceRect.Height; y++)
                {
                    for (int x = sourceRect.X; x < sourceRect.X + sourceRect.Width; x++)
                    {
                        byte* srcPtr = source + stride * y + 4 * x;
                        byte a = *(srcPtr + 3);
                        *(dstPtr++) = (byte)(*(srcPtr + 2) * a / 256); // pre-multiplied B
                        *(dstPtr++) = (byte)(*(srcPtr + 1) * a / 256); // pre-multiplied G
                        *(dstPtr++) = (byte)(*srcPtr * a / 256); // pre-multiplied R
                        *(dstPtr++) = a;
                    }
                }
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new RgbaBitmapSource(rgbaBuffer, pixelWidth);
        }

        public override event EventHandler<DownloadProgressEventArgs> DownloadProgress;
        public override event EventHandler DownloadCompleted;
        public override event EventHandler<ExceptionEventArgs> DownloadFailed;
        public override event EventHandler<ExceptionEventArgs> DecodeFailed;

        public override double DpiX => 96;

        public override double DpiY => 96;

        public override PixelFormat Format => PixelFormats.Pbgra32;

        public override int PixelWidth => pixelWidth;

        public override int PixelHeight => pixelHeight;

        public override double Width => pixelWidth;

        public override double Height => pixelHeight;
    }
}
