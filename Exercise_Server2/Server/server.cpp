#include "classTcp.h"
#include "classUdp.h"
#include "totalserver.h"

int main()
{

	while (1) {
		ClientState clientState[CLIENT_MAX];
		for (int i = 0; i < CLIENT_MAX; i++) {
			clientState[i].pos = { -1, -1, -1, -1, -1, -1 };
			clientState[i].clientUDPAddrSize = sizeof(clientState[i].clientUDPAddr);
		}
		int connectNum = 0;

		TCPServer TCPserver = TCPServer(clientState, &connectNum);
		TCPserver.serverStart();				//TCP server 설정
		cout << "TCP start" << endl;	
		cout << endl;

		UDPServer UDPserver = UDPServer(clientState, &connectNum);
		UDPserver.serverStart();				//UDP server 설정

		TCPserver.clientAccept();				//TCP server가 client의 접속 요청을 받아들이고 client들에게 ID값을 전달

		UDPserver.receiveClientAddr();			//UDP server가 client들과 통신을 위해 client들의 주소값을 저장하고 sync를 맞추는 부분


		UDPserver.recvUDPThreadFunc();			//UDP server가 client들의 좌표정보를 받는 thread
		TCPserver.sendRandomIdx();				//client들에게 random index전달
		UDPserver.sendUDPThreadFunc();			//UDP server가 client들로부터 받은 좌표정보를 전달하는 thread

		TCPserver.TCPThreadStart();

		TCPserver.TCPThreadJoin();
		cout << "TCP thread end" << endl;

		UDPserver.UDPThreadJoin();
		cout << "UDP thread end" << endl;

		UDPserver.UDPServerClosed();
		TCPserver.TCPServerClosed();

		cout << "server closed" << endl;
		cout << endl;
	}
	
	return 0;
}

