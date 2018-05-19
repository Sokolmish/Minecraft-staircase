#include <cmath>

#define DARK 0
#define NORM 1
#define LIGH 2

#define FLAT 0
#define LITE 1
#define FULL 2

int pow2(int num) { return num * num; }

double Similarity(int* col1, int* col2, int set) {
	switch (set) {
	case LIGH:
		return sqrt(pow2(col2[0] - col1[0]) +
			pow2(col2[1] - col1[1]) +
			pow2(col2[2] - col1[2]));
	case NORM:
		return sqrt(pow2(col2[0] - col1[0] * 220 / 255) +
			pow2(col2[1] - col1[1] * 220 / 255) +
			pow2(col2[2] - col1[2] * 220 / 255));
	case DARK:
		return sqrt(pow2(col2[0] - col1[0] * 180 / 255) +
			pow2(col2[1] - col1[1] * 180 / 255) +
			pow2(col2[2] - col1[2] * 180 / 255));
	default:
		return 0;
	}
}

//double* RGBtoXYZ(int x1, int y1, int z1) {
//	return new double[3]{
//		0.4124564 * x1 + 0.3575761 * y1 + 0.1804375 * z1,
//		0.2126729 * x1 + 0.7151522 * y1 + 0.0721750 * z1,
//		0.0193339 * x1 + 0.1191920 * y1 + 0.9503041 * z1
//	};
//}

__declspec(dllexport)
int* Convert(int *image/*r-g-b*/, int length, int type, bool chromatic, int *notes/*=id-rgb=*/, int colCount, void(*Progress)(), void(*SaveUses)(int *cou)) {
	//>>r1,g1,b1,r2,g2,b2, ri,gi,bi
	//<<id1,set1,id2,set2, idi,seti

	int* result = new int[length / 3 * 2];
	int* uses = new int[colCount / 4];
	for (int i = 0; i < colCount / 4; i++)
		uses[i] = 0;
	for (int i = 0; i < length / 3; i++) {
		int betterSimilarity = 99999;
		int betterId = 0;
		int betterSet = NORM;
		int betterIdNum = 0;
		for (int col = 0; col < colCount / 4; col++) {
			if (Similarity(&notes[col * 4 + 1], &image[i * 3], NORM) < betterSimilarity) {
				betterSimilarity = Similarity(&notes[col * 4 + 1], &image[i * 3], NORM);
				betterId = notes[col * 4];
				betterSet = NORM;
				betterIdNum = col;
			}
			if (type != FLAT) {
				if (Similarity(&notes[col * 4 + 1], &image[i * 3], LIGH) < betterSimilarity) {
				betterSimilarity = Similarity(&notes[col * 4 + 1], &image[i * 3], LIGH);
				betterId = notes[col * 4];
				betterSet = LIGH;
				betterIdNum = col;
				}
				if (type != LITE)
					if (Similarity(&notes[col * 4 + 1], &image[i * 3], DARK) < betterSimilarity) {
						betterSimilarity = Similarity(&notes[col * 4 + 1], &image[i * 3], DARK);
						betterId = notes[col * 4];
						betterSet = DARK;
						betterIdNum = col;
					}
			}
		}
		result[i * 2] = betterId;
		result[i * 2 + 1] = betterSet;
		++uses[betterIdNum];
		if (i % 128 == 0)
			Progress();
	}
	SaveUses(uses);
	return result;
}