using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    [Table("Maintenance.SiteVisitAWS")]
    public class SiteVisitAWS
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
        public string AWSDataLoggerGSMSerialNumber { get; set; }

        [StringLength(255)]
        public string AirTemperatureHumiditySerialNumber { get; set; }

        [StringLength(255)]
        public string WindSpeedWindDirectionSerialNumber { get; set; }

        [StringLength(255)]
        public string ATMSerialNumber { get; set; }

        [StringLength(255)]
        public string SolarRadiationSerialNumber { get; set; }

        [StringLength(255)]
        public string TrippingBucketSerialNumber { get; set; }

        [StringLength(255)]
        public string PanEvaporationSerialNumber { get; set; }

        [StringLength(255)]
        public string AHBatterySerialNumber { get; set; }

        [StringLength(255)]
        public string SolarPanelSerialNumber { get; set; }

        [StringLength(255)]
        public string UHFSatelliteSerialNumber { get; set; }

        [StringLength(255)]
        public string EnclosureSerialNumber { get; set; }

        [StringLength(255)]
        public string YagiAntennaSerialNumber { get; set; }

        [StringLength(255)]
        public string Temperature { get; set; }

        [StringLength(255)]
        public string RH { get; set; }

        [StringLength(255)]
        public string WindSpeed { get; set; }

        [StringLength(255)]
        public string WindDirection { get; set; }

        [StringLength(255)]
        public string SolarRadiation { get; set; }

        [StringLength(255)]
        public string ATMPressure { get; set; }

        [StringLength(255)]
        public string RainFall { get; set; }

        [StringLength(255)]
        public string PanEvaporation { get; set; }

        [StringLength(255)]
        public string BatteryVoltage { get; set; }

        [StringLength(255)]
        public string SolarPanelVoltage { get; set; }

        [StringLength(255)]
        public string SystemDateTime { get; set; }

        public string Photo1 { get; set; }

        public string Photo2 { get; set; }

        public string Photo3 { get; set; }

        public string Photo4 { get; set; }

        [StringLength(255)]
        public string AWSSolarPanelCleaning { get; set; }

        [StringLength(255)]
        public string AWSEnclosureCleaning { get; set; }

        [StringLength(255)]
        public string ATRHSensorSiteCleaning { get; set; }

        [StringLength(255)]
        public string SolarRadiationSiteCleaning { get; set; }

        [StringLength(255)]
        public string RainGaugeSiteCleaning { get; set; }

        [StringLength(255)]
        public string WindspeedDirectionSiteCleaning { get; set; }

        [StringLength(255)]
        public string AWSSiteSiteCleaning { get; set; }

        [StringLength(255)]
        public string BeforeCheckListAWSDataLoggerGSM { get; set; }

        [StringLength(255)]
        public string BeforeCheckListAirTempHumidity { get; set; }

        [StringLength(255)]
        public string BeforeCheckListWindSpeedDirection { get; set; }

        [StringLength(255)]
        public string BeforeCheckListATMP { get; set; }

        [StringLength(255)]
        public string BeforeCheckListSolarRadiation { get; set; }

        [StringLength(255)]
        public string BeforeCheckListTippingBucket { get; set; }

        [StringLength(255)]
        public string BeforeCheckListPanEvaporation { get; set; }

        [StringLength(255)]
        public string BeforeCheckListBatteryAWS { get; set; }

        [StringLength(255)]
        public string BeforeCheckListSolarPanelAWS { get; set; }

        [StringLength(255)]
        public string BeforeCheckListUHFAWS { get; set; }

        [StringLength(255)]
        public string BeforeCheckListSolarBatteryCharger { get; set; }

        [StringLength(255)]
        public string BeforeCheckListEnclosureAWS { get; set; }

        [StringLength(255)]
        public string BeforeCheckListYagiAntenaAWS { get; set; }

        [StringLength(255)]
        public string BeforeCheckListAWSFencing { get; set; }

        [StringLength(255)]
        public string BeforeCheckListGPRSDataAWS { get; set; }

        [StringLength(255)]
        public string BeforeCheckListINSATDataAWS { get; set; }

        [StringLength(255)]
        public string AfterCheckListAWSDataLoggerGSM { get; set; }

        [StringLength(255)]
        public string AfterCheckListAirTempHumidity { get; set; }

        [StringLength(255)]
        public string AfterCheckListWindSpeedDirection { get; set; }

        [StringLength(255)]
        public string AfterCheckListATMP { get; set; }

        [StringLength(255)]
        public string AfterCheckListSolarRadiation { get; set; }

        [StringLength(255)]
        public string AfterCheckListTippingBucket { get; set; }

        [StringLength(255)]
        public string AfterCheckListPanEvaporation { get; set; }

        [StringLength(255)]
        public string AfterCheckListBatteryAWS { get; set; }

        [StringLength(255)]
        public string AfterCheckListSolarPanelAWS { get; set; }

        [StringLength(255)]
        public string AfterCheckListUHFAWS { get; set; }

        [StringLength(255)]
        public string AfterheckListSolarBatteryCharger { get; set; }

        [StringLength(255)]
        public string AfterCheckListEnclosureAWS { get; set; }

        [StringLength(255)]
        public string AfterCheckListYagiAntenaAWS { get; set; }

        [StringLength(255)]
        public string AfterCheckListAWSFencing { get; set; }

        [StringLength(255)]
        public string AfterCheckListGPRSDataAWS { get; set; }

        [StringLength(255)]
        public string AfterCheckListINSATDataAWS { get; set; }

        [StringLength(255)]
        public string RemarksCheckListAWSDataLoggerGSM { get; set; }

        [StringLength(255)]
        public string RemarksCheckListAirTempHumidity { get; set; }

        [StringLength(255)]
        public string RemarksCheckListWindSpeedDirection { get; set; }

        [StringLength(255)]
        public string RemarksCheckListATMP { get; set; }

        [StringLength(255)]
        public string RemarksCheckListSolarRadiation { get; set; }

        [StringLength(255)]
        public string RemarksCheckListTippingBucket { get; set; }

        [StringLength(255)]
        public string RemarksCheckListPanEvaporation { get; set; }

        [StringLength(255)]
        public string RemarksCheckListBatteryAWS { get; set; }

        [StringLength(255)]
        public string RemarksCheckListSolarPanelAWS { get; set; }

        [StringLength(255)]
        public string RemarksCheckListUHFAWS { get; set; }

        [StringLength(255)]
        public string RemarksheckListSolarBatteryCharger { get; set; }

        [StringLength(255)]
        public string RemarksCheckListEnclosureAWS { get; set; }

        [StringLength(255)]
        public string RemarksCheckListYagiAntenaAWS { get; set; }

        [StringLength(255)]
        public string RemarksCheckListAWSFencing { get; set; }

        [StringLength(255)]
        public string RemarksCheckListGPRSDataAWS { get; set; }

        [StringLength(255)]
        public string RemarksCheckListINSATDataAWS { get; set; }

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