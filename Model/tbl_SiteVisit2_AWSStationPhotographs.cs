using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_SiteVisit2_AWSStationPhotographs
    {
        public int ID { get; set; }
        public int? GeneralInformationID { get; set; }
        public int? SystemParameterID { get; set; }
        public string TypesOfPhotographs { get; set; }
        public string PhotoStatus { get; set; }
    }

}