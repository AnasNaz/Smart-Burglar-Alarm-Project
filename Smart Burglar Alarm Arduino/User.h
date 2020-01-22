#pragma once
#include <string>
using namespace std;

class User
{
public:
	bool verifyLogin();
private:
	string _userID;
	string _password;
};

