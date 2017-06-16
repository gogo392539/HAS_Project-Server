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

void UDPServer::receiveClientAddr() {
	int iResult = 0;
	char* tempFlag = new char[4];
	ZeroMemory(tempFlag, sizeof(int));
	for (int i = 0; i < CLIENT_MAX; i++) {
		iResult = recvfrom(serverUDPSock, tempFlag, sizeof(int), 0, (SOCKADDR*)&clientState[i].clientUDPAddr, &clientState[i].clientUDPAddrSize);
		if (iResult == SOCKET_ERROR) {
			ErrorHandling("UDP recvfrom12");
		}
	}
	delete[]tempFlag;
}

void UDPServer::recvUDPThreadFunc() {
	//recvUDPPosThread = thread(recvPosThread, clientState, serverUDPSock);
	recvUDPPosThread = thread([&] {recvPosThread(clientState, serverUDPSock); });
}

void UDPServer::sendUDPThreadFunc() {
	//sendUDPPosThread = thread(sendPosThread, clientState, serverUDPSock, connectNum);			//이 경우에는 클래스의 멤버함수를 static으로 선언해야 한다.
	sendUDPPosThread = thread([&] {sendPosThread(clientState, serverUDPSock); });	//람다 식을 이용하여 클래스의 멤버 함수를 thread로 실행시킨다.
}

void UDPServer::UDPThreadJoin() {
	if (recvUDPPosThread.joinable() == true) {
		recvUDPPosThread.join();
	}
	if (sendUDPPosThread.joinable() == true) {
		sendUDPPosThread.join();
	}
}

void UDPServer::UDPServerClosed() {
	closesocket(serverUDPSock);
}

int UDPServer::recvPosThread(ClientState clientState[], SOCKET serverUDPSock) {
	//클라이언트의 좌표 정보를 받아오는 thread
	mutex mutex;

	bool addr1 = true;
	bool addr2 = true;
	bool addr3 = true;
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

		mutex.lock();
		memcpy(&clientState[tempId].pos, recvBuf + sizeof(int), sizeof(Pos));
		mutex.unlock();

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

		if (count == 3 && sendThreadSet) {
			sendUDPPosThread = thread([&] {sendPosThread(clientState, serverUDPSock); });	//람다 식을 이용하여 클래스의 멤버 함수를 thread로 실행시킨다.
			sendThreadSet = false;
		}
	}
	delete[]recvBuf;

	cout << "UDP recv thread end" << endl;

	return 0;
}

int UDPServer::sendPosThread(ClientState clientState[], SOCKET serverUDPSock) {
	//클라이언트들에게 좌표 정보를 전달하는 thread
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
			//recv thread를 종료 시키는 코드 
			sendUDPPosThread.detach();
			SuspendThread(recvUDPPosThread.native_handle());		//일시 정지
			TerminateThread(recvUDPPosThread.native_handle(), 0);	//종료
			break;
		}
		Sleep(10);
	}//while end

	delete[]sendBuf;
	cout << "UDP send thread end" << endl;
	return 0;
}