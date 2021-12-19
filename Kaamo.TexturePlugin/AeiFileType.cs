using System;
using PaintDotNet;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Kaamo.Texture;
using Kaamo.Texture.Utils;

namespace Kaamo.TexturePlugin;

internal sealed class AeiFileType : PropertyBasedFileType
{
    private const string AeiName = "Abyss Engine Image";

    private static FileTypeOptions AeiOptions => new()
    {
        LoadExtensions = new[] { ".aei" },
        SaveExtensions = new[] { ".aei" }
    };

    private enum PropertyName
    {
        Creator
    }

    internal AeiFileType() : base(AeiName, AeiOptions)
    {
    }

    public override PropertyCollection OnCreateSavePropertyCollection()
    {
        List<Property> props = new List<Property>();
        props.Add(new StringProperty(PropertyName.Creator, "", 256));

        List<PropertyCollectionRule> propRules = new List<PropertyCollectionRule>();

        return new PropertyCollection(props, propRules);
    }

    public override ControlInfo OnCreateSaveConfigUI(PropertyCollection props)
    {

        ControlInfo configUI = CreateDefaultSaveConfigUI(props);
        configUI.SetPropertyControlValue(PropertyName.Creator, ControlInfoPropertyNames.DisplayName, "Name of creator");

        return configUI;
        //return base.OnCreateSaveConfigUI(props);
    }

    protected override Document OnLoad(Stream input)
    {
        AeImage image;
        try
        {
            image = AeImageReader.Read(input);
        }
        catch (Exception ex)
        {
            throw new FormatException(ex.Message);
        }

        var surface = image.Surfaces[0];
        var document = new Document(image.Width, image.Height);
        BitmapLayer layer = new BitmapLayer(image.Width, image.Height);
        Marshal.Copy(surface, 0, layer.Surface.Scan0.Pointer, surface.Length);
        document.Layers.Add(layer);
        return document;
    }

    protected override void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)
    {
    }
}