#include <iostream>
#include <thread>
#include <WinSock2.h>
#include <string>

#define CLIENT_MAX 3
#define DEFAULT_PORT 30000
#define DEFAULT_MESSAGE 1024

struct clientChatState {
	int id;
	SOCKET clientSock;
};

int chat_client(clientChatState* clients, int myID);
void error_handling(char* message);

using namespace std;

int main(void) {
	
	SOCKET serverSock;
	clientChatState clients[CLIENT_MAX];

	SOCKADDR_IN serverAddr;
	SOCKADDR_IN clientAddr;
	thread client_threads[CLIENT_MAX];
	int connectNum = 0;
	int clientAddrsz;

	WSADATA wsaData;
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
		error_handling("WSAStartup() error");
	}

	serverSock = socket(PF_INET, SOCK_STREAM, 0);
	if (serverSock == SOCKET_ERROR) {
		error_handling("socket() error");
	}

	memset(&serverAddr, 0, sizeof(serverAddr));
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serverAddr.sin_port = htons(DEFAULT_PORT);

	if (::bind(serverSock, (sockaddr*)&serverAddr, sizeof(serverAddr))) {
		error_handling("bind() error");
	}
	if (listen(serverSock, 5)) {
		error_handling("listen() error");
	}
	cout << "chatting Server Start" << endl;
	while (connectNum < CLIENT_MAX) {
		clientAddrsz = sizeof(clientAddr);
		clients[connectNum].clientSock = accept(serverSock, (sockaddr*)&clientAddr, &clientAddrsz);
		if (clients[connectNum].clientSock == SOCKET_ERROR) {
			error_handling("accept() error");
		}
		clients[connectNum].id = connectNum;

		send(clients[connectNum].clientSock, (char*)&clients[connectNum].id, sizeof(int), 0);			//id РќДо

		client_threads[connectNum] = thread(chat_client, clients, connectNum);
		connectNum++;
	}

	for (int i = 0; i < CLIENT_MAX; i++) {
		client_threads[i].join();
	}

	closesocket(serverSock);
	for (int i = 0; i < CLIENT_MAX; i++) {
		closesocket(clients[i].clientSock);
	}


	WSACleanup();
	return 0;
}

int chat_client(clientChatState* clients, int ID) {
	int myID = ID;
	clientChatState* Clients = clients;

	string message = "";
	char tempMessage[DEFAULT_MESSAGE] = "";

	while (1) {
		memset(tempMessage, 0, DEFAULT_MESSAGE);

		int recvResult = recv(clients[myID].clientSock, tempMessage, DEFAULT_MESSAGE, 0);
		if (recvResult != SOCKET_ERROR) {
			if (strcmp("", tempMessage)) {
				message = "Client #" + std::to_string(clients[myID].id) + ": " + tempMessage;
			}
			cout << message << endl;

			for (int i = 0; i < CLIENT_MAX; i++) {
				if (clients[i].clientSock != INVALID_SOCKET) 
					if (clients[myID].id != i) 
						int sendResult = send(clients[i].clientSock, message.c_str(), strlen(message.c_str()), 0);
			}
		}
		else {
			message = "Client #" + std::to_string(clients[myID].id) + " Disconnected";
			clients[myID].clientSock = INVALID_SOCKET;
			break;
		}
	}

	return 0;
}

void error_handling(char* message) {
	fputs(message, stderr);
	fputc('\n', stderr);
	exit(1);
}