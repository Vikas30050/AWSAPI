using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace AWSAPI.Model
{
    public class clsStationAuth
    {
        public string ProfileName { get; set; }
        public string ProfileType { get; set; }
    }

    public class clsStationSummary
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string StationID { get; set; }
        public string StationName { get; set; }
        public string StationType { get; set; }
        public string Location { get; set; }
        public int RefreshRate { get; set; }
        public string InstallationDate { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string  centerLat { get; set; }
        public string centerLng { get; set; }

        public string Status { get; set; }
        public List<clsParaSummaryDetail> paraDetails { get; set; }
    }

    public class clsParaSummaryDetail
    {
        public string ParameterName { get; set; }
        public string Type { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterUnit { get; set; }
    }

    public class clsParaDetail
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterUnit { get; set; }
    }

    public class clsStationDetail
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string StationID { get; set; }
        public string StationName { get; set; }
        public string StationType { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }

        public List<clsParaDetail> ParaDetails { get; set; }
    }

    public class clsParaMinMax
    {
        public string ParameterName { get; set; }
        public string ParameterUnit { get; set; }
        public string MinDate { get; set; }
        public string MinTime { get; set; }
        public string MinValue { get; set; }
        public string MaxDate { get; set; }
        public string MaxTime { get; set; }
        public string MaxValue { get; set; }
    }

    public class clsStationGraphDetail
    {
        public string ParameterName { get; set; }
        public string Type { get; set; }
        public string GraphType { get; set; }
        public string xAxisTitle { get; set; }
        public string yAxisTile { get; set; }
        public string MinDate { get; set; }
        public string MinTime { get; set; }
        public string MinValue { get; set; }
        public string MaxDate { get; set; }
        public string MaxTime { get; set; }
        public string MaxValue { get; set; }

        public clsCurrentData CurrentData { get; set; }
        public clsRainDetail RainData { get; set; }
        public IList<clsGraphData> GraphData { get; set; }
        public IList<clsGraphData> GraphBarData { get; set; }
        public IList<clsGraphData> GraphBarDataWG { get; set; }
    }

    public class clsGraphData
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterUnit { get; set; }
    }

    public class clsForecastData
    {
        public clsDailyData dailyData { get; set; }
        public clsHourlyData hourlyData { get; set; }
    }

    public class clsDailyData
    {
        public string[] dailyParaTitles { get; set; }
        public List<clsDailyValues> dailyValues { get; set; }
    }

    public class clsDailyValues
    {
        public List<clsDynamicDailyData> dynamicDailyData { get; set; }
    }

    public class clsDynamicDailyData
    {
        public string dailyParaValue { get; set; }
        public string dailyParaUnit { get; set; }
    }

    public class clsHourlyData
    {
        public string[] hourlyParaTitles { get; set; }
        public List<clsHourlyValues> hourlyValues { get; set; }
    }

    public class clsHourlyValues
    {
        public List<clsDynamicHourlyData> dynamicHourlyData { get; set; }
    }

    public class clsDynamicHourlyData
    {
        public string hourlyParaValue { get; set; }
        public string hourlyParaUnit { get; set; }
    }

    public class Daily
    {
        public int dt { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
        public int moonrise { get; set; }
        public int moonset { get; set; }
        public double moon_phase { get; set; }
        public Temp temp { get; set; }
        public FeelsLike feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        public double wind_gust { get; set; }
        public List<Weather> weather { get; set; }
        public int clouds { get; set; }
        public double pop { get; set; }
        public double uvi { get; set; }
        public double rain { get; set; }
    }
    public class FeelsLike
    {
        public double day { get; set; }
        public double night { get; set; }
        public double eve { get; set; }
        public double morn { get; set; }
    }
    public class Hourly
    {
        public int dt { get; set; }
        public double temp { get; set; }
        public double feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double uvi { get; set; }
        public int clouds { get; set; }
        public int visibility { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        public double wind_gust { get; set; }
        public List<Weather> weather { get; set; }
        public double pop { get; set; }
        public Rain rain { get; set; }
    }
    public class Rain
    {
        [JsonProperty("1h")]
        //Change by vikas --> 29-07-2024
        //public double _1h { get; set; }
        public double onehour { get; set; }
        public double hr { get; set; }
    }

    public class Root
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string timezone { get; set; }
        public int timezone_offset { get; set; }
        public List<Hourly> hourly { get; set; }
        public List<Daily> daily { get; set; }
    }
    public class Temp
    {
        public double day { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double night { get; set; }
        public double eve { get; set; }
        public double morn { get; set; }
    }
    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Hourly_Rep
    {
        public string hour { get; set; }
        public double temp { get; set; }
        public double feels_like { get; set; }
        public string icon { get; set; }
        public double rain { get; set; }
        public int pop { get; set; }
        // public double Chanceofrain { get; set; }
    }

    public class Daily_Rep
    {
        public string day { get; set; }
        public string date { get; set; }
        public double HT { get; set; }
        public double LT { get; set; }
        public string icon { get; set; }
        public double rain { get; set; }
        public int pop { get; set; }
        //public double Accrain { get; set; }
        //public double Chanceofrain { get; set; }
    }

    public class clsHistoryHelper
    {
        public string Parameter { get; set; }
        public string DateMin { get; set; }
        public string TimeMin { get; set; }
        public string MinVal { get; set; }
        public string DateMax { get; set; }
        public string TimeMax { get; set; }
        public string MaxVal { get; set; }
        public string Unit { get; set; }
    }

    public class clsHistoricalData
    {
        public string paraTitle { get; set; }
        public List<clsHistoryData> historyData { get; set; }
    }

    public class clsHistoryData
    {
        public string title { get; set; }
        public string paraValue { get; set; }
        public string paraUnit { get; set; }
        public string dateTime { get; set; }
    }

    public class clsCurrentData
    {
        public string Time { get; set; }
        public string Temperature { get; set; }
        public string HighTemp { get; set; }
        public string LowTemp { get; set; }
        public string Humidity { get; set; }
        public string Wind { get; set; }
        public string WindSpeed { get; set; }
        public string Pressure { get; set; }
    }

    public class clsRainDetail
    {
        public string RainUnit { get; set; }
        
        public string CurrentDay { get; set; }

        public string LastDay { get; set; }

        public string Rain24HR { get; set; }

        public string LastHour { get; set; }

        public string CurrentRainRate { get; set; }

        public string StormValue { get; set; }

        public string StormUnit { get; set; }

        public string StormStartDateTime { get; set; }

        public string StormDuration { get; set; }

        public string MonthTotal { get; set; }

        public string YearTotal { get; set; }
    }

    public class clsStationData
    {
        public string  Date { get; set; }
        public string Time { get; set; }
        public string StationID { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
    }
}