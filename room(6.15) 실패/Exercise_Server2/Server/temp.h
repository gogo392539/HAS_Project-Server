#pragma once

#include "clientInfo.h"

class temp {
private:
	thread tempThread;
	ClientState* clients;
	int* clientNum;

public:
	temp(ClientState client[], int* Num);
	void tempStart();
	int tempThreadMain();
	void tempThreadJoin();
};