//// etcpack v2.74
//// 
//// NO WARRANTY 
//// 
//// BECAUSE THE PROGRAM IS LICENSED FREE OF CHARGE THE PROGRAM IS PROVIDED
//// "AS IS". ERICSSON MAKES NO REPRESENTATIONS OF ANY KIND, EXTENDS NO
//// WARRANTIES OR CONDITIONS OF ANY KIND; EITHER EXPRESS, IMPLIED OR
//// STATUTORY; INCLUDING, BUT NOT LIMITED TO, EXPRESS, IMPLIED OR
//// STATUTORY WARRANTIES OR CONDITIONS OF TITLE, MERCHANTABILITY,
//// SATISFACTORY QUALITY, SUITABILITY AND FITNESS FOR A PARTICULAR
//// PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE
//// PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME
//// THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION. ERICSSON
//// MAKES NO WARRANTY THAT THE MANUFACTURE, SALE, OFFERING FOR SALE,
//// DISTRIBUTION, LEASE, USE OR IMPORTATION UNDER THE LICENSE WILL BE FREE
//// FROM INFRINGEMENT OF PATENTS, COPYRIGHTS OR OTHER INTELLECTUAL
//// PROPERTY RIGHTS OF OTHERS, AND THE VALIDITY OF THE LICENSE IS SUBJECT
//// TO YOUR SOLE RESPONSIBILITY TO MAKE SUCH DETERMINATION AND ACQUIRE
//// SUCH LICENSES AS MAY BE NECESSARY WITH RESPECT TO PATENTS, COPYRIGHT
//// AND OTHER INTELLECTUAL PROPERTY OF THIRD PARTIES.
//// 
//// FOR THE AVOIDANCE OF DOUBT THE PROGRAM (I) IS NOT LICENSED FOR; (II)
//// IS NOT DESIGNED FOR OR INTENDED FOR; AND (III) MAY NOT BE USED FOR;
//// ANY MISSION CRITICAL APPLICATIONS SUCH AS, BUT NOT LIMITED TO
//// OPERATION OF NUCLEAR OR HEALTHCARE COMPUTER SYSTEMS AND/OR NETWORKS,
//// AIRCRAFT OR TRAIN CONTROL AND/OR COMMUNICATION SYSTEMS OR ANY OTHER
//// COMPUTER SYSTEMS AND/OR NETWORKS OR CONTROL AND/OR COMMUNICATION
//// SYSTEMS ALL IN WHICH CASE THE FAILURE OF THE PROGRAM COULD LEAD TO
//// DEATH, PERSONAL INJURY, OR SEVERE PHYSICAL, MATERIAL OR ENVIRONMENTAL
//// DAMAGE. YOUR RIGHTS UNDER THIS LICENSE WILL TERMINATE AUTOMATICALLY
//// AND IMMEDIATELY WITHOUT NOTICE IF YOU FAIL TO COMPLY WITH THIS
//// PARAGRAPH.
//// 
//// IN NO EVENT WILL ERICSSON, BE LIABLE FOR ANY DAMAGES WHATSOEVER,
//// INCLUDING BUT NOT LIMITED TO PERSONAL INJURY, ANY GENERAL, SPECIAL,
//// INDIRECT, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF OR IN
//// CONNECTION WITH THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT
//// NOT LIMITED TO LOSS OF PROFITS, BUSINESS INTERUPTIONS, OR ANY OTHER
//// COMMERCIAL DAMAGES OR LOSSES, LOSS OF DATA OR DATA BEING RENDERED
//// INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF
//// THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS) REGARDLESS OF THE
//// THEORY OF LIABILITY (CONTRACT, TORT OR OTHERWISE), EVEN IF SUCH HOLDER
//// OR OTHER PARTY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
//// 
//// (C) Ericsson AB 2005-2013. All Rights Reserved.
//// 

#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <math.h> 
#include <time.h>
#include <sys/timeb.h>

using namespace System;

// Functions needed for decrompession ---- in etcdec.cxx
void read_big_endian_4byte_word(unsigned int* blockadr, const uint8_t* buf);
void unstuff57bits(unsigned int planar_word1, unsigned int planar_word2, unsigned int& planar57_word1, unsigned int& planar57_word2);
void unstuff59bits(unsigned int thumbT_word1, unsigned int thumbT_word2, unsigned int& thumbT59_word1, unsigned int& thumbT59_word2);
void unstuff58bits(unsigned int thumbH_word1, unsigned int thumbH_word2, unsigned int& thumbH58_word1, unsigned int& thumbH58_word2);
void decompressColor(int R_B, int G_B, int B_B, uint8_t(colors_RGB444)[2][3], uint8_t(colors)[2][3]);
void calculatePaintColors59T(uint8_t d, uint8_t p, uint8_t(colors)[2][3], uint8_t(possible_colors)[4][3]);
void calculatePaintColors58H(uint8_t d, uint8_t p, uint8_t(colors)[2][3], uint8_t(possible_colors)[4][3]);
void decompressBlockTHUMB59T(unsigned int block_part1, unsigned int block_part2, uint8_t* img, int width, int height, int startx, int starty);
void decompressBlockTHUMB58H(unsigned int block_part1, unsigned int block_part2, uint8_t* img, int width, int height, int startx, int starty);
void decompressBlockPlanar57(unsigned int compressed57_1, unsigned int compressed57_2, uint8_t* img, int width, int height, int startx, int starty);
void decompressBlockDiffFlip(unsigned int block_part1, unsigned int block_part2, uint8_t* img, int width, int height, int startx, int starty);
void decompressBlockETC2(unsigned int block_part1, unsigned int block_part2, uint8_t* img, int width, int height, int startx, int starty);
void decompressBlockDifferentialWithAlpha(unsigned int block_part1, unsigned int block_part2, uint8_t* img, uint8_t* alpha, int width, int height, int startx, int starty);
void decompressBlockETC21BitAlpha(unsigned int block_part1, unsigned int block_part2, uint8_t* img, uint8_t* alphaimg, int width, int height, int startx, int starty);
void decompressBlockTHUMB58HAlpha(unsigned int block_part1, unsigned int block_part2, uint8_t* img, uint8_t* alpha, int width, int height, int startx, int starty);
void decompressBlockTHUMB59TAlpha(unsigned int block_part1, unsigned int block_part2, uint8_t* img, uint8_t* alpha, int width, int height, int startx, int starty);
uint8_t getbit(uint8_t input, int frompos, int topos);
int clamp(int val);
void decompressBlockAlpha(uint8_t* data, uint8_t* img, int width, int height, int ix, int iy);
uint16_t get16bits11bits(int base, int table, int mul, int index);
void decompressBlockAlpha16bit(uint8_t* data, uint8_t* img, int width, int height, int ix, int iy);
int16_t get16bits11signed(int base, int table, int mul, int index);
void setupAlphaTable();

// Remove warnings for unsafe functions such as strcpy
#pragma warning(disable : 4996)
// Remove warnings for conversions between different time variables
#pragma warning(disable : 4244)
// Remove warnings for negative or too big shifts
//#pragma warning(disable : 4293)

#define CLAMP(ll,x,ul) (((x)<(ll)) ? (ll) : (((x)>(ul)) ? (ul) : (x)))
// The below code works as CLAMP(0, x, 255) if x < 255
#define CLAMP_LEFT_ZERO(x) ((~(((int)(x))>>31))&(x))
// The below code works as CLAMP(0, x, 255) if x is in [0,511]
#define CLAMP_RIGHT_255(x) (((( ((((int)(x))<<23)>>31)  ))|(x))&0x000000ff)   

#define SQUARE(x) ((x)*(x))
#define JAS_ROUND(x) (((x) < 0.0 ) ? ((int)((x)-0.5)) : ((int)((x)+0.5)))
#define JAS_MIN(a,b) ((a) < (b) ? (a) : (b))
#define JAS_MAX(a,b) ((a) > (b) ? (a) : (b))

// The error metric Wr Wg Wb should be definied so that Wr^2 + Wg^2 + Wb^2 = 1.
// Hence it is easier to first define the squared values and derive the weights
// as their square-roots.

#define PERCEPTUAL_WEIGHT_R_SQUARED 0.299
#define PERCEPTUAL_WEIGHT_G_SQUARED 0.587
#define PERCEPTUAL_WEIGHT_B_SQUARED 0.114

#define PERCEPTUAL_WEIGHT_R_SQUARED_TIMES1000 299
#define PERCEPTUAL_WEIGHT_G_SQUARED_TIMES1000 587
#define PERCEPTUAL_WEIGHT_B_SQUARED_TIMES1000 114

#define RED(img,width,x,y)   img[3*(y*width+x)+0]
#define GREEN(img,width,x,y) img[3*(y*width+x)+1]
#define BLUE(img,width,x,y)  img[3*(y*width+x)+2]

#define SHIFT(size,startpos) ((startpos)-(size)+1)
#define MASK(size, startpos) (((2<<(size-1))-1) << SHIFT(size,startpos))
#define PUTBITS( dest, data, size, startpos) dest = ((dest & ~MASK(size, startpos)) | ((data << SHIFT(size, startpos)) & MASK(size,startpos)))
#define SHIFTHIGH(size, startpos) (((startpos)-32)-(size)+1)
#define MASKHIGH(size, startpos) (((1<<(size))-1) << SHIFTHIGH(size,startpos))
#define PUTBITSHIGH(dest, data, size, startpos) dest = ((dest & ~MASKHIGH(size, startpos)) | ((data << SHIFTHIGH(size, startpos)) & MASKHIGH(size,startpos)))
#define GETBITS(source, size, startpos)  (( (source) >> ((startpos)-(size)+1) ) & ((1<<(size)) -1))
#define GETBITSHIGH(source, size, startpos)  (( (source) >> (((startpos)-32)-(size)+1) ) & ((1<<(size)) -1))

// Thumb macros and definitions
#define	R_BITS59T 4
#define G_BITS59T 4
#define	B_BITS59T 4
#define	R_BITS58H 4
#define G_BITS58H 4
#define	B_BITS58H 4
#define	MAXIMUM_ERROR (255*255*16*1000)
#define R 0
#define G 1
#define B 2
#define BLOCKHEIGHT 4
#define BLOCKWIDTH 4
#define BINPOW(power) (1<<(power))
//#define RADIUS 2
#define	TABLE_BITS_59T 3
#define	TABLE_BITS_58H 3

// Global tables
static uint8_t table59T[8] = { 3,6,11,16,23,32,41,64 };  // 3-bit table for the 59 bit T-mode
static uint8_t table58H[8] = { 3,6,11,16,23,32,41,64 };  // 3-bit table for the 58 bit H-mode
uint8_t weight[3] = { 1,1,1 };			// Color weight

// Enums
static enum {
	PATTERN_H = 0,
	PATTERN_T = 1
};

static enum { MODE_ETC1, MODE_THUMB_T, MODE_THUMB_H, MODE_PLANAR };
// The ETC2 package of codecs includes the following codecs:
//
// codec                                             enum
// --------------------------------------------------------
// GL_COMPRESSED_R11_EAC                            0x9270
// GL_COMPRESSED_SIGNED_R11_EAC                     0x9271
// GL_COMPRESSED_RG11_EAC                           0x9272
// GL_COMPRESSED_SIGNED_RG11_EAC                    0x9273
// GL_COMPRESSED_RGB8_ETC2                          0x9274
// GL_COMPRESSED_SRGB8_ETC2                         0x9275
// GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2      0x9276
// GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2     0x9277
// GL_COMPRESSED_RGBA8_ETC2_EAC                     0x9278
// GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC              0x9279
// 
// The older codec ETC1 is not included in the package 
// GL_ETC1_RGB8_OES                                 0x8d64
// but since ETC2 is backwards compatible an ETC1 texture can
// be decoded using the RGB8_ETC2 enum (0x9274)
// 
// In a PKM-file, the codecs are stored using the following identifiers
// 
// identifier                         value               codec
// --------------------------------------------------------------------
// ETC1_RGB_NO_MIPMAPS                  0                 GL_ETC1_RGB8_OES
// ETC2PACKAGE_RGB_NO_MIPMAPS           1                 GL_COMPRESSED_RGB8_ETC2
// ETC2PACKAGE_RGBA_NO_MIPMAPS_OLD      2, not used       -
// ETC2PACKAGE_RGBA_NO_MIPMAPS          3                 GL_COMPRESSED_RGBA8_ETC2_EAC
// ETC2PACKAGE_RGBA1_NO_MIPMAPS         4                 GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2
// ETC2PACKAGE_R_NO_MIPMAPS             5                 GL_COMPRESSED_R11_EAC
// ETC2PACKAGE_RG_NO_MIPMAPS            6                 GL_COMPRESSED_RG11_EAC
// ETC2PACKAGE_R_SIGNED_NO_MIPMAPS      7                 GL_COMPRESSED_SIGNED_R11_EAC
// ETC2PACKAGE_RG_SIGNED_NO_MIPMAPS     8                 GL_COMPRESSED_SIGNED_RG11_EAC
//
// In the code, the identifiers are not always used strictly. For instance, the
// identifier ETC2PACKAGE_R_NO_MIPMAPS is sometimes used for both the unsigned
// (GL_COMPRESSED_R11_EAC) and signed (GL_COMPRESSED_SIGNED_R11_EAC) version of 
// the codec.
// 
static enum { ETC1_RGB_NO_MIPMAPS, ETC2PACKAGE_RGB_NO_MIPMAPS, ETC2PACKAGE_RGBA_NO_MIPMAPS, ETC2PACKAGE_RGBA1_NO_MIPMAPS };
static enum { CODEC_ETC, CODEC_ETC2 };

int verbose = true;
extern int formatSigned;
int ktxFile = 0;
bool first_time_message = true;

static int scramble[4] = { 3, 2, 0, 1 };
static int unscramble[4] = { 2, 3, 1, 0 };

static int compressParams[16][4];
const int compressParamsFast[32] = { -8,  -2,  2,   8,
									 -17,  -5,  5,  17,
									 -29,  -9,  9,  29,
									 -42, -13, 13,  42,
									 -60, -18, 18,  60,
									 -80, -24, 24,  80,
									-106, -33, 33, 106,
									-183, -47, 47, 183 };

bool readCompressParams(void)
{
	compressParams[0][0] = -8; compressParams[0][1] = -2; compressParams[0][2] = 2; compressParams[0][3] = 8;
	compressParams[1][0] = -8; compressParams[1][1] = -2; compressParams[1][2] = 2; compressParams[1][3] = 8;
	compressParams[2][0] = -17; compressParams[2][1] = -5; compressParams[2][2] = 5; compressParams[2][3] = 17;
	compressParams[3][0] = -17; compressParams[3][1] = -5; compressParams[3][2] = 5; compressParams[3][3] = 17;
	compressParams[4][0] = -29; compressParams[4][1] = -9; compressParams[4][2] = 9; compressParams[4][3] = 29;
	compressParams[5][0] = -29; compressParams[5][1] = -9; compressParams[5][2] = 9; compressParams[5][3] = 29;
	compressParams[6][0] = -42; compressParams[6][1] = -13; compressParams[6][2] = 13; compressParams[6][3] = 42;
	compressParams[7][0] = -42; compressParams[7][1] = -13; compressParams[7][2] = 13; compressParams[7][3] = 42;
	compressParams[8][0] = -60; compressParams[8][1] = -18; compressParams[8][2] = 18; compressParams[8][3] = 60;
	compressParams[9][0] = -60; compressParams[9][1] = -18; compressParams[9][2] = 18; compressParams[9][3] = 60;
	compressParams[10][0] = -80; compressParams[10][1] = -24; compressParams[10][2] = 24; compressParams[10][3] = 80;
	compressParams[11][0] = -80; compressParams[11][1] = -24; compressParams[11][2] = 24; compressParams[11][3] = 80;
	compressParams[12][0] = -106; compressParams[12][1] = -33; compressParams[12][2] = 33; compressParams[12][3] = 106;
	compressParams[13][0] = -106; compressParams[13][1] = -33; compressParams[13][2] = 33; compressParams[13][3] = 106;
	compressParams[14][0] = -183; compressParams[14][1] = -47; compressParams[14][2] = 47; compressParams[14][3] = 183;
	compressParams[15][0] = -183; compressParams[15][1] = -47; compressParams[15][2] = 47; compressParams[15][3] = 183;

	return true;
}

extern int alphaTable[256][8];
extern int alphaBase[16][4];

// valtab holds precalculated data used for compressing using EAC2.
// Note that valtab is constructed using get16bits11bits, which means
// that it already is expanded to 16 bits.
// Note also that it its contents will depend on the value of formatSigned.
int* valtab;

void setupAlphaTableAndValtab()
{
	setupAlphaTable();

	//fix precomputation table..!
	valtab = new int[1024 * 512];
	int16_t val16;
	int count = 0;
	for (int base = 0; base < 256; base++)
	{
		for (int tab = 0; tab < 16; tab++)
		{
			for (int mul = 0; mul < 16; mul++)
			{
				for (int index = 0; index < 8; index++)
				{
					if (formatSigned)
					{
						val16 = get16bits11signed(base, tab, mul, index);
						valtab[count] = val16 + 256 * 128;
					}
					else
						valtab[count] = get16bits11bits(base, tab, mul, index);
					count++;
				}
			}
		}
	}
}

// Decompresses a file
// NO WARRANTY --- SEE STATEMENT IN TOP OF FILE (C) Ericsson AB 2005-2013. All Rights Reserved.
int uncompressFile(const uint8_t *compressedData, uint8_t*& img, uint8_t*& alphaimg, int& active_width, int& active_height, uint8_t *output)
{
	int cur = 0;
	int width, height;
	unsigned int block_part1, block_part2;
	uint8_t* newimg = nullptr, * newalphaimg = nullptr, * alphaimg2 = nullptr;
	unsigned short w, h;
	int xx, yy;

	readCompressParams();
	int codec = CODEC_ETC2;
	int format = ETC2PACKAGE_RGBA_NO_MIPMAPS;

	w = ((active_width + 3) / 4) * 4;
	h = ((active_height + 3) / 4) * 4;
	width = w;
	height = h;
	img = (uint8_t*)malloc(3 * width * height);
	if (!img)
	{
		printf("Error: could not allocate memory\n");
		exit(0);
	}
	if (format == ETC2PACKAGE_RGBA_NO_MIPMAPS || format == ETC2PACKAGE_RGBA1_NO_MIPMAPS)
	{
		//printf("alpha channel decompression\n");
		alphaimg = (uint8_t*)malloc(width * height * 2);
		setupAlphaTableAndValtab();
		if (!alphaimg)
		{
			printf("Error: could not allocate memory for alpha\n");
			exit(0);
		}
	}

	for (int y = 0; y < height / 4; y++)
	{
		for (int x = 0; x < width / 4; x++)
		{
			//decode alpha channel for RGBA
			if (format == ETC2PACKAGE_RGBA_NO_MIPMAPS)
			{
				uint8_t alphablock[8];
				memcpy(alphablock, compressedData + cur, 8);
				cur += 8;
				decompressBlockAlpha(alphablock, alphaimg, width, height, 4 * x, 4 * y);
			}
			//color channels for most normal modes
				//we have normal ETC2 color channels, decompress these
			read_big_endian_4byte_word(&block_part1, compressedData + cur);
			cur += 4;
			read_big_endian_4byte_word(&block_part2, compressedData + cur);
			cur += 4;
			if (format == ETC2PACKAGE_RGBA1_NO_MIPMAPS)
				decompressBlockETC21BitAlpha(block_part1, block_part2, img, alphaimg, width, height, 4 * x, 4 * y);
			else
				decompressBlockETC2(block_part1, block_part2, img, width, height, 4 * x, 4 * y);
		}
	}

	// Ok, and now only write out the active pixels to the .ppm file.
	// (But only if the active pixels differ from the total pixels)

	if (!(height == active_height && width == active_width))
	{
		newimg = (uint8_t*)malloc(3 * active_width * active_height);

		if (format == ETC2PACKAGE_RGBA_NO_MIPMAPS || format == ETC2PACKAGE_RGBA1_NO_MIPMAPS)
		{
			newalphaimg = (uint8_t*)malloc(active_width * active_height * 2);
		}

		if (!newimg)
		{
			printf("Error: could not allocate memory\n");
			exit(0);
		}

		// Convert from total area to active area:

		for (yy = 0; yy < active_height; yy++)
		{
			for (xx = 0; xx < active_width; xx++)
			{

				newimg[(yy * active_width) * 3 + xx * 3 + 0] = img[(yy * width) * 3 + xx * 3 + 0];
				newimg[(yy * active_width) * 3 + xx * 3 + 1] = img[(yy * width) * 3 + xx * 3 + 1];
				newimg[(yy * active_width) * 3 + xx * 3 + 2] = img[(yy * width) * 3 + xx * 3 + 2];
				if (format == ETC2PACKAGE_RGBA_NO_MIPMAPS || format == ETC2PACKAGE_RGBA1_NO_MIPMAPS)
				{
					newalphaimg[((yy * active_width) + xx)] = alphaimg[((yy * width) + xx)];
				}
			}
		}

		free(img);
		img = newimg;
		if (format == ETC2PACKAGE_RGBA_NO_MIPMAPS || format == ETC2PACKAGE_RGBA1_NO_MIPMAPS)
		{
			free(alphaimg);
			alphaimg = newalphaimg;
		}
	}

	// Interleave two images for RGBA output
	int px = 0;
	for (int y = 0; y < active_height; y++)
	{
		for (int x = 0; x < active_width; x++)
		{
			memcpy(output + px * 4, img + px * 3, 3);
			output[px * 4 + 3] = alphaimg[px];
			px++;
		}
	}

	return cur;
}
