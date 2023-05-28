using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AWSAPI.Model
{
    public class SiteVisit2AWLR
    {
        public string CustomerName { get; set; }
        public IEnumerable<SelectListItem> StationName { get; set; }
        public string SelectedUserStId { get; set; }
        public string FillDate { get; set; }
        public string StationID { get; set; }
        public string FillTime { get; set; }
        public string Division { get; set; }
        public string VisitType { get; set; }
        public int? UserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? GeneralInformationID { get; set; }
        public string Parameter { get; set; }
        public string altitude { get; set; }

        public string[] backupStatus { get; set; }

        public string[] IDF { get; set; }
        public string[] DataLogger { get; set; }
        public int? SystemParameterID { get; set; }
        public string TypesOfPhotographs { get; set; }
        public string[] PhotoStatus { get; set; }
        public int? StationPhotographsID { get; set; }
        public int? ObservationID { get; set; }
        public string PurposeOfVisit { get; set; }
        public string OtherObservation { get; set; }
        public string ActionTaken { get; set; }
        public string Conclusion { get; set; }
        public string AzistaRepresentative { get; set; }
        public string MaterialDescription { get; set; }
        public string ObservationType { get; set; }
        public string cabinLatitude { get; set; }
        public string cabinLongitude { get; set; }
        public string AWLRSensorLatitude { get; set; }
        public string AWLRSensorLongitude { get; set; }
        public string[] ObservationStatus { get; set; }

        public partial class StationIDs
        {
            public string StationID { get; set; }
            public string StationName { get; set; }
        }

    }
}