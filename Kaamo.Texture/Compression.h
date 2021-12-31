#pragma once
#include "third_party/detex/detex.h"

#define EXTERN_DLL_EXPORT extern "C" __declspec(dllexport)

/// <summary>
/// Pixel formats supported by native decompression libraries.
/// </summary>
enum class NativePixelFormat
{
	// PVR
	Pvrtc2,
	Pvrtc4,
	// detex
	Bc1A = DETEX_TEXTURE_FORMAT_BC1A,
	Bc3 = DETEX_TEXTURE_FORMAT_BC2,
	Bc5 = DETEX_TEXTURE_FORMAT_BC3,
	Etc1 = DETEX_TEXTURE_FORMAT_ETC1,
	Etc2 = DETEX_TEXTURE_FORMAT_ETC2,
	Etc2Eac = DETEX_TEXTURE_FORMAT_ETC2_EAC,
	Etc2Punchthrough = DETEX_TEXTURE_FORMAT_ETC2_PUNCHTHROUGH,
	// detex uncompressed formats
	Rgba8 = DETEX_PIXEL_FORMAT_RGBA8,
	Bgra8 = DETEX_PIXEL_FORMAT_BGRA8
};

/// <summary>
/// Decompresses the specified texture data.
/// </summary>
/// <param name="data">The data to decompress.</param>
/// <param name="offset">The offset to decompress data from.</param>
/// <param name="format">The data format.</param>
/// <param name="x">Texture pixel width.</param>
/// <param name="y">Texture pixel height.</param>
/// <param name="output">Array to store output into.</param>
/// <param name="output_format">Output pixel format.</param>
/// <returns>The length of consumed compressed data.</returns>
EXTERN_DLL_EXPORT uint32_t Decompress(
	uint8_t* data, int offset,
	NativePixelFormat format, int x, int y,
	uint8_t* output, NativePixelFormat output_format);

/// <summary>
/// Converts the format of a pixel buffer in-place.
/// </summary>
/// <param name="data">Pixels to convert.</param>
/// <param name="n_pixels">Number of pixels to convert.</param>
/// <param name="from">Source pixel format.</param>
/// <param name="to">Desired pixel format.</param>
/// <returns>Whether the conversion is successful.</returns>
EXTERN_DLL_EXPORT bool ConvertInPlace(
	uint8_t* data, int n_pixels, NativePixelFormat from, NativePixelFormat to
);