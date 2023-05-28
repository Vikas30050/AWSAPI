namespace AWSAPI.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Maintenance.SiteSurvey")]
    public partial class SiteSurvey
    {
        public int ID { get; set; }

        public string StationName { get; set; }

        public string StationID { get; set; }

        public string Division { get; set; }

        public string Typeofstation { get; set; }

        public string AWLRSiteType { get; set; }

        public string Siteinchargecontactdetails { get; set; }

        public string LatasperRFPdocument { get; set; }

        public string LongasperRFPdocument { get; set; }

        public string Lataspersitesurvey { get; set; }

        public string Longaspersitesurvey { get; set; }

        public string DeviationinLatlongobserveddurinsitesurvey { get; set; }

        public string AntennaAzimuth { get; set; }

        public string AntennaElevation { get; set; }

        public string FencingArea { get; set; }

        public string NormaltypeBlackRedRocky { get; set; }

        public string Wateravailabilityonsite { get; set; }

        public string Radarsensortowatersurfacedistance { get; set; }

        public string Radarsensortoriverreservoirbedleveldistance { get; set; }

        public string Staffgaugavailabilityonsite { get; set; }

        public string AWLRsiteMSL { get; set; }

        public string ExistingCabinsize { get; set; }

        public string ExistingCabin4Sidesopenclose { get; set; }

        public string SurfaceforAWSAWLRMastfoundation { get; set; }

        public string SurfaceforRainSensomastfoundation { get; set; }

        public string SurfaceforFencinganglefoundation { get; set; }

        public string Surfaceforgyrosanchorfoundation { get; set; }

        public string CableroutingDistancefromdataloggertoradarsensorlocation { get; set; }

        public string Paraphetwallavailabiltyonbridge { get; set; }

        public string BridgesiteYesNo { get; set; }

        public string Onsitenearbyaccomodationavailablefromcustomer { get; set; }

        public string Nearbybusstationnamedistance { get; set; }

        public string Nearbyrailwaystationnamedistance { get; set; }

        public string NearbyCityTownVilagename { get; set; }

        public string NearbyCityTownVilagenametositeDistance { get; set; }

        public string Onsitecivilworklabouravailability { get; set; }

        public string NearbyGovtwarehouseavailabilityfortemprarymaterialstorage { get; set; }

        public string SecurityonsiteavaibilityYesNo { get; set; }

        public string NetworkonsiteSitesurvey { get; set; }
        public string Sitesurveydoneby { get; set; }

        
        public string Sitesurveydoneondate { get; set; }

        public string Photoslink { get; set; }
        public string Remarks { get; set; }


        public int? UserID { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
    public enum StationType
    {
        AWS,
        AWLR,
        AWSAWLR
    }
}
