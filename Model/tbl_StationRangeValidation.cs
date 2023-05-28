using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_StationRangeValidation
    {
        public int ID { get; set; }
        public string ProfileName { get; set; }
        public string ValidationString { get; set; }

        public string Unit { get; set; }
    }
}