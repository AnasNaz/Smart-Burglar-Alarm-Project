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

//Contribution to Code 
//Anas Nazha 
//Adding Serial Port communication to send data to Arduino 
using System.IO.Ports;
//using System.Threading; Says it appears previously, no idea where

namespace Facial_Recognition_Smart_Alarm
{
    public partial class Form1 : Form
    {
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
        Thread FacialVerification_Listening_Thread;
        //String List to hold names of authorized users and image data 
        List<string> NameList;
        List<string> ImageDataList;

        bool SecurityRunning = false;// ListenForBell; //removed this as we are not awaiting a listen for bell
        bool Facial_Verification_Request; //Similar logic to ListenForBell but here we wait for Arduino's request to automate Facial Verification
        //Initialize Windows Form
        public Form1()
        {
            InitializeComponent();
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
            Console.Write("Welcome!\n");
            Console.SetWindowPosition(0, 0);
            // Console.ReadKey(); 
            //Connecting to serial port

            SerialCommunication._port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            try
            {
                SerialCommunication._port.Open();
                //Check if port is opened and updated Arduino Status on the app 
                if (SerialCommunication._port.IsOpen)
                {
                    ArduinoStatus.Text = "CONNECTED";
                    ArduinoStatus.ForeColor = Color.Green;
                    StartMonitorButton.Enabled = false;
                }
            }

            catch
            {
                ArduinoStatus.Text = "PORT NOT FOUND!";
                ArduinoStatus.ForeColor = Color.Red;
            }
            string ArduinoData = null;
            /*ArduinoData = SerialCommunication._port.ReadLine(); //reads until "\n" is encountered 
            Console.WriteLine(ArduinoData); 
            ArduinoData = Console.ReadLine(); 
            SerialCommunication._port.Write(ArduinoData); 
            ArduinoData = SerialCommunication._port.ReadLine(); //reads until "\n" is encountered 
            Console.WriteLine(ArduinoData); 
            SerialCommunication._port.Write(ArduinoData); 
            Console.ReadKey(true); 
            ArduinoData = SerialCommunication._port.ReadLine(); //reads until "\n" is encountered 
            Console.WriteLine(ArduinoData); 
            ArduinoData = SerialCommunication._port.ReadLine(); //reads until "\n" is encountered 
            Console.WriteLine(ArduinoData);*/

            StartAuthenticationInterface();

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
            CurrentCam.Start();
            FacialVerification_Listening_Thread = new Thread(new ThreadStart(VerificationListener));
            //Facial_Verification_Request = true; 
            FacialVerification_Listening_Thread.Start();
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
   
        private async void RingBell_Click(object sender, EventArgs e) // Method that verifies a face and performs door unlock, upon successfull verification.
        {
            if (!SecurityRunning) return;

            RingBell.Enabled = false;
            FrontDoorGB.Enabled = false;
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
                    synthesizer.SpeakAsync("Welcome " + NameList[i] + ". Door is now unlocked.");
                    StatusLabel.Text = "Opening Door..";
                    StatusLabel.ForeColor = Color.Green;
                   //Contributed code
                   //Send Result to Arduino: Access Allowed
                    Send_Verification_Results(true);
                   

                    StatusLabel.Text = "Running";
                    RingBell.Enabled = true;
                    FrontDoorGB.Enabled = true;
                    MonitorControls.Enabled = true;
                    return;
                }
              
            }

            synthesizer.SpeakAsync("You're not authorized.");

            /*Contributed code 
              Anas Nazha 
              Send Result to Arduino: Access Denied*/
            Send_Verification_Results(false);
            
            StatusLabel.Text = "Authentication Failed";
            StatusLabel.ForeColor = Color.Red;
            await Task.Delay(2500);
            StatusLabel.Text = "Running";
            StatusLabel.ForeColor = Color.Green;

            RingBell.Enabled = true;
            FrontDoorGB.Enabled = true;
            MonitorControls.Enabled = true;
        }

        //Insert Method that sends Verification result to Arduino using Serial Port
        private void Send_Verification_Results(bool allowed)
        {
            /*Contributed code 
          Anas Nazha 
          Sends verification result to Arduino using Serial Port*/
            try
            {
                if (allowed)
                {
                    SerialCommunication._port.Write("1");
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
                FrontDoorGB.Enabled = false;

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
                FrontDoorGB.Enabled = true;

                StatusLabel.Text = "Running";
                StatusLabel.ForeColor = Color.Green;
                Invoke(new Action(() => { RingBell_Click(null, null); }));
                /*Removed:   BellButtonListningThread = new Thread(new ThreadStart(BellListner)); 
                  ListenForBell = true; 
                  BellButtonListningThread.Start(); */

                //Start a new thread that continously checks for arduino request for facial verification 
                //FacialVerification_Listening_Thread = new Thread(new ThreadStart(VerificationListener)); 
                //Facial_Verification_Request = true; 
                // FacialVerification_Listening_Thread.Start();        
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

        //Listen for arduino request for facial verification 
        private void VerificationListener()
        {
            //Testing logic 
            string ArduinoData = null; //String variable that holds dat sent from arduino 
            bool recievedData = false;
            /*while(!recievedData) 
            { 
                try 
                { 
                    ArduinoData = SerialCommunication._port.ReadLine(); //reads until "\n" is encountered 
                    ArduinoData = ArduinoData.Trim(); //Trim any whitespaces with the data 
                    if (string.Equals(ArduinoData, "1")) 
                    { 
                        recievedData = true; 
                    } 
 
                } 
 
                catch 
                { 
                    SetFormChanges(this.AddFaceButton,this.ArduinoStatus,"Connect Arduino!",'N'); 
                    return; //Exit out of function 
                } 
            } 
 
           /* while (Facial_Verification_Request)  
           { 
                try 
                { 
                    ArduinoData = SerialCommunication._port.ReadLine(); //reads until "\n" is encountered     
                } 
 
                catch 
                { 
                    SetArduinoStatusText( "Connect Arduino!"); 
                    return; //Exit out of function 
                } 
                ArduinoData = ArduinoData.Trim(); //Trim any whitespaces with the data 
                //For debugging any issues, view the data and its length 
                Console.WriteLine(ArduinoData); 
                Console.WriteLine(ArduinoData.Length); 
 
                if (string.Equals(ArduinoData,"1")) 
                {*/
            // Console.WriteLine("\nREQUEST RECEIVED"); 
            SecurityRunning = true;
            SetFormChanges(this.StartMonitorButton, this.ArduinoStatus, "Please Press Start!", 'N');
            SetFormChanges(this.StartMonitorButton, this.ArduinoStatus, "NA", '1');



            /* 
                StopMonitorButton.Enabled = true; 
                StartMonitorButton.Enabled = false; 
                FrontDoorGB.Enabled = true; 
 
                StatusLabel.Text = "Running"; 
                StatusLabel.ForeColor = Color.Green;*/
            /*   SetArduinoStatusText(this.StopMonitorButton,this.ArduinoStatus, "NOT CONNECTED!", '1'); 
           SetArduinoStatusText(this.StartMonitorButton, this.ArduinoStatus, "NOT CONNECTED!", '0'); 
           SetArduinoStatusText(this.StartMonitorButton, this.StatusLabel, "Running!", 'N');*/
            //FrontDoorGB.Enabled = true; 
            // Invoke(new Action(() => { RingBell_Click(null, null); })); 
            //    Thread.Sleep(2000); 
            //Facial_Verification_Request = false; 
            // } 

            Thread.Sleep(2000); //Suspends execution for 2 seconds. This is to avoid any reading errors through serial port 
                                // } 
        }
        private void StopMonitorButton_Click_1(object sender, EventArgs e)
        {
            SecurityRunning = false;
            FrontDoorGB.Enabled = false;
            StopMonitorButton.Enabled = false;
            StartMonitorButton.Enabled = true;
         //   Send_Verification_Results(false);
            StatusLabel.Text = "Not Running";
            StatusLabel.ForeColor = Color.Red;
            Facial_Verification_Request = false;
            ////SetArduinoStatusText(this.StartMonitorButton,this.ArduinoStatus,"NOT CONNECTED!",'N'); 
            //ListenForBell = false; 
        }



        //Original code had no method for the status label

        //Code kept for reference 
        /*   private async void LockDoorButton_Click(object sender, EventArgs e) // Method that gets called when user presses lock button
           {
               MonitorControls.Enabled = false;
               FrontDoorGB.Enabled = false;
               StatusLabel.Text = "Locking Door..";
               StatusLabel.ForeColor = Color.Red;

              // await LockDoor();

               MonitorControls.Enabled = true;
               FrontDoorGB.Enabled = true;
               StatusLabel.Text = "Running";
               StatusLabel.ForeColor = Color.Green;
           }*/
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) // Methor that gets called when the application is exiting
        {
            //Release the camera device before exiting
            CurrentCam.Stop();
        }

        //Following Code from https://stackoverflow.com/questions/10775367/cross-thread-operation-not-valid-control-textbox1-accessed-from-a-thread-othe 
        //Allows Thread Safe Calls to Windows forms application 
        delegate void SetTextCallback(Button B, Label L, string text, char t_or_f);

        /*Contributed code 
            Anas Nazha 
            Changes: Added the ability to perform changes to buttons and text 
                     not the best code but it works. 
            */
        private void SetFormChanges(Button B, Label L, string text, char T_or_F)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (T_or_F == 'N') //N means no changes for true or false and thus only change text 
            {


                if (B.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetFormChanges);
                    this.Invoke(d, new object[] { B, L, text, T_or_F });
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
                    this.Invoke(d, new object[] { B, L, text, T_or_F });
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
                    this.Invoke(d, new object[] { B, L, text, T_or_F });

                }
                else
                {
                    B.Enabled = true;
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
            string ArduinoData = null;
            Console.WriteLine("Enter Authentication Method, P for pin, or F for facial");
            ArduinoData = Console.ReadLine();
            SerialCommunication._port.Write(ArduinoData);
            ArduinoData = SerialCommunication._port.ReadLine();
            Console.WriteLine(ArduinoData);

        }
        private void StartAuthenticationInterface()
        {
            string ArduinoData = null;
            while (SerialCommunication._port.IsOpen)
            {
                ArduinoData = SerialCommunication._port.ReadLine();
                //ArduinoData = Console.ReadLine(); 
                ArduinoData = ArduinoData.Trim();
                if (string.Equals(ArduinoData, "S"))
                {
                    AuthenticationInterface();
                    return;
                }
            }
        }
    }
}

