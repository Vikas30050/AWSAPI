using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class clsVMCDataReport
    {
        [JsonProperty("StationID")]
        public string StationID { get; set; }

        [JsonProperty("Date")]
        public string Date { get; set; }

        [JsonProperty("Time")]
        public string Time { get; set; }

        //[JsonProperty("Battery Voltage(V)")]
        //public double? BatteryVoltage { get; set; }

        [JsonProperty("15mins RAINFALL(mm)")]
        public double? Rainfall15mins { get; set; }

        [JsonProperty("Daily Rain(mm)")]
        public double? DailyRain { get; set; }

        /*[JsonProperty("Air Temperature(C)")]
        public double? AirTemperature { get; set; }

        [JsonProperty("Wind Speed(m/s)")]
        public double? WindSpeed { get; set; }

        [JsonProperty("Wind Direction(DEG)")]
        public double? WindDirection { get; set; }

        [JsonProperty("Atmospheric Pressure(hPa)")]
        public double? AtmosphericPressure { get; set; }

        [JsonProperty("Humidity(%)")]
        public double? Humidity { get; set; }

        [JsonProperty("Dew Point(◦C)")]
        public double? DewPoint { get; set; }

        [JsonProperty("Wind Run(m)")]
        public double? WindRun { get; set; }

        [JsonProperty("Wind Chill(kgcal/m2/h)")]
        public double? WindChill { get; set; }

        [JsonProperty("Heat Index(◦C)")]
        public double? HeatIndex { get; set; }

        [JsonProperty("THW Index(◦C)")]
        public double? THWIndex { get; set; }

        [JsonProperty("Rain Rate(mm/hr)")]
        public double? RainRate { get; set; }

        [JsonProperty("High Speed(m/s)")]
        public double? HighSpeed { get; set; }

        [JsonProperty("High Direction(Deg)")]
        public double? HighDirection { get; set; }*/
    }

    public class clsResponseDataReport
    {
        [JsonProperty("StationID")]
        public string StationID { get; set; }

        [JsonProperty("Date")]
        public string Date { get; set; }

        [JsonProperty("Time")]
        public string Time { get; set; }

        //[JsonProperty("BatteryVoltage")]
        //public double? BatteryVoltage { get; set; }

        //[JsonProperty("15minsRAINFALL")]
        [JsonProperty("15minsRainFall")]
        public double? Rainfall15mins { get; set; }

        [JsonProperty("DailyRain")]
        public double? DailyRain { get; set; }

       /* [JsonProperty("AirTemperature")]
        public double? AirTemperature { get; set; }

        [JsonProperty("WindSpeed")]
        public double? WindSpeed { get; set; }

        [JsonProperty("WindDirection")]
        public double? WindDirection { get; set; }

        [JsonProperty("AtmosphericPressure")]
        public double? AtmosphericPressure { get; set; }

        [JsonProperty("Humidity")]
        public double? Humidity { get; set; }

        [JsonProperty("DewPoint")]
        public double? DewPoint { get; set; }

        [JsonProperty("WindRun")]
        public double? WindRun { get; set; }

        [JsonProperty("WindChill")]
        public double? WindChill { get; set; }

        [JsonProperty("HeatIndex")]
        public double? HeatIndex { get; set; }

        [JsonProperty("THWIndex")]
        public double? THWIndex { get; set; }

        [JsonProperty("RainRate")]
        public double? RainRate { get; set; }

        [JsonProperty("HighSpeed")]
        public double? HighSpeed { get; set; }

        [JsonProperty("HighDirection")]
        public double? HighDirection { get; set; }*/
    }

}