#pragma once
#include <stdint.h>

int uncompressFile(const uint8_t* compressedData, uint8_t*& img, uint8_t*& alphaimg, int& active_width, int& active_height, uint8_t *output);
