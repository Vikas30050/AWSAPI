namespace AWSAPI.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_FTPRawData
    {
        public int ID { get; set; }

        [StringLength(500)]
        public string FileName { get; set; }

        public string Data { get; set; }

        public string StationID { get; set; }

        public bool? IsCheckSum { get; set; }
    }
}
