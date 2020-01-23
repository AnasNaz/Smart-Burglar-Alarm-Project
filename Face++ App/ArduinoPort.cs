using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Contribution to Code
//Anas Nazha
//Adding Serial Port communication to send data to Arduino
using System.IO.Ports;

namespace Facial_Recognition_Smart_Alarm
{
    static class SerialCommunication //Static class allows acces for all functions in the Form1.cs code 
    {
        public static SerialPort _port;
    }
}
