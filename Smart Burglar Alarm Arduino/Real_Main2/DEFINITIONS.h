#pragma once
/*
	Description: DEFINITIONS.h is a header file that defines useful variables that will be available throughout the whole program such as
				 the time delay to wait for input from user, the data that will be sent to the Face++ App, the types of data that Face++ App 
				 will send back and what they mean, etc.
	Purpose:	 Greatly improves readability and allows for easy adjustments to such variables.
*/

/*Capacity of Users Allowed*/
#define ALLOWED_USERS_SIZE 5

/*Number of Attempts Allowed*/
#define ATTEMPTS_LIMIT 3

/*Serial Communication Variables*/
#define ALL_METHODS_CHECKED "D"
#define VERIFICATION_FALSE "0"
#define VERIFICATION_TRUE "1"
#define START_AUTHENTICATION_INTERFACE "S"

//Avoiding Strings for communication as they are turning into ASCII values and integers make the program run faster
//Using int values for now
#define PIN_METHOD 80 //80 in ASCII is P, which will be entered by the user
#define FACIAL_METHOD 70 //70 in ASCII is F, which will be entered by the user
#define RECORDED_LOGINS_REQUESTED "R" //Character sent from the Face++ App that requests the recorded logins
#define CONTINUE_DETECTING_USERS "C"  //Character sent to the Face++ App so it continues searching for users
#define TRIGGER_ALARM 65 //Face++ App will send "A" or 65 in ASCII Code to notify arduino that input was not provided within the alotted time so trigger alarm (Face++ app triggers this event as the check is done on the app)
#define ALARM_TRIGGERED "A" //Arduino will send "A" to Face++ to inform that alarm has been triggered

/*Events
  This helps the system identify what has occured in terms of enetering authentication (failure or success), and consequently if it is an intruder or no event
  in the case it was an authorized user
*/
#define NOTIFICATION_FAILURE 99
#define NOTIFICATION_SUCCESS 98
#define INTRUDER_DETECTED 97
#define NO_EVENT 96

/*Variables for CheckAuthentication Method*/
#define FACIAL_VERIFICATION "F"
#define PIN_VERIFICATION "P"
#define BOTH "B" 

/*PIN DEFINITIONS*/
#define BUZZER 2
#define PIR 1
#define MAGNETIC_SENSOR 4
#define SOLENOID 3
#define GREEN_LED 5
#define RED_LED 6
#define BUTTON_PIR_BACKUP 7

/*Timer Duration in milliseconds //Due to serial communication problems with the timer, the timer is now implemented on C# where it is actually witing for user input 
#define TIMER_DURATION 30000*/
