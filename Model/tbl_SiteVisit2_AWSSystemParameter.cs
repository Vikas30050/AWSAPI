using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_SiteVisit2_AWSSystemParameter
    {
        public int ID { get; set; }
        public int? GeneralInformationID { get; set; }
        public string Parameter { get; set; }
        public string IDF { get; set; }
        public string DataLogger { get; set; }

    }
}