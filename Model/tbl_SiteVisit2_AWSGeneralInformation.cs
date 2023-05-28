using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_SiteVisit2_AWSGeneralInformation
    {
        public int ID { get; set; }
        public string CustomerName { get; set; }
        public string StationName { get; set; }
        public string StationID { get; set; }
        public string FillDate { get; set; }
        public string FillTime { get; set; }
        public string Division { get; set; }
        public string VisitType { get; set; }
        public int? UserID { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}