#pragma once

#include "clientInfo.h"

class UDPServer {
private:
	SOCKET serverUDPSock;
	SOCKADDR_IN serverUDPAddr;
	ClientState* clientState;
	thread recvPosThread;
	thread sendPosThread;
	int *connectNum;

	bool sendThreadSet;

public:
	UDPServer();
	UDPServer(ClientState clients[], int *clientNum);
	void serverStart();
	void receiveClientAddr();
	void recvThreadStart();
	void threadJoin();
	void serverClosed();

	int recvPosThreadMain(ClientState clientState[], SOCKET serverUDPSock);
	int sendPosThreadMain(ClientState clientState[], SOCKET serverUDPSock);
};