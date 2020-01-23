#include "SmartAlarm.h"
/*SmartAlarm.cpp includes the function declaration
All methods appear in order as defined in the header file except void run() which is the last method
*/


//Takes in an array of users. Maybe initialize size to avoid any issue ?
SmartAlarm::SmartAlarm(User users[])
{
	//Initializes allowed users 
	for (int i = 0;i < 5;i++)
	{
		this->Allowed[i] = users[i];
	}
	//Run the system 
	this->run();
}

//Checks whether the systme is idle or acive
bool SmartAlarm::CheckSystemState()
{
	//Retrieve Sensor information 
	this->GetMagnetData(); //Get window status 
	this->GetSolenoidData(); //Get door status 
	this->GetPIRData(); //Get motion sensor status 
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
	/*Sends a signal, for now "1", to the Face++ App
  Face++ App responds by unlocking the Start button and asks the user to take a photo.
  Verification results are sent back to the Arduino with "true" being succesfull and "false" otherwise*/
	string receivedData = NULL; //String that receives data from the app 
	//delay(100); UNCOMMENT when in arduino IDE 
	//Serial.println(REQUEST_FACIAL_VERIFICATION); 
	while (Serial.available() > 0)
	{
		receivedData = Serial.readStringUntil("\n"); //Might need to filter incoming data 
		if (receivedData == FACIAL_VERIFICATION_FALSE)
		{
			return false;
		}
		else if (receivedData == FACIAL_VERIFICATION_TRUE)
		{
			return true;
		}
	}
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
