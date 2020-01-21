using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Facial_Recognition_Smart_Alarm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
       // static SerialPort _port;
        static void Main()
        {
            Application.EnableVisualStyles();
             Application.SetCompatibleTextRenderingDefault(false);
             Application.Run(new Form1());
          /*  _port = new SerialPort();
            _port.PortName = "COM3";
            _port.BaudRate = 9600;
            _port.Open();
            while(true)
            {
                string a = _port.ReadExisting();
                Console.WriteLine(a);
                _port.Write("1");
                Thread.Sleep(1000);
                _port.Write("0");
                Thread.Sleep(1000);
            }*/
        }

    }
}
