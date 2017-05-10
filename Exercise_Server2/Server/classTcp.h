#pragma once

#include "clientInfo.h"

class TCPServer {
private:
	WSADATA wsaData;
	int *connectNum;							//������ client�� ���� ��Ÿ���� ���� ����
	SOCKET serverListenSock;
	ClientState* clientState;
	SOCKADDR_IN serverTCPAddr;
	SOCKADDR_IN clientTCPAddr;
	int clientTCPAddrsz;
	
	thread TCPThread;
	thread Client_Thread[CLIENT_MAX];

	//Puzzle puzzle[PUZZLE_MAX];
	eventPacket eventpacket;

	thread tempThread;

public:
	TCPServer();
	TCPServer(ClientState clients[], int *clientNum);
	void serverStart();
	void clientAccept();
	
	void TCPServerClosed();
	void sendRandomIdx();
	void randomPos(int arrayPos[]);

	void TCPThreadJoin();
	void TCPThreadStart();
	int TCPThreadFunc();

	int ClientMainThread(int myID);

	void puzzleEventFunc(eventPacket packet);
	void playerkillEventFunc(eventPacket packet);
	void trapEventFunc();
	void animationEventFunc(eventPacket packet);
};