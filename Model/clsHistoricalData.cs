using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_VMC_StationData.Model
{
    public class clsHistoricalData
    {
        public string StationID { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Day { get; set; }

    }
}