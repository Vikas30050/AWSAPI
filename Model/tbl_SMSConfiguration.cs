using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    [Table("tbl_SMSConfiguration")]
    public class tbl_SMSConfiguration
    {
        public int ID { get; set; }
        public string ProfileName { get; set; }
        public string StationID { get; set; }
        public string PhoneNumber { get; set; }
    }
}