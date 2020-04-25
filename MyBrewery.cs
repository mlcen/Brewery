using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using OxyPlot;
using Brewery.Shared;

namespace Brewery
{
    public delegate void MyDelegate(string[] a);
    public class MashStagesCollection
    {
        // Declare an array to store the data elements.
        //private ImportantTemperatureHold[] arr = new ImportantTemperatureHold[4];
        // Define the indexer to allow client code to use [] notation.
        //public ImportantTemperatureHold this[int i]
        //{
        //    //get { return arr[i]; }
        //    //set { arr[i] = value; }
        //    get => arr[i];
        //    set => arr[i]=value;
        //}
        private readonly ImportantTemperatureHold[] arr;
        public MashStagesCollection(int length)
        {
            arr = new ImportantTemperatureHold[length];
        }
        // read-only version
        int nextIndex = 0;
        public ImportantTemperatureHold this[int index] => arr[index];

        public void Add(ImportantTemperatureHold value)
        {
            if (nextIndex >= arr.Length)
                throw new IndexOutOfRangeException($"The collection can hold only {arr.Length} elements.");
            arr[nextIndex++] = value;
        }

    }
    public class MyBrewery : INotifyPropertyChanged
    {
        public MashTun MashTun { get; set; }
        public FirstMash FirstMash { get; set; }
        public SecondMash SecondMash { get; set; }

        private ExternalDataHandling _externalDataHandling;

        public ExternalDataHandling ExternalDataHandling
        {
            get { return _externalDataHandling; }
            set { _externalDataHandling = value; }
        }
        private float _calibrationOfMashTunThermometerCorrection;

        public float CalibrationOfMashTunThermometerCorrectionfloat
        {
            get { return _calibrationOfMashTunThermometerCorrection; }
            set { _calibrationOfMashTunThermometerCorrection = value; OnPropertyChanged(nameof(CalibrationOfMashTunThermometerCorrectionfloat)); }
        }

        private float _mashTunTemperatureSettingFromModel;

        public float MashTunTemperatureSettingFromModel
        {
            get { return (float)Math.Round(_mashTunTemperatureSettingFromModel, 1); }
            set { _mashTunTemperatureSettingFromModel = value; OnPropertyChanged(nameof(MashTunTemperatureSettingFromModel)); }
        }
        private float _temperatureSettingFromModelOfMashing;

        public float TemperatureSettingFromModelOfMashing
        {
            get { return _temperatureSettingFromModelOfMashing; }
            set
            {
                _temperatureSettingFromModelOfMashing = value;
                ExternalDataHandling.SendDataToHardware(Controls.Heater, _temperatureSettingFromModelOfMashing.ToString());
                OnPropertyChanged(nameof(TemperatureSettingFromModelOfMashing));
            }
        }

        private string _newTemperatureSetting;
        public string NewTemperatureSetting
        {
            get { return _newTemperatureSetting; }
            set
            {
                _newTemperatureSetting = value;
                if (value == "Test")
                {
                    MashTun.waiting = true;
                }
                //temperature from model overriding
                //TemperatureSettingFromModelOfMashing = float.Parse(NewTemperatureSetting);

                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged(nameof(NewTemperatureSetting));
            }
        }

        private float _actualTemperatureOfMashTun;

        public float ActualTemperatureOfMashTun
        {
            get { return _actualTemperatureOfMashTun; }
            set
            {
                //if (value!=-127.00F)
                if (Math.Abs(value - ActualTemperatureOfMashTun) < 30.00 || ActualTemperatureOfMashTun == 0)
                {
                    _actualTemperatureOfMashTun = value + _calibrationOfMashTunThermometerCorrection;
                }
                OnPropertyChanged(nameof(ActualTemperatureOfMashTun));
            }
        }
        private float _actualTemperatureOfMashing;

        public float ActualTemperatureOfMashing
        {
            get { return _actualTemperatureOfMashing; }
            set { _actualTemperatureOfMashing = value; OnPropertyChanged(nameof(ActualTemperatureOfMashing)); }
        }

        private string[] _timeTemperaturePair;

        public string[] TimeTemperatureInfo
        {
            get { return _timeTemperaturePair; }
            set { _timeTemperaturePair = value; OnPropertyChanged(nameof(TimeTemperatureInfo)); }
        }

        private float _actualTime;

        public float ActualTime
        {
            get { return _actualTime; }
            set
            {
                _actualTime = value;

                if (BrewingStarted != 0)
                {
                    ProgressTime = (ActualTime - BrewingStarted * 60) / 60;
                }
            }
        }
        private float _brewingStarted;

        public float BrewingStarted
        {
            get { return _brewingStarted; }
            set { _brewingStarted = value / 60; ProgressTime = (ActualTime - BrewingStarted * 60) / 60; OnPropertyChanged(nameof(BrewingStarted)); }
        }

        private float _progressTime;

        public float ProgressTime
        {
            get { return (float)Math.Round(_progressTime, 1); }
            set { _progressTime = value; OnPropertyChanged(nameof(ProgressTime)); }
        }
        private float _progressOfMashIn;

        public float ProgressOfMashIn
        {
            get { return (float)Math.Round(_progressOfMashIn, 1); }
            set { _progressOfMashIn = value; OnPropertyChanged(nameof(ProgressOfMashIn)); }
        }

        private float _progressOfProtease;

        public float ProgressOfProtease
        {
            get { return (float)Math.Round(_progressOfProtease, 1); }
            set { _progressOfProtease = value; OnPropertyChanged(nameof(ProgressOfProtease)); }
        }

        private float _progressOfMashTunFirstAmylase;

        public float ProgressOfMashTunFirstAmylase
        {
            get { return _progressOfMashTunFirstAmylase; }
            set { _progressOfMashTunFirstAmylase = value; }
        }


        private float _progressOfFirstMash;

        public float ProgressOfFirstMash
        {
            get { return (float)Math.Round(_progressOfFirstMash, 1); }
            set { _progressOfFirstMash = value; OnPropertyChanged(nameof(ProgressOfFirstMash)); }
        }
        private float _progressOfSecondMash;

        public float ProgressOfSecondMash
        {
            get { return _progressOfSecondMash; }
            set { _progressOfSecondMash = value; OnPropertyChanged(nameof(ProgressOfSecondMash)); }
        }
        private float _progressOfFirstMashFirstAmylase;

        public float ProgressOfFirstMashFirstAmylase
        {
            get { return (float)Math.Round(_progressOfFirstMashFirstAmylase, 1); }
            set { _progressOfFirstMashFirstAmylase = value; OnPropertyChanged(nameof(ProgressOfFirstMashFirstAmylase)); }
        }
        private float _progressOfFirstMashSecondAmylase;

        public float ProgressOfFirstMashSecondAmylase
        {
            get { return (float)Math.Round(_progressOfFirstMashSecondAmylase, 1); }
            set { _progressOfFirstMashSecondAmylase = value; OnPropertyChanged(nameof(ProgressOfFirstMashSecondAmylase)); }
        }

        private float _progressOfFirstMashBoiling;

        public float ProgressOfFirstMashBoiling
        {
            get { return (float)Math.Round(_progressOfFirstMashBoiling, 1); }
            set { _progressOfFirstMashBoiling = value; OnPropertyChanged(nameof(ProgressOfFirstMashBoiling)); }
        }
        private float _progressSecondMashSecondAmylase;

        public float ProgressSecondMashSecondAmylase
        {
            get { return _progressSecondMashSecondAmylase; }
            set { _progressSecondMashSecondAmylase = value; OnPropertyChanged(nameof(ProgressSecondMashSecondAmylase)); }
        }
        private float _progressSecondMashBoiling;

        public float ProgressSecondMashBoiling
        {
            get { return _progressSecondMashBoiling; }
            set { _progressSecondMashBoiling = value; OnPropertyChanged(nameof(ProgressSecondMashBoiling)); }
        }

        private IList<DataPoint> _realSituationOfMashTun;
        public IList<DataPoint> RealSituationOfMashTun
        {
            get { return _realSituationOfMashTun; }
            private set { _realSituationOfMashTun = value; OnPropertyChanged(nameof(RealSituationOfMashTun)); }
        }
        private DataPoint _actualDataPointOfMashTun;

        public DataPoint ActualDataPointOfMashTun
        {
            get { return _actualDataPointOfMashTun; }
            set { _actualDataPointOfMashTun = value; OnPropertyChanged(nameof(RealSituationOfMashTun)); }
        }
        private IList<DataPoint> _realSituationOfMashing;

        public IList<DataPoint> RealSituationOfMashing
        {
            get { return _realSituationOfMashing; }
            private set { _realSituationOfMashing = value; }
        }
        private DataPoint _actualDataPointOfMashing;

        public DataPoint ActualDataPointOfMashing
        {
            get { return _actualDataPointOfMashing; }
            set { _actualDataPointOfMashing = value; }
        }
        private string _titleOfChart;

        public string TitleOfChart
        {
            get { return _titleOfChart; }
            set { _titleOfChart = value; OnPropertyChanged(nameof(TitleOfChart)); }
        }
        private bool _stirrerState;

        public bool StirrerState
        {
            get { return _stirrerState; }
            set { _stirrerState = value; ExternalDataHandling.SendDataToHardware(Controls.Stirrer, (_stirrerState == true ? 1 : 0).ToString()); OnPropertyChanged(nameof(StirrerState)); }
        }

        private int _invalidateFlag = 0;
        public int InvalidateFlag
        {
            get { return _invalidateFlag; }
            set { _invalidateFlag = value; OnPropertyChanged(nameof(InvalidateFlag)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #region ChartModelData
        public (float mashInTemperature, float proteaseTemperature, float firstAmylaseTemperature,
            float secondAmylaseTemperature, float boilingTemperature, float mashOutTemperature) temperatures =
             (
                mashInTemperature: 40,
                proteaseTemperature: 52,
                firstAmylaseTemperature: 62,
                secondAmylaseTemperature: 72,
                boilingTemperature: 100,
                mashOutTemperature: 85
            );
        public (float mashInTime, float mashInTimeHold, float proteaseTime,
            float proteaseTimehold, float firstMashFinishedTime, float secondMashFinishedTime, float mashOutTimeHold) timesMashTun =
         (
            mashInTime: 10,
            mashInTimeHold: 10,
            proteaseTime: 50,
            proteaseTimehold: 10,
            firstMashFinishedTime: 130,
            secondMashFinishedTime: 215,
            mashOutTimeHold: 10
        );
        public (float firstAmylaseTemperatureStartTime, float firstAmylaseTemperatureTimeHold, float secondAmylaseTemperatureStartTime,
             float secondAmylaseTemperatureHoldTime, float boilingStartTime, float boilingHoldTime) timesFirstMash =
            (
                firstAmylaseTemperatureStartTime: 25,
                firstAmylaseTemperatureTimeHold: 15,
                secondAmylaseTemperatureStartTime: 45,
                secondAmylaseTemperatureHoldTime: 15,
                boilingStartTime: 75,
                boilingHoldTime: 10
            //prvniRmutKonec: 150
            );
        public (float firstMashEndTime, float secondAmylaseTemperatureStartTime, float secondAmylaseHoldTime,
                float secondAmylaseEndTime, float boilingStartTime, float boilingHoldTime) timesSecondMash =
            (
                firstMashEndTime: 140,
                secondAmylaseTemperatureStartTime: 155,
                secondAmylaseHoldTime: 15,
                secondAmylaseEndTime: 205,
                boilingStartTime: 225,
                boilingHoldTime: 15
            //druhyRmutKonec: 215
            );
        public (short odrmutovaniVyhrev, short odrmutovaniDosazeniTeploty, short odrmutovaniKonec) timesOdrmutovani =
            (
                odrmutovaniVyhrev: 235,
                odrmutovaniDosazeniTeploty: 250,
                odrmutovaniKonec: 260
            );
        #endregion
        //public ChartViewModel chartViewModel = new ChartViewModel();
        internal ChartViewModel chartViewModel;

        //internal ExternalDataHandling dataHandling;
        //private static int time; 
        internal MyBrewery(ChartViewModel chartViewModel)
        {
            this.chartViewModel = chartViewModel;
            this.TitleOfChart = "Brewing Diagram";
            this.MashTun = new MashTun(temperatures, timesMashTun);
            this.FirstMash = new FirstMash(temperatures, timesFirstMash);
            this.SecondMash = new SecondMash(temperatures, timesSecondMash);
            this._externalDataHandling = new ExternalDataHandling(this);
            this.RealSituationOfMashTun = new List<DataPoint>();
            this.RealSituationOfMashing = new List<DataPoint>();
        }
        internal MyBrewery()
        {
            this.RealSituationOfMashTun = new List<DataPoint>();
        }
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)// => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        internal void ExtractTemperatureFromString(string str, string startSequence, string stopSequence)
        {
            if (str.StartsWith(startSequence) && str.EndsWith(stopSequence + $"\r"))
            {
                string[] serialArray = str.Substring(str.IndexOf('>') + 1, (str.LastIndexOf('<') - str.IndexOf('>')) - 1).Split(':');

                TimeTemperatureInfo = serialArray;
                if (float.TryParse(serialArray[0], out float timeValue) && float.TryParse(serialArray[1], out float temperatuerValue) && float.TryParse(serialArray[2], out float temperatuerValue1))
                {
                    //    if (float.Parse(serialArray[0]) / 60 < 5000)
                    //    {
                    ActualTime = timeValue;
                    ActualTemperatureOfMashTun = temperatuerValue;
                    ActualTemperatureOfMashing = temperatuerValue1;
                    ActualDataPointOfMashTun = new DataPoint(ProgressTime, ActualTemperatureOfMashTun);
                    ActualDataPointOfMashing = new DataPoint(ProgressTime, ActualTemperatureOfMashing);

                    WriteDataPoint(RealSituationOfMashTun, ActualDataPointOfMashTun);
                    WriteDataPoint(RealSituationOfMashing, ActualDataPointOfMashing);

                    if (MashTun.Finished == false && MashTun.waiting == false) MashTun.CheckProgressOfMashing(this);
                    else if (FirstMash.Finished == false) FirstMash.CheckProgressOfMashing(this);
                    else if (SecondMash.Finished == false) SecondMash.CheckProgressOfMashing(this);
                    //    }
                }
            }
        }
        internal void WriteDataPoint(IList<DataPoint> dataPoints, DataPoint dataPoint)
        {
            dataPoints.Add(dataPoint);
            InvalidateFlag++;
        }
        internal void WriteSimulatedData()
        {
        }
        internal void ControlBrewery()
        {

        }
    }
    public abstract class Mash
    {
        private bool _finished;

        public bool Finished
        {
            get { return _finished; }
            set { _finished = value; }
        }
        private byte _numberOfStages;

        public byte NumberOfStages
        {
            get { return _numberOfStages; }
            set { _numberOfStages = value; }
        }

        protected ImportantTemperatureHold _firstAmylaseActivation;

        public ImportantTemperatureHold FirstAmylaseActivation
        {
            get { return _firstAmylaseActivation; }
            set { _firstAmylaseActivation = value; }
        }
        protected ImportantTemperatureHold _secondAmylaseActivation;

        public ImportantTemperatureHold SecondAmylaseActivation
        {
            get { return _secondAmylaseActivation; }
            set { _secondAmylaseActivation = value; }
        }
        private MashStagesCollection _mashStagesCollection;

        public MashStagesCollection MashStagesCollection
        {
            get { return _mashStagesCollection; }
            set { _mashStagesCollection = value; }
        }
        public virtual void CheckProgressOfMashing(MyBrewery myBrewery)
        {
            if (Finished == false)
            {
                for (int stage = 0; stage < NumberOfStages; stage++)
                {
                    //myBrewery.MashTunTemperatureSettingFromModel = this.MashStagesCollection[stage].Temperature;
                    myBrewery.TemperatureSettingFromModelOfMashing = this.MashStagesCollection[stage].Temperature;
                    SetStartTime(this.MashStagesCollection[stage], myBrewery);
                    SetFinish(this.MashStagesCollection[stage], myBrewery);
                    if (MashStagesCollection[stage].Finished == false) break;
                }
            }
            else StepForwardToNextMashProcedure("Press OK to continue when ready!");
        }
        public virtual void SetStartTime(ImportantTemperatureHold importantTemperatureHold, MyBrewery myBrewery)
        {
            if (importantTemperatureHold.Reached == false)
            {
                if (importantTemperatureHold.Name == "Boiling")
                {
                    importantTemperatureHold.Reached = myBrewery.ActualTemperatureOfMashing >= importantTemperatureHold.Temperature - Shared.Constants.ToleranceOfSensor ? true : false;
                }
                else
                    importantTemperatureHold.Reached = myBrewery.ActualTemperatureOfMashing >= importantTemperatureHold.Temperature ? true : false;
                if (importantTemperatureHold.Reached)
                {
                    importantTemperatureHold.StartTime = myBrewery.ProgressTime;
                }
                return;
            }
        }
        public virtual void SetFinish(ImportantTemperatureHold importantTemperatureHold, MyBrewery myBrewery)
        {
            if (importantTemperatureHold.Reached == true && importantTemperatureHold.Elapsed == false)
            {
                importantTemperatureHold.Elapsed = myBrewery.ProgressTime >= importantTemperatureHold.StartTime + importantTemperatureHold.HoldingTime ? true : false;
                if (importantTemperatureHold.Elapsed == true)
                {
                    importantTemperatureHold.Finished = true;
                }
            }
        }
        protected virtual void StepForwardToNextMashProcedure(string message)
        {
            System.Windows.MessageBox.Show(message);
        }
    }
    public class MashTun : Mash
    {

        public static float startOfMash;

        private ImportantTemperatureHold _mashIn;

        public ImportantTemperatureHold MashIn
        {
            get { return _mashIn; }
            set { _mashIn = value; }
        }
        private ImportantTemperatureHold _proteaseActivation;

        public ImportantTemperatureHold ProteaseActivation
        {
            get { return _proteaseActivation; }
            set { _proteaseActivation = value; }
        }

        private ImportantTemperatureHold _mashOut;

        public ImportantTemperatureHold MashOut
        {
            get { return _mashOut; }
            set { _mashOut = value; }
        }
        public static byte actualStage;
        public static byte mashTunNumberOfStages;
        public static bool waiting;
        private (float mashInTemperature, float proteaseTemperature, float firstAmylaseTemperature, float secondAmylaseTemperature, float boilingTemperature, float mashOutTemperature) temperatures;
        private (float mashInTimeHold, float proteaseTime, float proteaseTimehold, float firstMashFinishedTime, float secondMashFinishedTime, float mashOutTimeHold) timesMashTun;

        public MashTun((float mashInTemperature, float proteaseTemperature, float firstAmylaseTemperature,
                        float secondAmylaseTemperature, float boilingTemperature, float mashOutTemperature) temperatures,
                        (float mashInTime, float mashInTimeHold, float proteaseTime,
                        float proteaseTimehold, float firstMashFinishedTime, float secondMashFinishedTime, float mashOutTimeHold) times)
        {
            this.NumberOfStages = 5;
            this.MashStagesCollection = new MashStagesCollection(NumberOfStages);
            this.MashIn = new ImportantTemperatureHold()
            {
                Temperature = temperatures.mashInTemperature,
                StartTime = times.mashInTime,
                HoldingTime = times.mashInTimeHold,
                Reached = false,
                Elapsed = false
            };
            this.ProteaseActivation = new ImportantTemperatureHold()
            {
                Temperature = temperatures.proteaseTemperature,
                StartTime = times.proteaseTime,
                HoldingTime = times.proteaseTimehold,
                Reached = false,
                Elapsed = false
            };
            this.FirstAmylaseActivation = new ImportantTemperatureHold()
            {
                Temperature = temperatures.firstAmylaseTemperature
            };
            this.SecondAmylaseActivation = new ImportantTemperatureHold()
            {
                Temperature = temperatures.secondAmylaseTemperature
            };
            this.MashOut = new ImportantTemperatureHold()
            {
                Temperature = temperatures.mashOutTemperature,
                HoldingTime = times.mashOutTimeHold
            };

            this.MashStagesCollection.Add(this.MashIn);
            this.MashStagesCollection.Add(this.ProteaseActivation);
            this.MashStagesCollection.Add(this.FirstAmylaseActivation);
            this.MashStagesCollection.Add(this.SecondAmylaseActivation);
            this.MashStagesCollection.Add(this.MashOut);
            actualStage = 0;
            mashTunNumberOfStages = 3;
        }

        public MashTun((float mashInTemperature, float proteaseTemperature, float firstAmylaseTemperature, float secondAmylaseTemperature, float boilingTemperature, float mashOutTemperature) temperatures, (float mashInTimeHold, float proteaseTime, float proteaseTimehold, float firstMashFinishedTime, float secondMashFinishedTime, float mashOutTimeHold) timesMashTun)
        {
            this.temperatures = temperatures;
            this.timesMashTun = timesMashTun;
        }

        public override void CheckProgressOfMashing(MyBrewery myBrewery)
        {
            //mashTunNumberOfStages = 3;
            if (Finished == false || waiting == false)
            {

                for (int stage = actualStage; stage < mashTunNumberOfStages; stage++)
                {
                    SetStartTime(this.MashStagesCollection[stage], myBrewery);
                    SetFinish(this.MashStagesCollection[stage], myBrewery);

                    switch (stage)
                    {
                        case 0:
                            if (MashStagesCollection[stage].Reached)
                            {
                                myBrewery.ProgressOfMashIn = myBrewery.ProgressTime - MashStagesCollection[stage].StartTime;
                            }
                            break;
                        case 1:
                            if (MashStagesCollection[stage].Reached)
                            {
                                myBrewery.ProgressOfProtease = myBrewery.ProgressTime - MashStagesCollection[stage].StartTime;
                            }
                            if (MashStagesCollection[stage].Elapsed)
                            {
                                this.StepForwardToNextMashProcedure($"Split one third. {Environment.NewLine}{Environment.NewLine} Click OK when ready!");
                                waiting = true;
                                FirstMash.startOfMash = myBrewery.ProgressTime;
                                myBrewery.ProgressOfFirstMash = myBrewery.ProgressTime - FirstMash.startOfMash;
                            }
                            break;
                        case 2:
                            if (MashStagesCollection[stage].Reached)
                            {
                                myBrewery.ProgressOfMashTunFirstAmylase = myBrewery.ProgressTime - MashStagesCollection[stage].StartTime;
                            }
                            if (MashStagesCollection[stage].Elapsed)
                            {
                                this.StepForwardToNextMashProcedure($"Split one third. {Environment.NewLine}{Environment.NewLine} Click OK when ready!");
                                waiting = false;
                                SecondMash.startOfMash = myBrewery.ProgressTime;
                                myBrewery.ProgressOfSecondMash = myBrewery.ProgressTime - SecondMash.startOfMash;
                            }
                            break;
                        default:
                            break;
                    }

                    myBrewery.MashTunTemperatureSettingFromModel = this.MashStagesCollection[stage].Temperature;

                    if (MashStagesCollection[stage].Finished == false) break;
                    else { actualStage += 1; }
                }
            }
            //else base.StepForwardToNextMashProcedure();
        }
        public override void SetStartTime(ImportantTemperatureHold importantTemperatureHold, MyBrewery myBrewery)
        {
            if (importantTemperatureHold.Reached == false)
            {
                importantTemperatureHold.Reached = myBrewery.ActualTemperatureOfMashTun >= importantTemperatureHold.Temperature ? true : false;
                if (importantTemperatureHold.Reached)
                {
                    importantTemperatureHold.StartTime = myBrewery.ProgressTime;
                }
                return;
            }
        }

        protected override void StepForwardToNextMashProcedure(string message)
        {
            System.Windows.MessageBox.Show(message);
        }
    }
    public class FirstMash : Mash
    {
        public static float startOfMash;
        public static byte actualStage;
        public static bool waiting;

        private ImportantTemperatureHold _boiling;

        public ImportantTemperatureHold Boiling
        {
            get { return _boiling; }
            set { _boiling = value; }
        }

        public FirstMash((float mashInTemperature, float proteaseTemperature, float firstAmylaseTemperature, float secondAmylaseTemperature, float boilingTemperature, float mashingOutTemperature) temperatures,
                    (float firstAmylaseTemperatureStartTime, float firstAmylaseTemperatureTimeHold, float secondAmylaseTemperatureStartTime,
             float secondAmylaseTemperatureHoldTime, float boilingStartTime, float boilingHoldTime) times)
        {

            this.NumberOfStages = 3;
            this.MashStagesCollection = new MashStagesCollection(NumberOfStages);

            FirstAmylaseActivation = new ImportantTemperatureHold()
            {
                Temperature = temperatures.firstAmylaseTemperature,
                HoldingTime = times.firstAmylaseTemperatureTimeHold,
                Reached = false,
                Elapsed = false
            };
            SecondAmylaseActivation = new ImportantTemperatureHold()
            {
                Temperature = temperatures.secondAmylaseTemperature,
                HoldingTime = times.secondAmylaseTemperatureHoldTime,
                Reached = false,
                Elapsed = false
            };
            Boiling = new ImportantTemperatureHold()
            {
                Temperature = temperatures.boilingTemperature,
                HoldingTime = times.boilingHoldTime
            };

            this.MashStagesCollection.Add(FirstAmylaseActivation);
            this.MashStagesCollection.Add(SecondAmylaseActivation);
            this.MashStagesCollection.Add(Boiling);


            //System.Windows.MessageBox.Show(base.FirstAmylaseActivation.ToString());
            //System.Windows.MessageBox.Show(this.SecondAmylaseActivation.ToString());

        }
        public override void CheckProgressOfMashing(MyBrewery myBrewery)
        {
            //mashTunNumberOfStages = 3;
            myBrewery.ProgressOfFirstMash = myBrewery.ProgressTime - FirstMash.startOfMash;

            if (Finished == false)
            {
                for (int stage = actualStage; stage < NumberOfStages; stage++)
                {
                    //myBrewery.MashTunTemperatureSettingFromModel = this.MashStagesCollection[stage].Temperature;
                    myBrewery.TemperatureSettingFromModelOfMashing = this.MashStagesCollection[stage].Temperature;

                    switch (stage)
                    {
                        case 0:
                            if (MashStagesCollection[stage].Reached)
                            {
                                myBrewery.ProgressOfFirstMashFirstAmylase = myBrewery.ProgressTime - MashStagesCollection[stage].StartTime;
                            }
                            break;
                        case 1:
                            if (MashStagesCollection[stage].Reached)
                            {
                                myBrewery.ProgressOfFirstMashSecondAmylase = myBrewery.ProgressTime - MashStagesCollection[stage].StartTime;
                            }
                            break;
                        case 2:
                            if (MashStagesCollection[stage].Reached)
                            {
                                myBrewery.ProgressOfFirstMashBoiling = myBrewery.ProgressTime - MashStagesCollection[stage].StartTime;
                            }
                            break;
                        default:
                            Finished = true;
                            break;
                    }

                    SetStartTime(this.MashStagesCollection[stage], myBrewery);
                    SetFinish(this.MashStagesCollection[stage], myBrewery);

                    if (MashStagesCollection[stage].Finished == false) { break; }
                    else actualStage += 1;
                }
                if (actualStage == NumberOfStages)
                {
                    Finished = true;
                    myBrewery.TemperatureSettingFromModelOfMashing = 0;
                    myBrewery.StirrerState = false;
                    waiting = true;
                    this.StepForwardToNextMashProcedure($"First mash has just finished!{Environment.NewLine}Don't Click Ok before you take out part of Mashing kettle and put a third again Mashin kettle!");
                }
            }
            //else this.StepForwardToNextMashProcedure("First mash has just finished!");
        }
        public FirstMash()
        { }
        protected override void StepForwardToNextMashProcedure(string message = "Nothing to say!")
        {
            MashTun.mashTunNumberOfStages = 5;
            System.Windows.MessageBox.Show(message);

        }
    }
    public class SecondMash : FirstMash
    {
        static new byte actualStage = 1;
        public SecondMash((float mashInTemperature, float proteaseTemperature, float firstAmylaseTemperature, float secondAmylaseTemperature, float boilingTemperature, float mashingOutTemperature) temperatures,
                    (float firstMashEndTime, float secondAmylaseTemperatureStartTime, float secondAmylaseHoldTime,
                float secondAmylaseEndTime, float boilingStartTime, float boilingHoldTime) times)

        {
            this.NumberOfStages = 3;
            this.MashStagesCollection = new MashStagesCollection(NumberOfStages);
            FirstAmylaseActivation = new ImportantTemperatureHold()
            {
                Temperature = temperatures.firstAmylaseTemperature,
                HoldingTime = 0,
                Reached = true,
                Elapsed = true
            };
            SecondAmylaseActivation = new ImportantTemperatureHold()
            {
                Temperature = temperatures.secondAmylaseTemperature,
                HoldingTime = times.secondAmylaseHoldTime,
                Reached = false,
                Elapsed = false
            };
            Boiling = new ImportantTemperatureHold()
            {
                Name = "Boiling",
                Temperature = temperatures.boilingTemperature,
                HoldingTime = times.boilingHoldTime
            };

            MashStagesCollection.Add(FirstAmylaseActivation);
            MashStagesCollection.Add(SecondAmylaseActivation);
            MashStagesCollection.Add(Boiling);
        }
        public override void CheckProgressOfMashing(MyBrewery myBrewery)
        {
            if (FirstMash.waiting == true) return; // To avoid heating and run while MEssageBox is blocking the code
            //mashTunNumberOfStages = 3;
            myBrewery.ProgressOfSecondMash = myBrewery.ProgressTime - SecondMash.startOfMash;

            if (Finished == false)
            {
                //actualStage = 1;
                for (int stage = actualStage; stage < NumberOfStages; stage++)
                {
                    //myBrewery.MashTunTemperatureSettingFromModel = this.MashStagesCollection[stage].Temperature;
                    myBrewery.TemperatureSettingFromModelOfMashing = this.MashStagesCollection[stage].Temperature;

                    switch (stage)
                    {
                        case 1:
                            if (MashStagesCollection[stage].Reached)
                            {
                                myBrewery.ProgressSecondMashSecondAmylase = myBrewery.ProgressTime - MashStagesCollection[stage].StartTime;
                            }
                            break;
                        case 2:
                            if (MashStagesCollection[stage].Reached)
                            {
                                myBrewery.ProgressSecondMashBoiling = myBrewery.ProgressTime - MashStagesCollection[stage].StartTime;
                            }
                            break;
                        default:
                            Finished = true;
                            break;
                    }

                    SetStartTime(this.MashStagesCollection[stage], myBrewery);
                    SetFinish(this.MashStagesCollection[stage], myBrewery);

                    if (MashStagesCollection[stage].Finished == false) { break; }
                    else actualStage += 1;
                }
                if (actualStage == NumberOfStages)
                {
                    Finished = true;
                    myBrewery.TemperatureSettingFromModelOfMashing = 0;
                    myBrewery.StirrerState = false;
                    this.StepForwardToNextMashProcedure($"Second mash has just finished!" +
                        $"{Environment.NewLine} Don't Click Ok before you take out part of Mashing kettle! {Environment.NewLine} When you are ready click to Run Second Mash! ");
                }
            }
            //else base.StepForwardToNextMashProcedure("Second mash has just finnished!");
        }
        protected override void StepForwardToNextMashProcedure(string message = "Nothing to say!")
        {
            MashTun.mashTunNumberOfStages = 5;
            System.Windows.MessageBox.Show(message);
            MashTun.waiting = false;
        }
    }

    public class ImportantTemperatureHold
    {
        //private float temperature;
        //private float holdingTime;
        //private float started;
        //private bool reached;
        //private bool elapsed;

        //public ImportantTemperatureHold(float temperature, float holdingTime, float started, bool reached, bool elapsed)
        //{
        //    this.temperature = temperature;
        //    this.holdingTime = holdingTime;
        //    this.started = started;
        //    this.reached = reached;
        //    this.elapsed = elapsed;
        //}
        public string Name { get; set; }
        public float StartTime { get; set; }
        public float Temperature { get; set; }
        public float HoldingTime { get; set; }
        public bool Reached { get; set; }
        public bool Elapsed { get; set; }
        public bool Finished { get; set; }


        public override string ToString()
        {
            return $"Temperature: {this.Temperature}, Reached: {this.Reached} HoldingTime: {this.HoldingTime} Elapsed: {this.Elapsed}";
        }
        public void CheckPtogress(MyBrewery myBrewery)
        {
            if (this.Reached == false)
            {
                myBrewery.MashTunTemperatureSettingFromModel = this.Temperature;

                this.Reached = myBrewery.ActualTemperatureOfMashTun >= this.Temperature ? true : false;
                if (this.Reached)
                {
                    this.StartTime = myBrewery.ProgressTime;
                }
            }
            else if (this.Elapsed == false)
            {
                this.Elapsed = myBrewery.ProgressTime >= (this.StartTime + this.HoldingTime) ? true : false;

                if (this.Elapsed)
                {
                    Finished = true;
                }
            }
        }
        public void SetTemperature()
        {

        }
        public void SetStartTime()
        {

        }
    }
    class ImportRecipe{}
    class Recipe { }
}