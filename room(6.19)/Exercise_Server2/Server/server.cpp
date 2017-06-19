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
		TCPserver.serverStart();							//TCP server ����
		cout << "--------------------------------------------------------------" << endl;
		cout << "Server Start" << endl;	
		cout << endl;

		UDPServer UDPserver = UDPServer(clientState, &connectNum);
		UDPserver.serverStart();							//UDP server ����

		TCPserver.roomThreadStart();						//���� ����
		
		UDPserver.recvThreadStart();						//UDP server�� client���� ��ǥ������ �޴� thread

		TCPserver.sendRandomIdx();							//client�鿡�� random position index����
		TCPserver.sendTaggerUserID();						//client�鿡�� ����id ����		
		TCPserver.threadCallingStart();						//Game event ó�� thread

		//temp tmp = temp(clientState, &connectNum);		//client���� ��ǥ ������ 1�ʸ��� ����ϴ� class
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

