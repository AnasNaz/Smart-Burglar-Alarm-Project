#include <Arduino.h>
#include <WString.h>
#include "SmartAlarm.h"
#include "User.h"

//bool flag=false;
//millisDelay timer;
void setup() {
pinMode(MAGNETIC_SENSOR,INPUT_PULLUP);
pinMode(13, OUTPUT);
Serial.begin(9600);
//Serial.println("Program Starting");
//Serial.println("Creating Anas User");
User Anas;
Anas.SetName("Anas");
Anas.SetPin("123");
Anas.SetAuthenticationMethod("B");
//Or this way
User Francesco("Francesco","456","P");
User Saud("Saud","789","P");
User N("Nihar","000","P");
User UserArray[4];
UserArray[0]=Anas;
UserArray[1]=Francesco;
UserArray[2]=Saud;
UserArray[3]=N;
/*Serial.println("User Information");
Serial.println("Name: " + Francesco.GetName());
Serial.println("Pass: " + Francesco.GetPIN());
Serial.println("Authentication Method: " + Francesco.GetAuthenticationMethod());*/
SmartAlarm System(UserArray);
/*System.VerifyUser();
delay(2000);
System.VerifyUser();
delay(2000);
System.VerifyUser();
delay(2000);
System.VerifyUser();*/
/*String recordedLogins[10][2];
recordedLogins[0][0]=Anas.GetName();
recordedLogins[0][1]="1/10/2020";
recordedLogins[1][0]="Fra";
recordedLogins[1][1]="1/10/2020";
//Serial.println(recordedLogins[0][0]);Serial.print(recordedLogins[0][1]);
setTime(4,0,0,1,1,2020); //setTime(hr,min,sec,day,mnth,yr);*/
//timer.start(2000);
System.Timer();
}

void loop() 
{
/*
  while(!flag)
  {
    if(timer.justFinished())
  {
    Serial.println("Timer Finished");
    digitalWrite(13,HIGH);
    flag=true;

  }
  else
  {
     Serial.println("Check for input");
     digitalWrite(13,LOW);
  }
  }
  if(!timer.isRunning())
 {
  Serial.println("Timer is done"); 
  Serial.println("Restarting Timer");
  delay(1000);
  timer.restart();
  flag=false;
 }*/

}
