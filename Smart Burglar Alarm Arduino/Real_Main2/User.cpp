#include "User.h"
/*User.cpp includes the function declaration*/
 User::User() //Empty User
{
}
void User::SetName(String name)
{
  this->_userID=name;
}
void User::SetAuthenticationMethod(String security)
{
  this->_authenticationMethod=security;
}
void User::SetPin(String Pin)
{
  this->_password=Pin;
}
String User::GetPIN()
{
  return this->_password;
}
String User::GetName()
{
  return this->_userID;
}
String User::GetAuthenticationMethod()
{
  return this->_authenticationMethod;
}
