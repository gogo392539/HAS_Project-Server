#pragma once

#include "clientInfo.h"

class TCPServer {
private:
	WSADATA wsaData;
	int *connectNum;							//접속한 client의 수를 나타내기 위한 변수
	SOCKET serverListenSock;
	ClientState* clientState;
	SOCKADDR_IN serverTCPAddr;
	SOCKADDR_IN clientTCPAddr;
	int clientTCPAddrsz;
	
	thread callingThread;
	thread event_Thread[CLIENT_MAX];
	eventPacket eventpacket;

	thread room_Thread[CLIENT_MAX];
	int clientListSet[CLIENT_MAX];
	int tempid;
	bool acceptStartSet;

public:
	TCPServer();
	TCPServer(ClientState clients[], int *clientNum);
	void serverStart();	
	void serverClosed();

	void roomThreadStart();
	int roomThreadMain(int id);
	void roomThreadJoin();

	void sendRandomIdx();
	void randomPos(int arrayPos[]);

	void sendTaggerUserID();
	int selectTaggerUser();

	void threadCallingStart();
	int eventThreadCallingThreadMain();
	int eventThreadMain(int myID);
	void eventThreadCallingThreadJoin();

	void puzzleEventFunc(eventPacket packet, int ID);
	void playerkillEventFunc(eventPacket packet, int ID);
	void animationEventFunc(eventPacket packet);
	void trapEventFunc(eventPacket packet, int ID);
};