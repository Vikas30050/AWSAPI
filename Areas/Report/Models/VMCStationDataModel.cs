using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Areas.Report.Models
{
    public class VMCStationDataModel
    {
        public string  stationName { get; set; }

        public string fromDate { get; set; }

        public string toDate { get; set; }
    }
}