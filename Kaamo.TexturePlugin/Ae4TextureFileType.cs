using System;
using System.IO;
using PaintDotNet;
using PaintDotNet.PropertySystem;

namespace Kaamo.TexturePlugin
{
    internal sealed class Ae4TextureFileType : PropertyBasedFileType
    {
        private const string Ae4TextureName = "Abyss Engine 4 Texture";

        private static FileTypeOptions Ae4TextureOptions => new()
        {
            LoadExtensions = new[] { ".ae4texture" },
            SaveExtensions = new[] { ".ae4texture" }
        };

        public Ae4TextureFileType() : base(Ae4TextureName, Ae4TextureOptions)
        {
        }

        protected override Document OnLoad(Stream input)
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveT(
            Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface,
            ProgressEventHandler progressCallback)
        {
            throw new NotImplementedException();
        }

        public override PropertyCollection OnCreateSavePropertyCollection()
        {
            throw new NotImplementedException();
        }
    }
}
