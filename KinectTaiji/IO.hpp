#pragma once
#include"stadfx.h"
#include"feature.hpp"

using namespace std;

// 数据格式规定，一行为一帧
const int FEATURE_CHAR_LEN = sizeof(taiji::Feature) / sizeof(char);

class IO
{
public:
	IO(const string& fileName) {
		this->fileName = fileName;
	}
	vector<taiji::Feature> readFeatures() {
		vector<taiji::Feature> vf;
		taiji::Feature curr;
		ifstream fin(this->fileName.c_str(),ios::binary);
		while (fin.read((char *)&curr,FEATURE_CHAR_LEN)) {
			vf.push_back(curr);
		}
		fin.close();
		return vf;
	}
	bool appendFeature(const taiji::Feature& feature) {
		ofstream fout(this->fileName.c_str(), ios::app | ios::binary);
		fout.write((char*)&feature, FEATURE_CHAR_LEN);
		fout.close();
		return true;
	}
	bool appendFeature(const vector<taiji::Feature>& vFeatures) {
		for (int i = 0; i < vFeatures.size(); i++) {
			appendFeature(vFeatures[i]);
		}
		return true;
	}
	bool clear() {
		ofstream fout(this->fileName.c_str());
		fout.close();
		return true;
	}
	~IO() {
	}
private:
	string fileName;
};

