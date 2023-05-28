using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    [Table("Maintenance.SiteVisitAWLR")]
    public partial class SiteVisitAWLR
    {
        public int ID { get; set; }

        [StringLength(255)]
        public string CustomerName { get; set; }

        [StringLength(500)]
        public string StationName { get; set; }

        [StringLength(255)]
        public string StationID { get; set; }

        [StringLength(500)]
        public string Division { get; set; }

        public DateTime? FillDateTime { get; set; }

        [StringLength(255)]
        public string DataLoggerGSMSerialNumber { get; set; }

        [StringLength(255)]
        public string AWLRSensorSerialNumber { get; set; }

        [StringLength(255)]
        public string BatterySerialNumber { get; set; }

        [StringLength(255)]
        public string SolarPanelSerialNumber { get; set; }

        [StringLength(255)]
        public string UHFSerialNumber { get; set; }

        [StringLength(255)]
        public string Enclosure { get; set; }

        [StringLength(255)]
        public string CrossedYagiAntenna { get; set; }

        [StringLength(255)]
        public string WaterLevel { get; set; }

        [StringLength(255)]
        public string BatteryVoltage { get; set; }

        [StringLength(255)]
        public string SolarPanelVolatage { get; set; }

        [StringLength(255)]
        public string SystemDateTime { get; set; }

        public string Photo1 { get; set; }

        public string Photo2 { get; set; }

        public string Photo3 { get; set; }

        public string Photo4 { get; set; }

        [StringLength(255)]
        public string SolarPanelCleaning { get; set; }

        [StringLength(255)]
        public string EnclosureCleaning { get; set; }

        [StringLength(255)]
        public string AWLRSiteCleaning { get; set; }

        [StringLength(255)]
        public string BeforeCheckListDataLoggerGSM { get; set; }

        [StringLength(255)]
        public string BeforeCheckListAWLRSensor { get; set; }

        [StringLength(255)]
        public string BeforeCheckListBattery { get; set; }

        [StringLength(255)]
        public string BeforeCheckListSolarPanel { get; set; }

        [StringLength(255)]
        public string BeforeCheckListUHF { get; set; }

        [StringLength(255)]
        public string BeforeCheckListEnclosure { get; set; }

        [StringLength(255)]
        public string BeforeCheckListYagiAntena { get; set; }

        [StringLength(255)]
        public string BeforeCheckListSolar { get; set; }

        [StringLength(255)]
        public string BeforeCheckListAWLRCabin { get; set; }

        [StringLength(255)]
        public string BeforeCheckListGPRSData { get; set; }

        [StringLength(255)]
        public string BeforeCheckListINSATData { get; set; }

        [StringLength(255)]
        public string AfterCheckListDataLoggerGSM { get; set; }

        [StringLength(255)]
        public string AfterCheckListAWLRSensor { get; set; }

        [StringLength(255)]
        public string AfterCheckListBattery { get; set; }

        [StringLength(255)]
        public string AfterCheckListSolarPanel { get; set; }

        [StringLength(255)]
        public string AfterCheckListUHF { get; set; }

        [StringLength(255)]
        public string AfterCheckListEnclosure { get; set; }

        [StringLength(255)]
        public string AfterCheckListYagiAntena { get; set; }

        [StringLength(255)]
        public string AfterCheckListSolar { get; set; }

        [StringLength(255)]
        public string AfterCheckListAWLRCabin { get; set; }

        [StringLength(255)]
        public string AfterCheckListGPRSData { get; set; }

        [StringLength(255)]
        public string AfterCheckListINSATData { get; set; }

        public string RemarksCheckListDataLoggerGSM { get; set; }

        public string RemarksCheckListAWLRSensor { get; set; }

        public string RemarksCheckListBattery { get; set; }

        public string RemarksCheckListSolarPanel { get; set; }

        public string RemarksCheckListUHF { get; set; }

        public string RemarksCheckListEnclosure { get; set; }

        public string RemarksCheckListYagiAntena { get; set; }

        public string RemarksCheckListSolar { get; set; }

        public string RemarksCheckListAWLRCabin { get; set; }

        public string RemarksCheckListGPRSData { get; set; }

        public string RemarksCheckListINSATData { get; set; }

        [StringLength(255)]
        public string AzistaName { get; set; }

        [StringLength(255)]
        public string AzistaDesignation { get; set; }

        public string AzistaSignature { get; set; }

        [StringLength(255)]
        public string SiteInChargeName { get; set; }

        [StringLength(255)]
        public string SiteInChargeDesignation { get; set; }

        [StringLength(255)]
        public string SiteInChargeContactNumber { get; set; }

        public string SiteInChargeSignature { get; set; }

        public int? UserID { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}