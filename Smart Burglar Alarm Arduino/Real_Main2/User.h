#include "HardwareSerial.h"
#include "Arduino.h"
#include "WString.h"
#ifndef USER_H
#define USER_H
class User
{
public:
  User(String name,String Pin,String security,bool maintenance):_userID(name),_password(Pin),_authenticationMethod(security),_isMaintenance(maintenance){}; //Constructor that initializes the members
  User(); //Constructor that initalizes an empty user. This helps in creating a Current User in SmartAlarm.
  void SetName(String name); //Sets the user name or _userId
  String GetName();//Getter function for the user name
  void SetPin(String Pin);//Sets user's pin
  String GetPIN();//Gets user's pin
  void SetAuthenticationMethod(String security);//Sets user's authentication method
  String GetAuthenticationMethod();// Gets user's authentication method
  void Set_isMaintenance(bool yes_or_no); //Sets whether user is part of the maintenance team or not
  bool Get_isMaintenance();//Returns true if user is part of the maintenance team or false if not
private:
  String _userID;
  String _password;
  String _authenticationMethod;
  bool _isMaintenance; //Variable to check if user is part of the maintenance team, in order to give sensor information (non-functional requirement)
};
#endif
