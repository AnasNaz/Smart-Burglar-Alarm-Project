#include "User.h" 
#include "DEFINITIONS.h" 
#include <millisDelay.h> //Provides a non-disturbing timer. Unlike delay, it allows other statements to be executed
#include <TimeLib.h> //Provides a time stamp for recording logins
/*
Name: SmartAlarm.h
Description: Defines the appropriate attributes and methods required for the Security Alarm System.
Author: Anas Nazha.
Note: Function types and arguments may change when necessary.

*/
#ifndef SMARTALARM_H
#define SMARTALARM_H
class SmartAlarm
{
public:
  SmartAlarm(User users[]); //Takes in an array of users. Maybe initialize size to avoid any issue ? 
  bool CheckSystemState(); //Checks whether the systme is idle or acive 
  void VerifyUser(); //Verifies User login by calling pin verification method or facial verification method or both 
  void GetLoginRecord(); //Reads file that holds user logins with corresponding name, time stamp and date. 
  void run(); // Run function is called in main. Run function runs the main logic of the security alarm (UML Activity Diagram) so logic is not created in main but in SmartAlarm.cpp 
  void Timer();
private:
  int _event; //Events are special cases such as notifcation failure, notification success, or intruder alert that help the Buzzer and LED functions appropriately repsond to each event
  String type_of_verification_entered;// Type of verification entered by Current User, F or P
  User Allowed[ALLOWED_USERS_SIZE]; //Will hold the arrays of users sent through constructor function SmartAlarm(User Allowed[])
  User CurrentUser;//Current User is a user who is currently trying to log in
  bool _status; //System active (true) or not (false) 
  void CheckAuthenticationMethod(); //Checks whether Pin only, Facial only or both 
  void triggerAlarm(); //Triggers sequence of events to sound the alarm that intruder detected. This method calls the notificationLED, notificationBuzzer and prints aler message on serial monitor 
  void PinVerification(); //Verifies Pin 
  void FacialVerification(); //Verifies User's Face 
  void notificationLED();//Controls LEDS. Activates red LED for password error, flashes red LED if intruder detected and flashes green LED for succesful authentication 
  void notificationBuzzer();//Plays sounds for incorrect authentication, correct authentication, and alarm for intruder 
  void recordLogin();//Records users logged in to a file. We may need an extra public function to read from that file 
  void GetMagnetData();//Reads the status of the magnetic switch & changes _status variable to idle (false) or active (true). When active, it immediately triggers the alarm as stated in the designed requirements. 
  void GetPIRData(); //Detects movement through the motion sensor & changes _status variable to idle (false) or active (true). 
  void GetSolenoidData();//Detects if door is locked or unlocked & changes _status variable to idle (false) or active (true). 
  int num_of_verifications_done=0; //Keeps track of succesful verifications that helps CheckAuthenticationMethod verify if methods are complete
  int attempts=0; //Records the number of attempts allowed if authentication error occurs. If it exceeds a certain number of attempts (Defined in DEFINITIONS.H) then sound the alarm
  String recordedLogins[ALLOWED_USERS_SIZE][2]; //Records the correct user logins along with a time stamp
  String digitalClockDisplay(); //Returns a string of the current time to record Logins
  //void Timer();//Starts a timer with a custom duration set in the DEFINITIONS.h. This helps in checking if input is entered within a certain time.
};
#endif
