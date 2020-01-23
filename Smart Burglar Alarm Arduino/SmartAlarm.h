#pragma once 
#include "User.h" 
#include "DEFINITIONS.h" 
/*
Name: SmartAlarm.h
Description: Defines the appropriate attributes and methods required for the Security Alarm System.
Author: Anas Nazha.
Note: Function types and arguments may change when necessary.

*/

class SmartAlarm
{
public:
	SmartAlarm(User users[]); //Takes in an array of users. Maybe initialize size to avoid any issue ? 
	bool CheckSystemState(); //Checks whether the systme is idle or acive 
	string VerifyUser(); //Verifies User login by calling pin verification method or facial verification method or both 
	void GetLoginRecord(); //Reads file that holds user logins with corresponding name, time stamp and date. 
	void run(); // Run function is called in main. Run function runs the main logic of the security alarm (UML Activity Diagram) so logic is not created in main but in SmartAlarm.cpp 
private:
	User Allowed[5]; //Will hold the arrays of users sent through constructor function SmartAlarm(User Allowed[]) 
	bool _status; //System active (true) or not (false) 
	string CheckAuthenticationMethod(); //Checks whether Pin only, Facial only or both 
	void triggerAlarm(); //Triggers sequence of events to sound the alarm that intruder detected. This method calls the notificationLED, notificationBuzzer and prints aler message on serial monitor 
	bool PinVerification(); //Verifies Pin 
	bool FacialVerification(); //Verifies User's Face 
	void notificationLED();//Controls LEDS. Activates red LED for password error, flashes red LED if intruder detected and flashes green LED for succesful authentication 
	void notificationBuzzer();//Plays sounds for incorrect authentication, correct authentication, and alarm for intruder 
	void recordLogin();//Records users logged in to a file. We may need an extra public function to read from that file 
	void GetMagnetData();//Reads the status of the magnetic switch & changes _status variable to idle (false) or active (true). When active, it immediately triggers the alarm as stated in the designed requirements. 
	void GetPIRData(); //Detects movement through the motion sensor & changes _status variable to idle (false) or active (true). 
	void GetSolenoidData();//Detects if door is locked or unlocked & changes _status variable to idle (false) or active (true). 
};

