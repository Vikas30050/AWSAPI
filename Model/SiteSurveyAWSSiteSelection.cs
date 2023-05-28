namespace AWSAPI.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Maintenance.SiteSurveyAWSSiteSelection")]
    public partial class SiteSurveyAWSSiteSelection
    {

        public int ID { get; set; }

       
        public int GeneralInformationID { get; set; }

        public string SiteSize { get; set; }

        [StringLength(500)]
        public string LandType { get; set; }

        public bool? ElectromagneticInterface { get; set; }

        public bool? UndergroundObstructions { get; set; }

        public bool? AwayFromHighTensions { get; set; }

        public bool? AwayFromLargeIndustialHeat { get; set; }

        public bool? SurfaveShortGrassNaturalEarth { get; set; }

        public bool? FreeFromShadow { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? UserID { get; set; }
    }
}
