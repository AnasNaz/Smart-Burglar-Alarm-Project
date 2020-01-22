#pragma once
#include "User.h"
/*
Name: SmartAlarm.h
Description:
Note: Function types and arguments may change when necessary
	  User Allowed[] causing an error in type-> Francesco Google it! XD
*/

class SmartAlarm
{
public:
	SmartAlarm(User Allowed[]); //Takes in an array of users. Maybe initialize size to avoid any issue ?
	bool CheckSystemState(); //Checks whether the systme is idle or acive
	string VerifyUser(); //Verifies User login by calling pin verification method or facial verification method or both
	void GetLoginRecord(); //Reads file that holds user logins with corresponding name, time stamp and date.
	void run(); // Run function is called in main. Run function runs the main logic of the security alarm (UML Activity Diagram) so logic is not created in main but in SmartAlarm.cpp
private:
	//User Allowed[]; //Will hold the arrays of users sent through constructor function SmartAlarm(User Allowed[])
	bool _status; //System active (true) or not (false)
	string CheckAuthenticationMethod(); //Checks whether Pin only, Facial only or both
	void triggerAlarm(); //Triggers sequence of events to sound the alarm that intruder detected. This method calls the notificationLED, notificationBuzzer and prints aler message on serial monitor
	bool PinVerification(); //Verifies Pin
	bool FacialVerification(); //Verifies User's Face
	void notificationLED();//Controls LEDS. Activates red LED for password error, flashes red LED if intruder detected and flashes green LED for succesful authentication
	void notificationBuzzer();//Plays sounds for incorrect authentication, correct authentication, and alarm for intruder
	void recordLogin();//Records users logged in to a file. We may need an extra public function to read from that file
	//(CAN BE USED) - (Tentative plan) //byte pin; // Define pin for each of the component that needs to be connected to Arduino (for example: Buzzer is connected to pin 10 of Arduino)
};
