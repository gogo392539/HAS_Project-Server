#pragma once

#include "clientInfo.h"

class UDPServer {
private:
	SOCKET serverUDPSock;
	SOCKADDR_IN serverUDPAddr;
	ClientState* clientState;
	thread recvUDPPosThread;
	thread sendUDPPosThread;
	int *connectNum;

public:
	UDPServer();
	UDPServer(ClientState clients[], int *clientNum);
	void serverStart();
	void receiveClientAddr();
	void recvUDPThreadFunc();
	void sendUDPThreadFunc();
	void UDPThreadJoin();
	void UDPServerClosed();

	int recvPosThread(ClientState clientState[], SOCKET serverUDPSock);
	int sendPosThread(ClientState clientState[], SOCKET serverUDPSock);
	/*int recvPosThread();
	int sendPosThread();*/
};