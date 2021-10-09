#include "Compression.h"

#include "third_party/pvr/PVRTDecompress.h"

using namespace System;
using namespace Kaamo::Texture;

UInt32 __clrcall Compression::Decompress(
	array<Byte>^ data, Int32 offset,
	NativePixelFormat format, Int32 x, Int32 y,
	array<Byte>^ output)
{
	if (!(data->Length && x && y))
	{
		// Input is empty, or invalid dimensions
		return 0;
	}

	uint32_t retLen;
	pin_ptr<Byte> pIn;
	pin_ptr<Byte> pOut;

	pOut = &output[0];
	pIn = &data[offset];

	switch (format)
	{
	case NativePixelFormat::Pvrtc2:
		// PVR 2bpp
		retLen = pvr::PVRTDecompressPVRTC((Byte*)pIn, true, x, y, (Byte*)pOut);
		break;
	case NativePixelFormat::Pvrtc4:
		// PVR 4bpp
		retLen = pvr::PVRTDecompressPVRTC((Byte*)pIn, false, x, y, (Byte*)pOut);
		break;
	case NativePixelFormat::Etc1:
		// ETC1 (detex works too)
		retLen = pvr::PVRTDecompressETC((Byte*)pIn, x, y, (Byte*)pOut, NULL);
		break;
	default:
		// ETC2, S3TC
		detexTexture tex;
		tex.format = (uint32_t)format;
		tex.data = (uint8_t*)pIn;
		tex.width = x;
		tex.height = y;
		tex.width_in_blocks = x / 4;
		tex.height_in_blocks = y / 4;
		retLen = detexDecompressTextureLinear(&tex, (uint8_t*)pOut, DETEX_PIXEL_FORMAT_RGBA8);
		break;
	}

	return retLen;
}
