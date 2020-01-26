#pragma once
#include <string>
using namespace std;

class User
{
public:
	bool verifyLogin();
	void SetName(string name);
	string GetPIN();
	string GetName();
	string GetAuthenticationMethod();
private:
	string _userID;
	string _password;
	string _authenticationMethod;
};

