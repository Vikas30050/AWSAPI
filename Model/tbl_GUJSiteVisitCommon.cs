using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AWSAPI.Model
{
    public class tbl_GUJSiteVisitCommon
    {

        public string CustomerName { get; set; }
        public IEnumerable<SelectListItem> StationName { get; set; }
        public string SelectedStationName { get; set; }
        public string StationID { get; set; }
        public string StationAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string TxTime { get; set; }
        public string StationType { get; set; }
        public string DataLoggerModelNumber { get; set; }
        public string DataLoggerModelSerialNumber { get; set; }
        public string DataLoggerModelReading1 { get; set; }
        public string AirTempHumidityModelNumber { get; set; }
        public string AirTempHumidityModelSerialNumber { get; set; }
        public string AirTempHumidityModelReading1 { get; set; }
        public string AirTempHumidityModelReading2 { get; set; }
        public string WindSpeedModelNumber { get; set; }
        public string WindSpeedModelSerialNumber { get; set; }
        public string WindSpeedModelReading1 { get; set; }
        public string WindSpeedModelReading2 { get; set; }
        public string AtmosphericPressureModelNumber { get; set; }
        public string AtmosphericPressureModelSerialNumber { get; set; }
        public string AtmosphericPressureModelReading1 { get; set; }
        public string GlobalRadiationModelNumber { get; set; }
        public string GlobalRadiationModelSerialNumber { get; set; }
        public string GlobalRadiationModelReading1 { get; set; }
        public string TippingBucketRainModelNumber { get; set; }
        public string TippingBucketRainModelSerialNumber { get; set; }
        public string TippingBucketRainModelReading1 { get; set; }
        public string TippingBucketRainModelReading2 { get; set; }
        public string UHFSatelliteModelNumber { get; set; }
        public string UHFSatelliteModelSerialNumber { get; set; }
        public string UHFSatelliteModelReading1 { get; set; }
        public string YagiAntennaModelNumber { get; set; }
        public string YagiAntennaModelSerialNumber { get; set; }
        public string YagiAntennaModelReading1 { get; set; }
        public string SMFBatteryModelNumber { get; set; }
        public string SMFBatteryModelSerialNumber { get; set; }
        public string SMFBatteryModelReading1 { get; set; }
        public string SolarPanelModelNumber { get; set; }
        public string SolarPanelModelSerialNumber { get; set; }
        public string SolarPanelModelReading1 { get; set; }
        public string EnclosureModelNumber { get; set; }
        public string EnclosureModelSerialNumber { get; set; }
        public string EnclosureModelReading1 { get; set; }
        public string SimcardModelNumber { get; set; }
        public string SimcardModelSerialNumber { get; set; }
        public string SimcardModelReading1 { get; set; }
        public string DataReceptionModelNumber { get; set; }
        public string DataReceptionModelSerialNumber { get; set; }
        public string DataReceptionModelReading1 { get; set; }
        public string DataBackupModelNumber { get; set; }
        public string DataBackupModelSerialNumber { get; set; }
        public string DataBackupModelReading1 { get; set; }
        public string BeforeMaintenance { get; set; }
        public string AfterMaintenance { get; set; }
        public string AzistaReprensentativeName { get; set; }
        public string AzistaReprensentativeDesignation { get; set; }
        public string ISTTime { get; set; }
        public string CustomerReprensentativeName { get; set; }
        public string CustomerReprensentativeDesignation { get; set; }
        public string CustomerReprensentativeNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserID { get; set; }

    }
    
}