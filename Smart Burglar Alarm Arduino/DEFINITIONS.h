#pragma once
/*
	Description: DEFINITIONS.h is a header file that defines useful variables that will be available throughout the whole program such as
				 the time delay to wait for input from user, the data that will be sent to the Face++ App, the types of data that Face++ App 
				 will send back and what they mean, etc.
	Purpose:	 Greatly improves readability and allows for easy adjustments to such variables.
*/

//Capacity of Users Allowed
#define ALLOWED_USERS_SIZE 5

//Serial Communication Variables
#define ALL_METHODS_CHECKED ="D"
#define FACIAL_VERIFICATION_FALSE "0"
#define START_AUTHENTICATION_INTERFACE "S"

//Events
#define NOTIFICATION_FAILURE 99
#define NOTIFICATION_SUCCESS 98
#define INTRUDER_DETECTED 97
#define NO_EVENT 96
#define FACIAL_VERIFICATION "F"
#define PIN_VERIFICATION "P"

//Variables for CheckAuthentication Method
#define PIN_ONLY "k"
#define FACIAL_ONLY "j"
#define BOTH "B"