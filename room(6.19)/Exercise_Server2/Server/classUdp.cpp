#include "classUdp.h"

UDPServer::UDPServer() {

}

UDPServer::UDPServer(ClientState clients[], int *clientNum) {
	clientState = clients;
	connectNum = clientNum;
	sendThreadSet = true;
}

void UDPServer::serverStart() {
	serverUDPSock = socket(PF_INET, SOCK_DGRAM, 0);
	if (serverUDPSock == INVALID_SOCKET)
		ErrorHandling("socket()");

	memset(&serverUDPAddr, 0, sizeof(serverUDPAddr));
	serverUDPAddr.sin_family = AF_INET;
	serverUDPAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serverUDPAddr.sin_port = htons(DEFAULT_PORT);

	if (::bind(serverUDPSock, (sockaddr*)&serverUDPAddr, sizeof(serverUDPAddr)) == SOCKET_ERROR)
		ErrorHandling("bind");
}

void UDPServer::recvThreadStart() {
	recvPosThread = thread([&] {recvPosThreadMain(clientState, serverUDPSock); });
}

int UDPServer::recvPosThreadMain(ClientState clientState[], SOCKET serverUDPSock) {
	//Ŭ���̾�Ʈ�� ��ǥ ������ �޾ƿ��� thread
	mutex mutex;

	bool addr1 = true;
	bool addr2 = true;
	bool addr3 = true;
	bool addr4 = true;
	bool addr5 = true;
	bool sendThreadSet = true;
	int count = 0;

	int bufLen = sizeof(int) + sizeof(Pos);
	char* recvBuf = new char[bufLen];

	while (1) {
		SOCKADDR_IN temp;
		int tempAddrSize = sizeof(temp);
		int tempId = -1;
		int iResult = 0;
		ZeroMemory(recvBuf, bufLen);

		iResult = recvfrom(serverUDPSock, recvBuf, bufLen, 0, (SOCKADDR *)&temp, &tempAddrSize);
		if (iResult == -1 && iResult == 0) {
			//ErrorHandling("UDP recvfrom");
			cout << "EOF" << endl;
			break;
		}

		memcpy(&tempId, recvBuf, sizeof(int));

		if (addr1 && tempId == 0) {
			clientState[tempId].clientUDPAddr = temp;
			clientState[tempId].clientUDPAddrSize = tempAddrSize;
			addr1 = false;
			count++;
		}

		if (addr2 && tempId == 1) {
			clientState[tempId].clientUDPAddr = temp;
			clientState[tempId].clientUDPAddrSize = tempAddrSize;
			addr2 = false;
			count++;
		}

		if (addr3 && tempId == 2) {
			clientState[tempId].clientUDPAddr = temp;
			clientState[tempId].clientUDPAddrSize = tempAddrSize;
			addr3 = false;
			count++;
		}

		if (addr4 && tempId == 3) {
			clientState[tempId].clientUDPAddr = temp;
			clientState[tempId].clientUDPAddrSize = tempAddrSize;
			addr4 = false;
			count++;
		}

		if (addr5 && tempId == 4) {
			clientState[tempId].clientUDPAddr = temp;
			clientState[tempId].clientUDPAddrSize = tempAddrSize;
			addr5 = false;
			count++;
		}

		mutex.lock();
		memcpy(&clientState[tempId].pos, recvBuf + sizeof(int), sizeof(Pos));
		mutex.unlock();

		if (count == CLIENT_MAX && sendThreadSet) {
			//client���� �ּ� ������ ��� �����ϰ� ���� sendThread�� �����Ѵ�.
			sendPosThread = thread([&] {sendPosThreadMain(clientState, serverUDPSock); });	//���� ���� �̿��Ͽ� Ŭ������ ��� �Լ��� thread�� �����Ų��.
			sendThreadSet = false;
			cout << "UDP send Thread start" << endl;
		}
	}
	delete[]recvBuf;

	cout << "UDP recv thread end" << endl;

	return 0;
}

int UDPServer::sendPosThreadMain(ClientState clientState[], SOCKET serverUDPSock) {
	//Ŭ���̾�Ʈ�鿡�� ��ǥ ������ �����ϴ� thread
	mutex mutex;

	int bufLen = sizeof(int) + sizeof(Pos);
	char* sendBuf = new char[bufLen];
	int disconnectedNum = 0;

	while (true) {
		for (int i = 0; i < CLIENT_MAX; i++) {
			int iResult = 0;
			/*ZeroMemory(sendBuf, bufLen);
			memcpy(sendBuf, &clientState[i], bufLen);*/
			if (clientState[i].id != -1) {
				ZeroMemory(sendBuf, bufLen);
				memcpy(sendBuf, &clientState[i], bufLen);

				for (int j = 0; j < CLIENT_MAX; j++) {
					if (clientState[i].id != j) {
						iResult = sendto(serverUDPSock, sendBuf, bufLen, 0, (SOCKADDR*)&clientState[j].clientUDPAddr, clientState[j].clientUDPAddrSize);
						if (iResult == SOCKET_ERROR) {
							ErrorHandling("UDP sendto");
						}						
						//cout << clientState[i].id << " -> " << j << " : " << clientState[i].pos.x << endl;
					}
				} // for end
			} 

		} // for end

		if ((*connectNum) == 0) {
			//recv thread�� ���� ��Ű�� �ڵ� 
			sendPosThread.detach();
			SuspendThread(recvPosThread.native_handle());		//�Ͻ� ����
			TerminateThread(recvPosThread.native_handle(), 0);	//����
			break;
		}
		Sleep(10);
	}//while end

	delete[]sendBuf;
	cout << "UDP send thread end" << endl;
	return 0;
}

void UDPServer::threadJoin() {
	if (recvPosThread.joinable() == true) {
		recvPosThread.join();
	}
	if (sendPosThread.joinable() == true) {
		sendPosThread.join();
	}
}

void UDPServer::serverClosed() {
	closesocket(serverUDPSock);
}