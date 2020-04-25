using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using System.ComponentModel;

namespace Brewery
{
    public class ChartViewModel//:INotifyPropertyChanged
    {
        public string Title { get;  set; }
        public TimeValues TimeValues { get; set; }
        public IList<DataPoint> MashTun { get; private set; }
        public IList<DataPoint> FirstMash { get; private set; }
        public IList<DataPoint> SecondMash { get; private set; }
        public MashTun MashTunBrewDiagram { get; set; }
        public FirstMash FirstMashBrewDiagram { get; set; }
        public SecondMash SecondMashBrewDiagram { get; set; }
        public IList<DataPoint> WholeMash { get; private set; }
        public IList<DataPoint> RealSituationMashTun { get; private set; }
        public IList<DataPoint> RealSituationMashing { get; private set; }
       
        public DataPoint startOfHeatingFrom62C = new DataPoint(30,55);

        public MyBrewery myBrewRecipe;

        float tapWaterTemperature = 15;
        float downTime = 10;
        #region Ramps
        float maxRampTo55degC = 18;
        float maxRampTo65degC = 15;
        float maxRampTo72degC = 12;
        float maxRampTo100degC = 7;
        #endregion

        //public event PropertyChangedEventHandler PropertyChanged;
        //MyDelegate myDelegate = new MyDelegate(WriteDataPoint);
        public ChartViewModel()
        {
            this.myBrewRecipe = new MyBrewery();
            this.TimeValues = new TimeValues(myBrewRecipe, tapWaterTemperature);
            this.MashTunBrewDiagram = new MashTun(myBrewRecipe.temperatures, myBrewRecipe.timesMashTun);
            this.FirstMashBrewDiagram = new FirstMash(myBrewRecipe.temperatures, myBrewRecipe.timesFirstMash);
            this.SecondMashBrewDiagram = new SecondMash(myBrewRecipe.temperatures, myBrewRecipe.timesSecondMash);
            this.Title = "Brewing diagram";


            #region Heating
            float timeFromTapWaterToMashIn = (myBrewRecipe.temperatures.mashInTemperature - tapWaterTemperature) / maxRampTo55degC;
            float timeFromMashInToProtease = (myBrewRecipe.temperatures.proteaseTemperature - myBrewRecipe.temperatures.mashInTemperature) / maxRampTo65degC;
            float timeFromProteaseToFirstAmylase = (myBrewRecipe.temperatures.firstAmylaseTemperature - myBrewRecipe.temperatures.proteaseTemperature) / maxRampTo65degC;
            float timeFromFirstAmylaseToSecondAmylase = (myBrewRecipe.temperatures.secondAmylaseTemperature - myBrewRecipe.temperatures.firstAmylaseTemperature) / maxRampTo72degC;
            float timeFromSecondAmylaseToBoiling = (myBrewRecipe.temperatures.boilingTemperature - myBrewRecipe.temperatures.secondAmylaseTemperature) / maxRampTo100degC; 
            #endregion
            // dtemprary 10 minutes for prepousteni
            //myDelegate+= WriteDataPoint();
            this.MashTun = new List<DataPoint> {
                new DataPoint(TimeValues.Start,tapWaterTemperature),
                new DataPoint(TimeValues.HeatToMashIn, tapWaterTemperature),
                new DataPoint(TimeValues.MashInStart, MashTunBrewDiagram.MashIn.Temperature),
                new DataPoint(TimeValues.HeatToProteaseActivation, MashTunBrewDiagram.MashIn.Temperature),
                new DataPoint(TimeValues.ProteaseStart, MashTunBrewDiagram.ProteaseActivation.Temperature),
                new DataPoint(TimeValues.FirstMashBoilingEnd,MashTunBrewDiagram.ProteaseActivation.Temperature),
                new DataPoint(TimeValues.FirstMashBoilingEnd + TimeValues.downTime, MashTunBrewDiagram.FirstAmylaseActivation.Temperature),
                new DataPoint(TimeValues.HeatToSecondMashSecondAmylaseActivation, MashTunBrewDiagram.FirstAmylaseActivation.Temperature),
                new DataPoint(TimeValues.SecondMashBoilingEnd, MashTunBrewDiagram.FirstAmylaseActivation.Temperature),
                new DataPoint(TimeValues.SecondMashBoilingEnd + TimeValues.downTime, MashTunBrewDiagram.SecondAmylaseActivation.Temperature),
                new DataPoint(TimeValues.HeatToMashOutActivation + TimeValues.downTime, MashTunBrewDiagram.SecondAmylaseActivation.Temperature),
                new DataPoint(TimeValues.MashOutStart, MashTunBrewDiagram.MashOut.Temperature),
                new DataPoint(TimeValues.MashOutEnd, MashTunBrewDiagram.MashOut.Temperature)
            };
            this.FirstMash = new List<DataPoint>
            {
                new DataPoint(TimeValues.HeatToFirstMashFirstAmylaseActivation, MashTunBrewDiagram.ProteaseActivation.Temperature),
                new DataPoint(TimeValues.FirstMashFirstAmylaseStart, FirstMashBrewDiagram.FirstAmylaseActivation.Temperature),
                new DataPoint(TimeValues.HeatToFirstMashSecondAmylaseActivation, FirstMashBrewDiagram.FirstAmylaseActivation.Temperature),
                new DataPoint(TimeValues.FirstMashSecondAmylaseStart,FirstMashBrewDiagram.SecondAmylaseActivation.Temperature),
                new DataPoint(TimeValues.FirstMashHeatToBoilingTemperature,FirstMashBrewDiagram.SecondAmylaseActivation.Temperature),
                new DataPoint(TimeValues.FirstMashBoilingStart,FirstMashBrewDiagram.Boiling.Temperature),
                new DataPoint(TimeValues.FirstMashBoilingEnd,FirstMashBrewDiagram.Boiling.Temperature),
                new DataPoint(TimeValues.FirstMashBoilingEnd + TimeValues.downTime, MashTunBrewDiagram.FirstAmylaseActivation.Temperature)
            };
            this.SecondMash = new List<DataPoint> 
            {
                new DataPoint(TimeValues.HeatToSecondMashSecondAmylaseActivation, MashTunBrewDiagram.FirstAmylaseActivation.Temperature),
                new DataPoint(TimeValues.SecondMashSecondAmylaseStart, SecondMashBrewDiagram.SecondAmylaseActivation.Temperature),
                new DataPoint(TimeValues.SecondMashHeatToBoiling, SecondMashBrewDiagram.SecondAmylaseActivation.Temperature),
                new DataPoint(TimeValues.SecondMashBoilingStart, SecondMashBrewDiagram.Boiling.Temperature),
                new DataPoint(TimeValues.SecondMashBoilingEnd, SecondMashBrewDiagram.Boiling.Temperature),
                new DataPoint(TimeValues.SecondMashBoilingEnd + TimeValues.downTime, MashTunBrewDiagram.SecondAmylaseActivation.Temperature)
            };
            this.RealSituationMashTun = new List<DataPoint>();
            this.RealSituationMashing = new List<DataPoint>();

        }
    }
    public struct TimeValues
    {
        public float downTime;
        //public float firstMashBoilingDuration;
        #region Ramps
        public MyBrewery MyBrewRecipe { get; set; }
        public Heating HeatingCharacteristics 
        { 
          get ;
          set ;  
        }
        public float Start { get; set; }
        public float HeatToMashIn 
        { 
            get { return Start; }
            set { ; }
        }
        public float MashInStart 
        { 
            get {return Start + HeatToMashIn + HeatingCharacteristics.timeFromTapWaterToMashIn; } 
            set {; } 
        }
        public float HeatToProteaseActivation
        {
            get {return MashInStart + MyBrewRecipe.timesMashTun.mashInTimeHold; }
            set {; }
        }
        public float ProteaseStart 
        {
            get { return HeatToProteaseActivation + HeatingCharacteristics.timeFromMashInToProtease; }
            set {; }
        }
        public float HeatToFirstMashFirstAmylaseActivation
        {
            get {return ProteaseStart + MyBrewRecipe.timesMashTun.proteaseTimehold; }
            set {; }
        }
        public float FirstMashFirstAmylaseStart 
        { 
            get {return HeatToFirstMashFirstAmylaseActivation + HeatingCharacteristics.timeFromProteaseToFirstAmylase; }
            set {; }
        }
        public float HeatToFirstMashSecondAmylaseActivation 
        {
            get {return FirstMashFirstAmylaseStart + MyBrewRecipe.timesFirstMash.firstAmylaseTemperatureTimeHold; }
            set {; }
        }
        public float FirstMashSecondAmylaseStart 
        {
            get {return HeatToFirstMashSecondAmylaseActivation +
                                                HeatingCharacteristics.timeFromFirstAmylaseToSecondAmylase; }
            set {; }
        }
        public float FirstMashHeatToBoilingTemperature 
        { 
            get {return FirstMashSecondAmylaseStart + MyBrewRecipe.timesFirstMash.secondAmylaseTemperatureHoldTime; }
            set {; }
        }
        public float FirstMashBoilingStart
        {
            get { return FirstMashHeatToBoilingTemperature + HeatingCharacteristics.timeFromSecondAmylaseToBoiling; }
            set {; }
        }
        public float FirstMashBoilingEnd 
        {
            get {return FirstMashBoilingStart + MyBrewRecipe.timesFirstMash.boilingHoldTime; }
            set {; }
        }
        public float HeatToSecondMashSecondAmylaseActivation
        {
            get { return FirstMashBoilingEnd + downTime + HeatingCharacteristics.timeFromFirstAmylaseToSecondAmylase; }
        }
        public float SecondMashSecondAmylaseStart
        {
            get {return HeatToSecondMashSecondAmylaseActivation + HeatingCharacteristics.timeFromFirstAmylaseToSecondAmylase; }
        }
        public float SecondMashHeatToBoiling
        {
            get {return SecondMashSecondAmylaseStart + MyBrewRecipe.timesSecondMash.secondAmylaseHoldTime; }
        }
        public float SecondMashBoilingStart
        {
            get {return SecondMashHeatToBoiling + HeatingCharacteristics.timeFromSecondAmylaseToBoiling; }
        }
        public float SecondMashBoilingEnd
        {
            get { return SecondMashBoilingStart + MyBrewRecipe.timesSecondMash.boilingHoldTime; }
        }
        public float HeatToMashOutActivation
        {
            get {return SecondMashBoilingEnd + downTime; }
        }
        public float MashOutStart
        {
            get {return HeatToMashOutActivation + downTime + HeatingCharacteristics.timeFromSecondAmylaseToMashOut; }
        }
        public float MashOutEnd
        {
            get {return MashOutStart + MyBrewRecipe.timesMashTun.mashOutTimeHold; }
        }
        public TimeValues(MyBrewery myBrewRecipe, float initialTemperature)
        {
            this.downTime = 10F;
            this.Start = 0F;
            this.MyBrewRecipe = myBrewRecipe;
            HeatingCharacteristics = new Heating(myBrewRecipe, initialTemperature);
        }
        public float SetHeatingCharacteristics()
        {

            return default;
        }
    }
    public struct Heating
    {
        public float timeFromTapWaterToMashIn;
        public float timeFromMashInToProtease;
        public float timeFromProteaseToFirstAmylase;
        public float timeFromFirstAmylaseToSecondAmylase;
        public float timeFromSecondAmylaseToMashOut;
        public float timeFromSecondAmylaseToBoiling;
        public Heating(MyBrewery myBrewRecipe, float initialTemperature)
        {
            float maxRampTo55degC = 2.5F;
            float maxRampTo65degC = 1.8F;
            float maxRampTo72degC = 1.5F;
            float maxRampTo85degC = 1.25F;
            float maxRampTo100degC = 1.2F;
            #endregion

            #region Heating
            this.timeFromTapWaterToMashIn = (myBrewRecipe.temperatures.mashInTemperature - initialTemperature) / maxRampTo55degC;
            this.timeFromMashInToProtease = (myBrewRecipe.temperatures.proteaseTemperature - myBrewRecipe.temperatures.mashInTemperature) / maxRampTo65degC;
            this.timeFromProteaseToFirstAmylase = (myBrewRecipe.temperatures.firstAmylaseTemperature - myBrewRecipe.temperatures.proteaseTemperature) / maxRampTo65degC;
            this.timeFromFirstAmylaseToSecondAmylase = (myBrewRecipe.temperatures.secondAmylaseTemperature - myBrewRecipe.temperatures.firstAmylaseTemperature) / maxRampTo72degC;
            this.timeFromSecondAmylaseToMashOut = (myBrewRecipe.temperatures.mashOutTemperature - myBrewRecipe.temperatures.secondAmylaseTemperature) / maxRampTo85degC;
            this.timeFromSecondAmylaseToBoiling = (myBrewRecipe.temperatures.boilingTemperature - myBrewRecipe.temperatures.secondAmylaseTemperature) / maxRampTo100degC;
            #endregion
        }

    }
}
