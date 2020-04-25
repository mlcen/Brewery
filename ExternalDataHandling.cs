using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;

namespace Brewery
{
    public static class Constants
    {
        public const string startSequence = "<transmision>";
        public const string endSequence = "</transmision>";
    }
    public enum ButtonConnectVsDisconnect
    {
        Connect,
        Disconnect
    }
    public enum Controls { TurboHeat = 8, PinOfSensorsDS = 9, Stirrer = 10, Pump = 11, Heater = 12 }
    public delegate string Print(string[] arr);

    struct Transmision {  string startSequence, endSequence;  }

    public class ExternalDataHandling : INotifyPropertyChanged
    {
        public StringBuilder stringBuilderForErrors;
        //Serial 
        private SerialPort _serial;

        public SerialPort Serial
        {
            get { return _serial; }
            set { _serial = value; }
        }

        //SerialPort serial;
        //string recieved_data;
        #region Properties
        private MyBrewery _myBrewery;

        public MyBrewery MyBrewery
        {
            get { return _myBrewery; }
            set { _myBrewery = value; }
        }

        private ButtonConnectVsDisconnect _actualConnectionState;

        public ButtonConnectVsDisconnect ActualConnectionState
        {
            get { return _actualConnectionState; }
            set
            {
                _actualConnectionState = value;
                OnPropertyChanged(nameof(ActualConnectionState));
            }
        }

        private string _newTemperatureSetting;
        public string NewTemperatureSetting
        {
            get { return _newTemperatureSetting; }
            set
            {
                _newTemperatureSetting = value;
                OnPropertyChanged(nameof(NewTemperatureSetting));
            }
        }
        
        private string _outPutFromHardware;

        public string OutPutFromHardware
        {
            get { return _outPutFromHardware; }
            set { _outPutFromHardware = value; OnPropertyChanged(nameof(OutPutFromHardware));}
        }

        private string _commErrors;

        public string CommErrors
        {
            get { return _commErrors; }
            set
            {
                _commErrors = value;
                OnPropertyChanged(nameof(CommErrors));
            }
        }
        private string _comPort;

        public string ComPort
        {
            get { return _comPort; }
            set
            {
                if (_comPort == null) _comPort = "COM12";
                else {_comPort = value;}
                OnPropertyChanged(nameof(ComPort));
            }
        }
        private int _baudRate;
        public int BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        } 
        #endregion

        public StringBuilder stringBuilder;

        internal ExternalDataHandling(MyBrewery myBrewery)
        {
            this.stringBuilderForErrors = new StringBuilder();
            //Set up myBrewery
            this._myBrewery = myBrewery;
            //Sets up serial port
            this._serial = new SerialPort();
            this._serial.PortName ="COM14";
            this._serial.BaudRate = 9600;
            this._serial.Parity = Parity.None;
            this._serial.DataBits = 8;
            this._serial.StopBits = StopBits.One;
            this._serial.ReadTimeout = 5000;
            this._serial.WriteTimeout = 50;
            //this.serial.Handshake = Handshake.None;
            this.stringBuilder = new StringBuilder();
            ActualConnectionState = (ButtonConnectVsDisconnect)ButtonConnectVsDisconnect.Connect;
            //Sets button State and Creates function call on data recieved

            this._serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Recieve);

        }
        public event PropertyChangedEventHandler PropertyChanged;
       
        #region Connection
        internal void ConnectToComPort()
        {
            if (!_serial.IsOpen)
            {
                try
                {
                    _serial.Open();
                    ActualConnectionState = ButtonConnectVsDisconnect.Disconnect;
                }
                catch (System.IO.IOException ex)
                {
                    System.Windows.MessageBox.Show($"{ex.ToString()}", "What next?",
                                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.OK);
                }
            }
            else System.Windows.MessageBox.Show("ComPort is already used for Brewery", "Everything just fine",
                                                System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Information);
        }
        internal void DisconnectFromComPort()
        {
            _serial.Close();
            ActualConnectionState = ButtonConnectVsDisconnect.Connect;
        }
        #endregion

        #region Recieving

        //private delegate void UpdateUiTextDelegate(string text);

        private void Recieve(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {

                MyBrewery.ExtractTemperatureFromString(_serial.ReadLine(), Constants.startSequence, Constants.endSequence);
                WriteOutPutToViewModel();
                //WriteDataToViewModel();

            }
            catch (TimeoutException ex)
            {
                stringBuilderForErrors.Append($"{DateTime.Now} This was red {_serial.ReadExisting()} {ex.ToString()}\r");
                CommErrors = stringBuilderForErrors.ToString();
            }
            catch (System.IO.IOException ex)
            {
                stringBuilderForErrors.Append($"{DateTime.Now} {ex.ToString()}\r");
                CommErrors = stringBuilderForErrors.ToString();
            }
            finally
            {
                //serial.Close();
            }
                //dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), recieved_data);
        }

        //public void WriteDataToViewModel()
        //{
        //    NewTemperatureSetting = MyBrewery.NewTemperatureSetting;
        //    //SendDataToHardware(MyBrewery.NewTemperatureSetting);
        //    SendDataToHardware(MyBrewery.TemperatureSettingFromModelOfMashing.ToString());
        //}

        #endregion

        #region Sending        
        public void SendDataToHardware(string data)
        {
            if (_serial.IsOpen)
            {
                try
                {
                    _serial.WriteLine(data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                //msgBox please Connect Device
            }
        }
        public void SendDataToHardware()
        {
            if (_serial.IsOpen)
            {
                try
                {
                    _serial.WriteLine(NewTemperatureSetting);
                }
                catch (Exception ex)
                {
                    stringBuilderForErrors.Append($"{DateTime.Now} This was not write as writeLine {NewTemperatureSetting} {ex.ToString()}\r");
                }
            }
            else
            {
                // msgbox please connect
            }
        }
        public void SendDataToHardware(Controls controls, string state)
        {
            if (_serial.IsOpen)
            {
                try
                {
                    string tmp = $"{(int)controls}:{state}";
                    _serial.WriteLine($"{(int)(controls)}:{state}");
                }
                catch (Exception ex)
                {
                    stringBuilderForErrors.Append($"{DateTime.Now} This was not write as writeLine {controls}:{state} {ex.ToString()}\r");
                }
            }
            else
            {
                // msgbox please connect
            }
        }
        public void WriteOutPutToViewModel()
        {
            if (MyBrewery.TimeTemperatureInfo != null)
            {
                try
                {
                    stringBuilder.Append($"{MyBrewery.TimeTemperatureInfo[0]} : {MyBrewery.TimeTemperatureInfo[1]} : {MyBrewery.TimeTemperatureInfo[2]} : {MyBrewery.TimeTemperatureInfo[3]} : {MyBrewery.TimeTemperatureInfo[4]} {Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    stringBuilderForErrors.Append($"{DateTime.Now} This was red {Serial.ReadExisting()} {ex.ToString()}\r");
                    System.Windows.MessageBox.Show($"{ex.ToString()} {Environment.NewLine} Continue anyway?");
                    CommErrors = stringBuilderForErrors.ToString();
                }

            }
            OutPutFromHardware = stringBuilder.ToString();
        }
        #endregion
        public void OnPropertyChanged(string name)
        {
            System.Diagnostics.Debug.Assert(name!="ConnectionState");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #region Garbage
        //public void SendDataToHardware(ref Paragraph para, string data)
        //{
        //    if (serial.IsOpen)
        //    {
        //        try
        //        {
        //            // Send the binary data out the port
        //            byte[] hexstring = Encoding.ASCII.GetBytes(data);
        //            //There is a intermitant problem that I came across
        //            //If I write more than one byte in succesion without a 
        //            //delay the PIC i'm communicating with will Crash
        //            //I expect this id due to PC timing issues ad they are
        //            //not directley connected to the COM port the solution
        //            //Is a ver small 1 millisecound delay between chracters
        //            foreach (byte hexval in hexstring)
        //            {
        //                byte[] _hexval = new byte[] { hexval }; // need to convert byte to byte[] to write
        //                serial.Write(_hexval, 0, 1);
        //                Thread.Sleep(1);
        //            }
        //            serial.Write(NewTemperatureSetting.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            para.Inlines.Add("Failed to SEND" + data + "\n" + ex + "\n");
        //        }
        //    }
        //    else
        //    {
        //    }
        //}
        //private void WriteData(string text)
        //{
        //    // Assign the value of the recieved_data to the RichTextBox.
        //    //para.Inlines.Add(text);
        //    //mcFlowDoc.Blocks.Add(para);
        //    //outputFromHardware.Document = mcFlowDoc;
        //    OutPutFromHardware = serial.ReadExisting().ToString();
        //} 
        #endregion
    }
}
