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

	int tempid;
	bool AcceptStartSet;

	int taggerUserID;
	
	thread TCPThread;
	thread Client_Thread[CLIENT_MAX];

	thread Room_Thread[CLIENT_MAX];
	int ClientListSet[CLIENT_MAX];

	eventPacket eventpacket;

	thread tempThread;

public:
	TCPServer();
	TCPServer(ClientState clients[], int *clientNum);
	void serverStart();
	//void clientAccept();
	
	void TCPServerClosed();
	void sendRandomIdx();
	void randomPos(int arrayPos[]);
	void sendTaggerUserID();
	int SelectTaggerUser();

	void TCPThreadJoin();
	void TCPThreadStart();
	int TCPThreadFunc();

	void RoomThreadStart();
	int RoomThreadMain(int id);
	void RoomThreadJoin();

	int ClientMainThread(int myID);

	void puzzleEventFunc(eventPacket packet, int ID);
	void playerkillEventFunc(eventPacket packet, int ID);
	void animationEventFunc(eventPacket packet);
	void trapEventFunc(eventPacket packet, int ID);
};