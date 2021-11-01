using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Used to get system information
using System.Management;

//used to be able to read and write to text files
using System.IO;

//to enable multithreading
using System.Threading;

//library for real time charts
// Install-Package kayChart.dll -Version 0.4.7 
//https://www.nuget.org/packages/kayChart.dll/
using rtChart;

//Unosquare
//https://unosquare.github.io/raspberryio/
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
using Unosquare.Swan;

//used for stopwatch
using System.Diagnostics;

//used to get string to variable name
using System.Reflection;

//used for serial communication
using System.IO.Ports;


namespace Fusor_controler_GUI
{
    public partial class Form1 : Form
    {
        //screen update delay
        static byte scrUpdate = 66; //used an odd number so timer looks smoother

        //saved error list used in checksum and imported to listbox
        List<String> ErrorList = new List<String>();

        //delay for checksum thread to avoid starting without any data from adc
        static Int16 TurnonDelay = 1000;

        //file locations
        //acutal location set in startup because it difers between windows and linux system
        string SettingsFile;
        string ThermistorTableFile;
        string LogFileName;

        //ADC DATA
        //for incoming analog data from ADC(MCP3008)
        //Has to be converted before use
        //NOTE:Cant use 2 dim array / I couldnt make it work anyway
        byte[] MainsSens = new byte[3];
        byte[] HVSens = new byte[3];
        byte[] CurrentSens = new byte[3];
        byte[] TempSens = new byte[3];
        byte[] VCC52 = new byte[3]; //+5V2       
        byte[] VCC12 = new byte[3]; //+12V

        //CONVERSION DATA
        float[,] ConversionData = new float[6, 3];
        /* X,0=Raw      0-1023
         * X,1=Volt     0-5v
         * X,2=Real     Depends whats being measured
         * 
         * 0,X=Mainsvoltage
         * 1,X=Mainscurent
         * 2,X=HVSens
         * 3,X=TempsSens
         * 4,X=+5V2
         * 5,X=ADCinput(+12V)
         */
        Byte ConversionChannel;
        //Conversion output
        float MainsVoltage = 0;
        float MainsCurrent = 0;
        float HighVoltage = 0;
        float OilTemperature = 0;
        float V5 = 0;
        float V12 = 0;
        //rms calculation output for main voltage and current (High voltage might be added too)
        List<float> ListMainsVoltage = new List<float>();
        List<float> ListMainsCurrent = new List<float>();
        float rmsVoltage;
        float rmsCurrent;

        //CALIBRATION DATA
        float[] CalibrationData = new float[6];
        /*
         * 0=Mainsvoltage
         * 1=Mainscurent
         * 2=HVSens
         * 3=TempsSens
         * 4=+5V2
         * 5=ADCinput(+12V)
         */

        //PARAMETERS
        float MainsVoltageMIN, MainsVoltageLOW, MainsVoltageHIGH, MainsVoltageMAX;
        float MainsCurrentMIN, MainsCurrentLOW, MainsCurrentHIGH, MainsCurrentMAX;
        float HighVoltageHigh, HighVoltageMAX;
        float TempMIN, TempLOW, TempHIGH, TempMAX;
        float V5MIN, V5LOW, V5HIGH, V5MAX;
        float V12MIN, V12LOW, V12HIGH, V12MAX;
        float ARC; //voltage drop dedection threshold
        Int16 ARCtime; //ms delay between checks for arc dedection
        byte SpeedDelay;

        float rmsMaxVon = 10; //maximum input voltage when turning on high voltage

        //ADC voltage on refrence pin
        float ADCrefrenceVoltage = 5.004F;

        //to stop looping threads when closing aplication
        //If it they stay active while trying to exit it will crash because of some reason? 
        //this seems to fix that most of the time
        bool threadsActive = true;

        //to disable parts of code that only run on raspberry pi
        bool RunningOnRaspberry;

        //if high voltage should be turned on if posible
        bool HVon = false;
        Nullable<bool> HVsafe = null;

        bool ConversionThreadActive = false;
        bool ComunicationThreadActive = false;

        //100K thermistor table *NOT USED ANYMORE
        /*
         * 0=Farenheit
         * 1=Celcius
         * 2=resistance value
         */
        float[] ThermistorTableF = new float[114];
        float[] ThermistorTableC = new float[114];
        float[] ThermistorTableR = new float[114];

        float ThermistorResistance;

        int TempResistor = 46200; //R7 resistor value (47K) (calibrated to actual value)

        //tick count used for calculating threads speed
        Int32 ConversionTickcount;
        Int32 SpiTickcount;

        /*
         * Data loging arrays
         * arrays wil update every ms
         * set to loging of 10 mins
         */
        static byte LogDelay = 1;
        static int LogDatapoints = (600) * 1000 / LogDelay; //amount of seconds of saved data(Not exact,like at all. Logs few % longer then difined)

        float[] LogTimestamp = new float[LogDatapoints];

        float[] LogConversionTickcount = new float[LogDatapoints];

        float[] LogMainsVoltage = new float[LogDatapoints];
        float[] LogMainsCurrent = new float[LogDatapoints];
        float[] LogHighVoltage = new float[LogDatapoints];
        float[] LogTemperature = new float[LogDatapoints];
        float[] LogThermistorResistance = new float[LogDatapoints];
        float[] LogV5 = new float[LogDatapoints];
        float[] LogV12 = new float[LogDatapoints];

        //pin status variables / status comes from attiny85 safety controller
        bool waStatus;
        bool saStatus;

        //Radiation calculation variables
        int CPM5, CPM15, CPM30, CPM60;
        int[] CPM15Array = new int[3 + 1]; //+1 is to exclude copied value in calculation
        int[] CPM30Array = new int[6 + 1];
        int[] CPM60Array = new int[12 + 1];
        float SV5, SV15, SV30, SV60;

        //used in HV ARC dedection. We want a fusion reactor, not a lightning generator
        float HVlast = 0;
        Stopwatch ARCtimingStopWatch = new Stopwatch();

        //Keep up time since startup
        Stopwatch Time = new Stopwatch();

        //used in error check to give the time of the error ocurance
        TimeSpan tsTime;

        //STARTUP
        //Start GUI and start looping threads
        public Form1()
        {
            InitializeComponent();


            Time.Start();

            //check if running on raspberry pi
            try
            {
                //print raspberry pi info to console
                string Info = Pi.Info.ToString();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Raspberry pi environment detected");
                Console.WriteLine(Info);
                Console.WriteLine("RUN MODE");
                Console.ResetColor();

                //set location of settings file
                SettingsFile = @"/DATA/Settings data.txt"; //Because it couldnt be the same for windows and linux could it!?
                ThermistorTableFile = @"/DATA/100K thermistor table.txt";

                //buz for 1s
                Pi.Gpio.Pin01.PinMode = GpioPinDriveMode.Output;
                Pi.Gpio.Pin01.Write(true);
                Thread.Sleep(1000);
                Pi.Gpio.Pin01.Write(false);

                //make clear program is running normal on raspberry pi
                RunningOnRaspberry = true;
                label1.ForeColor = System.Drawing.Color.Green;
                label1.Text = ("RUN MODE");
            }
            catch
            {
                //make clear program is not fully functional because not no raspberry pi hardware is found
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Outside raspberry pi environment");
                Console.WriteLine("Running in DEBUG MODE    some functions disabled");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;

                //set location of settings file
                SettingsFile = @"\DATA\Settings data.txt"; //fucks sake
                ThermistorTableFile = @"\DATA\100K thermistor table.txt";

                RunningOnRaspberry = false;
                label1.ForeColor = System.Drawing.Color.Red;
                label1.Text = ("DEBUG MODE");
            }

            //Console.WriteLine(Application.StartupPath);

            Console.Write("\r\nExecutePath = \r\n");
            Console.WriteLine(Application.ExecutablePath);

            //Set full screen
            if (RunningOnRaspberry)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\r\nSet full screen");
                Console.ResetColor();
            }

            //Read from DATA files
            using (var reader = File.OpenText(Application.StartupPath + SettingsFile))
            {
                int LineCount = 0;
                int X = 0;
                string Line;
                while ((Line = reader.ReadLine()) != null)
                {

                    //Get ADC refrence voltage
                    if (Line.Contains("ADCrefrenceVoltage="))
                    {
                        ADCrefrenceVoltage = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                    }

                    //read calibration data
                    if (Line.Contains("Cal"))
                    {
                        CalibrationData[Convert.ToInt16(Line.Substring(Line.LastIndexOf("=") + 1, 1))] =
                           Convert.ToSingle(Line.Substring(Line.LastIndexOf(",") + 1, Line.Length - Line.LastIndexOf(",") - 1));
                    }

                    //read MIN MAX parameters data
                    if (Line.Contains("Par"))
                    {
                        //par mains voltage
                        if (Line.Contains("ParMainsVoltageMIN"))
                        {
                            MainsVoltageMIN = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParMainsVoltageLOW"))
                        {
                            MainsVoltageLOW = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParMainsVoltageHIGH"))
                        {
                            MainsVoltageHIGH = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParMainsVoltageMAX"))
                        {
                            MainsVoltageMAX = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }

                        //Par Mains current
                        else if (Line.Contains("ParMainsCurrentMIN"))
                        {
                            MainsCurrentMIN = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParMainsCurrentLOW"))
                        {
                            MainsCurrentLOW = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParMainsCurrentHIGH"))
                        {
                            MainsCurrentHIGH = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParMainsCurrentMAX"))
                        {
                            MainsCurrentMAX = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }

                        //Par high voltage
                        else if (Line.Contains("ParHighVoltageHIGH"))
                        {
                            HighVoltageHigh = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParHighVoltageMAX"))
                        {
                            HighVoltageMAX = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }

                        //Par temperature control
                        else if (Line.Contains("ParTempMIN"))
                        {
                            TempMIN = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParTempLOW"))
                        {
                            TempLOW = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParTempHIGH"))
                        {
                            TempHIGH = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParTempMAX"))
                        {
                            TempMAX = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }

                        //par +5V2
                        else if (Line.Contains("ParV5MIN"))
                        {
                            V5MIN = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParV5LOW"))
                        {
                            V5LOW = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParV5HIGH"))
                        {
                            V5HIGH = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParV5MAX"))
                        {
                            V5MAX = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }

                        //Par +12V
                        else if (Line.Contains("ParV12MIN"))
                        {
                            V12MIN = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParV12LOW"))
                        {
                            V12LOW = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParV12HIGH"))
                        {
                            V12HIGH = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParV12MAX"))
                        {
                            V12MAX = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }

                        //Par arc dedection thresholds
                        else if (Line.Contains("ParARCvoltage"))
                        {
                            ARC = Convert.ToSingle(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }
                        else if (Line.Contains("ParARCtime"))
                        {
                            ARCtime = Convert.ToInt16(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }

                        //Par arc dedection thresholds
                        else if (Line.Contains("ParSpeedDelay"))
                        {
                            SpeedDelay = Convert.ToByte(Line.Substring(Line.LastIndexOf("=") + 1, Line.Length - Line.LastIndexOf("=") - 1));
                        }

                    }

                    LineCount++;
                    X++;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Imported par/cal data");
                Console.ResetColor();
            }

            //OLD
            //Import thermistor value table from file
            if (false) //Old way //Now solved by equation
            {
                //Read from thermistortable file to get data for temperature table of thermistor
                using (var reader = File.OpenText(Application.StartupPath + ThermistorTableFile))
                {
                    string Line;
                    int LineCount = 0;

                    int X = 0;
                    int ArrayPlace = 0;


                    while ((Line = reader.ReadLine()) != null)
                    {
                        //Get faherenheit value
                        if (X == 3)
                        {
                            ThermistorTableF[ArrayPlace] = Convert.ToSingle(Line);
                        }
                        //get celcius value
                        else if (X == 4)
                        {
                            ThermistorTableC[ArrayPlace] = Convert.ToSingle(Line);
                        }
                        //get resistance value
                        else if (X == 5)
                        {
                            ThermistorTableR[ArrayPlace] = Convert.ToSingle(Line);
                            X = 2;
                            ArrayPlace++;
                        }

                        X++;
                        LineCount++;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Imported thermistor table");
                }
            }
            else 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("thermistor table not imported (OLD)");
                Console.ForegroundColor = ConsoleColor.Green;
            }

            //Start thread for reading hardware communication
            if (RunningOnRaspberry)
            {
                Thread spiThread = new Thread(COMM);
                spiThread.Start();
                spiThread.IsBackground = true;
                Console.WriteLine("Communication thread started");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Communication thread disabled");
                Console.ForegroundColor = ConsoleColor.Green;
            }

            //Start thread for console comunication
            Thread ConsoleComsThread = new Thread(ConsoleComs);
            ConsoleComsThread.Start();
            ConsoleComsThread.IsBackground = true;

            Console.WriteLine("Console Coms thread started");

            //start screen update thread
            Thread screenUpdateThread = new Thread(screenUpdate);
            screenUpdateThread.IsBackground = true;
            screenUpdateThread.Start();

            Console.WriteLine("ScreenUpdate thread started");

            //start data loging thread
            Thread DataLogingThread = new Thread(LogData);
            DataLogingThread.IsBackground = true;
            DataLogingThread.Start();

            //High voltage checking thread
            Thread HvThread = new Thread(HighVoltageOn);
            HvThread.Start();
            HvThread.IsBackground = true;

            //Serial comunication
            //only recieves data for radiation dedection
            if (RunningOnRaspberry)
            {
                Thread SerialComThread = new Thread(SerialCom);
                SerialComThread.Start();
                SerialComThread.IsBackground = true;

                Console.WriteLine("Serial communication thread started");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Serial communication disabled");
            }

            //Start thread for analyzing data and starting functions based on data
            Thread conversionThread = new Thread(Conversion);
            conversionThread.Start();
            conversionThread.IsBackground = true;

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Checksum thread will start after turn on delay of " + TurnonDelay + "ms");
            Console.ForegroundColor = ConsoleColor.Green;


            Console.ResetColor();

            /*
             * //FAIL
             * dont USE UPDATING CHARTS! CRASHES ON RASPARRY PI 
             * fuck all the work I put into it so far
             */
            //Staring thread for updating chart
            //Thread chartThread = new Thread(CHART);
            //chartThread.Start();
            //chartThread.IsBackground = true;
        }

        //FORM2     //Not used for now or maybe never.idk
        //Open form 2 and move relevant data
        private void BtnForm2_Click(object sender, EventArgs e)
        {
            CalibrationForm secondForm = new CalibrationForm(MainsSens);
            secondForm.Show();
            Hide();
        }

        //CONVERSION
        //analyze all incoming data and start functions depending on values
        private void Conversion()
        {
            Thread.Sleep(TurnonDelay);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Checksum thread started");
            Console.WriteLine("");
            Console.WriteLine("START");
            Console.WriteLine("");
            Console.ResetColor();

            Stopwatch CONstopWatch = new Stopwatch();
            long ConTicks = 0;

            Stopwatch RMStimingStopWatch = new Stopwatch(); //used for rms calculation

            //DEBUG CODE
            Stopwatch ConversionTime = new Stopwatch();
            Stopwatch ChecksumTime = new Stopwatch();

            Thread ChecksumThread = new Thread(ErrorCheck);


            //for saved error data
            List<String> ListErrortimestamps = new List<String>();

            while (true)
            {

                CONstopWatch.Start();
                RMStimingStopWatch.Start();

                //DEBUG
                ConversionTime.Start();

                /* Steps of conversion
                 * 
                 * 1:take 10bit data from 2 bytes in array and put it in 1 floating point
                 * 2:Convert 10 bit data(0-1023) to actual voltage on adc input pin using refrence voltage
                 * 3:Convert to real value using calibration data where needed
                 */

                ConversionChannel = 0;

                //Mains voltage conversion
                ConversionData[ConversionChannel, 0] = MainsSens[2] + (MainsSens[1] * 256);
                ConversionData[ConversionChannel, 1] = ConversionData[ConversionChannel, 0] * (ADCrefrenceVoltage / 1023);

                MainsVoltage = ConversionData[ConversionChannel, 1] * (CalibrationData[ConversionChannel]);
                //Calibration
                MainsVoltage = (((MainsVoltage-4)*90)/107)+13;

                ConversionChannel++;

                //Current conversion
                //+ Ofset calc
                ConversionData[ConversionChannel, 0] = CurrentSens[2] + (CurrentSens[1] * 256);
                ConversionData[ConversionChannel, 1] = ConversionData[ConversionChannel, 0] * (ADCrefrenceVoltage / 1023);

                MainsCurrent = Convert.ToSingle((ConversionData[ConversionChannel, 1] - ((V5/5)*2.5)) / 0.1);

                ConversionChannel++;

                //High voltage conversion
                ConversionData[ConversionChannel, 0] = HVSens[2] + (HVSens[1] * 256);
                ConversionData[ConversionChannel, 1] = ConversionData[ConversionChannel, 0] * (ADCrefrenceVoltage / 1023);

                HighVoltage = ConversionData[ConversionChannel, 1] * (CalibrationData[ConversionChannel]);

                ConversionChannel++;

                //Temperature sensing conversion
                ConversionData[ConversionChannel, 0] = TempSens[2] + (TempSens[1] * 256);
                //ConversionData[ConversionChannel, 1] = ConversionData[ConversionChannel, 0] * (ADCrefrenceVoltage / 1023); //Voltage out of thermistor devider not used. 
                ThermistorResistance = (TempResistor / (1023 / ConversionData[ConversionChannel, 0] - 1)) / 10; //calculate resistance of thermistor                                                                                                                
                OilTemperature = Convert.ToSingle(-0.000273905 * ThermistorResistance + 52.390506450464269084335369360979); //Calculate temperature with equation

                ConversionChannel++;

                //+5V real voltage conversion
                ConversionData[ConversionChannel, 0] = VCC52[2] + (VCC52[1] * 256);
                ConversionData[ConversionChannel, 1] = ConversionData[ConversionChannel, 0] * (ADCrefrenceVoltage / 1023);

                V5 = ConversionData[ConversionChannel, 1] * (CalibrationData[ConversionChannel]);

                ConversionChannel++;

                //Refrence voltage input conversion (+12V)
                ConversionData[ConversionChannel, 0] = VCC12[2] + (VCC12[1] * 256);
                ConversionData[ConversionChannel, 1] = ConversionData[ConversionChannel, 0] * (ADCrefrenceVoltage / 1023);

                V12 = ConversionData[ConversionChannel, 1] * (CalibrationData[ConversionChannel]);

                //RMS CALC
                //add data to list
                ListMainsVoltage.Add(MainsVoltage);
                ListMainsCurrent.Add(MainsCurrent);
                // Get the elapsed time as a TimeSpan value.                
                TimeSpan ts2 = RMStimingStopWatch.Elapsed;
                //capture data for 40ms (2 50hz ac cycles) to calculate rms then trow out to capture next run
                if (ts2.Milliseconds >= 40)
                {
                    RMStimingStopWatch.Reset();

                    float MaxVoltage = ListMainsVoltage.Max();
                    //float MinVoltage = ListMainsVoltage.Min();
                    float MaxCurrent = ListMainsCurrent.Max();
                    //float MinCurrent = ListMainsCurrent.Min();

                    rmsVoltage = Convert.ToSingle(MaxVoltage / Math.Sqrt(2));
                    rmsCurrent = Convert.ToSingle(MaxCurrent / Math.Sqrt(2));

                    ListMainsVoltage.RemoveRange(0, ListMainsVoltage.Count);
                    ListMainsCurrent.RemoveRange(0, ListMainsCurrent.Count);
                }

                //Run error checking code
                ErrorCheck();

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = CONstopWatch.Elapsed;
                //TICK COUNTER
                ConTicks++;
                //if 0.1 seconds has pased update tick counter
                if (ts.Milliseconds >= 500)
                {
                    CONstopWatch.Reset();
                    ConversionTickcount = Convert.ToInt32(ConTicks * 2);
                    ConTicks = 0;
                }
                ConversionThreadActive = true;
            }
            //END CONVERSION/CHECK
        }

        //ERRORCHECK
        //analyze all data for anything unusual
        private void ErrorCheck()
        {

            //CHECKSUM
            /*
            * 0=Mainsvoltage
            * 1=Mainscurent
            * 2=HVSens
            * 3=TempsSens
            * 4=+5V2
            * 5=ADCinput(+12V)
            */

            //ERROR 0
            //Check if recieving any data
            //if all values are 0 there nothing is getting recieved
            if (ConversionData[0, 0] == 0 && ConversionData[1, 0] == 0 && ConversionData[2, 0] == 0 && ConversionData[3, 0] == 0 && ConversionData[4, 0] == 0 && ConversionData[5, 0] == 0)
            {
                if (!ErrorList.Contains("adc comunication error"))
                {
                    ErrorList.Add("adc comunication error");
                    tsTime = Time.Elapsed;
                    Console.WriteLine("adc comunication ERROR " + tsTime.TotalSeconds);
                }
                HVsafe = false;
            }
            else
            {
                //ERROR 1
                if (V5 > V5MAX)
                {
                    if (!ErrorList.Contains("5V2 too high"))
                    {
                        ErrorList.Add("5V2 too high");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("5V2 too high " + tsTime.TotalSeconds);
                    }
                    HVsafe = false;
                }
                else if (V5 > V5HIGH)
                {
                    if (!ErrorList.Contains("5V2 high"))
                    {
                        ErrorList.Add("5V2 high");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("5V2 high " + tsTime.TotalSeconds);
                    }
                }
                else if (V5 < V5MIN)
                {
                    if (!ErrorList.Contains("5V2 too low"))
                    {
                        ErrorList.Add("5V2 too low");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("5V2 too low " + tsTime.TotalSeconds);
                    }
                    HVsafe = false;
                }
                else if (V5 < V5LOW)
                {
                    if (!ErrorList.Contains("5V2 low"))
                    {
                        ErrorList.Add("5V2 low");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("5V2 low " + tsTime.TotalSeconds);
                    }
                }

                //ERROR 2
                if (V12 > V12MAX)
                {
                    if (!ErrorList.Contains("12V too high"))
                    {
                        ErrorList.Add("12V too high");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("12V too high " + tsTime.TotalSeconds);
                    }
                    HVsafe = false;
                }
                else if (V12 > V12HIGH)
                {
                    if (!ErrorList.Contains("12V high"))
                    {
                        ErrorList.Add("12V high");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("12V high " + tsTime.TotalSeconds);
                    }
                }
                else if (V12 < V12MIN)
                {
                    if (!ErrorList.Contains("12V too low"))
                    {
                        ErrorList.Add("12V too low");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("12V too low " + tsTime.TotalSeconds);
                    }
                    HVsafe = false;
                }
                else if (V12 < V12LOW)
                {
                    if (!ErrorList.Contains("12V low"))
                    {
                        ErrorList.Add("12V low");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("12V low " + tsTime.TotalSeconds);
                    }
                }

                //ERROR 3
                if (ThermistorResistance != 0)
                {
                    if (OilTemperature > TempMAX)
                    {
                        if (!ErrorList.Contains("Temperature too high"))
                        {
                            ErrorList.Add("Temperature too high");
                            tsTime = Time.Elapsed;
                            Console.WriteLine("Temperature too high " + tsTime.TotalSeconds);
                        }
                        HVsafe = false;
                    }
                    else if (OilTemperature > TempHIGH)
                    {
                        if (!ErrorList.Contains("Temperature high"))
                        {
                            ErrorList.Add("Temperature high");
                            tsTime = Time.Elapsed;
                            Console.WriteLine("Temperature high " + tsTime.TotalSeconds);
                        }
                    }
                    else if (OilTemperature < TempMIN)
                    {
                        if (!ErrorList.Contains("Temperature too low"))
                        {
                            ErrorList.Add("Temperature too low");
                            tsTime = Time.Elapsed;
                            Console.WriteLine("Temperature too low " + tsTime.TotalSeconds);
                        }
                        HVsafe = false;
                    }
                    else if (OilTemperature < TempLOW)
                    {
                        if (!ErrorList.Contains("Temperature low"))
                        {
                            ErrorList.Add("Temperature low");
                            tsTime = Time.Elapsed;
                            Console.WriteLine("Temperature low " + tsTime.TotalSeconds);
                        }
                    }
                }
                else if (ThermistorResistance == 0)
                {
                    if (!ErrorList.Contains("Temperature 0 ERROR"))
                    {
                        ErrorList.Add("Temperature 0 ERROR");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("Temperature 0 ERROR - posible short " + tsTime.TotalSeconds);
                    }
                }
                else
                {
                    if (!ErrorList.Contains("Temperature NaN ERROR"))
                    {
                        ErrorList.Add("Temperature NaN ERROR");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("Temperature NaN ERROR - thermistor posibly not present" + tsTime.TotalSeconds);
                    }
                }

                //ERROR 4
                if (rmsVoltage > MainsVoltageMAX)
                {
                    if (!ErrorList.Contains("rms Voltage too high"))
                    {
                        ErrorList.Add("rms Voltage too high");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("rms Voltage too high " + tsTime.TotalSeconds);
                    }
                    HVsafe = false;
                }
                else if (rmsVoltage > MainsVoltageHIGH)
                {
                    if (!ErrorList.Contains("rms Voltage high"))
                    {
                        ErrorList.Add("rms Voltage high");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("rms voltage high" + tsTime.TotalSeconds);
                    }
                }
                else if (rmsVoltage < MainsVoltageLOW)
                {
                    if (!ErrorList.Contains("rms Voltage too low"))
                    {
                        ErrorList.Add("rms Voltage too low");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("rms voltage too low " + tsTime.TotalSeconds);
                    }
                }
                else if (rmsVoltage < MainsVoltageMIN)
                {
                    if (!ErrorList.Contains("rms Voltage low"))
                    {
                        ErrorList.Add("rms Voltage low");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("rms voltage low " + tsTime.TotalSeconds);
                    }
                }

                //ERROR 5
                if (rmsCurrent > MainsCurrentMAX)
                {
                    if (!ErrorList.Contains("rms Current too high"))
                    {
                        ErrorList.Add("rms Current too high");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("rms current too high " + tsTime.TotalSeconds);
                    }
                    HVsafe = false;
                }
                else if (rmsCurrent > MainsCurrentHIGH)
                {
                    if (!ErrorList.Contains("rms Current high"))
                    {
                        ErrorList.Add("rms Current high");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("rms current high " + tsTime.TotalSeconds);
                    }
                }
                else if (rmsCurrent < MainsCurrentLOW)
                {
                    if (!ErrorList.Contains("rms Current too low"))
                    {
                        ErrorList.Add("rms Current too low");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("rms current too low " + tsTime.TotalSeconds);
                    }
                }
                else if (rmsCurrent < MainsCurrentMIN)
                {
                    if (!ErrorList.Contains("rms Current low"))
                    {
                        ErrorList.Add("rms Current low");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("rms current low " + tsTime.TotalSeconds);
                    }
                }

                //ERROR 6
                if (rmsVoltage >= 30 && rmsCurrent == 0)
                {
                    if (!ErrorList.Contains("voltage but no current"))
                    {
                        ErrorList.Add("voltage but no current");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("voltage but no current (50V 0A treshhold) " + tsTime.TotalSeconds);
                    }
                    HVsafe = false;
                }

                /*
                //ERROR 7
                if (rmsCurrent >= 3 && rmsVoltage >= 20)
                {
                    if (!ErrorList.Contains("Current but no voltage"))
                    {
                        ErrorList.Add("Current but no voltage");
                        tsTime = Time.Elapsed;
                        Console.WriteLine("Current but no voltage (3A treshold) " + tsTime.TotalSeconds);
                    }
                    HVsafe = false;
                }
                */

                //ERROR 8
                TimeSpan ts = ARCtimingStopWatch.Elapsed;
                //Check every 100ms for sudden voltage drop wich can indicate a high voltage arc
                if (ts.Milliseconds >= ARCtime)
                {
                    ARCtimingStopWatch.Reset();
                    if (HVlast >= HighVoltage + ARC)
                    {
                        if (!ErrorList.Contains("Posible ARC dedected"))
                        {
                            ErrorList.Add("Posible ARC dedected");
                            tsTime = Time.Elapsed;
                            Console.WriteLine("Big voltage drop on high voltage line dedected - posible ARC! " + tsTime.TotalSeconds);
                        }
                        HVsafe = false;
                    }
                    HVlast = HighVoltage;
                }
            }


            //ERROR 9
            if (saStatus == false)
            {
                if (!ErrorList.Contains("SA pin low"))
                {
                    ErrorList.Add("SA pin low");
                    tsTime = Time.Elapsed;
                    Console.WriteLine("safety controler SAFE pin low - unsafe conditions dedected or inactive " + tsTime.TotalSeconds);
                }
                HVsafe = false;
            }

            //ERROR 10
            if (waStatus == true)
            {
                if (!ErrorList.Contains("WA pin high"))
                {
                    ErrorList.Add("WA pin high");
                    tsTime = Time.Elapsed;
                    Console.WriteLine("Safety controler warning signal - aproaching safe limits or posible error " + tsTime.TotalSeconds);
                }
            }

            //ERROR 11
            tsTime = Time.Elapsed;
            if (ConversionTickcount < 10000 && tsTime.TotalSeconds > SpeedDelay)
            {
                if (!ErrorList.Contains("Low speed"))
                {
                    ErrorList.Add("Low speed");
                    tsTime = Time.Elapsed;
                    Console.WriteLine("Thread low speed error (" + ConversionTickcount + "/s) " + tsTime.TotalSeconds);
                    HVsafe = false;
                }
            }
            //END error check
        }

        //clear errors
        //remove errors from list and set safe to true
        //if an error is still ocuring the safe bool with be false within a milisecond
        //so it is imposible to turn the high voltage on with a dedected error
        private void Button3_Click(object sender, EventArgs e)
        {
            HVsafe = true;
            listBox1.Items.Clear();
            ErrorList.Clear();

            tsTime = Time.Elapsed;

            Console.WriteLine("");
            Console.WriteLine("Error log cleared at " + tsTime.TotalSeconds);
            Console.WriteLine("");
        }

        //Communication and hardware checks/control
        //spi communication with the adc(MCP3008)
        //Check SA and WA pin status(high,low) of the safety controller (attiny85)
        //I2C code for vacuum sensing(NOT FINISHED/NOT WORKING)
        private void COMM()
        {
            //used to calculate thread speed
            Stopwatch SPIstopWatch = new Stopwatch();
            Int32 SPITicks = 0;

            /* for more information
             * mcp3008 datasheet 6.1
             * https://cdn-shop.adafruit.com/datasheets/MCP3008.pdf#G1.1036393
             * 
             * bin comm example (ring buffer)
             * 0000 0001  0CCC XXXX  XXXX XXXX (sent data single channel)
             * ???? ????  ???? ?0DD  DDDD DDDD (recieved data)
             * (C=channel select)
             * (D=adc data 0-->1024)
             * (X=dont care bits)
             * (?=0)
             * 
             */

            //note! data clocked out at rising edge en clocked in at falling edge

            //128 +16 per channel 
            byte[] mosiData = { 1, 128, 0 }; //CH0

            while (true)
            {
                SPIstopWatch.Start();

                //set frequency in hertz (500Khz min / 32Mhz max) (3.6Mhz on +5V / 1Mhz on +3.3V max freq for mcp3008)
                Pi.Spi.Channel0Frequency = 3000000;

                mosiData[1] = 128; //CH0
                MainsSens = Pi.Spi.Channel0.SendReceive(mosiData);

                mosiData[1] = Convert.ToByte(mosiData[1] + 16); //CH1
                HVSens = Pi.Spi.Channel0.SendReceive(mosiData);

                mosiData[1] = Convert.ToByte(mosiData[1] + 16); //CH2
                CurrentSens = Pi.Spi.Channel0.SendReceive(mosiData);

                mosiData[1] = Convert.ToByte(mosiData[1] + 16); //CH3
                TempSens = Pi.Spi.Channel0.SendReceive(mosiData);

                mosiData[1] = Convert.ToByte(mosiData[1] + 16); //CH4
                VCC52 = Pi.Spi.Channel0.SendReceive(mosiData);

                mosiData[1] = Convert.ToByte(mosiData[1] + 16); //CH5
                VCC12 = Pi.Spi.Channel0.SendReceive(mosiData);

                //TEST
                //spi comunication with geiger counter to get readings
                Pi.Spi.Channel1Frequency = 500000;

                //get wa and sa pin status
                saStatus = Pi.Gpio.Pin21.Read();
                waStatus = Pi.Gpio.Pin22.Read();

                //TICK COUNTER
                SPITicks++;
                TimeSpan ts = SPIstopWatch.Elapsed;
                if (ts.Seconds >= 1)
                {
                    SPIstopWatch.Reset();
                    SpiTickcount = SPITicks;
                    SPITicks = 0;
                }

                //OLD
                if (false)
                {
                    // Register a device on the bus
                    var myDevice = Pi.I2C.AddDevice(0x20);

                    return;

                    // Simple Write and Read (there are algo register read and write methods)
                    myDevice.Write(0x44);
                    var response = myDevice.Read();

                    Thread.Sleep(100);

                    return;
                    // List registered devices on the I2C Bus
                    foreach (var device in Pi.I2C.Devices)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Registered I2C Device: {device.DeviceId}");
                    }

                    Thread.Sleep(1000);
                }

                ComunicationThreadActive = true;
            }
        }

        //Screen update
        //Every label that displays data like current,time,... 
        //will gets is value updated every defined period (Few hundred miliseconds is good enough, best les then a second)
        private void screenUpdate()
        {
            while (true)
            {
                //change labels to data
                if (InvokeRequired)
                {
                    try
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            //Mains labels
                            if (("Mains rms voltage = " + Math.Round(rmsVoltage, 2) + "V") != lblMainsVoltage.Text)
                            {
                                lblMainsVoltage.Text = "Mains rms voltage = " + Math.Round(rmsVoltage, 0) + "VAC";
                            }

                            //Mains current
                            if (ErrorList.Contains("adc comunication error") && ConversionData[1, 0] == 0)
                            {
                                if ("Mains rms current = 0A" != lblMainsCurrent.Text) lblMainsCurrent.Text = "Mains rms current = 0A";
                            }
                            else
                            {
                                if (("Mains rms current = " + Math.Round(rmsCurrent, 0) + "A") != lblMainsCurrent.Text) //so it only updates when value changed
                                {
                                    if(Math.Round(rmsCurrent, 0) == 0)
                                    {
                                        lblMainsCurrent.Text = "Mains rms current = 0";
                                    }
                                    else
                                    {
                                        lblMainsCurrent.Text = "Mains rms current = " + Math.Round(rmsCurrent, 0) + "A";
                                    }
                                }
                            }

                            //High voltage label
                            //lblHV.Text = "HIGH VOLTAGE OUTPUT = " + Math.Round(HighVoltage, 0) + "V";
                            lblHV.Text = "HV OUT = " + Math.Round(rmsVoltage*50.55, 0) + "V";

                            //voltage labels
                            lbl12V.Text = "+12V Input voltage = " + Math.Round(V12, 2) + "V";
                            lbl5V2.Text = "+5V2 Input voltage = " + Math.Round(V5, 2) + "V";

                            //temp label
                            if (ThermistorResistance == 0)
                            {
                                //lblTemp.Text = "Temperature = ERROR";
                            }
                            else
                            {
                                //lblTemp.Text = "Temperature = " + Math.Round(OilTemperature, 1);
                                lblTemp.Text = "Temp = 32C°";
                            }


                            //attiny85 pins status
                            if (saStatus == false)
                            {
                                lblSAPIN.Text = "SA Pin = LOW";
                                lblSAPIN.ForeColor = Color.Red;
                            }
                            else if (saStatus == true)
                            {
                                lblSAPIN.ForeColor = Color.Green;
                                lblSAPIN.Text = "SA Pin = HIGH";
                            }
                            if (waStatus == false)
                            {
                                lblWAPIN.Text = "Wa Pin = LOW";
                                lblWAPIN.ForeColor = Color.Green;
                            }
                            else if (waStatus == true)
                            {
                                lblWAPIN.Text = "Wa Pin = HIGH";
                                lblWAPIN.ForeColor = Color.Red;
                            }

                            //Speed labels
                            lblCheckSpeed.Text = "Checksum speed = " + Convert.ToDouble(ConversionTickcount).ToString("N0") + "/s";
                            lblComSpeed.Text = "Comunication speed = " + Convert.ToDouble(SpiTickcount).ToString("N0") + "/s";

                            //update listbox
                            listBox1.Items.Clear();
                            listBox1.Items.AddRange(ErrorList.ToArray());

                            //Safety status
                            if (HVsafe == true)
                            {
                                lblSafe.Text = "safe";
                                lblSafe.ForeColor = Color.Lime;
                            }
                            else if (HVsafe == false)
                            {
                                lblSafe.Text = "unsafe";
                                lblSafe.ForeColor = Color.Red;
                            }
                            else
                            {
                                lblSafe.Text = "ERROR";
                                lblSafe.ForeColor = Color.Orange;
                            }

                            //HV status
                            if (HVon == true)
                            {
                                lblHVstatus.Text = "HIGH VOLTAGE:ON";
                                lblHVstatus.ForeColor = Color.Red;
                            }
                            else
                            {
                                lblHVstatus.Text = "HIGH VOLTAGE:OFF";
                                lblHVstatus.ForeColor = Color.Lime;
                            }

                            //Radiation stats
                            lblCPM5.Text = Convert.ToString("CPM(5s)   = " + CPM5);
                            lblCPM15.Text = Convert.ToString("CPM(15s) = " + CPM15);
                            lblCPM30.Text = Convert.ToString("CPM(30s) = " + CPM30);
                            lblCPM60.Text = Convert.ToString("CPM(60s) = " + CPM60);

                            lblSV5.Text = Convert.ToString("Rad last 5s   = " + Math.Round(SV5,2) + "µSv/h");
                            lblSV15.Text = Convert.ToString("Rad last 15s = " + Math.Round(SV15,2) + "µSv/h");
                            lblSV30.Text = Convert.ToString("Rad last 30s = " + Math.Round(SV30,2) + "µSv/h");
                            lblSV60.Text = Convert.ToString("Rad last 60s = " + Math.Round(SV60,2) + "µSv/h");

                            //set time since startup / also used for debugin and errors
                            TimeSpan tsTime = Time.Elapsed;
                            lblTime.Text = "Time = " + Convert.ToString(tsTime.Hours) + ":" + Convert.ToString(tsTime.Minutes) + ":" + Convert.ToString(tsTime.Seconds) + "," + Convert.ToString(tsTime.Milliseconds);
                            //lblTime.Text = "Time = " + Convert.ToString(Time.Elapsed);
                        }
                            ));
                    }
                    catch
                    {
                        Console.WriteLine("invoke error");
                    }


                }

                Thread.Sleep(scrUpdate);
            }
        }

        //exit en completly close program
        //will also turn of high voltage might it stil be on
        //ERROR closing on raspberry pi gives some errors sometimes and can take a few seconds to close. Nothing to worry about though(i hope)
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Sure you want to close aplication?", "EXIT", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Pi.Gpio.Pin01.PinMode = GpioPinDriveMode.Output;
                Pi.Gpio.Pin01.Write(true);
                if (RunningOnRaspberry)
                {
                    Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Output;
                    Pi.Gpio.Pin02.Write(false);

                    if (_serialPort.IsOpen) _serialPort.Close(); //Carshes or does weird when shuting down with port still open
                }
                HVon = false;
                Thread.Sleep(1000);
                Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Output;
                Pi.Gpio.Pin02.Write(true);
                Shutdown();
            }
            else if (dialogResult == DialogResult.No)
            {
                MessageBox.Show("Ok? why did you press exit then?");
            }
        }

        //HVon button
        private void SPI_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Turn on high voltage?", "HV ON", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (rmsVoltage < rmsMaxVon && HVsafe == true)
                {
                    HVon = true;
                }
                else
                {
                    MessageBox.Show("Unsafe conditions", "ERROR");
                }

            }
            else if (dialogResult == DialogResult.No)
            {
                MessageBox.Show("Make up your mind");
            }
        }
        //HVoff button
        private void btnHVoff_Click(object sender, EventArgs e)
        {
            if(RunningOnRaspberry)
            {
                Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Output;
                Pi.Gpio.Pin02.Write(false);
            }
            HVon = false;
        }

        //HVN TURN ON / OFF
        //Checks every 5ms if all is safe to keep high voltage on
        //Switching time of the relay is about 5ms acording to the datasheet
        //faster speed is not that crucial then
        private void HighVoltageOn()
        {
            if (RunningOnRaspberry)
            {
                while (true)
                {
                    if (ComunicationThreadActive == true && ConversionThreadActive == true && HVsafe == true && HVon == true)
                    {
                        //should be safe to turn on? I hope
                        /*
                        Pi.Gpio.Pin27.PinMode = GpioPinDriveMode.Output;
                        Pi.Gpio.Pin27.Write(GpioPinValue.High);
                        */
                        Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Output;
                        Pi.Gpio.Pin02.Write(true);

                    }
                    else if (HVsafe == false && HVon == true)
                    {
                        //safety error
                        Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Output;
                        Pi.Gpio.Pin02.Write(false); //shut down high voltage
                        Pi.Gpio.Pin01.PinMode = GpioPinDriveMode.Output;
                        Pi.Gpio.Pin01.Write(true); //beep buzzer
                        HVon = false;
                        MessageBox.Show("High voltage safety error!", "SAFETY ERROR");
                        Console.WriteLine("High voltage safety error!");
                        CreateLog();
                        Thread.Sleep(100);
                        Pi.Gpio.Pin01.PinMode = GpioPinDriveMode.Output;
                        Pi.Gpio.Pin01.Write(false); //stop buzzer
                    }
                    else if (ComunicationThreadActive == true && ConversionThreadActive == false && HVon == true)
                    {
                        Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Output;
                        Pi.Gpio.Pin02.Write(false);
                        Pi.Gpio.Pin01.PinMode = GpioPinDriveMode.Output;
                        Pi.Gpio.Pin01.Write(true); //beep buzzer
                        //speed error
                        MessageBox.Show("Thread too slow or not running", "Speed Error");
                        Console.WriteLine("Thread activity error");
                        HVon = false;
                        CreateLog();
                        Thread.Sleep(100);
                        Pi.Gpio.Pin01.PinMode = GpioPinDriveMode.Output;
                        Pi.Gpio.Pin01.Write(false); //stop buzzer
                    }
                    else
                    {
                        if (RunningOnRaspberry == true) Pi.Gpio.Pin27.PinMode = GpioPinDriveMode.Output; Pi.Gpio.Pin27.Write(false);
                    }
                    ConversionThreadActive = false; //will be set to true by conversion loop before this should run again. to check if all checks are still running

                    Thread.Sleep(5);
                }
            }
            //END
        }

        //Console commands
        private void ConsoleComs()
        {
            while (threadsActive == true)
            {
                String Command = Console.ReadLine();

                if (Command.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("");
                    Console.WriteLine("exit \t\t close program");
                    //Console.WriteLine("cal \t\t set calibration data"); //not used
                    Console.WriteLine("caldata \t get all stored calibration data");
                    Console.WriteLine("hvon \t\t turn on high voltage");
                    Console.WriteLine("hvoff \t\t turn off high voltage");
                    Console.WriteLine("status \t\t get values");
                    Console.WriteLine("clr \t clear error list");
                }
                else if (Command == "exit")
                {
                    Console.WriteLine("are you sure you want to exit? y/n");
                    Command = Console.ReadLine();

                    if (Command == "y" || Command == "yes")
                    {
                        Shutdown();
                    }
                    else Console.WriteLine("program continuing");
                }
                else if (Command == "caldata")
                {
                    for (int X = 0; X <= 5; X++)
                    {
                        Console.WriteLine("Calibration data channel " + X + "=" + CalibrationData[X]);
                    }
                }
                else if (Command == "hvon")
                {
                    if (rmsVoltage < rmsMaxVon && HVsafe == true)
                    {
                        HVon = true;
                        Console.WriteLine("HV on!");
                    }
                    else
                    {
                        Console.WriteLine("UNSAFE! HV turn on not posible");
                    }
                }
                else if (Command == "hvoff")
                {
                    Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Output;
                    Pi.Gpio.Pin02.Write(false);
                    HVon = false;
                }
                else if (Command == "status")
                {
                    Console.WriteLine("");
                    Console.Write("Time="); Console.WriteLine(Time.Elapsed);
                    Console.Write("hvsafe="); Console.WriteLine(HVsafe);
                    Console.Write("hvon="); Console.WriteLine(HVon);
                    Console.WriteLine("error list=");
                    Console.ForegroundColor = ConsoleColor.Red;
                    ErrorList.ForEach(Console.WriteLine);
                    Console.ResetColor();

                    Console.Write("MainsVoltage="); Console.WriteLine(Math.Round(rmsVoltage, 5) + "V");
                    Console.Write("MainsCurrent="); Console.WriteLine(Math.Round(rmsCurrent, 5) + "A");
                    Console.Write("V12="); Console.WriteLine(Math.Round(V12, 5) + "V");
                    Console.Write("V5="); Console.WriteLine(Math.Round(V5, 5) + "V");


                    if (ThermistorResistance == 0)
                    {
                        Console.WriteLine("temp=error");
                    }
                    else
                    {
                        Console.Write("temp="); Console.WriteLine(Math.Round(OilTemperature, 2) + "C");
                    }

                }
                else if (Command == "clr")
                {
                    HVsafe = true;
                    ErrorList.Clear();

                    Console.WriteLine("");
                    Console.WriteLine("Error log cleared");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("unkown command");
                }
            }
        }

        //Shutdown and exit btn
        private void Shutdown()
        {
            //Close down everything
            if (RunningOnRaspberry)
            {
                Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Output;
                Pi.Gpio.Pin02.Write(false);
                _serialPort.Close();
            }
            threadsActive = false;
            System.Windows.Forms.Application.Exit();
        }

        //RESTART btn
        private void button4_Click(object sender, EventArgs e)
        {
            if (RunningOnRaspberry) Pi.Gpio.Pin27.Write(false);
            HVon = false;
            Thread.Sleep(1000);
            System.Diagnostics.Process.Start(Application.ExecutablePath); // to start new instance of application
            this.Close(); //to turn off current app
        }

        //save log btn
        private void button1_Click_1(object sender, EventArgs e)
        {
            CreateLog();
        }

        //Log data thread. here all data is put into an array every defined delay and these will be stored in a text file
        private void LogData()
        {
            while (true)
            {
                //time
                TimeSpan tsTime = Time.Elapsed;
                LogTimestamp[LogTimestamp.Length - 1] = Convert.ToSingle(tsTime.TotalSeconds);
                Array.Copy(LogTimestamp, 1, LogTimestamp, 0, LogTimestamp.Length - 1);
                //Checksum tick count
                LogConversionTickcount[LogConversionTickcount.Length - 1] = ConversionTickcount;
                Array.Copy(LogConversionTickcount, 1, LogConversionTickcount, 0, LogConversionTickcount.Length - 1);
                //Main voltage
                LogMainsVoltage[LogMainsVoltage.Length - 1] = MainsVoltage;
                Array.Copy(LogMainsVoltage, 1, LogMainsVoltage, 0, LogMainsVoltage.Length - 1);
                //Main Current
                LogMainsCurrent[LogMainsCurrent.Length - 1] = MainsCurrent;
                Array.Copy(LogMainsCurrent, 1, LogMainsCurrent, 0, LogMainsCurrent.Length - 1);
                //High Voltage
                LogHighVoltage[LogHighVoltage.Length - 1] = HighVoltage;
                Array.Copy(LogHighVoltage, 1, LogHighVoltage, 0, LogHighVoltage.Length - 1);
                //Temperature and themistor resistance
                LogTemperature[LogTemperature.Length - 1] = OilTemperature;
                Array.Copy(LogTemperature, 1, LogTemperature, 0, LogTemperature.Length - 1);
                LogThermistorResistance[LogThermistorResistance.Length - 1] = ThermistorResistance;
                Array.Copy(LogThermistorResistance, 1, LogThermistorResistance, 0, LogThermistorResistance.Length - 1);
                //V5
                LogV5[LogV5.Length - 1] = V5;
                Array.Copy(LogV5, 1, LogV5, 0, LogV5.Length - 1);
                //V12
                LogV12[LogV12.Length - 1] = V12;
                Array.Copy(LogV12, 1, LogV12, 0, LogV12.Length - 1);

                Thread.Sleep(LogDelay);
            }
        }

        //create log of all recent data points to be able to debug if anything goes haywire
        private void CreateLog()
        {
            //create file name and path(difrent for windows and linux)
            if (RunningOnRaspberry == false)
            {
                //location of log file
                @LogFileName = Path.Combine(Application.StartupPath + @"\logs", DateTime.Now.ToString(@"yyyy-MM-dd HH-mm-ss") + @".txt");
                String TestFile = LogFileName.Replace(@"\\", @"\");
            }
            else
            {
                LogFileName = Path.Combine(Application.StartupPath + @"/logs", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt");
            }

            try
            {
                //create and open file
                using (StreamWriter file =
                        new StreamWriter(LogFileName, true))
                {
                    file.WriteLine("Log file");
                    file.WriteLine("");

                    //write data to file
                    long Index = Convert.ToInt64(LogDatapoints);
                    while (Index > 0 && LogTimestamp[Index - 1] > 1)
                    {
                        file.Write("Time=" + Convert.ToString(LogTimestamp[Index - 1]) + "\t");
                        file.Write("ChecksumSpeed/s=" + Convert.ToString(LogConversionTickcount[Index - 1]) + "\t");
                        file.Write("MainsVoltage=" + Convert.ToString(LogMainsVoltage[Index - 1]) + "\t");
                        file.Write("MainsCurrent=" + Convert.ToString(LogMainsCurrent[Index - 1]) + "\t");
                        file.Write("HighVoltage=" + Convert.ToString(LogHighVoltage[Index - 1]) + "\t");
                        file.Write("Temperature=" + Convert.ToString(LogTemperature[Index - 1]) + "\t");
                        file.Write("ThermistorResistance=" + Convert.ToString(LogThermistorResistance[Index - 1]) + "\t");
                        file.Write("V5=" + Convert.ToString(LogV5[Index - 1]) + "\t");
                        file.Write("V12=" + Convert.ToString(LogV12[Index - 1]) + "\t");
                        file.WriteLine("");

                        Index--;
                    }
                    Console.ResetColor();
                    Console.WriteLine("Log file created");
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("log file creation error");
                Console.ResetColor();
            }
        }

        //Serial communication test btn. shows all serial ports
        private void button2_Click(object sender, EventArgs e)
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }
        }

        //read any incoming serial data from geiger counter and calculate cpm en uSiever per hour
        //geiger counter / radiation dedection data reciever en calculator
        static SerialPort _serialPort;
        public void SerialCom()
        {
            float CPMtoSV = 0.0057F; //CPM multiplied by this number equals to micro sievert per hour (not sure if 100% correct value since i found multiple online)
            List<int> RadList = new List<int>();
            try
            {
                _serialPort = new SerialPort();
                _serialPort.PortName = "/dev/ttyS0";//Set your board COM
                _serialPort.BaudRate = 9600;
                _serialPort.Open();
                //port opened now going on the lookout for incomming data
                while (true)
                {
                    string SerialIn = _serialPort.ReadExisting();
                    Thread.Sleep(2000); //Data wont come out in one line without delay, Not sure why :(
                    if (SerialIn != "")
                    {
                        try
                        {
                            String Input = SerialIn.Substring(SerialIn.LastIndexOf("=") + 1, SerialIn.Length - SerialIn.LastIndexOf("=") - 1);
                            RadList.Add(Convert.ToInt16(Input));

                            //5s data
                            CPM5 = Convert.ToInt16(RadList.Last()) * 12; //Calculate CPM from 5s data(inacurate because of short period)
                            SV5 = Convert.ToSingle(CPM5 * CPMtoSV); //sievert calc

                            /*
                            //15s data
                            List<int> listCPM15 = RadList.GetRange(RadList.Count - 3, RadList.Count); //get last 3 values from list
                            CPM15 = Convert.ToInt16(listCPM15.Sum()*4);
                            SV15 = Convert.ToSingle(CPM15 * CPMtoSV);

                            //30s data
                            List<int> listCPM30 = RadList.GetRange(RadList.Count - 6, RadList.Count); //get last 3 values from list
                            CPM30 = Convert.ToInt16(listCPM30.Sum() * 2);
                            SV30 = Convert.ToSingle(CPM15 * CPMtoSV);

                            //60s data
                            List<int> listCPM60 = RadList.GetRange(RadList.Count - 12, RadList.Count); //get last 3 values from list
                            CPM60 = Convert.ToInt16(listCPM60.Sum());
                            SV60 = Convert.ToSingle(CPM60 * CPMtoSV);
                            */
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Radiation calculation error");
                            Console.ResetColor();
                        }
                    }
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Serial Communication ERROR");
                Console.WriteLine("Ending serial communication thread");
                Console.ResetColor();
            }
        }
        //END CODE
    }
}
