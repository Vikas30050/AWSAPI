using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AWSAPI.Model
{

    public class tbl_SiteSurvey
    {
        public int ID { get; set; }
        public string projectName { get; set; }
        public string SiteName { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Basin { get; set; }
        public string Village { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Altitude { get; set; }
        public string Landmark { get; set; }
        public string Address { get; set; }
        public string CoordinatorName { get; set; }
        public string CoordinatorContact { get; set; }
        public string CoordinatorAddress { get; set; }
        public string SiteInchargeName { get; set; }
        public string SiteInchargeContact { get; set; }
        public string SiteAddress { get; set; }
        public string WorkingWeekDays { get; set; }
        public DateTime? DateofVisit { get; set; }
        //public string StationType { get; set; }
        public string Site { get; set; }
        public string LandType { get; set; }
        public string ElectromagneticInterface { get; set; }
        public string UndergroundObstruction { get; set; }
        public string HighTensionspowerLines { get; set; }
        public string HeatSources { get; set; }
        public string Grass { get; set; }
        public string Speficprocedure { get; set; }
        public string SiteCleaning { get; set; }
        public string LaborAvailability { get; set; }
        public string CivilMaterial { get; set; }
        public string ListOfGSM { get; set; }
        public string DistanceFrompoint { get; set; }
        public string Remarks { get; set; }
        public string Accommodation { get; set; }
        public string Transportation { get; set; }

        public string ATMS { get; set; }
        public string SiteDistance { get; set; }
        public string LocalLanguage { get; set; }
        public string CustomerLanguage { get; set; }
        public string photo1 { get; set; }
        public string photo2 { get; set; }
        public string photo3 { get; set; }
        public string photo4 { get; set; }
        public string photo5 { get; set; }
        public string SiteSurveyName { get; set; }
        public string SiteIncharge { get; set; }
        public string SiteCareTaker { get; set; }
        public string Lodging { get; set; }
    }
}