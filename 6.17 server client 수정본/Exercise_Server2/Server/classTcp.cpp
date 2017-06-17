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

void TCPServer::roomThreadJoin() {
	for (int i = 0; i < CLIENT_MAX; i++) {
		room_Thread[i].join();
	}
}

void TCPServer::roomThreadStart() {
	while (acceptStartSet) {
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

			clientListSet[tempid] = 1;

			sendNum = sendn(clientState[tempid].clientTCPSock, (char*)&clientListSet, sizeof(int) * CLIENT_MAX, 0);
			if (sendNum == SOCKET_ERROR)
				ErrorHandling("sendn");

			room_Thread[tempid] = thread([&] {roomThreadMain(tempid); });
		}
	} //while end
	cout << "RoomThreadStart end" << endl;
}

int TCPServer::roomThreadMain(int id) {
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
				//room master exit
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
				//other client exit
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
			clientListSet[recvPacket.id] = -1;
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
			acceptStartSet = false;
			cout << "Game Start" << endl;			
		}
		else if (recvPacket.flag == 4) {
			//Thread exit
			break;
		}
	} // while end

	cout << "RoomThreadMain#" << ID << " end" << endl;
	room_Thread[ID].detach();
	return 0;
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

void TCPServer::threadCallingStart() {
	callingThread = thread([&] {eventThreadCallingThreadMain(); });
}


void TCPServer::eventThreadCallingThreadJoin() {
	if (callingThread.joinable() == true) {
		callingThread.join();
	}
}

int TCPServer::eventThreadCallingThreadMain() {
	int i=0;
	while (true) {
		for (; i < CLIENT_MAX; i++) {
			event_Thread[i] = thread([&] {eventThreadMain(i); });
		}
	
		if ((*connectNum) == 0) {
			break;
		}
	} //while end

	for (int j = 0; j < CLIENT_MAX; j++) {
		event_Thread[j].join();
	} //for end	
	cout << "TCP thread end" << endl;
	return 0;
}

int TCPServer::eventThreadMain(int myID) {
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
				animationEventFunc(eventpacket);
				break;
			case 4:
				for (int i = 0; i < CLIENT_MAX; i++) {
					//cout << "ani ID : " << eventpacket.id << " " << "ani set : " << eventpacket.Set << endl;
					if (eventpacket.id != i) {
						sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
					}
				}
				break;
			case 5:
				for (int i = 0; i < CLIENT_MAX; i++) {
					//cout << "ani ID : " << eventpacket.id << " " << "ani set : " << eventpacket.Set << endl;
					if (eventpacket.id != i) {
						sendResult = sendn(clientState[i].clientTCPSock, (char*)&eventpacket, EVENTPACKET_SIZE, 0);
					}
				}
				break;
			case 6:
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