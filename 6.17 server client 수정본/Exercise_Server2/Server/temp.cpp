#include "temp.h"

temp::temp(ClientState client[], int* Num){
	clients = client;
	clientNum = Num;
}

void temp::tempStart(){
	tempThread = thread([&] {tempThreadMain(); });
}

int temp::tempThreadMain(){
	while (1) {
		
		if ((*clientNum) == 0) {
			return 0;
		}

		for (int i = 0; i < CLIENT_MAX; i++) {			
			cout << "client#" << clients[i].id << " : " << clients[i].pos.x << endl;
			Sleep(1000);
		}

	}
	return 0;
}

void temp::tempThreadJoin() {
	tempThread.join();
}
