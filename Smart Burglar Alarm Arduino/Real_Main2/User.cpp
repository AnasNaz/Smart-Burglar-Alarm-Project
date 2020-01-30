#include "User.h"
/*User.cpp includes the functions logic*/
 User::User() //Empty User
{
}
void User::SetName(String name) //Sets the name of the user
{
  this->_userID=name;
}
void User::SetAuthenticationMethod(String security) //Sets authentication method, P, F or Both
{
  this->_authenticationMethod=security;
}
void User::SetPin(String Pin) //Sets password
{
  this->_password=Pin;
}
String User::GetPIN() //Gets password
{
  return this->_password;
}
String User::GetName()//Gets name
{
  return this->_userID;
}
String User::GetAuthenticationMethod()//Gets authentication method
{
  return this->_authenticationMethod;
}
void User::Set_isMaintenance(bool yes_or_no) //Sets whether user is part of the maintenance team or not
{
  this->_isMaintenance=yes_or_no;
}
bool User::Get_isMaintenance()//Returns true if user is part of the maintenance team or false if not
{
  return this->_isMaintenance;
}
