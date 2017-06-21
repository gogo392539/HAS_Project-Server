#include "classTcp.h"

TCPServer::TCPServer() {

}

TCPServer::TCPServer(ClientState clients[], int *clientNum) {
	clientTCPAddrsz = sizeof(clientTCPAddr);
	clientState = clients;
	connectNum = clientNum;
	taggerUserID = -1;

	AcceptStartSet = true;
	tempid = -1;

	for (int i = 0; i < CLIENT_MAX; i++) {
		ClientListSet[i] = -1;
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
	//serverAddr.sin_addr.s_addr = inet_addr("192.168.63.41");
	serverTCPAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serverTCPAddr.sin_port = htons(DEFAULT_PORT);

	if (::bind(serverListenSock, (sockaddr*)&serverTCPAddr, sizeof(serverTCPAddr)))
		ErrorHandling("bind() Join");
	if (listen(serverListenSock, 5))
		ErrorHandling("listen() Join");
}

void TCPServer::RoomThreadJoin() {
	for (int i = 0; i < CLIENT_MAX; i++) {
		Room_Thread[i].join();
	}
}

void TCPServer::RoomThreadStart() {
	while (AcceptStartSet) {
		clientTCPAddrsz = sizeof(clientTCPAddr);
		SOCKET incoming = INVALID_SOCKET;

		tempid = -1;

		if ((*connectNum) != CLIENT_MAX) {
			incoming = accept(serverListenSock, (sockaddr*)&clientTCPAddr, &clientTCPAddrsz);
			if (incoming == INVALID_SOCKET)
				continue;

			for (int i = 0; i < CLIENT_MAX; i++) {
				if (clientState[i].clientTCPSock == INVALID_SOCKET && tempid == -1) {
					clientState[i].clientTCPSock = incoming;
					clientState[i].id = i;
					tempid = i;
					(*connectNum)++;
					
				}
			} // for end
			cout << "clientNum : " << (*connectNum) << endl;
		}

		if (tempid != -1) {
			cout << "connect" << tempid << endl;

			int sendNum = sendn(clientState[tempid].clientTCPSock, (char*)&clientState[tempid].id, sizeof(clientState[tempid].id), 0);
			if (sendNum == SOCKET_ERROR)
				ErrorHandling("sendn");

			ClientListSet[tempid] = 1;

			sendNum = sendn(clientState[tempid].clientTCPSock, (char*)&ClientListSet, sizeof(int) * CLIENT_MAX, 0);
			if (sendNum == SOCKET_ERROR)
				ErrorHandling("sendn");

			Room_Thread[tempid] = thread([&] {RoomThreadMain(tempid); });
		}
	} //while end
	cout << "RoomThreadStart end" << endl;
}

int TCPServer::RoomThreadMain(int id) {
	mutex mx;
	int ID = id;
	int sendNum;
	int recvNum;

	while (1) {
		roomPacket recvPacket;
		recvPacket.flag = -1;
		recvPacket.id = -1;
		recvNum = recvn(clientState[ID].clientTCPSock, (char*)&recvPacket, ROOMPACKET_SIZE, 0);

		if (recvNum == -1 && recvNum == 0) {
			cout << "recvNum == EOF" << endl;
			break;
		}

		if (recvPacket.flag == 1) {
			//client entrance
			roomPacket enterPacket;
			enterPacket.flag = 1;
			enterPacket.id = recvPacket.id;
			for (int i = 0; i < CLIENT_MAX; i++) {
				if ( (i != ID) && (clientState[i].id != -1) ) {
					sendNum = sendn(clientState[i].clientTCPSock, (char*)&enterPacket, ROOMPACKET_SIZE, 0);
				}
			}			
			cout << "client#" << recvPacket.id << " enter" << endl;

			if ((*connectNum) == CLIENT_MAX) {
				// All client entrance
				roomPacket AllEnter;
				AllEnter.flag = 10;
				AllEnter.id = 0;
				sendNum = sendn(clientState[0].clientTCPSock, (char*)&AllEnter, ROOMPACKET_SIZE, 0);
				cout << "All client entrance" << endl;
			}
		}
		else if (recvPacket.flag == 2) {
			//client exit
			if (recvPacket.id == 0) {
				roomPacket AllexitPacket;
				AllexitPacket.flag = 9;
				AllexitPacket.id = recvPacket.id;
				for (int i = 0; i < CLIENT_MAX; i++) {
					if ((i != ID) && (clientState[i].id != -1)) {
						sendNum = sendn(clientState[i].clientTCPSock, (char*)&AllexitPacket, ROOMPACKET_SIZE, 0);
					}
				}			
			}
			else {
				roomPacket exitPacket;
				exitPacket.flag = 2;
				exitPacket.id = recvPacket.id;
				for (int i = 0; i < CLIENT_MAX; i++) {
					if ((i != ID) && (clientState[i].id != -1)) {
						sendNum = sendn(clientState[i].clientTCPSock, (char*)&exitPacket, ROOMPACKET_SIZE, 0);
					}
				}			
			}
			mx.lock();
			(*connectNum)--;
			ClientListSet[recvPacket.id] = -1;
			mx.unlock();
			closesocket(clientState[ID].clientTCPSock);
			clientState[ID].clientTCPSock = INVALID_SOCKET;			
			cout << "client#" << recvPacket.id << " exit" << endl;
			break;
		}
		else if (recvPacket.flag == 3) {
			//Game Start
			roomPacket startPacket;
			startPacket.flag = 3;
			startPacket.id = recvPacket.id;
			for (int i = 0; i < CLIENT_MAX; i++) {
				if ((clientState[i].id != -1)) {
					sendNum = sendn(clientState[i].clientTCPSock, (char*)&startPacket, ROOMPACKET_SIZE, 0);
				}
			}
			AcceptStartSet = false;
			cout << "Game Start" << endl;			
		}
		else if (recvPacket.flag == 4) {
			//Thread exit
			break;
		}
	} // while end

	cout << "RoomThreadMain#" << ID << " end" << endl;
	Room_Thread[ID].detach();
	return 0;
}

void TCPServer::TCPServerClosed() {
	for (int i = 0; i < CLIENT_MAX; i++) {
		closesocket(clientState[i].clientTCPSock);
	}
	closesocket(serverListenSock);	
	WSACleanup();
}

void TCPServer::sendTaggerUserID() {
	//��� client���� ������ ID ���� ����
	taggerUserID = SelectTaggerUser();

	for (int i = 0; i < CLIENT_MAX; i++) {
		int sendNum = sendn(clientState[i].clientTCPSock, (char*)&taggerUserID, sizeof(int), 0);
	}

	cout << "tagger user ID : " << taggerUserID << endl;
}

int TCPServer::SelectTaggerUser() {
	//tagger user ID �� ������ ����
	srand((unsigned int)time(NULL));
	int taggerID= rand() % CLIENT_MAX;

	return taggerID;
}

void TCPServer::sendRandomIdx() {
	//client�� ���� ��ġ index ���� ����
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
		//client�� ������ġ�� ������.		
		int iResult = sendn(clientState[i].clientTCPSock, posData, dataLen, 0);
		if (iResult == SOCKET_ERROR)
			ErrorHandling("sendn");
	}
	delete[]posData;
}

void TCPServer::randomPos(int arrayPos[]) {
	//client�� ���� ��ġ index ���� ������ ����
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

	//������ ���� eventpacket�� �ڱ��ڽ��� ���� �ʱ� ���� ���ڷ� ID ���� �޾Ƽ� ó���ϰ� ����
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
		if (i != myID) {				//������ ���� eventpacket�� �ڱ� �ڽ��� ���� �ʵ��� ����
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
	//������ ���� eventpacket�� �ڱ��ڽ��� ���� �ʱ� ���� ���ڷ� ID ���� �޾Ƽ� ó���ϰ� ����
	for (int i = 0; i < CLIENT_MAX; i++) {
		if (i != myID) {
			sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
			if (sendResult == SOCKET_ERROR) {
				cout << "trap sendn error" << endl;
			}
		}
	}
}