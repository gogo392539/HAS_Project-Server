#include "classTcp.h"

TCPServer::TCPServer() {

}

TCPServer::TCPServer(ClientState clients[], int *clientNum) {
	clientTCPAddrsz = sizeof(clientTCPAddr);
	clientState = clients;
	connectNum = clientNum;
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
	//serverAddr.sin_addr.s_addr = inet_addr("192.168.63.41");
	serverTCPAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serverTCPAddr.sin_port = htons(DEFAULT_PORT);

	if (::bind(serverListenSock, (sockaddr*)&serverTCPAddr, sizeof(serverTCPAddr)))
		ErrorHandling("bind() Join");
	if (listen(serverListenSock, 5))
		ErrorHandling("listen() Join");
}

void TCPServer::clientAccept() {
	//accept() and client에게 id값 전달 (연결 요청 수락)
	while (*connectNum < CLIENT_MAX) {
		clientTCPAddrsz = sizeof(clientTCPAddr);
		clientState[*connectNum].clientTCPSock = accept(serverListenSock, (sockaddr*)&clientTCPAddr, &clientTCPAddrsz);
		if (clientState[*connectNum].clientTCPSock == INVALID_SOCKET)
			ErrorHandling("accept()");

		clientState[*connectNum].id = *connectNum;
		cout << "connect" << *connectNum << endl;

		int sendNum = sendn(clientState[*connectNum].clientTCPSock, (char*)&clientState[*connectNum].id, sizeof(clientState[*connectNum].id), 0);
		if (sendNum == SOCKET_ERROR)
			ErrorHandling("sendn");
		(*connectNum)++;
	}
}

void TCPServer::TCPServerClosed() {
	for (int i = 0; i < CLIENT_MAX; i++) {
		closesocket(clientState[i].clientTCPSock);
	}
	closesocket(serverListenSock);	
	WSACleanup();
}

void TCPServer::sendTaggerUserID() {
	//모든 client에게 술래의 ID 값을 전달
	taggerUserID = SelectTaggerUser();

	for (int i = 0; i < CLIENT_MAX; i++) {
		int sendNum = sendn(clientState[i].clientTCPSock, (char*)&taggerUserID, sizeof(int), 0);
	}

	cout << "tagger user ID : " << taggerUserID << endl;
}

int TCPServer::SelectTaggerUser() {
	//tagger user ID 를 난수로 생성
	srand((unsigned int)time(NULL));
	int taggerID = rand() % CLIENT_MAX;

	return taggerID;
}

void TCPServer::sendRandomIdx() {
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

void TCPServer::TCPThreadStart() {
	//TCPThread = thread(wholeTCPStateThread, clientState);
	TCPThread = thread([&] {TCPThreadFunc(); });
}


void TCPServer::TCPThreadJoin() {
	if (TCPThread.joinable() == true) {
		TCPThread.join();
	}
	SuspendThread(tempThread.native_handle());			//일시 정지
	TerminateThread(tempThread.native_handle(), 0);		//종료
}

int TCPServer::TCPThreadFunc() {
	int i=0;
	while (true) {
		for (; i < CLIENT_MAX; i++) {
			Client_Thread[i] = thread([&] {ClientMainThread(i); });
		}
	
		if ((*connectNum) == 0) {
			break;
		}
	} //while end

	for (int j = 0; j < CLIENT_MAX; j++) {
		Client_Thread[j].join();
	} //for end	
	cout << "TCP thread end" << endl;
	return 0;
}

int TCPServer::ClientMainThread(int myID) {
	mutex mutex;
	int recvResult;
	int sendResult;
	char* eventData = new char[EVENTPACKET_SIZE];

	while (true) {
		ZeroMemory(eventData, 0);
		eventpacket = { -1, -1, -1 };
		
		recvResult = recvn(clientState[myID].clientTCPSock, eventData, EVENTPACKET_SIZE, 0);	
		if (recvResult == -1) {
			cout << "client#" << myID << "closed!" << endl;
			mutex.lock();
			(*connectNum)--;
			clientState[myID].id = -1;
			mutex.unlock();
			break;
		}
		else if (recvResult > 0) {
			memcpy(&eventpacket, eventData, EVENTPACKET_SIZE);

			switch (eventpacket.flag) {
			case 1:
				puzzleEventFunc(eventpacket, myID);
				break;
			case 2:
				playerkillEventFunc(eventpacket, myID);
				break;
			case 3:
				trapEventFunc(eventpacket, myID);
				break;
			case 4:				
				//animationEventFunc(eventpacket);
				for (int i = 0; i < CLIENT_MAX; i++) {
					//cout << "ani ID : " << eventpacket.id << " " << "ani set : " << eventpacket.Set << endl;
					if (eventpacket.id != i) {
						sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
					}
				}
				break;
			case 5:
				//animationEventFunc(eventpacket);
				for (int i = 0; i < CLIENT_MAX; i++) {
					//cout << "ani ID : " << eventpacket.id << " " << "ani set : " << eventpacket.Set << endl;
					if (eventpacket.id != i) {
						sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
					}
				}
				break;
			case 6:
				//animationEventFunc(eventpacket);
				for (int i = 0; i < CLIENT_MAX; i++) {
					//cout << "ani ID : " << eventpacket.id << " " << "ani set : " << eventpacket.Set << endl;
					if (eventpacket.id != i) {
						sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
					}
				}
				break;
			}
		}
	} //while end

	delete[]eventData;
	cout << "Client Thread #" << myID << " end!" << endl;
	return 0;
}

void TCPServer::puzzleEventFunc(eventPacket packet, int ID){
	eventPacket eventpacket = packet;
	int myID = ID;
	int sendResult;

	//본인이 보낸 eventpacket을 자기자신이 받지 않기 위해 인자로 ID 값을 받아서 처리하고 있음
	for (int i = 0; i < CLIENT_MAX; i++) {
		if (i != myID) {
			cout << eventpacket.flag << eventpacket.id << eventpacket.Set << endl;	//
			sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
		}
	}	
}

void TCPServer::playerkillEventFunc(eventPacket packet, int ID){
	mutex mutex;
	eventPacket eventpacket = packet;
	int myID = ID;
	int sendResult;

	mutex.lock();
	clientState[eventpacket.id].id = -1;
	mutex.unlock();

	cout << "killed ID : " << eventpacket.id << endl;

	for (int i = 0; i < CLIENT_MAX; i++) {
		if (i != myID) {				//본인이 보낸 eventpacket을 자기 자신이 받지 않도록 설정
			sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
		}
	}
}

void TCPServer::animationEventFunc(eventPacket packet){
	eventPacket eventpacket = packet;
	int sendResult;

	for (int i = 0; i < CLIENT_MAX; i++) {
		if (eventpacket.id != i) {
			sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
		}
	}
}

void TCPServer::trapEventFunc(eventPacket packet, int ID) {
	eventPacket eventpacket = packet;
	int myID = ID;
	int sendResult;
	cout << eventpacket.flag << " " << eventpacket.id << " " << eventpacket.Set << endl;
	//본인이 보낸 eventpacket을 자기자신이 받지 않기 위해 인자로 ID 값을 받아서 처리하고 있음
	for (int i = 0; i < CLIENT_MAX; i++) {
		if (i != myID) {
			sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
			if (sendResult == SOCKET_ERROR) {
				cout << "trap sendn error" << endl;
			}
		}
	}
}