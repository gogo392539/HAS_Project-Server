#include "classTcp.h"

TCPServer::TCPServer() {

}

TCPServer::TCPServer(ClientState clients[], int *clientNum) {
	clientTCPAddrsz = sizeof(clientTCPAddr);
	clientState = clients;
	connectNum = clientNum;
	/*for (int i = 0; i < CLIENT_MAX; i++) {
		AccessState[i].id = i;
		AccessState[i].set = 0;
	}*/
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
	for (int i = 0; i < CLIENT_MAX; i++) {
		Accept_Thread[i] = thread([&] {AcceptThreadMain(i); });
	}
}

int TCPServer::AcceptThreadMain(int ID) {
	mutex mx;
	//char* LDATA = new char[4];
	bool check = true;
	while (check) {
		//accept()
		clientTCPAddrsz = sizeof(clientState[ID].clientTCPAddr);
		clientState[ID].clientTCPSock = accept(serverListenSock, (sockaddr*)&clientState[ID].clientTCPAddr, &clientTCPAddrsz);
		if (clientState[ID].clientTCPSock == INVALID_SOCKET)
			ErrorHandling("accept()");

		clientState[ID].id = ID;
		cout <<"Client#"<<ID<<" "<< "connect" << endl;

		int sendNum = sendn(clientState[ID].clientTCPSock, (char*)&clientState[ID].id, sizeof(clientState[ID].id), 0);
		if (sendNum == SOCKET_ERROR)
			ErrorHandling("sendn");

		mx.lock();
		(*connectNum)++;
		mx.unlock();

		char* LDATA = new char[4];
		while (1) {
			ZeroMemory(LDATA, 0);
			int recvNum = recvn(clientState[ID].clientTCPSock, LDATA, sizeof(LDATA), 0);
			if (recvNum == -1) {
				//client가 방에 들어와서 나가면 accept부터 다시 한다.
				mx.lock();
				(*connectNum)--;
				mx.unlock();
				cout << "client#" << ID << " " << "exit" << endl;
				closesocket(clientState[ID].clientTCPSock);
				break;
			}
			else {
				//client가 방에 들어와서 ready button을 누르면 thread 종료
				int readyValue = -1;
				memcpy(&readyValue, LDATA, sizeof(int));
				if (readyValue == 1) {
					cout << "client#" << ID << " " << "ready" << endl;
					check = false;
					break;
					//delete[] LDATA;
					//return 0;
				}
			}
			delete[] LDATA;
		} // end while
	} // end while
	//cout << "AcceptThreadMain 종료" << endl;
	return 0;
}

void TCPServer::GameStart() {
	cout << "start1" << endl;
	char *startData = new char[4];
	int startValue = 0;
	while (1) {
		ZeroMemory(startData, 0);
		//방장(일단 첫 번째 client를 방장으로 설정)으로부터 시작하겠다는 message 수신
		int sendResult = recvn(clientState[0].clientTCPSock, startData, sizeof(startData), 0);
		cout << "start3" << endl;
		memcpy(&startValue, startData, sizeof(int));
		if (startValue == 2) {

			for (int i = 0; i < CLIENT_MAX; i++) {
				//게임 시작 send
				sendn(clientState[i].clientTCPSock, startData, sizeof(startData), 0);
			}

			cout << "Game Start!" << endl;
			break;
		}
	}// end while
	delete[] startData;
}

void TCPServer::AcceptThreadJoin() {
	for (int i = 0; i < CLIENT_MAX; i++) {
		Accept_Thread[i].join();
	}
}

void TCPServer::TCPServerClosed() {
	for (int i = 0; i < CLIENT_MAX; i++) {
		closesocket(clientState[i].clientTCPSock);
	}
	closesocket(serverListenSock);	
	WSACleanup();
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
	//int sendResult;
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
				animationEventFunc(eventpacket);
				break;
			case 4:
				trapEventFunc(eventpacket, myID);
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

	//본인이 보낸 eventpacket을 자기자신이 받지 않기 위해 인자로 ID 값을 받아서 처리하고 있음
	for (int i = 0; i < CLIENT_MAX; i++) {
		if (i != myID) {
			sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
		}
	}
}