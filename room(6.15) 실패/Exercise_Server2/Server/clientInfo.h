#pragma once

#include <iostream>
#include <WinSock2.h>
#include <thread>
#include <mutex>
#include <cstdlib>
#include <ctime>
#include <string>

using namespace std;

#define CLIENT_MAX 5
#define DEFAULT_PORT 36872
#define PUZZLE_MAX 8
#define TRAP_MAX 3

#define ROOMPACKET_SIZE 8
#define EVENTPACKET_SIZE 12

//client�� ��ǥ ������ ���� ����
struct Pos {
	float x;
	float y;
	float z;
	float rotX;
	float rotY;
	float rotZ;
};

//Ŭ���̾�Ʈ�� ������ �����ϴ� ����ü
struct ClientState {
	int id;
	Pos pos;
	SOCKET clientTCPSock;
	SOCKADDR_IN clientUDPAddr;
	int clientUDPAddrSize;
};

struct eventPacket {
	int flag;
	int id;	//����/���� �̺�Ʈ�� ��� �ش� ����/������ ID��
	int Set;
};

struct roomPacket {
	int flag;
	int id;
};

void ErrorHandling(string);
int sendn(SOCKET socket, const char *buf, int len, int flag);
int recvn(SOCKET socket, char* buf, int len, int flaf);