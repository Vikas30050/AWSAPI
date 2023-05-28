using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_StationConfigTemp
    {
        public int ID { get; set; }

        public string SensorID { get; set; }
        public string SensorName { get; set; }

        public string Gain { get; set; }
        public string Offset { get; set; }
        public string SerialNumber { get; set; }
        public string UserID { get; set; }
        public bool? ShowInGrid { get; set; }
        public bool? ShowInGraph { get; set; }
         
    }
}