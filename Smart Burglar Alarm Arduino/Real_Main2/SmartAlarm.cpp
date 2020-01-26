#include "SmartAlarm.h"
#include "HardwareSerial.h"
#include "Arduino.h"
#include "WString.h"

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
	//this->run();
}

//Checks whether the systme is idle or acive
bool SmartAlarm::CheckSystemState()
{
	//Retrieve Sensor information 
  if(this->_status == false)
  {
  this->GetMagnetData(); //Get window status 
  //this->GetSolenoidData(); //Get door status 
  this->GetPIRData(); //Get motion sensor status 
  }
	
}

//Verifies User login by asking which method is desired: pin verification method or facial verification method
void SmartAlarm::VerifyUser()
{
  digitalWrite(13, LOW);
	Serial.println(START_AUTHENTICATION_INTERFACE);
	int FaceAppData;
	//Wait for Serial Data from the Face++ App
	while (Serial.available() == 0)
	{

	}
	FaceAppData = Serial.read();
 // Serial.println(FaceAppData);//FaceAppData == 80
	if (FaceAppData==PIN_METHOD)  //User has chosen Pin verification
	{
   // Serial.println("CHOSEN P");
		this->type_of_verification_entered = PIN_VERIFICATION;
		this->PinVerification(); // verifies User Pin
	}
	else if (FaceAppData == FACIAL_METHOD) //User has chosen Facial verification
	{
    //Serial.println("CHOSEN F");
		this->type_of_verification_entered = FACIAL_VERIFICATION;
		this->FacialVerification();
	}
}
/*Listens for Face++ App requestfor user logins
  Reads the 2D array which holds user logins with corresponding name, time stamp and date that is stored in the system.*/
void SmartAlarm::GetLoginRecord()
{
  String FaceAppData; //Variable to hold data from the Face++ App
  while(Serial.available()==0)
  {
    
  }
  FaceAppData=Serial.readString();
  if(FaceAppData==RECORDED_LOGINS_REQUESTED) //If User from Face++ App requests to see recorded logins, then send the data
  {
    Serial.println("Smart Alarm Logins");
    Serial.println("Name\tDate");
    for(int i=0;i<ALLOWED_USERS_SIZE;i++) //Loops through allowed users size and prints recorded logins
    {
      Serial.print(recordedLogins[i][0]);Serial.print("\t");Serial.print(recordedLogins[i][1]);Serial.print("\n");
      if(recordedLogins[i][0]=="")//no more names
        {
          Serial.println("End of Logs");
          return;
        }
    }
  }
  else //Else no request sent
  {
    return;
  }

}

//Checks whether CurrentUser has a double verification security, i.e Pin and Facial verification
void SmartAlarm::CheckAuthenticationMethod()
{
  //Serial.println(this->CurrentUser.GetAuthenticationMethod());
  if(this->num_of_verifications_done==1) //1 correct verification, check if user chose only pin or only facial
  {
      if (this->CurrentUser.GetAuthenticationMethod() == PIN_VERIFICATION || this->CurrentUser.GetAuthenticationMethod() == FACIAL_VERIFICATION)
    {
      //User allowed, send something to the app and record login
     // Serial.println("All methods checked! Welcome!");
      Serial.println(ALL_METHODS_CHECKED);
      this->recordLogin();
      digitalWrite(13, HIGH);
      this->GetLoginRecord();
      this->num_of_verifications_done=0; //Reset number of verifications for a new user
    }
  else if (this->CurrentUser.GetAuthenticationMethod() == BOTH)//User chose both methods, thus check which method was entered and tell the app to request the other input
    {
      if (this->type_of_verification_entered == FACIAL_VERIFICATION) //User inputted facial verification, thus request PIN from app
      {
        //Serial.println("You still have pin verification");
        Serial.println(PIN_VERIFICATION);
        this->PinVerification();
      }
      else if(this->type_of_verification_entered == PIN_VERIFICATION) //User inputted pin verification, thus request facial from app)
      {
       // Serial.println("You still have facial verification");
        Serial.println(FACIAL_VERIFICATION);
        this->FacialVerification();
      }
    }
 }
 else if(this->num_of_verifications_done==2)//2 correct verifications, thus double security satisfied as desired in the user's account
 {
    //User allowed, send something to the app and record login
    Serial.println(ALL_METHODS_CHECKED);
    Serial.println("All methods checked! Welcome!");
    this->recordLogin();
    digitalWrite(13, HIGH);
    this->GetLoginRecord();
    this->num_of_verifications_done=0;
 }

	
}

//Triggers sequence of events to sound the alarm that intruder detected. This method calls the notificationLED, notificationBuzzer and prints aler message on serial monitor
void SmartAlarm::triggerAlarm()
{
  Serial.println("ALARM TRIGGERED");
	this->notificationBuzzer();
	this->notificationLED();
  for(int i=0;i<5;i++)
  {
    digitalWrite(13, HIGH);
    delay(500);
    digitalWrite(13, LOW);
    delay(500);
  }
}

//Verifies Pin.
//Include some logic to check the name if successful pin
void SmartAlarm::PinVerification()
{
	String Entered_Pin;//name variable holds the name of the user identified from the Face++ App
	//Wait for Serial Data from the Face++ App
  //Serial.println("\nEntered PIN verification METHOD!");
	while (Serial.available() == 0)
	{

	}
 Entered_Pin = Serial.readString();
 //Serial.println("You entered: " + Entered_Pin);

	//Verify Pin with allowed users

	for (int i = 0;i < ALLOWED_USERS_SIZE;i++)
	{
		if (this->Allowed[i].GetPIN() == Entered_Pin) //if entered Pin matches one the PINS saved then assign Current User as the User Saved in the system
		{
      Serial.println(VERIFICATION_TRUE);
      this->num_of_verifications_done++;
			this->CurrentUser=this->Allowed[i]; //Get Current User's Name
      Serial.println(this->CurrentUser.GetName()); //Send the Face++ App the name of the user
      //Serial.println("Checking Authentication Method Now");
      this->CheckAuthenticationMethod();
      return;
		}
	}
    Serial.println(VERIFICATION_FALSE);
   // delay(1000);
 // If it passed the for loop, then PIN not verified, notify App
     Serial.println("Pin not identified");
     this->attempts++; //Track the number of attempts taken
    if(this->attempts <ATTEMPTS_LIMIT)
    {
      Serial.print("Try again, number of attempts left: ");Serial.print(ATTEMPTS_LIMIT - this->attempts);
      this->PinVerification();
      return;
    }
    else
    {
      this->triggerAlarm();
    }  
}
//Verifies User's Face
//Should send request to the C# Application
void SmartAlarm::FacialVerification()
{
  // Serial.println("\nEntered Facial Verification!");
	String Face_Name;//name variable holds the name of the user identified from the Face++ App
	//Wait for Serial Data from the Face++ App
	while (Serial.available() == 0)
	{

	}
	Face_Name = Serial.readString();
  //Serial.println(Face_Name);
	//Serial.println(this->CurrentUser.GetName());
  //Check if name associated with the face belongs to any saved user's name
  for (int i = 0;i < ALLOWED_USERS_SIZE;i++)
  {
    if (this->Allowed[i].GetName() == Face_Name) //if recieved Name Pin matches one the names saved then assign Current User as the User Saved in the system
    {
      this->num_of_verifications_done++;
      this->CurrentUser=this->Allowed[i]; //Get Current User's Name
      //Serial.println("You are: "+ this->CurrentUser.GetName());
      //Serial.println("Face Verified, Looking good today!:D");
      this->CheckAuthenticationMethod();
      return;
    }
  }
  //If it exits the for loop without findind a saved name then face is not identified
      Serial.println("Face Not Identified");	
      this->attempts++; //Track the number of attempts taken
    if(this->attempts <ATTEMPTS_LIMIT) //If still less than allowed attempts, ask for verification again
    {
      Serial.print("Try again, number of attempts left: ");Serial.print(ATTEMPTS_LIMIT - this->attempts);
      this->FacialVerification();
      return;
    }
    else //else trigger alarm
    {
      this->triggerAlarm();
    }
}

//Controls LEDS. Activates red LED for password error, flashes red LED if intruder detected and flashes green LED for succesful authentication
void SmartAlarm::notificationLED()
{
 // Serial.println("RED LEDS FLASHING");
 /*switch(_event)
 {
  case(NOTIFICATION_SUCCESS):
  digitalWrite(GREEN_LED,HIGH);
  break;
  
  case(NOTIFICATION_FAILURE):
  digitalWrite(RED_LED,HIGH);
  break;

  case(INTRUDER_DETECTED):
  for (int i = 0;i < 16;i++) {
      digitalWrite(RED_LED,HIGH); //Red LED flashes on and off for 30 seconds if intruder is detected
      delay(1000);
      digitalWrite(RED_LED,LOW);
      delay(1000);
    }
 break;
 default: //no event occured, do not trigger anything
 break;
 }*/ 
}
//Plays sounds for incorrect authentication, correct authentication, and alarm for intruder
void SmartAlarm::notificationBuzzer()
{
  Serial.println("ALARM BUZZER PLAYING");
}

//Records users logged in to a file. We may need an extra public function to read from that file
void SmartAlarm::recordLogin()
{
   for(int i=0;i<ALLOWED_USERS_SIZE;i++)
  {
    if(this->recordedLogins[i][0]=="") //If empty, then store Current User Name with date/time. Note, we may need to protect it if it goes out of bounds ?
    {
      this->recordedLogins[i][0]=this->CurrentUser.GetName();
      this->recordedLogins[i][1]=this->digitalClockDisplay();
      return; //Task done, so exit function
    }
  }
}
void SmartAlarm::GetMagnetData()
{
  if(digitalRead(MAGNETIC_SENSOR))
  {
    this->_event=INTRUDER_DETECTED;
  }
  else
  {
    this->_event=NO_EVENT;
  }
}
String SmartAlarm::digitalClockDisplay() {
  // Digital clock display of the time
  String TimeStamp= String(hour());
  TimeStamp.concat(":"+String(minute())+" "+String(dayShortStr(day()))+" "+String(monthShortStr(month()))+" "+String(year()));
  return TimeStamp;
}
void SmartAlarm::Timer()
{
  millisDelay Check_Input_Timer;
  millisDelay input;
  Check_Input_Timer.start(TIMER_DURATION);
  input.start(2000); //Just testing if inout will be detected in 3 seconds (2000 milliseconds should be 2 sec but here it adds 1 exra second)
  do{ if(Check_Input_Timer.justFinished())
    {
    Serial.println("Timer Finished");
    while(1)
    {
      
    }
    }
    else if(input.justFinished())
    {
      Serial.println("Input detected");
      while(1)
      {
        
      }
    }
    else
    {
      Serial.println("Still Counting");
    }
  }while(Check_Input_Timer.isRunning());
}
//Main logic
// Run function is called in main. Run function runs the main logic of the security alarm (UML Activity Diagram) so logic is not created in main but in SmartAlarm.cpp
void SmartAlarm::run()
{
 
}
