namespace AWSAPI.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Maintenance.SiteSurveyAWLRSiteSelection")]
    public partial class SiteSurveyAWLRSiteSelection
    {
        public int ID { get; set; }
        public int GeneralInformationID { get; set; }
        public string SiteSize { get; set; }
        [StringLength(500)]
        public string LandType { get; set; }

        public bool? ElectromagneticInterface { get; set; }

        public bool? UndergroundObstructions { get; set; }

        public bool? AwayFromHighTensions { get; set; }

        public bool? FreeFromShadow { get; set; }

        [StringLength(255)]
        public string RadarMinimumMaxDistance { get; set; }


        [StringLength(255)]
        public string WaterMaximumMaxDistance { get; set; }

      
        [StringLength(255)]
        public string DistanceDataLoggerWaterLevel { get; set; }

        [StringLength(255)]
        public string FencingSpace { get; set; }

        public bool? StaffGauge { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? UserID { get; set; }
    }
}
