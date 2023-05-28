using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class tbl_parametertemp
    {
        public int ID { get; set; }
        public string ParameterName { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }
    }
}