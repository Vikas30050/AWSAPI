using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_MissingDataPermission
    {
        public int ID { get; set; }
        public int? UserID { get; set; }
        public bool? ShowMissingData { get; set; }
    }
}