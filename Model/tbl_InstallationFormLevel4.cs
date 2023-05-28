using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_InstallationFormLevel4
    {
        public int ID { get; set; }
        public int Level1ID { get; set; }
        public string DescriptionOfWork { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int UserID { get; set; }
    }
    public class tbl_InstallationFormLevel4Transform
    {
        public int ID { get; set; }
        public int Level1ID { get; set; }
        public string DescriptionOfWork { get; set; }
        public string[] Status { get; set; }
        public string[] lv4Remarks { get; set; }
        public int UserID { get; set; }
    }
}