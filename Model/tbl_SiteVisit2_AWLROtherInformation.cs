using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_SiteVisit2_AWLROtherInformation
    {
        public int ID { get; set; }
        public int? GeneralInformationID { get; set; }
        public int? SystemParameterID { get; set; }
        public int? StationPhotographsID { get; set; }
        public int? ObservationID { get; set; }
        public string PurposeOfVisit { get; set; }
        public string OtherObservation { get; set; }
        public string ActionTaken { get; set; }
        public string Conclusion { get; set; }
        public string AzistaRepresentative { get; set; }
    }
}