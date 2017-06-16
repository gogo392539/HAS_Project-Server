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
		TCPserver.serverStart();				//TCP server ����
		cout << "--------------------------------------------------------------" << endl;
		cout << "Server Start" << endl;	
		cout << endl;

		UDPServer UDPserver = UDPServer(clientState, &connectNum);
		UDPserver.serverStart();				//UDP server ����

		TCPserver.RoomThreadStart();	
		//TCPserver.RoomThreadJoin();		

		
		//UDPserver.receiveClientAddr();			//UDP server�� client��� ����� ���� client���� �ּҰ��� �����ϰ� sync�� ���ߴ� �κ�

		UDPserver.recvUDPThreadFunc();			//UDP server�� client���� ��ǥ������ �޴� thread
		TCPserver.sendRandomIdx();				//client�鿡�� random index����
		//UDPserver.sendUDPThreadFunc();			//UDP server�� client��κ��� ���� ��ǥ������ �����ϴ� thread
		//TCPserver.sendRandomIdx();				//client�鿡�� random index����

		TCPserver.TCPThreadStart();

		temp tmp = temp(clientState, &connectNum);
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

