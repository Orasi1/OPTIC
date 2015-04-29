#define WINVER 0x0501
#define _WIN32_WINNT 0x0501

#include "stdafx.h"
#include < stdio.h >
#include < stdlib.h >
#include < vcclr.h >
#include < string >
#include < iostream >

using namespace System;
using namespace std;
using namespace OpticUtil;

extern void CallIncrementCounter(string, int);
extern void CallResetCounter(string, int);
extern void CallDeleteCounterCategory(string);
extern long CallGetCounter(string);

extern "C"
{
	__declspec(dllexport) void IncrementCounter(const char * counterPath, int value)
	{
		CallIncrementCounter(counterPath, value);
	}

	__declspec(dllexport) void ResetCounter(const char * counterPath, int value)
	{
		CallResetCounter(counterPath, value);
	}

	__declspec(dllexport) void DeleteCounterCategory(const char * counterPath)
	{
		CallDeleteCounterCategory(counterPath);
	}

	__declspec(dllexport) long GetCounter(const char * counterPath)
	{
		return CallGetCounter(counterPath);
	}
}

void CallIncrementCounter(string counterPath, int value)
{
	String^ clrCounterPath = gcnew String(counterPath.c_str());

	Counter::IncrementCounter(clrCounterPath, value);
}

void CallResetCounter(string counterPath, int value)
{
	String^ clrCounterPath = gcnew String(counterPath.c_str());

	Counter::ResetCounter(clrCounterPath, value);
}

long CallGetCounter(string counterPath)
{
	String^ clrCounterPath = gcnew String(counterPath.c_str());
	long value = Counter::GetCounter(clrCounterPath);
	return value;
}

void CallDeleteCounterCategory(string counterPath){
	String^ clrCounterPath = gcnew String(counterPath.c_str());

	Counter::DeleteCounterCategory(clrCounterPath);
}
