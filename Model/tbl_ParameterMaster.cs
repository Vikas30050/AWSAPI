namespace AWSAPI.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ParameterMaster
    {
        public int ID { get; set; }

        
        [StringLength(1000)]
        public string Name { get; set; }

        public int SensorID { get; set; }

        public string MinimumRange { get; set; }
        public string MaximumRange { get; set; }
        public string Unit { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
