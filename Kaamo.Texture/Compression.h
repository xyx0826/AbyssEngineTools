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
	Etc2Punchthrough = DETEX_TEXTURE_FORMAT_ETC2_PUNCHTHROUGH
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
/// <returns>The length of consumed compressed data.</returns>
EXTERN_DLL_EXPORT uint32_t Decompress(
	uint8_t* data, int offset,
	NativePixelFormat format, int x, int y,
	uint8_t* output);