#pragma once

#include"angle.hpp"
#include"feature.hpp"
#include"IO.hpp"

using namespace std;
using namespace cv;

int testFeature() {
	Mat skeletonImage;
	skeletonImage.create(240, 320, CV_8UC3);
	IO io(".//seq.dat");
	io.clear();

	//1����ʼ��NUI��ע��������USES_SKELETON  
	HRESULT hr = NuiInitialize(NUI_INITIALIZE_FLAG_USES_SKELETON);
	if (FAILED(hr))
	{
		cout << "NuiInitialize failed" << endl;
		return hr;
	}

	//2����������ź��¼����   
	HANDLE skeletonEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

	//3���򿪹��������¼�  
	hr = NuiSkeletonTrackingEnable(skeletonEvent, 0);
	if (FAILED(hr))
	{
		cout << "Could not open color image stream video" << endl;
		NuiShutdown();
		return hr;
	}
	namedWindow("skeletonImage", CV_WINDOW_AUTOSIZE);

	//4����ʼ��ȡ������������   
	while (1)
	{
		NUI_SKELETON_FRAME skeletonFrame = { 0 };  //����֡�Ķ���   

												   //4.1�����޵ȴ��µ����ݣ��ȵ��󷵻�  
		if (WaitForSingleObject(skeletonEvent, INFINITE) == 0)
		{
			//4.2���ӸղŴ���������������еõ���֡���ݣ���ȡ�������ݵ�ַ����skeletonFrame  
			hr = NuiSkeletonGetNextFrame(0, &skeletonFrame);
			if (!SUCCEEDED(hr))
			{
				continue;
			}
			taiji::Feature feature(skeletonFrame);
			system("cls");
			feature.print();
			if (!feature.ok()) {
				continue;
			}
			io.appendFeature(feature);
			skeletonImage = feature.getSkeletonImage();
			imshow("skeletonImage", skeletonImage); //��ʾͼ��   
		}
		else
		{
			cout << "Buffer length of received texture is bogus\r\n" << endl;
		}

		if (cvWaitKey(20) == 27)
			break;
	}
	//5���ر�NUI����   
	NuiShutdown();
	return 0;
}

int testIORead() {
	Mat skeletonImage;
	skeletonImage.create(240, 320, CV_8UC3);
	IO io(".//seq.dat");
	namedWindow("skeletonImage", CV_WINDOW_AUTOSIZE);
	
	cout << "���ڶ�ȡ�ļ�\n";
	vector<taiji::Feature> vFeature = io.readFeatures();
	cout << "�ļ���ȡ���\n";
	int size = vFeature.size();
	cout << "֡����" << size<<endl;
	while (1) {
		for (int i = 0; i < size; i++) {
			taiji::Feature feature = vFeature[i];
			system("cls");
			feature.print();
			if (!feature.ok()) {
				continue;
			}
			skeletonImage = feature.getSkeletonImage();
			imshow("skeletonImage", skeletonImage); //��ʾͼ�� 
			if (cvWaitKey(20) == 27)
				break;

			Sleep(1000/30);
		}
	}
	
	return 0;

}