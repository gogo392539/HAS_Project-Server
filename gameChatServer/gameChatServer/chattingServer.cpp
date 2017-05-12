#include <iostream>
#include <thread>
#include <WinSock2.h>

#define CLIENT_MAX 5
#define DEFAULT_PORT 30000

struct clientChatState {
	int id;
	SOCKET clientSock;
};

int chat_client();

using namespace std;

int main(void) {
	
	SOCKET serverSock;
	clientChatState clients;

	SOCKADDR_IN serverAddr;
	SOCKADDR_IN clientAddr;
	thread client_threads[CLIENT_MAX];
	int connectNum = 0;
	int clientAddrsz;

	WSADATA wsaData;
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
		error_handling("WSAStartup() error");
	}






	WSACleanup();
	return 0;
}

int chat_client() {

}

void error_handling(char* message) {
	fputs(message, stderr);
	fputc('\n', stderr);
	exit(1);
}