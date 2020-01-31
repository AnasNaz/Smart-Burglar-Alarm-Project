# Smart Burglar Alarm Project
Smart Burglar Alarm is an Arduino project that provides a security system with facial recognition using [Face++ API](https://www.faceplusplus.com/). For this course project, we were given a kit of sensors, which will be shown in a schematic diagram below, and stakeholder requirements.
## Schematic Diagram
![Image of Schematic Diagram](https://github.com/AnasNaz/Smart-Burglar-Alarm-Project/blob/master/Schematic.png)
## Designing Requirements
In order to successfully capture the stakeholder needs as well as **nonfunctional requirements** such as dealing with maintenance and design all of the desirable behaviors of the system, a Systems Engineering approach was taken in which we design the User and System requirements, get feedback from the stakeholder, create UML diagrams such as a **Use case** diagram and an **Activity** diagram and finally verify that the requirements are traceable. The following images illustrate the use case and activity diagrams:
![Image of Use Case Diagram](https://github.com/AnasNaz/Smart-Burglar-Alarm-Project/blob/master/SMART%20Burglar%20System-%20Use-Case%20Diagram.png)
![Image of Activity Diagram](https://github.com/AnasNaz/Smart-Burglar-Alarm-Project/blob/master/UML%20Activity%20Diagram%20Example%20-%20Revised%203.0.png)
## Object Oriented Programming
For managing the software, I have introduced Object Oriented Programming to the team as I knew that simple procedural programming will be harder to work with and organize especially since some functions will relate only to the user, some only to the Smart Alarm and the remaining are for the facial recognition; thus it felt suitable to utilize concepts of OOP such as abstraction and encapsulation to create two classes that holds all the appropriate attributes and methods: one class called User and another class called SmartAlarm for the system. The following image is a UML class diagram that better illustrates how the code is organized.
![Image of CLass Diagram](https://github.com/AnasNaz/Smart-Burglar-Alarm-Project/blob/master/UML%20Class%20Diagram%20Smart%20Burglar%20Alarm%20-%20UML%20CLASS%20DIAGRAM%202.0.png)
#### Note: In order to send data from the Arduino to the facial recognition software, I had to study C# and implement Serial Communication through the USB port, process the data by trimming whitespaces as it was causing issues and finally set a terminating character to be the line feed to know when the information was done.
## Creating the Application
The application is inspired by another [Arduino project](https://create.arduino.cc/projecthub/divinsmathew/smart-door-with-face-unlock-273e06), which has created the layout of the Winforms application. However, major contributions have been added to the source code to tailor the application for this project: estabilishing serial communication with exception handling and data processing, auto detecting if port is connected and informing the user, automatic startup of camera when needed, and recording a history of logins as well as events where intruder was detected and by which sensor.

![Splash Screen](https://github.com/AnasNaz/Smart-Burglar-Alarm-Project/blob/master/Face%20%2B%2B%20App%20Splash%20Screen.png)
![Main View of Face++ App](https://github.com/AnasNaz/Smart-Burglar-Alarm-Project/blob/master/Face%2B%2B%20App%20Main%20View.PNG)
## Demo
This GIF shows a scenario where the user has chosen double verification method: Facial verification and Pin verification.
![Demo 1](https://github.com/AnasNaz/Smart-Burglar-Alarm-Project/blob/master/Face-App-demo_1.gif)

This GIF shows a scenario where an **intruder** is detected and the event recorded in the system for authorized users to view.
![Demo 2](https://github.com/AnasNaz/Smart-Burglar-Alarm-Project/blob/master/Face-App-demo-2.gif)
**This application interfaces with Arduino to control the hardware which the console shows such as the red and green leds, alarm buzzer and door solenoid**
#### Note: To view them with audio (as there is a speech assistant talking) please check the repository as the videos are uploaded as well.
