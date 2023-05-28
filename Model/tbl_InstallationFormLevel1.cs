using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_InstallationFormLevel1
    {
        public int ID { get; set; }
        public string ProjectName { get; set; }
        public string Division { get; set; }
        public DateTime? FormDate { get; set; }
        public string StationName { get; set; }
        public string StationAddress { get; set; }
        public string Pincode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Altitude { get; set; }
        public string SiteCondition { get; set; }
        public string SiteConditionRemarks { get; set; }
        public string SitePreparation { get; set; }
        public string SitePhotosTakenBefore { get; set; }
        public string SitePhotosTakenAfter { get; set; }
        public string MastFoundattion { get; set; }
        public string Installation { get; set; }
        public int? UserID { get; set; }
    }
}