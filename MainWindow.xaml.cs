using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using System.IO.Ports;
using System.IO;
using OxyPlot;

namespace Brewery
{

    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MyBrewery myBrewery;
        //internal ExternalDataHandling externalDataHandling;
        internal ChartViewModel chartViewModel;

        public MainWindow()
        {
            InitializeComponent();
            //myBrewery = new MyBrewery();
            chartViewModel = new ChartViewModel();
            myBrewery = new MyBrewery(chartViewModel);
            //externalDataHandling = new ExternalDataHandling(myBrewery);
            //MyBrewery
            plot.DataContext = myBrewery;
            actualMashTunTemperature.DataContext = myBrewery;
            actualTemperatureOfMashing.DataContext = myBrewery;
            setMashTunTemperature.DataContext = myBrewery;
            setTemperatureOfMashing.DataContext = myBrewery;
            realTemperatureOfMashTun.DataContext = myBrewery;
            realTemperatureOfMashing.DataContext = myBrewery;
            newTemperatureSetting.DataContext = myBrewery;
            outputFromHardware.DataContext = myBrewery;
            stirrerState.DataContext = myBrewery;
            txtBoxCalibrationOfMashTunThermometer.DataContext = myBrewery;
            // Progress times
            progressTime.DataContext = myBrewery;
            progressTimeOfFirstMash.DataContext = myBrewery;
            progressOfSecondMash.DataContext = myBrewery;

            progressTimeMashIn.DataContext = myBrewery;
            progressTimeProtease.DataContext = myBrewery;
            progressMashTunProtease.DataContext = myBrewery;
            progressMashTunFirstAmylase.DataContext = myBrewery;

            progressTimeFirstMashFirstAmylase.DataContext = myBrewery;
            progressTimeFirstMashSecondAmylase.DataContext = myBrewery;
            progressTimeFirstMashBoiling.DataContext = myBrewery;

            progressOfSecondMashSecondAmylase.DataContext = myBrewery;
            progressOfSecondMashBoiling.DataContext = myBrewery;
            //ChartModel

            mashTun.DataContext = chartViewModel;
            firstMash.DataContext = chartViewModel;
            secondMash.DataContext = chartViewModel;
            
            //ExternalComunication
            connect.DataContext = myBrewery.ExternalDataHandling;
            commErrors.DataContext = myBrewery.ExternalDataHandling;
            comPort.DataContext = myBrewery.ExternalDataHandling;
            outputFromHardware.DataContext = myBrewery.ExternalDataHandling;
            //Controls
            //turnStirerOff.DataContext = myBrewery;
        }
        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {

            if (myBrewery.ExternalDataHandling.ActualConnectionState == ButtonConnectVsDisconnect.Connect)
            {
                myBrewery.ExternalDataHandling.ConnectToComPort();
            }
            else
            {
                try // just in case serial port is not open could also be acheved using if(serial.IsOpen)
                {
                    myBrewery.ExternalDataHandling.DisconnectFromComPort();
                }
                catch
                {
                }
            }
        }
        private void SendDataToHardware_Click(object sender, RoutedEventArgs e)
        {
            //string newTemperatureSettingToBeSend = newTemperatureSetting.Text;
            //externalDataHandling.SendDataToHardware(newTemperatureSettingToBeSend);
            myBrewery.ExternalDataHandling.SendDataToHardware(Controls.Heater,newTemperatureSetting.Text);
        }

        private void StartBrewing_Click(object sender, RoutedEventArgs e)
        {
            myBrewery.BrewingStarted = myBrewery.ActualTime;
            myBrewery.ExternalDataHandling.SendDataToHardware(Controls.Heater, myBrewery.TemperatureSettingFromModelOfMashing.ToString());
            //myBrewery.BrewingStarted = 10;
        }
        private void Scroll(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.ScrollToEnd();
        }

        private void TurnCoolingPumpOn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TurnHeaterOn_Click(object sender, RoutedEventArgs e)
        {
            myBrewery.ExternalDataHandling.SendDataToHardware();
        }
        private void StartFirstMash_Click(object sender, RoutedEventArgs e)
        {
            myBrewery.MashTun.MashIn.Reached = true;
            myBrewery.MashTun.MashIn.Elapsed = true;
            myBrewery.MashTun.ProteaseActivation.Reached = true;
            myBrewery.MashTun.ProteaseActivation.Elapsed = true;
            MashTun.waiting = true;
        }

        private void StartSeconMash_Click(object sender, RoutedEventArgs e)
        {
            FirstMash.waiting = false;
        }

        private void ExportData_Click(object sender, RoutedEventArgs e)
        {
           var exportMashing = myBrewery.RealSituationOfMashing;
            using (System.IO.FileStream fs = new System.IO.FileStream(System.IO.Path.Combine(@"C:\Users\radek.lehnert\OneDrive - ALS Limited\Documents", "exportOfMashing.csv"), FileMode.Append))
            {
                using (StreamWriter streamWriter = new StreamWriter(fs))
                {
                    foreach (var kv in exportMashing)
                    {
                        streamWriter.WriteLine($"{kv.X.ToString()}:{kv.Y.ToString()}");
                    }
                }
            }
        }
    }
}
