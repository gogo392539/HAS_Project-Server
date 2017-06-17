#include "classTcp.h"

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
				if ((i != ID) && (clientState[i].id != -1)) {
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