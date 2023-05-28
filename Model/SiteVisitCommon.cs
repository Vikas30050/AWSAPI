using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AWSAPI.Model
{
    public class SiteVisitCommon
    {
        public string CustomerName { get; set; }

        [StringLength(500)]
        public string StationName { get; set; }
        public string MainDate { get; set; }
        public string StationType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Altitude { get; set; }

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

        public string awsazistaname { get; set; }
        public string awsazistadesgination { get; set; }
        public string awssitename { get; set; }
        public string awssitedesgination { get; set; }
        public string awssitecontact { get; set; }
        public string cabinLatitude { get; set; }
        public string cabinLongitude { get; set; }
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
        public string SolarPanelVoltage { get; set; }
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
        public string AWSAzistaSignature { get; set; }
        public string AWSClientsignature { get; set; }

        
    }
}