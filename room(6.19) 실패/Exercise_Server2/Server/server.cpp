#include "classTcp.h"
#include "classUdp.h"
#include "temp.h"
//#include "totalserver.h"

int main()
{
	while (1) {
		ClientState clientState[CLIENT_MAX];
		for (int i = 0; i < CLIENT_MAX; i++) {
			clientState[i].id = -1;
			clientState[i].pos = { -1, -1, -1, -1, -1, -1 };
			clientState[i].clientUDPAddrSize = sizeof(clientState[i].clientUDPAddr);
			clientState[i].clientTCPSock = INVALID_SOCKET;
		}
		int connectNum = 0;

		TCPServer TCPserver = TCPServer(clientState, &connectNum);
		TCPserver.serverStart();							//TCP server 설정
		cout << "--------------------------------------------------------------" << endl;
		cout << "Server Start" << endl;	
		cout << endl;

		UDPServer UDPserver = UDPServer(clientState, &connectNum);
		UDPserver.serverStart();							//UDP server 설정

		TCPserver.roomThreadStart();						//대기방 설정
		
		UDPserver.recvThreadStart();						//UDP server가 client들의 좌표정보를 받는 thread

		TCPserver.sendRandomIdx();							//client들에게 random position index전달
		TCPserver.sendTaggerUserID();						//client들에게 술래id 전달		
		TCPserver.threadCallingStart();						//Game event 처리 thread

		//temp tmp = temp(clientState, &connectNum);		//client들의 좌표 정보를 1초마다 출력하는 class
		//tmp.tempStart();
		//tmp.tempThreadJoin();

		UDPserver.threadJoin();
		cout << "UDP thread end" << endl;

		TCPserver.threadCallingStart();
		cout << "TCP thread end" << endl;

		TCPserver.serverClosed();
		UDPserver.serverClosed();

		cout << endl;
		cout << "server closed" << endl;
		cout <<"--------------------------------------------------------------"<< endl;
	}
	
	return 0;
}

