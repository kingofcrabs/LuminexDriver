// CountGrayLines.cpp : �������̨Ӧ�ó������ڵ㡣
//

#include "stdafx.h"

using namespace cv;
using namespace std;
int GetFirstGrayLinePosition(Mat& src);

int _tmain(int argc, _TCHAR* argv[])
{
	Mat img = imread("f:\\LuminexControl\\batches.jpg");
	//threshold(img, gray, 200, 255, 0);
	int position = GetFirstGrayLinePosition(img);
	imwrite("f:\\LuminexControl\\processed.jpg",img);
	ofstream ofs("f:\\LuminexControl\\result.txt");
	ofs << position << endl;
	return 0;
}



int GetFirstGrayLinePosition(Mat& src)
{
	int height = src.rows;
	int width = src.cols;
	int channels = src.channels();
	int nc = width * channels;
	vector<int> counts;
	int cntThisLine = 0;
	for (int y = 0; y < height; y++)
	{
		uchar *data = src.ptr(y);
		cntThisLine = 0;
		for (int x = 0; x < width; x++)
		{
			int xStart = x*channels;
			bool isWantedGray = data[xStart] == 128 && data[xStart + 1] == 128 && data[xStart + 2] == 128;
			if (isWantedGray)
				cntThisLine++;
		}
		counts.push_back(cntThisLine);
	}
	int threshold = 600;
	int linePos = 1;
	for (int i = 0; i < counts.size(); i++)
	{
		if (counts[i] > threshold)
			break;
		linePos++;
	}
	return linePos;
}

