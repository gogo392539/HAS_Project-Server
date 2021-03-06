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
		TCPserver.serverStart();				//TCP server 설정
		cout << "--------------------------------------------------------------" << endl;
		cout << "Server Start" << endl;	
		cout << endl;

		UDPServer UDPserver = UDPServer(clientState, &connectNum);
		UDPserver.serverStart();				//UDP server 설정

		TCPserver.RoomThreadStart();	
		
		UDPserver.recvUDPThreadFunc();			//UDP server가 client들의 좌표정보를 받는 thread
		TCPserver.sendRandomIdx();				//client들에게 random index전달
		TCPserver.sendTaggerUserID();
		
		TCPserver.TCPThreadStart();

		//temp tmp = temp(clientState, &connectNum);
		//tmp.tempStart();
		//tmp.tempThreadJoin();

		UDPserver.UDPThreadJoin();
		cout << "UDP thread end" << endl;

		TCPserver.TCPThreadJoin();
		cout << "TCP thread end" << endl;

		TCPserver.TCPServerClosed();
		UDPserver.UDPServerClosed();

		cout << endl;
		cout << "server closed" << endl;
		cout <<"--------------------------------------------------------------"<< endl;
	}
	
	return 0;
}

