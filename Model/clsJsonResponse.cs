using AWSAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class clsAuthSuccess
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public IList<clsStationAuth> data { get; set; }

    }

    public class clsJsonDetail
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public IList<clsParaDetail> data { get; set; }

    }

    public class clsJsonMinMax
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public IList<clsParaMinMax> data { get; set; }

    }

    public class clsJsonSuccess
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public IList<clsStationSummary> data { get; set; }
    }

    public class clsJsonFail
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }

    public class clsJsonDataMinMax
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public IList<clsStationGraphDetail> data { get; set; }
    }

    public class Data
    {
        public string Date { get; set; }
        public string Time {    get; set; }
        public string StationID { get; set; }
        public string StationName { get; set; }
        public string StationType { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public List<clsParaDetail> ParaDetails { get; set; }
    }

    public class clsJsonForecast
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public clsForecastData data { get; set; }
    }


    
    public class clsJsonHistory
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public List<clsHistoricalData> data { get; set; }
    }


    public class clsJsonSignUp
    {
        public int code { get; set; }
        public string  message { get; set; }
        public bool status { get; set; }
        public string data { get; set; }
    }


}