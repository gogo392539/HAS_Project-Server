#include "classTcp.h"

TCPServer::TCPServer() {

}

TCPServer::TCPServer(ClientState clients[], int *clientNum) {
	clientTCPAddrsz = sizeof(clientTCPAddr);
	clientState = clients;
	connectNum = clientNum;

	acceptStartSet = true;
	tempid = -1;

	for (int i = 0; i < CLIENT_MAX; i++) {
		clientListSet[i] = -1;
	}
}

void TCPServer::serverStart() {
	//socket(), bind(), listen()
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
		ErrorHandling("WSAStartup() error");
	}

	serverListenSock = socket(PF_INET, SOCK_STREAM, 0);
	if (serverListenSock == INVALID_SOCKET)
		ErrorHandling("TCP socket()");

	memset(&serverTCPAddr, 0, sizeof(serverTCPAddr));
	serverTCPAddr.sin_family = AF_INET;
	serverTCPAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serverTCPAddr.sin_port = htons(DEFAULT_PORT);

	if (::bind(serverListenSock, (sockaddr*)&serverTCPAddr, sizeof(serverTCPAddr)))
		ErrorHandling("bind() Join");
	if (listen(serverListenSock, 5))
		ErrorHandling("listen() Join");
}

void TCPServer::serverClosed() {
	for (int i = 0; i < CLIENT_MAX; i++) {
		closesocket(clientState[i].clientTCPSock);
	}
	closesocket(serverListenSock);	
	WSACleanup();
}

void TCPServer::sendTaggerUserID() {
	//모든 client에게 술래의 ID 값을 전달
	int taggerUserID = selectTaggerUser();

	for (int i = 0; i < CLIENT_MAX; i++) {
		int sendNum = sendn(clientState[i].clientTCPSock, (char*)&taggerUserID, sizeof(int), 0);
	}

	cout << "tagger user ID : " << taggerUserID << endl;
}

int TCPServer::selectTaggerUser() {
	//tagger user ID 를 난수로 생성
	srand((unsigned int)time(NULL));
	int taggerID= rand() % CLIENT_MAX;

	return taggerID;
}

void TCPServer::sendRandomIdx() {
	//client의 시작 위치 index 값을 전달
	int arrayPos[CLIENT_MAX];
	randomPos(arrayPos);

	for (int i = 0; i < CLIENT_MAX; i++) {
		cout << arrayPos[i] << ", ";
	}
	cout << endl;

	int dataLen = sizeof(arrayPos);
	char* posData = new char[dataLen];

	ZeroMemory(posData, dataLen);
	memcpy(posData, arrayPos, dataLen);

	for (int i = 0; i < CLIENT_MAX; i++) {
		//client의 시작위치를 보낸다.		
		int iResult = sendn(clientState[i].clientTCPSock, posData, dataLen, 0);
		if (iResult == SOCKET_ERROR)
			ErrorHandling("sendn");
	}
	delete[]posData;
}

void TCPServer::randomPos(int arrayPos[]) {
	//client의 시작 위치 index 값을 난수로 생성
	srand((unsigned int)time(NULL));
	for (int i = 0; i < CLIENT_MAX; i++) {
		int tempPos = rand() % 5;							
		arrayPos[i] = tempPos;

		for (int j = 0; j < i; j++) {
			if (arrayPos[i] == arrayPos[j]) {
				i--;
				break;
			}
		}
	}
}