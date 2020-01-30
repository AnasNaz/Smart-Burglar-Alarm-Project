using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.Collections.Specialized;

using AForge.Video;
using AForge.Video.DirectShow;
using Newtonsoft.Json.Linq;

/*Contribution to Code 
  Anas Nazha 
  Adding Serial Port communication to send data to Arduino */
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace Facial_Recognition_Smart_Alarm
{
    public partial class Form1 : Form
    {
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);
        //Key and passowrd to access Face++ API
        private readonly string FPP_API_KEY = "zKj8xBnvrQ6MHdrhGYS2KYoFQKzTdt-X";
        private readonly string FPP_API_SECRET = "G6CvlAJVkiKQ9ZaU59hd4j_DEbKduMAX";

        //Classes for Getting and Filtering video
        VideoCaptureDevice CurrentCam;
        FilterInfoCollection VideoDeviceCollection;

        //Contribution to Code 
        //Anas Nazha 
        //All I know is that this is a thread (?) and that it monitors the status of an event 
        //which in this case is a bell ringing, and once a bell is rung then commence Facial recognition 
        //  Thread BellButtonListningThread; 
        Thread StartAuthenticationInterface_Listening_Thread;
        //String List to hold names of authorized users and image data 
        List<string> NameList;
        List<string> ImageDataList;

        bool SecurityRunning = false;
        bool StartUserInterface;
        //Initialize Windows Form
        public Form1()
        {
            //Creating a thread for the splash screen
            Thread SplashThread = new Thread(new ThreadStart(SplashStart));
            SplashThread.Start();
            Thread.Sleep(5000); //Give interval so splash screen can load completely
            InitializeComponent();
            SplashThread.Abort(); //Abort Splash Screen Thread
        }
        /*Contributed to code
          Anas Nazha
          Adding a splash screen with a loading bar
          The following function calls our splash screen*/
        public void SplashStart()
        {
            Application.Run(new Splash_Screen());
        }
        //Added Contribution to code
        //Adding Serial Port Initialization at startup 
        private void Form1_Load(object sender, EventArgs e) //Gets called when the Application starts
        {
            /*Contribution to Code 
              Anas Nazha 
              Adding Serial Port Initialization at startup */
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.SetWindowSize(Console.WindowWidth / 2, Console.WindowHeight / 2);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Smart Burglar Alarm v1.0");
            Console.SetWindowPosition(0, 0);
            //Connecting to serial port

            SerialCommunication._port = new SerialPort("COM8", 9600, Parity.None, 8, StopBits.One);
            try
            {
                SerialCommunication._port.Open();
                //Check if port is opened and updated Arduino Status on the app 
                if (SerialCommunication._port.IsOpen)
                {
                    ArduinoStatus.Text = "CONNECTED";
                    ArduinoStatus.ForeColor = Color.Green;
                    StartMonitorButton.Enabled = false;
                    RequestLogins.Enabled = false;
                    FrontDoorGB.Enabled = true;
                }
            }

            catch
            {
                ArduinoStatus.Text = "PORT NOT FOUND!";
                ArduinoStatus.ForeColor = Color.Red;
            }
            // Settings will be NULL in the first run. Create new list if so.
            NameList = Properties.Settings.Default.FaceNames ?? new List<string>(); //List that holds names of trusted faces
            ImageDataList = Properties.Settings.Default.Base64ImageData ?? new List<string>(); //List that holds base64 encoded strings of trusted face images
            VideoDeviceCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice); //Fetches camera devices connected to the system

            //Registering button click event for Remove button. 'TrustedFacesList_CellContentClick' will be called when it is pressed.
            TrustedFacesList.CellContentClick += TrustedFacesList_CellContentClick;

            for (int i = 0; i < NameList.Count; i++)
                TrustedFacesList.Rows.Add(i + 1, NameList[i], "Remove");

            if (VideoDeviceCollection.Count != 0) //Populates the dropdown menu with a list of camera devices connected to the system
            {
                foreach (FilterInfo cams in VideoDeviceCollection)
                    VideoDevices.Items.Add(cams.Name);

                VideoDevices.SelectedIndex = 0;
            }
            else //No camera device detected
            {
                MessageBox.Show("No Camera devices attached to this system. Please restart application after conneting the required device.", "Camera Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            CurrentCam = new VideoCaptureDevice(VideoDeviceCollection[VideoDevices.SelectedIndex].MonikerString);
            CurrentCam.NewFrame += new NewFrameEventHandler(NewFrame); // Start live display of feed from camera
            StartAuthenticationInterface_Listening_Thread = new Thread(new ThreadStart(Listen_For_VerifyUser_Request));
            StartUserInterface = true;
            StartAuthenticationInterface_Listening_Thread.Start();

        }
        private void TrustedFacesList_CellContentClick(object sender, DataGridViewCellEventArgs e) //Method that removes a trusted face
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                //Removing face information at specified position in the list
                NameList.RemoveAt(e.RowIndex);
                ImageDataList.RemoveAt(e.RowIndex);
                senderGrid.Rows.RemoveAt(e.RowIndex);

                //Saving the the list after removal of a face
                Properties.Settings.Default.FaceNames = NameList;
                Properties.Settings.Default.Base64ImageData = ImageDataList;
                Properties.Settings.Default.Save();

                for (int i = 0; i < senderGrid.RowCount; i++)
                    senderGrid.Rows[i].Cells[0].Value = i + 1;
            }
        }
        private void RefreshButton_Click(object sender, EventArgs e) //Refreshes the list of camera devices connected to the system
        {
            VideoDevices.Items.Clear();
            VideoDeviceCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (VideoDeviceCollection.Count != 0)
            {
                foreach (FilterInfo cams in VideoDeviceCollection)
                    VideoDevices.Items.Add(cams.Name);

                VideoDevices.SelectedIndex = 0;
            }
        }
        private void VideoDevices_SelectedIndexChanged(object sender, EventArgs e) // Gets called when user selects a new camera device from the list
        {
            if (CurrentCam != null && VideoDeviceCollection.Count != 0)
            {
                CurrentCam.Stop();
                CurrentCam = new VideoCaptureDevice(VideoDeviceCollection[VideoDevices.SelectedIndex].MonikerString);
                CurrentCam.NewFrame += new NewFrameEventHandler(NewFrame);
                CurrentCam.Start();
            }
        }
        private void NewFrame(object sender, NewFrameEventArgs eventArgs) // Method that updates picture in PictureBox during each frame
        {
            try
            {
                Monitor.Image = (Bitmap)eventArgs.Frame.Clone();
            }
            catch { }
        }

        /// <summary>
        /// Method that compares two faces
        /// </summary>
        /// <param name="face1Base64">base664 encoded string of image of face 1</param>
        /// <param name="face2Base64">base664 encoded string of image of face 2</param>
        /// <returns>Confidence percentage that two faces are similar</returns>
        public async Task<double> VerifyFace(string face1Base64, string face2Base64)
        {
            WebClient client = new WebClient();
            byte[] response = null;

            await Task.Run(delegate
            {
                response = client.UploadValues("https://api-us.faceplusplus.com/facepp/v3/compare", new NameValueCollection() //Calls API to compare faces
                                              {
                                                   { "api_key", FPP_API_KEY },
                                                   { "api_secret", FPP_API_SECRET },
                                                   { "image_base64_1", face1Base64},
                                                   { "image_base64_2", face2Base64}
                                              });
            });

            string data = "";
            try
            {
                data = JObject.Parse(System.Text.Encoding.UTF8.GetString(response))["confidence"].ToString();
            }
            catch
            {
                return 0;
            }

            return double.Parse(data);
        }
        private async void SnapFaceButton_Click(object sender, EventArgs e) //Method that detects and adds new trusted face
        {
            AddNewGB.Enabled = false;
            DecFacLabel.Visible = true;

            PreviewBox.Image = null;

            Image Snap = (Bitmap)Monitor.Image.Clone();
            WebClient client = new WebClient();
            byte[] response = null;

            await Task.Run(delegate
            {
                response = client.UploadValues("https://api-us.faceplusplus.com/facepp/v3/detect", new NameValueCollection()
                           {
                                { "api_key", FPP_API_KEY },
                                { "api_secret", FPP_API_SECRET},
                                { "image_base64", ImageToBase64(Snap)}
                           });
            }); // Checks if the captured image contains a face

            JObject data = JObject.Parse(System.Text.Encoding.UTF8.GetString(response));
            string faces = data["faces"].ToString();

            if (faces == "[]") // No face was detected
            {
                MessageBox.Show("No Face Detected. Try Again", "Invalid Picture", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FaceNameTextBox.Enabled = false;
                AddFaceButton.Enabled = false;
                NameLabel.Enabled = false;
            }
            else if (faces.Split('[').Length > 2) //Multiple faces detected
            {
                MessageBox.Show("Multiple Faces Detected. Try Again", "Invalid Picture", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FaceNameTextBox.Enabled = false;
                AddFaceButton.Enabled = false;
                NameLabel.Enabled = false;
            }
            else
            {
                //Getting the dimentions of the detected face from the API response
                int width = int.Parse(data["faces"][0]["face_rectangle"]["width"].ToString());
                int height = int.Parse(data["faces"][0]["face_rectangle"]["height"].ToString());
                int top = int.Parse(data["faces"][0]["face_rectangle"]["top"].ToString());
                int left = int.Parse(data["faces"][0]["face_rectangle"]["left"].ToString());

                //Displaying a cropped preview of the new trusted face to be added 
                PreviewBox.Image = CropImage(Snap, new Rectangle(left - 20, top - 20, width + 20, height + 20));

                FaceNameTextBox.Enabled = true;
                AddFaceButton.Enabled = true;
                NameLabel.Enabled = true;
            }

            DecFacLabel.Visible = false;
            AddNewGB.Enabled = true;
        }

        private void AddFaceButton_Click(object sender, EventArgs e) //Method that saves a newly added trusted face
        {
            if (string.IsNullOrWhiteSpace(FaceNameTextBox.Text))
            {
                MessageBox.Show("Enter A Name For The Face.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TrustedFacesList.Rows.Add(TrustedFacesList.Rows.Count + 1, FaceNameTextBox.Text.Trim(), "Remove");

            //Converting image to base64 string and adding it to the list. 
            ImageDataList.Add(ImageToBase64((Image)PreviewBox.Image.Clone()));
            //Adding name of the face to the list
            NameList.Add(FaceNameTextBox.Text.Trim());

            //Saves the face image data as a base encoded string, along with its name
            Properties.Settings.Default.Base64ImageData = ImageDataList;
            Properties.Settings.Default.FaceNames = NameList;
            Properties.Settings.Default.Save();

            MessageBox.Show("Face Added Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            PreviewBox.Image = null;
            FaceNameTextBox.Text = "";

            FaceNameTextBox.Enabled = false;
            AddFaceButton.Enabled = false;
            NameLabel.Enabled = false;
        }
        private static Image CropImage(Image img, Rectangle cropArea) // Method that crops an image
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
        private string ImageToBase64(Image image) // Method to convert an image to it's corresponding base ecoded string
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private async void StartFacialVerification(object sender, EventArgs e) // Method that verifies a face and performs door unlock, upon successfull verification.
        {
            string ArduinoData = null;
            if (!SecurityRunning) return;
            this.Monitor.Visible = true;
            MonitorControls.Enabled = false;
            StatusLabel.Text = "VERIFYING FACE..";
            StatusLabel.ForeColor = Color.Orange;

            //Plays ding dong caling bell sound
            string FileName = string.Format("{0}Resources\\dingdong.mp3", Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\")));

            WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer(); //Setup player to play sound
            wplayer.URL = FileName;
            wplayer.controls.play();

            string currentImage = ImageToBase64((Image)Monitor.Image.Clone());

            //Verbally greets or informs user with the verification result
            SpeechSynthesizer synthesizer = new SpeechSynthesizer
            {
                Volume = 100,  // 0...100
                Rate = 1     // -10...10
            };
            for (int i = 0; i < ImageDataList.Count; i++)// Iterates through each trusted face and compares it with current captured face
            {
                double confidence = await VerifyFace(ImageDataList[i], currentImage);

                if (confidence > 80) //Trusted face encountered, opening door.
                {
                    synthesizer.SpeakAsync("Welcome " + NameList[i]);
                    // StatusLabel.Text = "Opening Door..";
                    StatusLabel.ForeColor = Color.Green;
                    //Contributed code
                    //Send Result to Arduino: Access Allowed
                    Console.WriteLine("GREEN NOTIFICATION LED ON");
                    Send_Verification_Results(true, NameList[i]);
                    CheckAuthenticationMethod();
                    StatusLabel.Text = "Running";
                    MonitorControls.Visible = true;
                    Monitor.Visible = false;
                    Invoke(new Action(() => { StopMonitorButton_Click_1(null, null); }));
                    SerialCommunication.count = 0;//Reset attempts counter
                    CurrentCam.Stop();
                    return;
                }
            }
            SerialCommunication.count++;//increment counter to know when arduino will sound the alarm
            //Console.WriteLine(SerialCommunication.count);
            Send_Verification_Results(false, "NA");
            synthesizer.SpeakAsync("You're not authorized.");
            if(SerialCommunication.count==3)
            {
                SerialCommunication.count = 0;//Reset attempts counter
                synthesizer.SpeakAsync("ALARM TRIGGERED, RED LEDS FLASHING, BUZZER BUZZING");
                Console.WriteLine("ALARM TRIGGERED\nRED LEDS FLASHING\nBUZZER BUZZING");
                StatusLabel.Text = "Authentication Failed";
                StatusLabel.ForeColor = Color.Red;
                await Task.Delay(2500);
                StatusLabel.Text = "Not Running";
                StatusLabel.ForeColor = Color.Red;
                MonitorControls.Enabled = true;
                SetFormChanges(this.Monitor, this.StartMonitorButton, this.StatusLabel, "", 'M', "N"); //Turn off the monitor
                Invoke(new Action(() => { StopMonitorButton_Click_1(null, null); }));
                StartMonitorButton.Enabled = false;
                CurrentCam.Stop();
                Listen_For_VerifyUser_Request();
                return;
            }
            /*Contributed code 
              Anas Nazha 
              Send Result to Arduino: Access Denied*/
            Console.WriteLine("RED NOTIFICATION LED ON");
            StatusLabel.Text = "Authentication Failed";
            StatusLabel.ForeColor = Color.Red;
            await Task.Delay(2500);
            //StatusLabel.Text = "Running";
            // StatusLabel.ForeColor = Color.Green;
            Invoke(new Action(() => { StopMonitorButton_Click_1(null, null); }));
            MonitorControls.Enabled = true;
            CurrentCam.Stop();
            SetFormChanges(this.Monitor, this.StartMonitorButton, this.StatusLabel, "", 'M', "N"); //Turn off the monitor
            //Reads three lines as sent by ardunino, to notify user that authentication was wrong and how any attempts left
            ArduinoData = SerialCommunication._port.ReadExisting();
            Console.WriteLine(ArduinoData);
            synthesizer.SpeakAsync(ArduinoData);
        }

        //Insert Method that sends Verification result to Arduino using Serial Port
        private void Send_Verification_Results(bool allowed, string name)
        {
            /*Contributed code 
          Anas Nazha 
          Sends verification result to Arduino using Serial Port*/
            try
            {
                if (allowed)
                {
                    SerialCommunication._port.Write(name);
                    Thread.Sleep(200);
                }
                else
                {
                    SerialCommunication._port.Write("0");
                    Thread.Sleep(200);
                }
            }
            catch
            {
                StatusLabel.Text = "PORT NOT FOUND";
                StopMonitorButton.Enabled = true;
                return;
            }
        }
        /*Contributed code 
            Anas Nazha 
            Automating Facial Recognition upon Arduino's request*/

        //The following method has been modified from original source code to suit ACS233 Project 
        private void StartMonitorButton_Click_1(object sender, EventArgs e)
        {
            /*Contributed code 
               Anas Nazha 
               Catches Port not open error*/

            try
            {
                StartMonitorButton.Enabled = false;
                //FrontDoorGB.Enabled = false;

                StatusLabel.Text = "Connecting..";
                StatusLabel.ForeColor = Color.Orange;

                if (SerialCommunication._port.IsOpen) //Do nothing if already connected 
                {

                }
                else
                {
                    SerialCommunication._port.Open();
                }
                //Check if port is opened and updated Status on the app 
                ArduinoStatus.Text = "CONNECTED";
                ArduinoStatus.ForeColor = Color.Green;
                SecurityRunning = true;
                StopMonitorButton.Enabled = true;
                StartMonitorButton.Enabled = false;
                //FrontDoorGB.Enabled = true;
                StatusLabel.Text = "Running";
                StatusLabel.ForeColor = Color.Green;
                Invoke(new Action(() => { StartFacialVerification(null, null); }));
            }
            catch (IOException)
            {
                StatusLabel.Text = "PORT NOT FOUND";
                ArduinoStatus.Text = "NOT CONNECTED";
                ArduinoStatus.ForeColor = Color.Red;
                StopMonitorButton.Enabled = true;
                return;
            }
        }

        //Contributed code
        //Listen for arduino request for facial verification
        /*Contributed code 
           Anas Nazha 
           Changes: async to void 
                    async apparently has an await method to wait for a certain event to occur 
                    Using void instead of async to listen for arduino request*/

        //Changed to Immediately activate start button and tell user to snap a picture
        private async void EnableStartButton()
        {
            CurrentCam.Start();
            SecurityRunning = true;
            Console.Clear();
            Console.WriteLine("Smart Burglar Alarm v1.0");
            Console.WriteLine("Start Button now enabled!\nPress start when ready to take photo\n");
            SetFormChanges(this.Monitor, this.StartMonitorButton, this.StatusLabel, "", 'M', "V"); //Turn on the monitor
            SetFormChanges(this.Monitor, this.StartMonitorButton, this.StatusLabel, "SETTING UP", 'N', "O");//Change text
            SetFormChanges(this.Monitor, this.StartMonitorButton, this.StatusLabel, "SETTING UP", 'C', "O");//Change color

            await Task.Delay(2000);
            SetFormChanges(this.Monitor, this.StartMonitorButton, this.ArduinoStatus, "Please Press Start!", 'N', "NA");
            SetFormChanges(this.Monitor, this.StartMonitorButton, this.StatusLabel, "READY", 'N', "O");//Change text
            SetFormChanges(this.Monitor, this.StartMonitorButton, this.StatusLabel, "READY", 'C', "G");//Change color
            SpeechSynthesizer synthesizer = new SpeechSynthesizer
            {
                Volume = 100,  // 0...100 
                Rate = 1     // -10...10 
            };
            synthesizer.SpeakAsync("Press Start to take picture");
            SetFormChanges(this.Monitor, this.StartMonitorButton, this.ArduinoStatus, "NA", '1', "NA"); //Set start button to true
            Thread.Sleep(2000); //Suspends execution for 2 seconds. This is to avoid any reading errors through serial port 
        }
        private void StopMonitorButton_Click_1(object sender, EventArgs e)
        {
            SecurityRunning = false;
            StopMonitorButton.Enabled = false;
            StartMonitorButton.Enabled = true;
            StatusLabel.Text = "Not Running";
            StatusLabel.ForeColor = Color.Red;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) // Method that gets called when the application is exiting
        {
            //Release the camera device before exiting
            CurrentCam.Stop();
        }

        //Following Code from https://stackoverflow.com/questions/10775367/cross-thread-operation-not-valid-control-textbox1-accessed-from-a-thread-othe 
        //Allows Thread Safe Calls to Windows forms application 
        delegate void SetTextCallback(PictureBox Monitor, Button B, Label L, string text, char T_or_F, string C);

        /*Contributed code 
            Anas Nazha 
            Changes: Added the ability to perform changes to buttons and text 
                     not the best code but it works. 
            */
        private void SetFormChanges(PictureBox Monitor, Button B, Label L, string text, char T_or_F, string C)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (T_or_F == 'N') //N means no changes for true or false and thus only change text 
            {
                if (B.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetFormChanges);
                    this.Invoke(d, new object[] { Monitor, B, L, text, T_or_F, C });
                }
                else
                {
                    L.Text = text;
                }
                return;
            }
            else if (T_or_F == '0') //change the button to false 
            {
                if (B.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetFormChanges);
                    this.Invoke(d, new object[] { Monitor, B, L, text, T_or_F, C });
                }
                else
                {
                    B.Enabled = false;
                }
            }
            else if (T_or_F == '1') //change the button to true 
            {
                if (B.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetFormChanges);
                    this.Invoke(d, new object[] { Monitor, B, L, text, T_or_F, C });

                }
                else
                {
                    B.Enabled = true;
                }
            }
            else if (T_or_F == 'C') //Change Color of text
            {
                if (B.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetFormChanges);
                    this.Invoke(d, new object[] { Monitor, B, L, text, T_or_F, C });
                }
                else
                {
                    if (C == "R")//Change it to red
                    {
                        L.ForeColor = Color.Red;

                    }
                    else if (C == "G")//Change it to green
                    {
                        L.ForeColor = Color.Green;

                    }
                    else if (C == "O")//Change it to orange
                    {
                        L.ForeColor = Color.Orange;

                    }
                }
                return;
            }
            else if (T_or_F == 'M') //M for monitor
            {
                if (Monitor.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetFormChanges);
                    this.Invoke(d, new object[] { Monitor, B, L, text, T_or_F, C });

                }
                else
                {
                    if (C == "V")//Set Visible
                    {
                        Monitor.Visible = true;
                    }
                    if (C == "N")//Set Not Visible
                    {
                        Monitor.Visible = false;
                    }
                }
            }

        }
        /*Contributed code 
            Anas Nazha 
             Adding Interface to get input for Verification method and notifying the Arduino. 
            */
        private void AuthenticationInterface()
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer
            {
                Volume = 100,  // 0...100 
                Rate = 1     // -10...10 
            };
            synthesizer.SpeakAsync("Please enter verification method");
            //string ArduinoData = null;
            bool flag = false; //Variable to re-ask for input if input is incorrect
            do
            {
                // Start the timeout
                var read = false;
                Task.Delay(30000).ContinueWith(_ =>
                {
                    if (!read)
                    {
                        // Timeout => cancel the console read
                        var handle = GetStdHandle(STD_INPUT_HANDLE);
                        CancelIoEx(handle, IntPtr.Zero);
                    }
                });
                try
                {
                    Console.WriteLine("Enter Authentication Method, P for pin, or F for facial");
                    var ArduinoData = Console.ReadKey();
                    read = true;
                    if (ArduinoData.KeyChar.ToString() == "P"|| ArduinoData.KeyChar.ToString() == "p" || ArduinoData.KeyChar.ToString() == "F"|| ArduinoData.KeyChar.ToString() == "f")
                    {
                        flag = true;
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid input, enter 'P' or 'F' only");
                        synthesizer.SpeakAsync("Invalid input, please enter again");
                    }
                    if (ArduinoData.KeyChar.ToString() == "F" || ArduinoData.KeyChar.ToString() == "f")
                    {
                        SerialCommunication._port.Write("F"); //Tell the arduino that the user has chosen Facial Verification
                        Console.WriteLine("\n");
                        EnableStartButton();
                        return;
                    }
                    else if (ArduinoData.KeyChar.ToString() == "P" || ArduinoData.KeyChar.ToString() == "p")
                    {
                        SerialCommunication._port.Write("P"); //Tell the arduino that the user has chosen Pin Verification
                        GetInputPin();
                        return;
                    }
                }
                // Handle the exception when the operation is canceled
                catch (InvalidOperationException)
                {
                    SerialCommunication._port.Write("A");
                    synthesizer.SpeakAsync("ALARM TRIGGERED, RED LEDS FLASHING, BUZZER BUZZING");
                    Console.WriteLine("ALARM TRIGGERED\nRED LEDS FLASHING\nBUZZER BUZZING");
                    Listen_For_VerifyUser_Request();
                    return;
                }
            } while (!flag);

            
        }
        /*Contributed code 
           Anas Nazha 
          Listen_For_VerifyUser_Request waits for Arduino's request to enable user interface i.e detects motion. 
           */
        private void Listen_For_VerifyUser_Request()
        {
            string ArduinoData = null;
            SpeechSynthesizer synthesizer = new SpeechSynthesizer
            {
                Volume = 100,  // 0...100 
                Rate = 1     // -10...10 
            };
            while (StartUserInterface)
            {
                try
                {
                    ArduinoData = SerialCommunication._port.ReadLine();
                    ArduinoData = ArduinoData.Trim();
                    Console.WriteLine(ArduinoData);
                    if (string.Equals(ArduinoData, "S"))
                    {
                        Console.WriteLine("PIR Motion Sensor Detected Someone!\n30 second timer to check for input started!\n");
                        AuthenticationInterface();
                        return;
                    }
                    else if(string.Equals(ArduinoData, "A"))//Magnet Switch is high
                    {
                        synthesizer.SpeakAsync("ALARM TRIGGERED, RED LEDS FLASHING, BUZZER BUZZING");
                        Console.WriteLine("ALARM TRIGGERED\nRED LEDS FLASHING\nBUZZER BUZZING");
                       // Listen_For_VerifyUser_Request();
                        //return;
                    }
                }
                catch //Catch port not found
                {

                }

            }
        }

        /*Contributed code 
           Anas Nazha 
           Takes user input for PIN from console and sends it to the arduino*/
        private async void GetInputPin()
        {
            bool flag = false;
            //Speech synthesizer to read out notifcation for the user
            SpeechSynthesizer synthesizer = new SpeechSynthesizer
            {
                Volume = 100,  // 0...100 
                Rate = 1     // -10...10 
            };
            await Task.Delay(300);
            synthesizer.SpeakAsync("Please enter your pin");
            Console.WriteLine("\nEnter your PIN");
            string ArduinoData = null;
            do
            {
                ArduinoData = Console.ReadLine();
                SerialCommunication._port.Write(ArduinoData);
                //await Task.Delay(500);
                ArduinoData = SerialCommunication._port.ReadLine();
                ArduinoData = ArduinoData.Trim();
                //Check for confirmation from arduino, correct pin or not, if correct, break out of the loop
                if (ArduinoData.Equals("1"))
                {
                    Console.WriteLine("GREEN NOTIFICATION LED ON");
                    await Task.Delay(500);
                    ArduinoData = SerialCommunication._port.ReadLine(); //Read name sent from arduino
                    synthesizer.SpeakAsync("Pin Successful. Welcome " + ArduinoData);
                    CheckAuthenticationMethod();
                    return;
                }
                else if (ArduinoData.Equals("0"))
                {
                    Console.WriteLine("RED NOTIFICATION LED ON");
                    await Task.Delay(1000); //Wait for the buffer
                    //Reads three lines as sent by ardunino, to notify user that authentication was wrong and how any attempts left
                    ArduinoData = SerialCommunication._port.ReadExisting();
                    synthesizer.SpeakAsync(ArduinoData);
                    Console.WriteLine(ArduinoData);
                }
                else if(ArduinoData.Equals("A"))//Alarm triggered
                {
                    synthesizer.SpeakAsync("ALARM TRIGGERED, RED LEDS FLASHING, BUZZER BUZZING");
                    Console.WriteLine("ALARM TRIGGERED\nRED LEDS FLASHING\nBUZZER BUZZING");
                    Listen_For_VerifyUser_Request();
                    return;
                }
            } while (!flag);
        }
        /*Contributed code 
                   Anas Nazha 
                   Listens for any serial communication from the CheckAuthentication Method from the arduino */
        private async void CheckAuthenticationMethod()
        {
            //Speech synthesizer to read out notifcation for the user
            SpeechSynthesizer synthesizer = new SpeechSynthesizer
            {
                Volume = 100,  // 0...100 
                Rate = 1     // -10...10 
            };
            await Task.Delay(800);
            string ArduinoData = null;
            ArduinoData = SerialCommunication._port.ReadLine();
            ArduinoData = ArduinoData.Trim();

            if (ArduinoData.Equals("D")) //All methods done
            {
                // ArduinoData = SerialCommunication._port.ReadLine(); //Read name sent from arduino
                await Task.Delay(3000);
                synthesizer.SpeakAsync("All Verification Methods Satisfied");
                Console.WriteLine("All Verification Methods Satisfied");
                CurrentCam.Stop();
                SetFormChanges(this.Monitor, this.StartMonitorButton, this.StatusLabel, "", 'M', "N");//Make monitor not visible
                Invoke(new Action(() => { StopMonitorButton_Click_1(null, null); }));
                SetFormChanges(this.Monitor, this.StartMonitorButton, this.StatusLabel, "", '0', "O");//Disable start button
                await Task.Delay(4000);
                Console.Clear();
                Console.WriteLine("Smart Burglar Alarm v1.0");
                Console.WriteLine("Door will unlock shortly");
                synthesizer.SpeakAsync("Door will unlock shortly. If you want to view recorded logins, the button is now enabled for a short period.");
                Console.WriteLine("Click the view button to see recorded logins!");
                SetFormChanges(this.Monitor, this.RequestLogins, this.StatusLabel, "", '1', "");//Enable the View logins button  
                await Task.Delay(10000);
                SetFormChanges(this.Monitor, this.RequestLogins, this.StatusLabel, "", '0', "");//Enable the View logins button
                Console.WriteLine("Solenoid on! Door Unlocked!");
                Listen_For_VerifyUser_Request();
                return;
            }
            else if (ArduinoData.Equals("P")) //Arduino still requesting for Pin verification
            {
                await Task.Delay(3000);
                synthesizer.SpeakAsync("Pin verification is still needed");
                Console.WriteLine("Pin Verification is still needed");
                await Task.Delay(2000);
                GetInputPin();
                return;
            }
            else if (ArduinoData.Equals("F")) //Arduino still requesting for Facial verification
            {
                await Task.Delay(3000);
                synthesizer.SpeakAsync("Facial Verification is still needed");
                Console.WriteLine("Facial Verification is still needed");
                await Task.Delay(2000);
                EnableStartButton();
                return;
            }
        }
        private async void DisplayRecordedLogins()
        {
            string ArduinoData = null;

            try
            {
               // Console.WriteLine("I am here");
                await Task.Delay(2000);
                ArduinoData = SerialCommunication._port.ReadExisting();
                Console.WriteLine(ArduinoData);
                await Task.Delay(10000); //Show the recorded logins for 10 seconds
                Console.Clear();
                Console.WriteLine("Smart Burglar Alarm v1.0");
                return;

            }
            catch //Catch port not found
            {

            }

        }

        private void RequestLogins_Click(object sender, EventArgs e)
        {
            SerialCommunication._port.Write("R");
            DisplayRecordedLogins();
        }
    }
}