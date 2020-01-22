#include "SmartAlarm.h"
/*SmartAlarm.cpp includes the function declaration
All methods appear in order as defined in the header file except void run() which is the last method
*/


//Takes in an array of users. Maybe initialize size to avoid any issue ?
SmartAlarm::SmartAlarm(User Allowed[])
{

}

//Checks whether the systme is idle or acive
bool SmartAlarm::CheckSystemState()
{

}

//Verifies User login by calling pin verification method or facial verification method or both
string SmartAlarm::VerifyUser()
{

}

//Reads file that holds user logins with corresponding name, time stamp and date.
void SmartAlarm::GetLoginRecord()
{

}

//Checks whether Pin only, Facial only or both
string SmartAlarm::CheckAuthenticationMethod()
{

}

//Triggers sequence of events to sound the alarm that intruder detected. This method calls the notificationLED, notificationBuzzer and prints aler message on serial monitor
void SmartAlarm::triggerAlarm()
{

}

//Verifies Pin.
//Include some logic to check the name if successful pin
bool SmartAlarm::PinVerification()
{

}

//Verifies User's Face
//Should send request to the C# Application
bool SmartAlarm::FacialVerification()
{

}

//Controls LEDS. Activates red LED for password error, flashes red LED if intruder detected and flashes green LED for succesful authentication
void SmartAlarm::notificationLED()
{

}
//Plays sounds for incorrect authentication, correct authentication, and alarm for intruder
void SmartAlarm::notificationBuzzer()
{

}

//Records users logged in to a file. We may need an extra public function to read from that file
void SmartAlarm::recordLogin()
{

}


//Main logic
// Run function is called in main. Run function runs the main logic of the security alarm (UML Activity Diagram) so logic is not created in main but in SmartAlarm.cpp
void SmartAlarm::run()
{

}
