# Kaamo Texture Plugin for Paint.NET

This is a [Paint.NET](https://www.getpaint.net/) file type plugin
for editing Abyss Engine (Galaxy on Fire) game images and
textures.

The plugin aims to support loading/saving `.aei`, `.ae4texture` formats with
various pixel formats.

## Installing

Download the latest version of Paint.NET.
Both the installer and portable versions will work.

In Paint.NET installation folder, create a folder called `FileTypes`.

Unzip the built plugin into a standalone folder inside `FileTypes`.
The resulting file structure should look like this:

```txt
< Paint.NET installation folder >
    -> FileTypes
        -> Kaamo.TexturePlugin
            -> < Plugin files >
```

## Usage

When this plugin is installed, Paint.NET can open any `.aei`
and `.ae4texture` (when supported) files, as well as saving
to these formats.

Some `.aei` files - especially item and font files - have "regions"
that define multiple individual icons/font glyphs within a single image.

To define and recreate regions, create layers named `r#` where `#` is the
region's numerical ID and draw a single rectangle or box over the desired
region.

Similarly, to define and recreate glyphs, create layers named `g?` where `?` is
the glyph's character and do the same thing.

When saving an image to `.aei` without any region or glyph layers defined,
the plugin will create a single region for the entire image.
This is often useful for 3D texture files.
