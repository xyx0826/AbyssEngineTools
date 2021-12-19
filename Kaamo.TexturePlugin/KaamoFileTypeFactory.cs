using System;
using System.Runtime.InteropServices;
using PaintDotNet;

namespace Kaamo.TexturePlugin
{
    /**
     * Creates FileType handlers for paint.net to load.
     */
    public sealed class KaamoFileTypeFactory : IFileTypeFactory
    {
        public FileType[] GetFileTypeInstances()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                && RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                return new FileType[] { new AeiFileType(), new Ae4TextureFileType() };
            }

            throw new PlatformNotSupportedException(
                "Your operating system and/or processor architecture is unsupported." +
                "This extension currently only runs on 64-bit Windows.");
        }
    }
}
