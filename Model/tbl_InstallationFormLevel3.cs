using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_InstallationFormLevel3
    {
        public int ID { get; set; }
        public int Level1ID { get; set; }
        public string ParameterDescription { get; set; }
        public string Observation { get; set; }
        public string Remarks { get; set; }
        public int UserID { get; set; }

    }
    public class tbl_InstallationFormLevel3Transform
    {
        public int ID { get; set; }
        public int Level1ID { get; set; }
        public string ParameterDescription { get; set; }
        public string Observation { get; set; }
        public string[] Remarks { get; set; }
        public int UserID { get; set; }

    }
}