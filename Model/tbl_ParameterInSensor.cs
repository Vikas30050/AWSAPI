using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_ParameterInSensor
    {
        public int ID { get; set; }
        public int SensorID { get; set; }
        public int ParameterID { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}