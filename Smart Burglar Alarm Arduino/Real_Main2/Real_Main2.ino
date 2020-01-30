#include <Arduino.h>
#include <WString.h>
#include "SmartAlarm.h"
#include "User.h"

void setup() {
//Declaring the inputs and outputs of the hardware modules
pinMode(BUTTON_PIR_BACKUP,INPUT);//this has replaced the PIR module
//pinMode(PIR,INPUT); BUTTON_PIR_BACKUP has replaced this pinMode
pinMode(MAGNETIC_SENSOR,INPUT_PULLUP);
pinMode(BUZZER,OUTPUT);
pinMode(SOLENOID,OUTPUT);
pinMode(RED_LED, OUTPUT);
pinMode(GREEN_LED, OUTPUT);
Serial.begin(9600);
//Creating Users for the system
//Two ways of creating a user: Create user then use the getter and setter methods of User.h 
//                             or simply pass in arguments to the constructor: User(String name,String Pin,String security,bool maintenance)                   
User Anas;
Anas.SetName("Anas");
Anas.SetPin("123");
Anas.SetAuthenticationMethod("B");
Anas.Set_isMaintenance(false);
//Or this way
User Francesco("Francesco","456","P",true);
User Saud("Saud","789","P",false);
User N("Nihar","000","P",false);
User UserArray[4];//Create a user array to store the created users
UserArray[0]=Anas;
UserArray[1]=Francesco;
UserArray[2]=Saud;
UserArray[3]=N;
setTime(12,46,0,2,1,2020); //setTime(hr,min,sec,day,mnth,yr);
SmartAlarm System(UserArray);//Initialize the system and pass in the allowed users
}
  
void loop() 
{ 
//Nothing is implemented here as SmartAlarm and User objects will not be defined in this scope
//Please refer to the void run() method of SmartAlarm.h to view the logic of the code
//Note that the run() method is called upon initialization from the constructor
}
