using Kaamo.Texture.Enums;
using Kaamo.Texture.Utils;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Kaamo.Texture
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Ae4TextureResource
    {
        public int Format;
        public short LinearMipmapMode;
        public short MagFilter;
        public short SAddressMode;
        public short TAddressMode;
        public short RAddressMode;
        public int Width;
        public int Height;
        public int Depth;
        public int MipmapLevelCount;
        public int Field_98;
        public int Field_A0;
        public bool IsCubeMap;
        public byte Gap;
        public bool SetFilterAndAddressMode;
    }

    internal class Ae4Texture
    {
        private readonly Ae4TextureResource _resource;

        public Ae4Texture(Ae4TextureResource resource)
        {
            _resource = resource;
            Format = PixelFormatUtils.GetPixelFormat(Engine.AbyssEngine4, (byte)_resource.Format);
            Width = _resource.Width;
            Height = _resource.Height;
            Depth = _resource.Depth;
            MipmapCount = _resource.MipmapLevelCount;
            IsCubemap = _resource.IsCubeMap;
        }

        public readonly PixelFormat Format;

        public readonly int Width;

        public readonly int Height;

        public readonly int Depth;

        public readonly int MipmapCount;

        public readonly bool IsCubemap;

        public byte[] Data;
    }

    public class Ae4TextureReader
    {
        private static bool CheckMagicAndVersion(Stream stream)
        {
            if (stream.Position + 8 <= stream.Length)
            {
                // Length OK
                return stream.ReadUInt() == 0x41345445 // SerializationId, "ET4A"
                       && stream.ReadUInt() == 2;   // SerializationVersion
            }

            throw new Exception("Not enough length.");
        }

        public static unsafe Ae4Texture Read(Stream stream)
        {
            if (!CheckMagicAndVersion(stream))
            {
                throw new Exception(
                    "Magic value does not match or unsupported serialization version.");
            }

            stream.Position += 4;   // Unknown u32, seems to always be zero
            Span<byte> buf = stackalloc byte[sizeof(Ae4TextureResource)];
            stream.Read(buf);
            var resource = MemoryMarshal.Read<Ae4TextureResource>(buf);

            stream.Position = 0x4000;
            var dataLength = (int)(stream.Length - stream.Position);
            var data = stream.ReadBytes(dataLength);

            return new Ae4Texture(resource)
            {
                Data = data
            };
        }
    }
}
