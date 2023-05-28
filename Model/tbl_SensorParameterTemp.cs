using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_SensorParameterTemp
    {
        public int ID { get; set; }

        public int SensorParameterID { get; set; }
        public int SensorID { get; set; }

        public int ParameterID { get; set; }

        [StringLength(1000)]
        public string ParameterName { get; set; }

        [StringLength(1000)]
        public string ParameterValue { get; set; }

        public int? UserID { get; set; }
    }
}