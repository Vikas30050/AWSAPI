namespace AWSAPI.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Maintenance.SiteSurveyGeneralInformation")]
    public partial class SiteSurveyGeneralInformation
    {
        public int ID { get; set; }
        public string SiteName { get; set; }
        public DateTime? DateOfVisit { get; set; }

        [StringLength(500)]
        public string StationName { get; set; }

        [StringLength(255)]
        public string StationType { get; set; }

        [StringLength(100)]
        public string Latitude { get; set; }

        [StringLength(100)]
        public string Longitude { get; set; }

        [StringLength(500)]
        public string CustomerName { get; set; }

        [StringLength(500)]
        public string CustomerAddress { get; set; }

        [StringLength(500)]
        public string CustomerEmailID { get; set; }

        [StringLength(500)]
        public string InChargeName { get; set; }

        [StringLength(500)]
        public string InChargeContact { get; set; }

        [StringLength(500)]
        public string InChargeAddress { get; set; }

        [StringLength(500)]
        public string InChargeEmailID { get; set; }

        [StringLength(500)]
        public string SiteAccessibilityTime { get; set; }

        public string ProcedureToFollow { get; set; }

        public bool? AccomodationFacility { get; set; }

        public bool? TransportationFromNearBy { get; set; }

        public bool? WeatherSiteCleaning { get; set; }

        [StringLength(500)]
        public string LodgingBoardingFacility { get; set; }

        [StringLength(500)]
        public string NearbyATM { get; set; }

        [StringLength(500)]
        public string NearbyCityWithDistance { get; set; }

        [StringLength(500)]
        public string LocalLanguage { get; set; }

        public string Photos1 { get; set; }

        public string Photos2 { get; set; }

        public string Photos3 { get; set; }

        public string Photos4 { get; set; }

        public string LaborAvailability { get; set; }

        public string CivilMaterialAvailability { get; set; }

        public bool? CleanPath { get; set; }

        public string MountingLocation { get; set; }

        public string GSMNetwork { get; set; }

        public string Distance230VACPoint { get; set; }

        public string Notes { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? UserID { get; set; }
    }

    public enum YesNo
    {
        True,
        False,
    }

}
