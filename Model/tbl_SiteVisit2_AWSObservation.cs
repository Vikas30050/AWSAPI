using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_SiteVisit2_AWSObservation
    {
        public int ID { get; set; }
        public int? GeneralInformationID { get; set; }
        public int? SystemParameterID { get; set; }
        public int? StationPhotographsID { get; set; }
        public string MaterialDescription { get; set; }
        public string ObservationType { get; set; }
        public string ObservationStatus { get; set; }
    }
}