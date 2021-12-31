#include "Compression.h"

#include "third_party/pvr/PVRTDecompress.h"

EXTERN_DLL_EXPORT uint32_t Decompress(
	uint8_t* data, int offset,
	NativePixelFormat format, int x, int y,
	uint8_t* output, NativePixelFormat output_format)
{
	if (!(x && y))
	{
		// Invalid dimensions
		return 0;
	}

	uint32_t retLen;

	auto pOut = output;
	auto pIn = data + offset;
	auto myOutputFormat = static_cast<uint32_t>(output_format);

	switch (format)
	{
	case NativePixelFormat::Pvrtc2:
		// PVR 2bpp
		retLen = pvr::PVRTDecompressPVRTC(pIn, true, x, y, pOut);
		if (!detexConvertPixelsInPlace(
			pOut, x * y, DETEX_PIXEL_FORMAT_RGBA8, myOutputFormat))
		{
			retLen = 0;
		}
		break;
	case NativePixelFormat::Pvrtc4:
		// PVR 4bpp
		retLen = pvr::PVRTDecompressPVRTC(pIn, false, x, y, pOut);
		if (!detexConvertPixelsInPlace(
			pOut, x * y, DETEX_PIXEL_FORMAT_RGBA8, myOutputFormat))
		{
			retLen = 0;
		}
		break;
	case NativePixelFormat::Etc1:
		// ETC1 (detex works too)
		retLen = pvr::PVRTDecompressETC(pIn, x, y, pOut, NULL);
		break;
	default:
		// ETC2, S3TC
		detexTexture tex;
		tex.format = (uint32_t)format;
		tex.data = (uint8_t*)pIn;
		tex.width = x;
		tex.height = y;
		tex.width_in_blocks = (x + 3) / 4;
		tex.height_in_blocks = (y + 3) / 4;

		retLen = detexDecompressTextureLinear(
			&tex, (uint8_t*)pOut, myOutputFormat);
		break;
	}

	return retLen;
}

EXTERN_DLL_EXPORT bool ConvertInPlace(
	uint8_t* data, int n_pixels, NativePixelFormat from, NativePixelFormat to
) {
	return detexConvertPixelsInPlace(data, n_pixels,
		static_cast<uint32_t>(from), static_cast<uint32_t>(to));
}