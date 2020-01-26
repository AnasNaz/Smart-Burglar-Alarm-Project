#include "SmartAlarm.h"
/*SmartAlarm.cpp includes the function declaration
All methods appear in order as defined in the header file except void run() which is the last method
*/


//Takes in an array of users. Maybe initialize size to avoid any issue ?
SmartAlarm::SmartAlarm(User users[])
{
	//Initializes allowed users 
	for (int i = 0;i < ALLOWED_USERS_SIZE;i++)
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

//Verifies User login by asking which method is desired: pin verification method or facial verification method
void SmartAlarm::VerifyUser()
{
	
	Serial.println(START_AUTHENTICATION_INTERFACE);
	string FaceAppData;
	//Wait for Serial Data from the Face++ App
	while (Serial.available() == 0)
	{

	}
	FaceAppData = Serial.read();
	if (FaceAppData == "P")  //User has chosen Pin verification
	{
		this->type_of_verification_entered = PIN_VERIFICATION;
		this->PinVerification(); // verifies User Pin
	}
	else if (FaceAppData == "F") //User has chosen Facial verification
	{
		this->type_of_verification_entered = FACIAL_VERIFICATION;
		this->FacialVerification();
	}
}

//Reads file that holds user logins with corresponding name, time stamp and date.
void SmartAlarm::GetLoginRecord()
{

}

//Checks whether CurrentUser has a double verification security, i.e Pin and Facial verification
string SmartAlarm::CheckAuthenticationMethod()
{
	if (this->CurrentUser.GetAuthenticationMethod() == PIN_ONLY || this->CurrentUser.GetAuthenticationMethod() == FACIAL_ONLY)
	{
		//User allowed, send something to the app and record login
		Serial.println(ALL_METHODS_CHECKED);
		this->recordLogin();
	}
	else if (this->CurrentUser.GetAuthenticationMethod() == BOTH)//User chose both methods, thus check which method was entered and tell the app to request the other input
	{
		if (this->type_of_verification_entered == FACIAL_VERIFICATION) //User inputted facial verification, thus request PIN from app
		{
			Serial.println(PIN_VERIFICATION);
		}
		else if(this->type_of_verification_entered == FACIAL_VERIFICATION) //User inputted pin verification, thus request facial from app)
		{
			Serial.println(FACIAL_VERIFICATION);
		}
	}
	
}

//Triggers sequence of events to sound the alarm that intruder detected. This method calls the notificationLED, notificationBuzzer and prints aler message on serial monitor
void SmartAlarm::triggerAlarm()
{
	this->notificationBuzzer();
	this->notificationLED();
}

//Verifies Pin.
//Include some logic to check the name if successful pin
bool SmartAlarm::PinVerification()
{
	String Entered_Pin;//name variable holds the name of the user identified from the Face++ App
	//Wait for Serial Data from the Face++ App
	while (Serial.available() == 0)
	{

	}
	Entered_Pin = Serial.read();
	//Verify Pin with allowed users

	for (int i = 0;i < ALLOWED_USERS_SIZE;i++)
	{
		if (this->Allowed[i].GetPIN() == Entered_Pin) //if entered Pin matches one the PINS saved then assign Current User the name of the saved User
		{
			this->CurrentUser = this->Allowed[i].GetName();
		}
	}
	this->CheckAuthenticationMethod();
}

//Verifies User's Face
//Should send request to the C# Application
bool SmartAlarm::FacialVerification()
{
	//The LOGIC HAS CHANGED NOW
	/*Sends a signal, for now "1", to the Face++ App
  Face++ App responds by unlocking the Start button and asks the user to take a photo.
  Verification results are sent back to the Arduino with "true" being succesfull and "false" otherwise*/
	/*string receivedData = NULL; //String that receives data from the app 
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
	}*/
	String Face_Name;//name variable holds the name of the user identified from the Face++ App
	//Wait for Serial Data from the Face++ App
	while (Serial.available() == 0)
	{

	}
	Face_Name = Serial.read();
	if (Face_Name == "0")//Face not identified
	{

	}
	else
	{
		this->CurrentUser.SetName(Face_Name); //Set the Current User name to the name sent by the Face++ App
		this->CheckAuthenticationMethod();
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
