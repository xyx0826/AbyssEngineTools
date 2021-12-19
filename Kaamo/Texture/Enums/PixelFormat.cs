namespace Kaamo.Texture.Enums
{
    /// <summary>
    /// <para>
    /// Pixel formats used by all Abyss Engine versions. Grouped by types.
    /// To convert from/to engine or library specific formats, use extension methods.
    /// </para>
    /// <para>
    /// Names are derived from corresponding Metal and OpenGL ES format enums,
    /// following the order [Compression] [[Channel] [Bits]] ... [Version].
    /// </para>
    /// <para>
    /// <see cref="Rgb16Bpb1"/> and <see cref="Rgb16Bpb2"/> are <see cref="Engine.AbyssEngine4"/>
    /// formats with unknown compression yet the same block count of 16 bytes per block (BPB).
    /// </para>
    /// </summary>
    public enum PixelFormat
    {
        // RGB(A) raw formats

        [PixelFormat(Engine.AbyssEngine4, 0)]
        Rgb8,

        [PixelFormat(Engine.AbyssEngine4, 3)]
        Rgba4,

        [PixelFormat(Engine.AbyssEngine2, 129)]
        [PixelFormat(Engine.AbyssEngine4, 1)]
        Rgba8,

        [PixelFormat(Engine.AbyssEngine4, 4)]
        Rgb5A1,

        [PixelFormat(Engine.AbyssEngine4, 31)]
        Rg11B10F,

        [PixelFormat(Engine.AbyssEngine4, 32)]
        Rgba16F,

        [PixelFormat(Engine.AbyssEngine4, 33)]
        Rgba32F,

        [PixelFormat(Engine.AbyssEngine2, 1)]
        [PixelFormat(Engine.AbyssEngine2, 3)]
        [PixelFormat(Engine.AbyssEngine4, 8)]
        Bgra8,

        // PVRTC compressed formats

        [PixelFormat(Engine.AbyssEngine4, 9)]
        PvrtcRgb2BppV1,

        [PixelFormat(Engine.AbyssEngine4, 10)]
        PvrtcRgb4BppV1,

        [PixelFormat(Engine.AbyssEngine2, 13)]
        [PixelFormat(Engine.AbyssEngine2, 15)]
        [PixelFormat(Engine.AbyssEngine4, 11)]
        PvrtcRgba2BppV1,

        [PixelFormat(Engine.AbyssEngine2, 16)]
        [PixelFormat(Engine.AbyssEngine2, 18)]
        [PixelFormat(Engine.AbyssEngine4, 12)]
        PvrtcRgba4BppV1,

        [PixelFormat(Engine.AbyssEngine4, 13)]
        PvrtcRgba2BppV2,

        [PixelFormat(Engine.AbyssEngine4, 14)]
        PvrtcRgba4BppV2,

        // ATC and ETC compressed formats

        [PixelFormat(Engine.AbyssEngine2, 17)]
        [PixelFormat(Engine.AbyssEngine2, 19)]
        [PixelFormat(Engine.AbyssEngine4, 24)]
        AtcRgb,

        [PixelFormat(Engine.AbyssEngine4, 25)]
        AtcRgbaExplicitAlpha,

        [PixelFormat(Engine.AbyssEngine4, 26)]
        AtcRgbaInterpolatedAlpha,

        [PixelFormat(Engine.AbyssEngine2, 64)]
        [PixelFormat(Engine.AbyssEngine2, 66)]
        [PixelFormat(Engine.AbyssEngine4, 15)]
        Etc1Rgb8,

        [PixelFormat(Engine.AbyssEngine4, 16)]
        Etc2Rgb8,

        [PixelFormat(Engine.AbyssEngine4, 17)]
        Etc2Rgba8,

        [PixelFormat(Engine.AbyssEngine4, 18)]
        Etc2Rgb8A1,

        // S3TC compressed formats

        [PixelFormat(Engine.AbyssEngine2, 32)]
        [PixelFormat(Engine.AbyssEngine2, 34)]
        [PixelFormat(Engine.AbyssEngine4, 19)]
        Dxt1Rgba,

        [PixelFormat(Engine.AbyssEngine2, 33)]
        [PixelFormat(Engine.AbyssEngine2, 35)]
        [PixelFormat(Engine.AbyssEngine4, 21)]
        Dxt3Rgba,

        [PixelFormat(Engine.AbyssEngine2, 36)]
        [PixelFormat(Engine.AbyssEngine2, 38)]
        [PixelFormat(Engine.AbyssEngine4, 23)]
        Dxt5Rgba,

        // Special formats
        Unknown,    // AE2 20, 22, 23

        [PixelFormat(Engine.AbyssEngine4, 5)]
        Alpha8,

        [PixelFormat(Engine.AbyssEngine4, 30)]
        Stencil8,

        [PixelFormat(Engine.AbyssEngine4, 6)]
        Luminance8,

        [PixelFormat(Engine.AbyssEngine4, 7)]
        LuminanceAlpha8,

        [PixelFormat(Engine.AbyssEngine4, 27)]
        Depth32F,

        [PixelFormat(Engine.AbyssEngine4, 28)]
        Depth24Stencil8,

        [PixelFormat(Engine.AbyssEngine4, 2)]
        Rgb16Bpp,

        [PixelFormat(Engine.AbyssEngine4, 20)]
        Rgb16Bpb1,

        [PixelFormat(Engine.AbyssEngine4, 22)]
        Rgb16Bpb2,

        [PixelFormat(Engine.AbyssEngine4, 29)]
        Rgb40Bpp,

        //[PixelFormat(Engine.AbyssEngine2, 11)]
        //Rgba8CubemapPc,

        //[PixelFormat(Engine.AbyssEngine2, 12)]
        //Rgba8Cubemap
    }
}
