using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_ShowInGraph
    {
        public int ID { get; set; }
        public string StationID { get; set; }
        public string SensorID { get; set; }
        public string SensorName { get; set; }
        public bool? ShowInGrpah { get; set; }
        public bool? ShowInGrid { get; set; }
        
    }
}