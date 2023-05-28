using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class Maintenance
    {
        public string SiteName { get; set; }
        public DateTime? DateOfVisit { get; set; }
        public string StationName { get; set; }

       
        public string StationType { get; set; }

   
        public string Latitude { get; set; }

  
        public string Longitude { get; set; }

        
        public string CustomerName { get; set; }

     
        public string CustomerAddress { get; set; }

      
        public string CustomerEmailID { get; set; }

        
        public string InChargeName { get; set; }

     
        public string InChargeContact { get; set; }

       
        public string InChargeAddress { get; set; }

       
        public string InChargeEmailID { get; set; }

        
        public string SiteAccessibilityTime { get; set; }

        public string ProcedureToFollow { get; set; }

        public bool? AccomodationFacility { get; set; }

        public bool? TransportationFromNearBy { get; set; }

        public bool? WeatherSiteCleaning { get; set; }

       
        public string LodgingBoardingFacility { get; set; }

        public string NearbyATM { get; set; }

     
        public string NearbyCityWithDistance { get; set; }

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
        public int GeneralInformationID { get; set; }
        public string SiteSize { get; set; }
       
        public string LandType { get; set; }

        public bool? ElectromagneticInterface { get; set; }

        public bool? UndergroundObstructions { get; set; }

        public bool? AwayFromHighTensions { get; set; }

        public bool? FreeFromShadow { get; set; }

        public string RadarMinimumMaxDistance { get; set; }


    
        public string WaterMaximumMaxDistance { get; set; }


       
        public string DistanceDataLoggerWaterLevel { get; set; }

       
        public string FencingSpace { get; set; }

        public bool? StaffGauge { get; set; }
        public bool? AwayFromLargeIndustialHeat { get; set; }
        public bool? SurfaveShortGrassNaturalEarth { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UserID { get; set; }
    }
}