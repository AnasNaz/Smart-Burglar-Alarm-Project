#include "SmartAlarm.h"
#include "HardwareSerial.h"
#include "Arduino.h"
#include "WString.h"

/*SmartAlarm.cpp includes the function declaration
All methods appear in order as defined in the header file except void run() which is the last method
*/

//Takes in an array of users.
SmartAlarm::SmartAlarm(User users[])
{
  this->_status=false; //System not active on initialization
	//Initializes allowed users 
	for (int i = 0;i < ALLOWED_USERS_SIZE;i++)
	{
	  this->Allowed[i] = users[i];
	}
	//Run the system 
	this->run();
}

//Checks whether the systme is idle or acive
void SmartAlarm::CheckSystemState()
{
	//Retrieve Sensor information 
  this->GetMagnetData(); //Get window status  
  this->GetPIRData(); //Get motion sensor status 	
}

//Verifies User login by asking which method is desired: pin verification method or facial verification method
void SmartAlarm::VerifyUser()
{
  this->attempts=0;
	Serial.println(START_AUTHENTICATION_INTERFACE);
	int FaceAppData;
	//Wait for Serial Data from the Face++ App
	while (Serial.available() == 0)
	{

	}
  FaceAppData = Serial.read();	
  if(FaceAppData==TRIGGER_ALARM) //If face app sends "A" that means user did not input in alotted time, thus trigger alarm
  {
    this->triggerAlarm();
    return; //Exit function
  }
	if (FaceAppData==PIN_METHOD)  //User has chosen Pin verification
	{
		this->type_of_verification_entered = PIN_VERIFICATION;
		this->PinVerification(); // verifies User Pin
	}
	else if (FaceAppData == FACIAL_METHOD) //User has chosen Facial verification
	{
		this->type_of_verification_entered = FACIAL_VERIFICATION;
		this->FacialVerification();
	}
}
/*Listens for Face++ App request for user logins
  Reads the 2D array which holds user logins with corresponding name, time stamp and date that is stored in the system.*/
void SmartAlarm::GetLoginRecord()
{
  String FaceAppData; //Variable to hold data from the Face++ App
  this->Timer.start(20000); //Timer to handle the event where user does not want to view recorded logins
  int flag=0; //Variable to check if user did not request logins
  while(Serial.available()==0)
  {
    if(this->Timer.justFinished()) //User did not request the data, thus break and do not show logins
    {
      flag=1; //Raise flag to indicate request timeout
      break;
    }
  }
  if(flag==0)
  {
      FaceAppData=Serial.readString();
  }
  
  if(FaceAppData==RECORDED_LOGINS_REQUESTED) //If User from Face++ App requests to see recorded logins, then send the data
  {
    Serial.println("Smart Alarm Logins");
    Serial.println("Name\tDate");
    for(int i=0;i<ALLOWED_USERS_SIZE;i++) //Loops through allowed users size and prints recorded logins
    {
      Serial.print(recordedLogins[i][0]);Serial.print("\t");Serial.print(recordedLogins[i][1]);Serial.print(recordedLogins[i][2]);Serial.print("\n");
      if(recordedLogins[i][0]=="")//no more names
        {
          Serial.println("End of User Logs");
          break; //Break and not return in order to print sensor satus for maintenance if the user is part of the maintenance team
        }
    }
    if(this->CurrentUser.Get_isMaintenance())
    {
      this->GetSensorStatus();
    }
    return;
  }
  else //Else no request sent
  {
    return; //Exit function
  }
}
//Checks whether CurrentUser has a double verification security, Pin and Facial verification, or a single verification method, Pin only or Facial only
//and accordingly informs the Face++ App
void SmartAlarm::CheckAuthenticationMethod()
{
  //if(this->_event==INTRUDER_DETECTED) //Check if intruder was detected in order to tell Face++ App to start listening for another user
 // {
  //  this->_event=NO_EVENT; //Reset the event
  //}
  //else //Else if intruder was not detected then proceed to check authentication method normally
  //{
    if(this->num_of_verifications_done==1) //1 correct verification, check if user chose only pin or only facial verification
  {
      if (this->CurrentUser.GetAuthenticationMethod() == PIN_VERIFICATION || this->CurrentUser.GetAuthenticationMethod() == FACIAL_VERIFICATION)
    {
      //User allowed, send something to the app and record login
      Serial.println(ALL_METHODS_CHECKED); //Notify Face++ App that all methods have been checked
      this->recordLogin(); //Record User Login
     // delay(8000);//Wait for the Face++ App to notify user that authentication is successful before unlocking door
      
      this->GetLoginRecord(); //Check if user requested to view logins from the Face++ App
      this->num_of_verifications_done=0; //Reset number of verifications for a new user
      this->UnlockDoor();//Unlocks door upon successful authentication
    }
  else if (this->CurrentUser.GetAuthenticationMethod() == BOTH)//User chose both methods, thus check which method was entered and tell the app to request the other input
    {
      if (this->type_of_verification_entered == FACIAL_VERIFICATION) //User inputted facial verification, thus request PIN from app
      {
        Serial.println(PIN_VERIFICATION);//Notify Face++ App that current user still has pin verification
        this->PinVerification(); //Call the pin verification method
      }
      else if(this->type_of_verification_entered == PIN_VERIFICATION) //User inputted pin verification, thus request facial from app
      {
        Serial.println(FACIAL_VERIFICATION);//Notify Face++ App that current user still has to input facial verification
        this->FacialVerification();//Call the facial verification method
      }
    }
  }
  else if(this->num_of_verifications_done==2)//2 correct verifications, thus double security satisfied as desired in the user's account
   {
    //User allowed, send something to the app and record login
      Serial.println(ALL_METHODS_CHECKED);//Notify Face++ App that all methods have been checked
      Serial.println("All methods checked! Welcome!");
      this->recordLogin();//Record User Login
      //delay(8000);//Wait for the Face++ App to notify user that authentication is successful before unlocking door//Removed as caused issues with the app
      this->GetLoginRecord();//Check if user requested to view logins from the Face++ App
      this->num_of_verifications_done=0;//Reset number of verifications for a new user
      this->UnlockDoor();//Unlocks door upon succesful authentication
   }
 //} 
}
/*Triggers sequence of events to sound the alarm that intruder detected. This method calls the notificationBuzzer, which sounds 
  the alarm, flashes the red led and prints alert message on serial monitor*/
void SmartAlarm::triggerAlarm()
{
  this->CurrentUser.SetName("INTRUDER DETECTED");//Record current user as an intruder
  this->_event=INTRUDER_DETECTED; //Change the event to intruder detected
	this->notificationBuzzer(); //Sound the alarm and flash the red led
  this->recordLogin(); //Record when was the intruder detected
  this->_event=NO_EVENT; //Reset the event
}
//Verifies Pin.
//Includes some logic to check the name of the current user if pin was successful
void SmartAlarm::PinVerification()
{
	String Entered_Pin;//name variable holds the name of the user identified from the Face++ App
	//Wait for Serial Data from the Face++ App
	while (Serial.available() == 0)
	{

	}
  Entered_Pin = Serial.readString();
  //Verify Pin with allowed users
	for (int i = 0;i < ALLOWED_USERS_SIZE;i++)
	{
	  if (this->Allowed[i].GetPIN() == Entered_Pin) //if entered Pin matches one the PINS saved then assign Current User as the User Saved in the system
		{
       Serial.println(VERIFICATION_TRUE);//Notify Face++ App that verification was true
       this->num_of_verifications_done++;//Increment number of verifications to tracks how many succesful verifications did the user do
			 this->CurrentUser=this->Allowed[i]; //Identify who is the current user with the saved users in Allowed
       Serial.println(this->CurrentUser.GetName()); //Send the Face++ App the name of the user
       this->_event=NOTIFICATION_SUCCESS; //Change the event to notification success so the notificationLED() method would know to turn on the Green led
       this->notificationLED();//Trigger notification led
       this->CheckAuthenticationMethod();//Check if user still has another authentication method
       return;
		}
	}
  //If it passed the for loop, then PIN not verified
    this->_event=NOTIFICATION_FAILURE;//Change the event to notification failure so the notificationLED() method would know to turn on the RED led
    this->notificationLED();//Trigger notification led
    this->attempts++; //Track the number of attempts taken
    
    if(this->attempts <ATTEMPTS_LIMIT) //Attempts still less than what is allowed, then notify the Face++ App
    {
       Serial.println(VERIFICATION_FALSE);
       Serial.println("Pin not identified");
       Serial.print("Try again, number of attempts left: ");Serial.print(ATTEMPTS_LIMIT - this->attempts);Serial.print("\n");
       this->PinVerification();
       return;
    }
    else //Attempts limit reached, notify the Face++ App
    {
       Serial.println(ALARM_TRIGGERED);
       this->triggerAlarm();
    }  
}
//Verifies User's Face
void SmartAlarm::FacialVerification()
{
	String Face_Name;//name variable holds the name of the user identified from the Face++ App
	//Wait for Serial Data from the Face++ App
	while (Serial.available() == 0)
	{

	}
	Face_Name = Serial.readString(); //Read the data sent from the Face++ App
  //Check if name associated with the face belongs to any saved user's name
  for (int i = 0;i < ALLOWED_USERS_SIZE;i++)
  {
    if (this->Allowed[i].GetName() == Face_Name) //if recieved name matches one of the names saved in the sysem, then assign Current User as the User Saved in the system
    {
      this->num_of_verifications_done++;//Increment number of verifications to tracks how many succesful verifications did the user do
      this->CurrentUser=this->Allowed[i];//Identify who is the current user with the saved users in Allowed
      this->_event=NOTIFICATION_SUCCESS;//Change the event to notification success so the notificationLED() method would know to turn on the Green led
      this->notificationLED();//Trigger notification led
      this->CheckAuthenticationMethod();//Check if user still has another authentication method
      return;//exit
    }
  }
    //If it exits the for loop without finding a saved name then face is not identified
      this->_event=NOTIFICATION_FAILURE;//Change the event to notification failure so the notificationLED() method would know to turn on the RED led
      this->notificationLED();//Trigger notification led
      Serial.println("Face Not Identified");	
      this->attempts++; //Track the number of attempts taken
    if(this->attempts <ATTEMPTS_LIMIT) //If still less than allowed attempts, ask for verification again
    {
      Serial.print("Try again, number of attempts left: ");Serial.print(ATTEMPTS_LIMIT - this->attempts);Serial.print("\n");
      this->FacialVerification();
      return;
    }
    else //else trigger alarm
    {
      this->triggerAlarm();
    }
}
//Controls LEDS. Activates red LED for password error or green LED for successful authentication
void SmartAlarm::notificationLED()
{
 switch(_event)
 {
  case(NOTIFICATION_SUCCESS): //Turn on the green LED for one second
    digitalWrite(GREEN_LED,HIGH);
    delay(1000);
    digitalWrite(GREEN_LED,LOW);
    this->_event=NO_EVENT;
    break;
  case(NOTIFICATION_FAILURE)://Turn on the red LED for one second
    digitalWrite(RED_LED,HIGH);
    delay(1000);
    digitalWrite(RED_LED,LOW);
    this->_event=NO_EVENT;
    break;
  default: //no event occured, do not trigger anything
    break;
 } 
}
//Activates buzzer alarm and flashes red LED upon intruder detection
void SmartAlarm::notificationBuzzer()
{
  if(this->_event==INTRUDER_DETECTED)
  {
    this->Timer.start(8000); //This runs for 16 seconds, the for loop adds to the timer
    do
    {
      //Flashes red led and plays a small alarm tune
      digitalWrite(RED_LED,HIGH);
      for(int hz = 440; hz < 1000; hz+=25)
        {
          tone(BUZZER, hz, 50);
          delay(5);
          digitalWrite(RED_LED,LOW);
        }
      for(int hz = 1000; hz > 440; hz-=25)
        {
          tone(BUZZER, hz, 50);
          delay(5);
        }
     }while(!this->Timer.justFinished());
    noTone(BUZZER);
  }
}

//Records users logged in to a 2D array with a time stamp
void SmartAlarm::recordLogin()
{
    for(int i=0;i<ALLOWED_USERS_SIZE;i++)
  {
    if(this->recordedLogins[i][0]=="") //If empty, then store Current User Name with date/time. Note, we may need to protect it if it goes out of bounds ?
    {
      this->recordedLogins[i][0]=this->CurrentUser.GetName();
      this->recordedLogins[i][1]=this->digitalClockDisplay();
      
       if(CurrentUser.Get_isMaintenance()) //If user is part of the maintenance team then indicate Maintenance login
      {
        this->recordedLogins[i][2]="  Maintenance Login";
      }
      return; //Task done, so exit function
    }
  }
}
//Gets data from the magnetic sensor and immediatley triggers alarm if it goes HIGH (as designed in the requirements)
void SmartAlarm::GetMagnetData()
{
    if(digitalRead(MAGNETIC_SENSOR))
  {
    Serial.println(ALARM_TRIGGERED);//Tell the Face++ App that alarm has been triggered
    this->_event=INTRUDER_DETECTED;
    this->triggerAlarm();
  }
  else
  {
    this->_event=NO_EVENT;
  }
  this->RecordSensorStatus();
}
void SmartAlarm::GetPIRData() //Get data from PIR sensor. NOTE: we are implementing a button as a backup, as the PIR data sensor is unreliable
{
  if(digitalRead(BUTTON_PIR_BACKUP)) //If PIR sensor is HIGH then system is active and change status to true
  {
    this->_status=true;
    this->RecordSensorStatus();
  }
}
void SmartAlarm::UnlockDoor() //Unlocks door for a short period than locks it again
{
  this->Timer.start(3000);
  digitalWrite(SOLENOID,HIGH);
  while(this->Timer.isRunning())
  {
    if(this->Timer.justFinished())
    {
      digitalWrite(SOLENOID,LOW);
      return;
    }
  }
}
//Provide a string of the current time
String SmartAlarm::digitalClockDisplay() 
{
  // Digital clock display of the time
  String TimeStamp= String(hour());
  TimeStamp.concat(":"+String(minute())+" "+String(dayShortStr(day()))+" "+String(monthShortStr(month()))+" "+String(year())); 
  return TimeStamp;
}
void SmartAlarm::RecordSensorStatus()//Records sensor status for Maintenance team
{
    for(int i=0;i<6;i++)
  {
    if(this->recordedLogins[i][0]=="") //If empty, then store sensor information.
    {
      if(this->_status)// Status goes true only when PIR sensor detects motion, thus record when the PIR motion sensor went high
        {
        this->sensorStatus[i][0]="PIR Motion Sensor HIGH";
        }
      else if(this->_event==INTRUDER_DETECTED) //Else intruder detected through magnetic sensor
        {
        this->sensorStatus[i][0]="Magnetic Sensor HIGH";
        }
       this->sensorStatus[i][1]=this->digitalClockDisplay();
       return;
    }  
  }
}
void SmartAlarm::GetSensorStatus()//Gets sensor status for Maintenance team
{
  Serial.println("Sensor\tTime");
     for(int i=0;i<6;i++) //Loops through allowed users size and prints recorded logins
    {
      Serial.print(sensorStatus[i][0]);Serial.print("\t");Serial.print(sensorStatus[i][1]);Serial.print("\n");
      if(recordedLogins[i][0]=="")//no more names
        {
          Serial.println("End of Sensor Logs");
          return;
        }
    }
}
//Main logic
//Run function runs the main logic of the security alarm (UML Activity Diagram) so logic is not created in main but in SmartAlarm.cpp
void SmartAlarm::run()
{
  while(1)
  {
   // Serial.println(digitalRead(7));
    this->CheckSystemState(); //Check system state
    if(this->_status) //if system is active (true) then verify user (the 30 second timer starts on the Face++ App, it will notify arduino if no input recieved within 30sec)
    {
      this->VerifyUser();
      this->_status=false; //reset system status to idle (false)
    }
    else
    {
      Serial.println("N");//This is just for debugging puposes for the Face++ App as there are issues with buffer in serial communication
    }
    delay(2000);
  }
}
