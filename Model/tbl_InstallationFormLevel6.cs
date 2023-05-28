using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_InstallationFormLevel6
    {
        public int ID { get; set; }
        public int Level1ID { get; set; }
        public string Description { get; set; }
        public string PlaceName { get; set; }
        public string DistanceInKMS { get; set; }
        public int UserID { get; set; }
    }
    public class tbl_InstallationFormLevel6Transform
    {
        public int ID { get; set; }
        public int Level1ID { get; set; }
        public string Description { get; set; }
        public string[] PlaceName { get; set; }
        public string[] DistanceInKMS { get; set; }
        public int UserID { get; set; }
    }
}