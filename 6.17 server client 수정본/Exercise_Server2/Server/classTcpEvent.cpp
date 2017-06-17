#include "classTcp.h"

void TCPServer::threadCallingStart() {
	callingThread = thread([&] {eventThreadCallingThreadMain(); });
	callingThread.join();
}


void TCPServer::eventThreadCallingThreadJoin() {
	if (callingThread.joinable() == true) {
		callingThread.join();
	}
}

int TCPServer::eventThreadCallingThreadMain() {
	int i = 0;
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

void TCPServer::puzzleEventFunc(eventPacket packet, int ID) {
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

void TCPServer::playerkillEventFunc(eventPacket packet, int ID) {
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

void TCPServer::animationEventFunc(eventPacket packet) {
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