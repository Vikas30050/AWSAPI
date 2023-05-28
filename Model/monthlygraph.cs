using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class monthlygraph
    {
        public string Time { get; set; }
        public string ColumnName { get; set; }
        public string min { get; set; }
        public string max { get; set; }
        public string avg { get; set; }

    }
}