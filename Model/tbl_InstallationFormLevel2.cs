using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_InstallationFormLevel2
    {
        public int ID { get; set; }
        public int Level1ID { get; set; }
        public string MakeModel { get; set; }
        public string SerialNo { get; set; }
        public string Recived { get; set; }
        public int? UserID { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class tbl_InstallationFormLevel2Transform
    {
        public int ID { get; set; }
        public int Level1ID { get; set; }
        public string MakeModel { get; set; }
        public string[] SerialNo { get; set; }
        public string[] Recived { get; set; }
        public int? UserID { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}