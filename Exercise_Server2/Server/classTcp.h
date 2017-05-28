#pragma once

#include "clientInfo.h"

class TCPServer {
private:
	WSADATA wsaData;
	int *connectNum;							//������ client�� ���� ��Ÿ���� ���� ����
	SOCKET serverListenSock;
	ClientState* clientState;
	SOCKADDR_IN serverTCPAddr;
	SOCKADDR_IN clientTCPAddr;	//
	int clientTCPAddrsz;
	
	thread TCPThread;
	thread Client_Thread[CLIENT_MAX];
	thread Accept_Thread[CLIENT_MAX];
	int clientReadyNum;

	//accessPacket AccessState[CLIENT_MAX];
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

	int AcceptThreadMain(int ID);
	void AcceptThreadJoin();
	void GameStart();

	void puzzleEventFunc(eventPacket packet, int ID);
	void playerkillEventFunc(eventPacket packet, int ID);
	void animationEventFunc(eventPacket packet);
	void trapEventFunc(eventPacket packet, int ID);
};