#include "HardwareSerial.h"
#include "Arduino.h"
#include "WString.h"
#ifndef USER_H
#define USER_H
class User
{
public:
  User(String name,String Pin,String security):_userID(name),_password(Pin),_authenticationMethod(security){}; //Constructor that initializes the members
  User(); //Constructor that initalizes an empty user. This helps in creating a Current User in SmartAlarm.
  void SetName(String name); //Sets the user name or _userId
  String GetName();//Getter function for the user name
  void SetPin(String Pin);//Sets user's pin
  String GetPIN();//Gets user's pin
  void SetAuthenticationMethod(String security);//Sets user's authentication method
  String GetAuthenticationMethod();// Gets user's authentication method
private:
  String _userID;
  String _password;
  String _authenticationMethod;
};
#endif
