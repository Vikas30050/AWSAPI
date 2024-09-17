using AWSAPI.HelperClass;
using AWSAPI.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Device.Location;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.DynamicData;
using System.Web.Http;

namespace AWSAPI
{
    [Route("awsapi")]
    public class AWSAPIController : ApiController
    {
        AWSDatabaseContext db = new AWSDatabaseContext();
        static clsDatabase ObjDB = new clsDatabase();
        static clsExceptionDataRoutine ObjEx = new clsExceptionDataRoutine();
        static clsFileOperationRoutines ObjFile = new clsFileOperationRoutines();
        static clsDerivedParameter ObjDP = new clsDerivedParameter();

        public string strModuleName = "Send AWS Data to All Client";
        public string strFunctionName = "";
        public string strExceptionMessage = "";
        public string strLogMessage = "";

        [HttpPost]
        [Route("userauthasync")]
        public async Task<HttpResponseMessage> AuthenticateUserAsync()
        {
            try
            {
                var objUser = await Request.Content.ReadAsFormDataAsync();

                string Unm = Regex.Unescape(objUser.Get(0));
                string Pwd = Regex.Unescape(objUser.Get(1));

                DataSet dsStationdetail = null;
                for (int a = 0; a < 3; a++)
                {
                    dsStationdetail = ObjDB.FetchData_SP_AuthenticateUser("USP_AuthenticateUser", Unm.Trim(), Pwd.Trim(), "Web");
                    if (dsStationdetail.Tables.Count > 0)
                        break;
                    Thread.Sleep(1000);
                }

                if (dsStationdetail.Tables[0].Rows.Count > 0)
                {
                    List<clsStationAuth> Station = new List<clsStationAuth>();

                    var profile = dsStationdetail.Tables[0].AsEnumerable()
                    .Select(r => new
                    {
                        ProfileName = r.Field<string>("AliasProfileName"),
                        ProfileType = r.Field<string>("StationType")
                    }).Distinct().ToList();

                    if (profile.Count == 1)
                    {
                        clsStationAuth stationAuth = new clsStationAuth();
                        stationAuth.ProfileName = profile[0].ProfileName;
                        stationAuth.ProfileType = profile[0].ProfileType;
                        Station.Add(stationAuth);
                    }
                    else if (profile.Count > 1)
                    {
                        for (int p = 0; p < profile.Count; p++)
                        {
                            clsStationAuth stationAuth = new clsStationAuth();
                            stationAuth.ProfileName = profile[p].ProfileName;
                            stationAuth.ProfileType = profile[p].ProfileType;
                            Station.Add(stationAuth);
                        }
                    }

                    clsAuthSuccess ObjJsonResp = new clsAuthSuccess();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                    ObjJsonResp.message = "success";
                    ObjJsonResp.status = true;
                    ObjJsonResp.data = Station;

                    return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                    ObjJsonResp.message = "data not found";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                }

            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
            }
        }

        #region Station Summary Detail
        [HttpPost]
        [Route("stationdatadetail")]
        public async Task<HttpResponseMessage> GetStationDataDetail()
        {
            strFunctionName = "Get Station Summary with Parameter Details";

            List<clsStationSummary> LstStationDataDetail = new List<clsStationSummary>();
            //List<GeoCoordinate> listGeoCoodinate = new List<GeoCoordinate>();

            try
            {
                var objStIDs = await Request.Content.ReadAsFormDataAsync();

                string Profile = Regex.Unescape(objStIDs.Get(0));
                string StIDs = Regex.Unescape(objStIDs.Get(1));
                string CurrDT = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");

                DataSet dsStationdetail = null;
                if (Profile == "VMC-AWS-GUJ")
                {
                    for (int j = 0; j < 3; j++)
                    {
                        dsStationdetail = ObjDB.FetchData_GenericSummary("[AWSAPI].[DemoGenericStationData]", Profile, StIDs, CurrDT, CurrDT, "Web");

                        if (dsStationdetail.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        dsStationdetail = ObjDB.FetchData_GenericSummary("[AWSAPI].[GenericStationData]", Profile, StIDs, CurrDT, CurrDT, "Web");

                        if (dsStationdetail.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }
                }

                string OnOffStatus = "Offline";

                if (dsStationdetail.Tables.Count > 0)
                {
                    if (dsStationdetail.Tables[0].Rows.Count > 0)
                    {
                        for (int s = 0; s < dsStationdetail.Tables[0].Rows.Count; s++)
                        {

                            if (DateTime.Now.ToString("yyyy-MM-dd") == dsStationdetail.Tables[0].Rows[s]["Date"].ToString())
                                OnOffStatus = "Online";
                            else
                                OnOffStatus = "Offline";

                            clsStationSummary clsSD = new clsStationSummary();

                            if (Profile.ToLower().Contains("vmc"))
                                clsSD.RefreshRate = 3;
                            else
                                clsSD.RefreshRate = 15;

                            string StationID = dsStationdetail.Tables[0].Rows[s]["StationID"].ToString();
                            string StationName = dsStationdetail.Tables[0].Rows[s]["StationName"].ToString();
                            string Location = dsStationdetail.Tables[0].Rows[s]["Location"].ToString();
                            string StationType = dsStationdetail.Tables[0].Rows[s]["StationType"].ToString();

                            string InstallationDate = dsStationdetail.Tables[0].Rows[s]["InstallationDate"].ToString();
                            string Latitude = dsStationdetail.Tables[0].Rows[s]["lat"].ToString();
                            string Longitude = dsStationdetail.Tables[0].Rows[s]["lng"].ToString();

                            clsSD.StationID = StationID;
                            clsSD.StationName = StationName;
                            clsSD.StationType = StationType;
                            clsSD.Location = Location;

                            clsSD.InstallationDate = InstallationDate;
                            clsSD.lat = Latitude;
                            clsSD.lng = Longitude;
                            //clsSD.centerLat = geoCoordinate.Latitude.ToString();
                            //clsSD.centerLng = geoCoordinate.Longitude.ToString();

                            //Change by vikas ---> 2023-05-29....
                            if (Profile.ToLower().Contains("vmc"))
                            {
                                clsSD.centerLat = "22.464854547568365";
                                clsSD.centerLng = "73.31506468355656";
                            }
                            else if (Profile.ToLower().Contains("forest-goa"))
                            {
                                clsSD.centerLat = "15.2993";
                                clsSD.centerLng = "74.1240";
                            }

                            clsSD.Status = OnOffStatus;
                            clsSD.Date = Convert.ToDateTime(dsStationdetail.Tables[0].Rows[s]["Date"]).ToString("dd/MM/yyyy");
                            clsSD.Time = Convert.ToDateTime(dsStationdetail.Tables[0].Rows[s]["Time"]).ToString("HH:mm tt");

                            //string[] UnitArr = dsStationdetail.Tables[0].Rows[s]["Unit"].ToString().Replace("NA", "").Split(',');

                            List<clsParaSummaryDetail> listparaDetails = new List<clsParaSummaryDetail>();

                            //string[] StID = StIDs.Split(',');

                            //Get column List which should be display in Grid.....
                            //string displayColumnQry = @"select top 1 STUFF( (
                            //    SELECT ',' + ''''+s.SensorName+'''' 
                            //    FROM tbl_Showingraph s where s.StationID='BDC00001' 
                            //    ORDER BY s.SensorName FOR XML PATH('')
                            //),
                            //    1, 1, '') as SensorName ,sm.Name as StationName,sm.ShowInGrid,srv.unit as Unit,sm.Profile as StationType from tbl_StationMaster sm join tbl_StationRangeValidation srv on sm.Profile=srv.ProfileName join 
                            //tbl_ShowInGraph as sg  on sm.StationID=sg.StationID where sm.StationID = 'BDC00001'  
                            //";

                            /*string displayColumnQry = @"
                            select top 1 pm.SensorName ,sm.Name as StationName,sm.ShowInGrid,srv.unit as Unit,sm.Profile as StationType from tbl_StationMaster sm join tbl_StationRangeValidation srv on sm.Profile=srv.ProfileName join 
                            tbl_ShowInGraph as sg  on sm.StationID=sg.StationID join tbl_ProfileMaster as pm on sm.Profile=pm.Name where sm.StationID = '" + StationID.Trim() + "'";*/

                            string displayColumnQry = @"select top 1 pm.SensorName ,sm.Name as StationName,sm.ShowInGrid,srv.unit as Unit,sm.Profile as StationType from tbl_StationMaster sm with(nolock)  join tbl_StationRangeValidation srv  with(nolock) on sm.Profile=srv.ProfileName join 
                            tbl_ShowInGraph as sg  with(nolock) on sm.StationID=sg.StationID join tbl_ProfileMaster as pm  with(nolock) on sm.Profile=pm.Name  where sm.StationID = '" + StationID.Trim() + "'";

                            DataTable dtDisplayColumn = null;

                            for (int d = 0; d < 3; d++)
                            {
                                dtDisplayColumn = ObjDB.FetchDataTable(displayColumnQry, "web");
                                if (dtDisplayColumn != null)
                                    break;
                                Thread.Sleep(1000);
                            }

                            if (dtDisplayColumn.Rows.Count > 0)
                            {
                                List<string> lstShortParaNM = new List<string>();

                                //Get Alias Name of Parameter (Short name of paraeter to display in mobile app)....
                                string strParaAliasNm = string.Empty;

                                DataTable dtSensorNM = null;

                                for (int n = 0; n < 3; n++)
                                {
                                    dtSensorNM = ObjDB.FetchDataTable("select SensorName from tbl_ProfileMaster WITH (NOLOCK)  where Name='" + Profile.Trim() + "'", "web");
                                    if (dtSensorNM != null)
                                        break;
                                    Thread.Sleep(1000);
                                }

                                if (dtSensorNM.Rows.Count > 0)
                                {
                                    string[] arrSensorNm = dtSensorNM.Rows[0][0].ToString().Split(',');

                                    DataTable dtAliasNm = null;

                                    for (int e = 0; e < arrSensorNm.Length; e++)
                                    {
                                        dtAliasNm = ObjDB.FetchDataTable("select Aliasname from AWSAPI.tbl_ParaShortName WITH (NOLOCK) where Parametername='" + arrSensorNm[e].Trim() + "'", "web");

                                        if (dtAliasNm.Rows.Count > 0)
                                        {
                                            strParaAliasNm += dtAliasNm.Rows[0][0].ToString() + ",";
                                        }

                                    }
                                }

                                //strParaAliasNm = strParaAliasNm.TrimEnd(',');
                                List<string> lstParaAliasNm = strParaAliasNm.Split(',').ToList();

                                if (Profile == "VMC-AWS-GUJ")
                                {
                                    lstParaAliasNm.RemoveAt(lstParaAliasNm.Count - 2);
                                }
                                else
                                {
                                    lstParaAliasNm.RemoveAt(lstParaAliasNm.Count - 1);
                                }
                                List<string> lstDisplayColumn = dtDisplayColumn.Rows[0]["SensorName"].ToString().Split(',').ToList();
                                List<string> lstGridColumn = dtDisplayColumn.Rows[0]["ShowInGrid"].ToString().Split(',').ToList();
                                List<string> lstUnitColumn = dtDisplayColumn.Rows[0]["Unit"].ToString().Split(',').ToList();

                                for (int c = 0; c < lstGridColumn.Count(); c++)
                                {
                                    if (lstGridColumn[c] == "1")
                                    {
                                        if (lstDisplayColumn[c] != "StationID" && lstDisplayColumn[c] != "Date" && lstDisplayColumn[c] != "Time" && lstDisplayColumn[c] != "Mobile" && lstDisplayColumn[c] != "WIND GUST" && lstDisplayColumn[c] != "WIND DIRECTION AT MAX WIND SPEED" && lstDisplayColumn[c] != "RAIN RATE")
                                        {
                                            if (lstDisplayColumn[c].ToString().ToLower().Contains("air temp"))
                                                lstUnitColumn[c] = "°C";
                                            else if (lstDisplayColumn[c].ToString().ToLower().Contains("wind dir"))
                                                lstUnitColumn[c] = "°";
                                            else if (lstDisplayColumn[c].ToString().ToLower().Contains("high dir"))
                                                if (Profile == "VMC-AWS-GUJ")
                                                    lstDisplayColumn[c] = "WIND DIRECTION AT MAX WIND SPEED";
                                            clsParaSummaryDetail paraSummary = new clsParaSummaryDetail();
                                            paraSummary.ParameterName = lstParaAliasNm[c].Trim(); //lstDisplayColumn[c].ToString();
                                            string paraType = funSensorType(lstDisplayColumn[c].ToString());
                                            paraSummary.Type = paraType;
                                            paraSummary.ParameterValue = dsStationdetail.Tables[0].Rows[s][lstDisplayColumn[c].ToString().Trim()].ToString();
                                            paraSummary.ParameterUnit = lstUnitColumn[c].ToString();
                                            listparaDetails.Add(paraSummary);
                                        }

                                    }
                                }

                                clsSD.paraDetails = listparaDetails;
                                LstStationDataDetail.Add(clsSD);

                            }

                        }

                        return Request.CreateResponse(HttpStatusCode.OK, LstStationDataDetail);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Authentication Fail...User does not have permission to access station");
                    }

                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Authentication Fail...User does not have permission to access station");
                }

            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                return Request.CreateResponse(HttpStatusCode.OK, "Authentication Fail");
            }

        }

        [HttpPost]
        [Route("finalStSummary")]
        public async Task<HttpResponseMessage> GetStSummary()
        {
            strFunctionName = "Get Station Summary";

            var objSts = await Request.Content.ReadAsFormDataAsync();
            string Profile = Regex.Unescape(objSts.Get(0));

            try
            {
                DataSet dsData = null;

                for (int j = 0; j < 3; j++)
                {
                    dsData = ObjDB.FetchDataset("select * from AWSAPI.tbl_Summary with(nolock) where Profile='" + Profile.Trim() + "'", "Web");
                    if (dsData.Tables.Count > 0)
                        break;
                    Thread.Sleep(1000);
                }

                if (dsData.Tables.Count > 0)
                {
                    if (dsData.Tables[0].Rows.Count > 0)
                    {
                        string jString = dsData.Tables[0].Rows[0]["JsonString"].ToString();

                        List<clsStationSummary> finalStr = JsonConvert.DeserializeObject<List<clsStationSummary>>(jString.Replace("?", "◦"));

                        clsJsonSuccess ObjJsonResp = new clsJsonSuccess();
                        ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                        ObjJsonResp.message = "success";
                        ObjJsonResp.status = true;
                        ObjJsonResp.data = finalStr;

                        return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                    }
                    else
                    {
                        clsJsonFail ObjJsonResp = new clsJsonFail();
                        ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                        ObjJsonResp.message = "data not found";
                        ObjJsonResp.status = false;
                        return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                    }
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                    ObjJsonResp.message = "data not found";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                }

            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
            }

        }

        public string WindRoseData(string winddirection, string windspeed)
        {
            string WindDir = "";

            if (!string.IsNullOrEmpty(winddirection) && !string.IsNullOrEmpty(windspeed))
            {
                double WindDirDegree = Convert.ToDouble(winddirection);
                string[] Sector = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
                WindDir = Sector[Convert.ToInt32(Math.Round((WindDirDegree % 360) / 22.5))];
            }

            return WindDir;
        }

        #endregion Station Summary Detail


        #region Station DataMinMax Detail

        [HttpPost]
        [Route("StDataMinMax")]
        public async Task<HttpResponseMessage> PostFinalStationDataMinMax()
        {
            strFunctionName = "Send Station Data with MinMax detail Async";

            List<clsStationGraphDetail> graphDetailList = new List<clsStationGraphDetail>();

            try
            {
                var objStation = await Request.Content.ReadAsFormDataAsync();

                if (objStation != null)
                {
                    string StID = Regex.Unescape(objStation.Get("StationID"));
                    string frDT = (Regex.Unescape(objStation.Get("fromDate")) == null || Regex.Unescape(objStation.Get("fromDate")) == "") ? "" : Regex.Unescape(objStation.Get("fromDate"));
                    string toDT = (Regex.Unescape(objStation.Get("toDate")) == null || Regex.Unescape(objStation.Get("toDate")) == "") ? "" : Regex.Unescape(objStation.Get("toDate"));
                    string status = Regex.Unescape(objStation.Get("Status"));

                    string selqry = @"select pm.Name,pm.SensorName,srm.unit,srm.ValidationString,sm.Name as StationName,sm.District,sm.ShowInGraph,sm.ShowInGrid from [tbl_StationMaster ] sm join tbl_StationRangeValidation srm on
                    sm.Profile=srm.ProfileName join tbl_ProfileMaster pm on sm.Profile = pm.Name where sm.StationID='" + StID.Trim() + "'";

                    string stUnit = string.Empty;
                    string ProfileName = string.Empty;
                    string StationName = string.Empty;
                    string Location = string.Empty;
                    List<string> units = new List<string>();

                    DataSet dsUnit = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsUnit = ObjDB.FetchDataset(selqry, "Web");
                        if (dsUnit.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsUnit.Tables[0].Rows.Count > 0)
                    {
                        ProfileName = dsUnit.Tables[0].Rows[0]["Name"].ToString();
                        StationName = dsUnit.Tables[0].Rows[0]["StationName"].ToString();
                        Location = dsUnit.Tables[0].Rows[0]["District"].ToString();
                        string[] displaypara = dsUnit.Tables[0].Rows[0]["ShowInGraph"].ToString().Split(',');
                        string[] sname = dsUnit.Tables[0].Rows[0]["SensorName"].ToString().Split(',');
                        units = dsUnit.Tables[0].Rows[0]["unit"].ToString().Split(',').ToList();

                        for (int u = 0; u < displaypara.Length; u++)
                        {
                            if (units[u].Trim() == "C")
                            {
                                units[u] = "°C";
                            }
                            else if (units[u].Trim().ToLower().Contains("deg"))
                                units[u] = "◦";

                            if (displaypara[u].Trim() == "1")
                            {
                                stUnit += units[u] + ",";
                            }
                        }
                    }

                    //Last24Hour Data.....
                    DataSet dsSTData = null;

                    for (int j = 0; j < 3; j++)
                    {
                        dsSTData = ObjDB.FetchData_GenericStation("[DBO].[USP_AWSAPI_GetDataV2]", StID, frDT, toDT, status, "Web");
                        //dsSTData = ObjDB.FetchData_GenericStation("[AWSAPI].[GenericPastStationData]", StID, frDT, toDT, status, "Web");
                        if (ProfileName == "VMC-AWS-GUJ")
                        {
                            dsSTData.Tables[0].Columns.Remove("Status");
                        }

                        if (dsSTData.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsSTData.Tables.Count > 0)
                    {

                        if (dsSTData.Tables[0].Rows.Count > 0)
                        {
                            string[] finalUnit = stUnit.Split(',');

                            List<string> columnNameList = dsSTData.Tables[0].Columns.Cast<DataColumn>().Where(x => x.ColumnName != "Status" && x.ColumnName != "Created Date")
                                            .Select(x => x.ColumnName)
                                            .ToList();

                            string[] NewUnit = stUnit.TrimEnd(',').Split(',');

                            //--------------------------------------------------------Current Summary Data....-----------------------------------

                            string CurrDT = DateTime.Now.ToString("yyyy-MM-dd");

                            //Change by vikas --> 10-Mar-2023     
                            DataTable dsCurrData = new DataTable();
                            var CurrData = dsSTData.Tables[0].AsEnumerable().Where(r => r.Field<string>("Date") == CurrDT);

                            if (CurrData.Any())
                            {
                                dsCurrData = CurrData.CopyToDataTable();
                            }

                            double highTemp = 0.0, lowTemp = 0.0;

                            if (!ProfileName.ToLower().Contains("awlr") || ProfileName == "NHP-GUJ-AWS+AWLR")
                            {
                                if (!ProfileName.ToLower().Contains("-arg"))
                                {
                                    highTemp = dsSTData.Tables[0].AsEnumerable()
                                                .Where(t => t.Field<string>("AirTemperature") != "--")
                                                .Select(t => Convert.ToDouble(t.Field<string>("AirTemperature")))
                                                .Max();

                                    lowTemp = dsSTData.Tables[0].AsEnumerable()
                                                .Where(t => t.Field<string>("AirTemperature") != "--")
                                                .Select(t => Convert.ToDouble(t.Field<string>("AirTemperature")))
                                                .Min();

                                    lowTemp = dsSTData.Tables[1].AsEnumerable()
                                                .Where(t => t.Field<string>("MinAirTemperature") != "--")
                                                .Select(t => Convert.ToDouble(t.Field<string>("MinAirTemperature")))
                                                .FirstOrDefault();

                                    highTemp = dsSTData.Tables[1].AsEnumerable()
                                                .Where(t => t.Field<string>("MaxAirTemperature") != "--")
                                                .Select(t => Convert.ToDouble(t.Field<string>("MaxAirTemperature")))
                                                .FirstOrDefault();

                                    //highTemp = dsSTData.Tables[0].AsEnumerable().Where(r => r.Field<string>("Air Temperature") != "--")
                                    //        .Select(x => Convert.ToDouble(x.Field<string>("Air Temperature")))
                                    //        .Max(x => x);

                                    //lowTemp = dsSTData.Tables[0].AsEnumerable().Where(r => r.Field<string>("Air Temperature") != "--")
                                    //        .Select(x => Convert.ToDouble(x.Field<string>("Air Temperature")))
                                    //        .Min(x => x);


                                    clsStationGraphDetail stationGraphCurrData = new clsStationGraphDetail();
                                    stationGraphCurrData.Type = "currentdata";

                                    /*
                                    clsCurrentData currentData = new clsCurrentData();
                                    currentData.Time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                    currentData.Temperature = dsSTData.Tables[0].Rows[dsSTData.Tables[0].Rows.Count - 1]["Air Temperature"].ToString();
                                    currentData.HighTemp = highTemp.ToString();
                                    currentData.LowTemp = lowTemp.ToString();

                                    int idx = columnNameList.IndexOf("Humidity");
                                    if (idx >= 0)
                                        currentData.Humidity = dsSTData.Tables[0].Rows[dsSTData.Tables[0].Rows.Count - 1]["Humidity"].ToString();
                                    else
                                        currentData.Humidity = dsSTData.Tables[0].Rows[dsSTData.Tables[0].Rows.Count - 1]["Relative Humidity"].ToString();

                                    string WR = WindRoseData(dsSTData.Tables[0].Rows[dsSTData.Tables[0].Rows.Count - 1]["Wind Direction"].ToString(), dsSTData.Tables[0].Rows[dsSTData.Tables[0].Rows.Count - 1]["Wind Speed"].ToString());
                                    currentData.Wind = WR;
                                    currentData.WindSpeed = dsSTData.Tables[0].Rows[dsSTData.Tables[0].Rows.Count - 1]["Wind Speed"].ToString();

                                    currentData.Pressure = dsSTData.Tables[0].Rows[dsSTData.Tables[0].Rows.Count - 1]["Atmospheric Pressure"].ToString();
                                    stationGraphCurrData.CurrentData = currentData;

                                    graphDetailList.Add(stationGraphCurrData);
                                    */

                                    DataSet dsSummaryData = null;


                                    for (int j = 0; j < 3; j++)
                                    {
                                        dsSummaryData = ObjDB.FetchDataset("select * from AWSAPI.tbl_Summary with(nolock) where Profile='" + ProfileName.Trim() + "' ", "Web");
                                        if (dsSummaryData.Tables.Count > 0)
                                            break;
                                        Thread.Sleep(1000);
                                    }

                                    if (dsSummaryData.Tables[0].Rows.Count > 0)
                                    {
                                        string jString = dsSummaryData.Tables[0].Rows[0]["JsonString"].ToString();

                                        List<clsStationSummary> finalStr = JsonConvert.DeserializeObject<List<clsStationSummary>>(jString.Replace("?", "◦"));

                                        clsCurrentData currentData = new clsCurrentData();

                                        for (int s = 0; s < finalStr.Count; s++)
                                        {
                                            if (finalStr[s].StationID.Trim() == StID.Trim())
                                            {
                                                currentData.Time = DateTime.Now.ToString("dd-MM-yyyy") + " " + finalStr[s].Time;

                                                for (int p = 0; p < finalStr[s].paraDetails.Count; p++)
                                                {


                                                    if (finalStr[s].paraDetails[p].ParameterName.ToLower().Contains("temp"))
                                                    {
                                                        if (!finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("temp min") && !finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("temp max") && !finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("temp dayminmax") && !finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("soiltemp 10cm") && !finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("soiltemp 70cm"))
                                                        {
                                                            currentData.Temperature = finalStr[s].paraDetails[p].ParameterValue.Trim();
                                                            currentData.HighTemp = highTemp.ToString();
                                                            currentData.LowTemp = lowTemp.ToString();
                                                        }
                                                    }
                                                    else if (finalStr[s].paraDetails[p].ParameterName.ToLower().Contains("hum"))
                                                    {
                                                        if (!finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("humi min") && !finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("humi max") && !finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("humi dayminmax"))
                                                            currentData.Humidity = finalStr[s].paraDetails[p].ParameterValue.Trim();
                                                    }
                                                    else if (finalStr[s].paraDetails[p].ParameterName.ToLower().Contains("wd"))
                                                        currentData.Wind = finalStr[s].paraDetails[p].ParameterValue.Trim();
                                                    else if (finalStr[s].paraDetails[p].ParameterName.ToLower().Contains("ws"))
                                                    {
                                                        if (finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("ws 10m1minavg"))
                                                            currentData.WindSpeed = finalStr[s].paraDetails[p].ParameterValue.Trim();
                                                        else if (!finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("ws 10m3minavg") && !finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("ws 10m10minavg") && !finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("maxws 10m") && !finalStr[s].paraDetails[p].ParameterName.ToLower().Equals("ws daymax10m"))
                                                            currentData.WindSpeed = finalStr[s].paraDetails[p].ParameterValue.Trim();

                                                    }
                                                    else if (finalStr[s].paraDetails[p].ParameterName.ToLower().Contains("press"))
                                                        currentData.Pressure = finalStr[s].paraDetails[p].ParameterValue.Trim();
                                                }

                                                break;
                                            }
                                        }

                                        string WR = WindRoseData(currentData.Wind, currentData.WindSpeed);
                                        currentData.Wind = WR;
                                        stationGraphCurrData.CurrentData = currentData;
                                        graphDetailList.Add(stationGraphCurrData);

                                    }

                                }

                                //---------------------------------------Rain Summary-----------------------------------------------------------

                                clsStationGraphDetail stationGraph_rain = new clsStationGraphDetail();
                                stationGraph_rain.ParameterName = "Rain";
                                stationGraph_rain.Type = "rainsummary";

                                List<clsRainDetail> LstRainDetails = RainSummary(ProfileName, StID);

                                if (LstRainDetails.Count > 0)
                                {
                                    clsRainDetail rainDetail = new clsRainDetail();
                                    rainDetail.CurrentDay = LstRainDetails[0].CurrentDay;
                                    rainDetail.LastDay = LstRainDetails[0].LastDay;
                                    rainDetail.Rain24HR = LstRainDetails[0].Rain24HR;
                                    rainDetail.LastHour = LstRainDetails[0].LastHour;
                                    rainDetail.MonthTotal = LstRainDetails[0].MonthTotal;
                                    rainDetail.YearTotal = LstRainDetails[0].YearTotal;
                                    rainDetail.CurrentRainRate = LstRainDetails[0].CurrentRainRate;
                                    rainDetail.StormDuration = LstRainDetails[0].StormDuration;
                                    rainDetail.StormValue = LstRainDetails[0].StormValue;
                                    rainDetail.StormUnit = "mm";
                                    rainDetail.StormStartDateTime = LstRainDetails[0].StormStartDateTime;
                                    rainDetail.RainUnit = "mm";
                                    stationGraph_rain.RainData = rainDetail;
                                    graphDetailList.Add(stationGraph_rain);
                                }
                                //--------------------------------------------------------------------------------------------------------------------
                            }

                            if (!ProfileName.Contains("VMC"))
                            {
                                clsStationGraphDetail stationGraphCummRain = new clsStationGraphDetail();
                                stationGraphCummRain.ParameterName = "Cummulative Rain";
                                stationGraphCummRain.Type = "rain";
                                stationGraphCummRain.GraphType = "both";

                                List<clsGraphData> LstgraphCummData = new List<clsGraphData>();
                                List<clsGraphData> LstgraphBarCummData = new List<clsGraphData>();

                                string CurrDate = DateTime.Now.ToString("yyyy-MM-dd");
                                DateTime time = DateTime.ParseExact("08:00", "HH:mm", CultureInfo.InvariantCulture);

                                for (int s = 1; s < dsSTData.Tables[0].Rows.Count; s++)
                                {
                                    clsGraphData graphData = new clsGraphData();
                                    graphData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                    graphData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                    graphData.ParameterValue = dsSTData.Tables[0].Rows[s]["Daily Rain"].ToString();
                                    graphData.ParameterUnit = "mm";
                                    LstgraphCummData.Add(graphData);
                                    stationGraphCummRain.GraphData = LstgraphCummData;

                                    clsGraphData graphBarData = new clsGraphData();
                                    graphBarData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                    graphBarData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                    graphBarData.ParameterValue = dsSTData.Tables[0].Rows[s]["Hourly Rainfall"].ToString();
                                    graphBarData.ParameterUnit = "mm";
                                    LstgraphBarCummData.Add(graphBarData);
                                    stationGraphCummRain.GraphBarData = LstgraphBarCummData;
                                }

                                graphDetailList.Add(stationGraphCummRain);

                                for (int c = 3; c < dsSTData.Tables[0].Columns.Count; c++)
                                {
                                    if (columnNameList[c].Trim() != "Hourly Rainfall" && columnNameList[c].Trim() != "Daily Rain")
                                    {

                                        clsStationGraphDetail stationGraphDetail = new clsStationGraphDetail();
                                        stationGraphDetail.ParameterName = dsSTData.Tables[0].Columns[c].ToString();

                                        string type = funSensorType(columnNameList[c]);
                                        stationGraphDetail.Type = type;

                                        //Change by vikas ---> 2023-05-27...
                                        //stationGraphDetail.GraphType = "Line";
                                        if (columnNameList[c].ToLower().Contains("speed"))
                                            stationGraphDetail.GraphType = "Bar";
                                        else
                                            stationGraphDetail.GraphType = "Line";


                                        stationGraphDetail.xAxisTitle = "Time";
                                        stationGraphDetail.yAxisTile = columnNameList[c].Trim();

                                        string MinQry = "select top 1 Date,Time, [" + columnNameList[c] + "] as [Min_" + columnNameList[c].Replace(" ", "") + "] from tbl_StationData_" + StID + " where ";
                                        MinQry += "[" + columnNameList[c] + "] = (select min([" + columnNameList[c] + "]) from tbl_StationData_" + StID + " with(nolock) ";
                                        MinQry += " where Date >=  '" + frDT + "' and  Date <='" + toDT + "') and  Date >=  '" + frDT + "' and  Date <='" + toDT + "' order by Date desc,Time desc";

                                        DataSet dsMinVal = null;
                                        for (int i = 0; i < 3; i++)
                                        {
                                            dsMinVal = ObjDB.FetchDataset(MinQry, "Web");
                                            if (dsMinVal.Tables.Count > 0)
                                                break;
                                            Thread.Sleep(1000);
                                        }

                                        for (int j = 0; j < dsMinVal.Tables[0].Rows.Count; j++)
                                        {
                                            stationGraphDetail.MinDate = dsMinVal.Tables[0].Rows[0][0].ToString();
                                            stationGraphDetail.MinTime = dsMinVal.Tables[0].Rows[0][1].ToString();
                                            stationGraphDetail.MinValue = dsMinVal.Tables[0].Rows[0][2].ToString();
                                        }

                                        string MaxQry = "select top 1 Date,Time, [" + columnNameList[c] + "] as [Max_" + columnNameList[c] + "] from tbl_StationData_" + StID + " where ";
                                        MaxQry += "[" + columnNameList[c] + "] = (select max([" + columnNameList[c] + "]) from tbl_StationData_" + StID + "  with(nolock) ";
                                        MaxQry += " where Date >=  '" + frDT + "' and  Date <='" + toDT + "') and  Date >=  '" + frDT + "' and  Date <='" + toDT + "' order by Date desc,Time desc";

                                        DataSet dsMaxVal = null;
                                        for (int i = 0; i < 3; i++)
                                        {
                                            dsMaxVal = ObjDB.FetchDataset(MaxQry, "Web");
                                            if (dsMaxVal.Tables.Count > 0)
                                                break;
                                            Thread.Sleep(1000);
                                        }

                                        for (int i = 0; i < dsMaxVal.Tables[0].Rows.Count; i++)
                                        {
                                            stationGraphDetail.MaxDate = dsMaxVal.Tables[0].Rows[0][0].ToString();
                                            stationGraphDetail.MaxTime = dsMaxVal.Tables[0].Rows[0][1].ToString();
                                            stationGraphDetail.MaxValue = dsMaxVal.Tables[0].Rows[0][2].ToString();
                                        }

                                        List<clsGraphData> LstgraphData = new List<clsGraphData>();
                                        List<clsGraphData> LstgraphBarData = new List<clsGraphData>();
                                        List<clsGraphData> LstgraphBarDataWG = new List<clsGraphData>();

                                        for (int s = 0; s < dsSTData.Tables[0].Rows.Count; s++)
                                        {
                                            if (columnNameList[c].ToLower().Contains("speed"))
                                            {
                                                clsGraphData graphBarData = new clsGraphData();
                                                graphBarData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                                graphBarData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                                graphBarData.ParameterValue = dsSTData.Tables[0].Rows[s][c].ToString().Contains("--") == true ? "0" : dsSTData.Tables[0].Rows[s][c].ToString();
                                                graphBarData.ParameterUnit = NewUnit[c] == "NA" ? "" : NewUnit[c];
                                                LstgraphBarData.Add(graphBarData);

                                                stationGraphDetail.GraphBarData = LstgraphBarData;

                                                clsGraphData graphBarDataWG = new clsGraphData();
                                                graphBarDataWG.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                                graphBarDataWG.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();

                                                //Change by vikas --> 10-Mar-2023
                                                if (dsSTData.Tables[0].Rows[s][c].ToString().Contains("--"))
                                                {
                                                    dsSTData.Tables[0].Rows[s][c] = "00.0";
                                                }

                                                graphBarDataWG.ParameterValue = "0"; //(Convert.ToDouble(dsSTData.Tables[0].Rows[s][c]) * 1.3).ToString();
                                                graphBarDataWG.ParameterUnit = NewUnit[c] == "NA" ? "" : NewUnit[c];
                                                LstgraphBarDataWG.Add(graphBarDataWG);

                                                stationGraphDetail.GraphBarDataWG = LstgraphBarDataWG;
                                            }
                                            else
                                            {
                                                clsGraphData graphData = new clsGraphData();
                                                graphData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                                graphData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();

                                                //Change by vikas --> 2023-05-26....
                                                //graphData.ParameterValue = dsSTData.Tables[0].Rows[s][c].ToString();
                                                graphData.ParameterValue = dsSTData.Tables[0].Rows[s][c].ToString().Contains("--") == true ? "0" : dsSTData.Tables[0].Rows[s][c].ToString();
                                                graphData.ParameterUnit = NewUnit[c] == "NA" ? "" : NewUnit[c];
                                                LstgraphData.Add(graphData);
                                                stationGraphDetail.GraphData = LstgraphData;
                                            }
                                        }

                                        graphDetailList.Add(stationGraphDetail);
                                    }
                                }
                            }
                            else if (ProfileName.Trim() == "VMC-AWS-GUJ")
                            {
                                string[] columnNames = dsSTData.Tables[0].Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                                List<string> finalcolList = columnNames.ToList();

                                clsStationGraphDetail stationGraphCummRain = new clsStationGraphDetail();
                                stationGraphCummRain.ParameterName = "Cummulative Rain";
                                stationGraphCummRain.Type = "rain";
                                stationGraphCummRain.GraphType = "both";

                                List<clsGraphData> LstgraphCummData = new List<clsGraphData>();
                                List<clsGraphData> LstgraphBarCummData = new List<clsGraphData>();

                                //string[] strCumRainFinal = new string[dsSTData.Tables[0].Rows.Count];


                                string CurrDate = DateTime.Now.ToString("yyyy-MM-dd");
                                DateTime time = DateTime.ParseExact("08:00", "HH:mm", CultureInfo.InvariantCulture);

                                for (int s = 0; s < dsSTData.Tables[0].Rows.Count; s++)
                                {
                                    string finalHR = "";

                                    string CurrTm = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                    //string PreTm = "";
                                    double DiffHR = 0;

                                    //16:00
                                    /*if (CurrTm != "00:00" && !CurrTm.Contains(":15"))
                                    {

                                        *//*DateTime PreviousTm = Convert.ToDateTime(CurrTm).Subtract(new TimeSpan(0, 15, 0));
                                        PreTm = PreviousTm.ToString("HH:mm");

                                        string PreHRSelQry = "select Date,Time,[Hourly Rainfall] from tbl_StationData_" + StID.Trim() + " with (nolock) Where Date = '2023-03-16' and Time = '" + PreTm + "'";

                                        DataSet dsPreHR = null;
                                        for (int p = 0; p < 3; p++)
                                        {
                                            dsPreHR = ObjDB.FetchDataset(PreHRSelQry, "web");
                                            if (dsPreHR.Tables.Count > 0)
                                                break;
                                            Thread.Sleep(1000);
                                        }

                                        DiffHR = Convert.ToDouble(dsSTData.Tables[0].Rows[s]["Hourly Rainfall"]) - Convert.ToDouble(dsPreHR.Tables[0].Rows[0]["Hourly Rainfall"]);
                                        *//*
                                        if(s == 0)
                                        {
                                            DateTime DT = Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Date"].ToString());
                                            string finalDT = DT.ToString("yyyy-MM-dd");

                                            DateTime Tm = Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Time"].ToString()).AddMinutes(-15);
                                            string finalTm = Tm.ToString("HH:mm");

                                            string fetchQry = "select Top 1 Date,Time, [Hourly Rainfall] from tbl_StationData_" + StID.Trim() + " with(nolock) where Date = '" + finalDT.Trim()  + "' and Time = '" + finalTm + "' order by Time desc";

                                            DataSet dsHR = null;

                                            for (int h = 0; h < 3; h++)
                                            {
                                                dsHR = ObjDB.FetchDataset(fetchQry, "web");
                                                if (dsHR.Tables.Count > 0)
                                                    break;
                                                Thread.Sleep(1000);
                                            }
                                        }
                                        else
                                            DiffHR = Convert.ToDouble(dsSTData.Tables[0].Rows[s]["Hourly Rainfall"]) - Convert.ToDouble(dsSTData.Tables[0].Rows[s - 1]["Hourly Rainfall"]);

                                        if (DiffHR <= 0)
                                            DiffHR = 000.0;

                                        finalHR = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                                    }
                                    else if (CurrTm.Contains(":15"))
                                    {
                                        DiffHR = Convert.ToDouble(dsSTData.Tables[0].Rows[s]["Hourly Rainfall"]);
                                        finalHR = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                                    }
                                    else if (CurrTm == "00:00")
                                    {
                                        DateTime PreDT = Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Date"].ToString()).AddDays(-1);
                                        string finalPreDT = PreDT.ToString("yyyy-MM-dd");

                                        string fetchQry = "select Top 1 Date,Time, [Hourly Rainfall] from tbl_StationData_" + StID.Trim() + " with(nolock) where Date = '" + finalPreDT + "' order by Time desc";

                                        DataSet dsHR = null;

                                        for (int h = 0; h < 3; h++)
                                        {
                                            dsHR = ObjDB.FetchDataset(fetchQry, "web");
                                            if (dsHR.Tables.Count > 0)
                                                break;
                                            Thread.Sleep(1000);
                                        }

                                        DiffHR = Convert.ToDouble(dsSTData.Tables[0].Rows[s]["Hourly Rainfall"]) - Convert.ToDouble(dsHR.Tables[0].Rows[0]["Hourly Rainfall"]);

                                        if (DiffHR <= 0)
                                            DiffHR = 000.0;

                                        finalHR = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                                    }*/

                                    //strCumRainFinal[s] = dsSTData.Tables[0].Rows[s]["Date"].ToString() + " , "  + dsSTData.Tables[0].Rows[s]["Time"].ToString() + " , " +  finalHR + " , " + dsSTData.Tables[0].Rows[s]["Daily Rain"].ToString();

                                    clsGraphData graphData = new clsGraphData();
                                    graphData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                    graphData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                    graphData.ParameterValue = dsSTData.Tables[0].Rows[s]["DailyRain"].ToString(); //finalHR;
                                    graphData.ParameterUnit = "mm";
                                    LstgraphCummData.Add(graphData);
                                    stationGraphCummRain.GraphData = LstgraphCummData;

                                    // if (dsSTData.Tables[0].Rows[s]["Date"].ToString() == CurrDate && Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Time"]) >= time)
                                    // {

                                    clsGraphData graphBarData = new clsGraphData();
                                    graphBarData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                    graphBarData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                    //Cummulative Rain...
                                    graphBarData.ParameterValue = finalHR; //dsSTData.Tables[0].Rows[s]["Daily Rain"].ToString();
                                    graphBarData.ParameterUnit = "mm";
                                    LstgraphBarCummData.Add(graphBarData);
                                    stationGraphCummRain.GraphBarData = LstgraphBarCummData;

                                    // }
                                }


                                /*
                                for (int s = 0; s < dsSTData.Tables[0].Rows.Count; s++)
                                {
                                    clsGraphData graphData = new clsGraphData();
                                    graphData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                    graphData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();

                                    //REMAINING TASK - [B]....
                                    //Implement Logic of Remove 15min summation for converting [Hourly Rainfall] to (15minRainfall)...

                                    string RemoveSumHR = funRemoveSummationHR(dsSTData, StID, s);

                                    graphData.ParameterValue = RemoveSumHR;

                                    //graphData.ParameterValue = dsSTData.Tables[0].Rows[s]["Hourly Rainfall"].ToString();

                                    graphData.ParameterUnit = "mm";
                                    LstgraphCummData.Add(graphData);
                                    stationGraphCummRain.GraphData = LstgraphCummData;

                                    if (dsSTData.Tables[0].Rows[s]["Date"].ToString() == CurrDate && Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Time"]) >= time)
                                    {
                                        clsGraphData graphBarData = new clsGraphData();
                                        graphBarData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                        graphBarData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();

                                        //Cummulative Rain...
                                        graphBarData.ParameterValue = dsSTData.Tables[0].Rows[s]["Daily Rain"].ToString();
                                        graphBarData.ParameterUnit = "mm";
                                        LstgraphBarCummData.Add(graphBarData);
                                        stationGraphCummRain.GraphBarData = LstgraphBarCummData;
                                    }

                                }
                                */

                                graphDetailList.Add(stationGraphCummRain);

                                //finalcolList.Add("Dew Point");
                                //finalcolList.Add("Wind Run");
                                //finalcolList.Add("Wind Chill");
                                //finalcolList.Add("Heat Index");
                                //finalcolList.Add("THW Index");

                                //"Dew Point","Wind Run","Wind Chill","Heat Index","THW Index"
                                stUnit += "°C,m,(kgcal/m2/h),°C,°C";

                                List<string> NewUnit1 = stUnit.Split(',').ToList();
                                NewUnit1.RemoveAt(3);
                                var dsStationData = dsSTData.Copy();
                                DataTable reportDT1 = funVMCDataSetV2(dsSTData);
                                DataTable reportDT = reportDT1.Copy();
                                var reportMin = funGetMinValueV2(dsStationData);
                                var reportMax = funGetMaxValueV2(dsStationData);

                                //DataTable reportDT1 = funVMCDataSet(StID, frDT, toDT, status);
                                //DataSet reportMin1 = funMinVal(reportDT, StID, frDT, toDT, status);
                                //DataSet reportMax1 = funMaxVal(reportDT, StID, frDT, toDT, status);

                                if (reportDT.Rows.Count > 0 && reportMin != null && reportMax != null)
                                {
                                    for (int c = 3; c < reportDT.Columns.Count; c++)
                                    {

                                        //if (finalcolList[c] != "15mins RAINFALL" && finalcolList[c] != "Daily Rain" && finalcolList[c] != "HIGH DIRECTION" && finalcolList[c] != "WIND GUST" && finalcolList[c] != "RAIN RATE")
                                        if (finalcolList[c] != "RainFall" && finalcolList[c] != "DailyRain" && finalcolList[c] != "HighDirection" && finalcolList[c] != "WindGust" && finalcolList[c] != "RainRate")
                                        {
                                            if (finalcolList[c] == "WindSpeed")
                                            {
                                                clsStationGraphDetail stationGraphDetail = new clsStationGraphDetail();

                                                stationGraphDetail.ParameterName = finalcolList[c];
                                                stationGraphDetail.GraphType = "Bar";

                                                string type = funSensorType(finalcolList[c]);
                                                stationGraphDetail.Type = type;

                                                stationGraphDetail.xAxisTitle = "Time";
                                                stationGraphDetail.yAxisTile = finalcolList[c].Trim();

                                                stationGraphDetail.MinDate = reportMin.Rows[c - 3][0].ToString();
                                                stationGraphDetail.MinTime = reportMin.Rows[c - 3][1].ToString();
                                                stationGraphDetail.MinValue = reportMin.Rows[c - 3][2].ToString();

                                                stationGraphDetail.MaxDate = reportMax.Rows[c - 3][0].ToString();
                                                stationGraphDetail.MaxTime = reportMax.Rows[c - 3][1].ToString();
                                                stationGraphDetail.MaxValue = reportMax.Rows[c - 3][2].ToString();

                                                List<clsGraphData> LstgraphBarData = new List<clsGraphData>();
                                                List<clsGraphData> LstgraphBarDataWG = new List<clsGraphData>();

                                                for (int r = 0; r < reportDT.Rows.Count; r++)
                                                {
                                                    clsGraphData graphBarData = new clsGraphData();
                                                    graphBarData.Date = reportDT.Rows[r]["Date"].ToString();
                                                    graphBarData.Time = reportDT.Rows[r]["Time"].ToString();
                                                    graphBarData.ParameterValue = reportDT.Rows[r][c].ToString();
                                                    graphBarData.ParameterUnit = NewUnit1[c] == "NA" ? "" : NewUnit1[c];
                                                    LstgraphBarData.Add(graphBarData);

                                                    stationGraphDetail.GraphBarData = LstgraphBarData;

                                                    clsGraphData graphBarDataWG = new clsGraphData();
                                                    graphBarDataWG.Date = reportDT.Rows[r]["Date"].ToString();
                                                    graphBarDataWG.Time = reportDT.Rows[r]["Time"].ToString();

                                                    //Change by vikas --> 10-Mar-2023
                                                    if (reportDT.Rows[r][c].ToString().Contains("--"))
                                                    {
                                                        reportDT.Rows[r][c] = "00.0";
                                                    }

                                                    graphBarDataWG.ParameterValue = reportDT.Rows[r]["WIND GUST"].ToString();
                                                    graphBarDataWG.ParameterUnit = NewUnit1[c] == "NA" ? "" : NewUnit1[c];
                                                    LstgraphBarDataWG.Add(graphBarDataWG);

                                                    stationGraphDetail.GraphBarDataWG = LstgraphBarDataWG;
                                                }

                                                graphDetailList.Add(stationGraphDetail);

                                            }
                                            else
                                            {
                                                clsStationGraphDetail stationGraphDetail = new clsStationGraphDetail();
                                                stationGraphDetail.ParameterName = finalcolList[c];
                                                stationGraphDetail.GraphType = "Line";

                                                string type = funSensorType(finalcolList[c]);
                                                stationGraphDetail.Type = type;

                                                stationGraphDetail.xAxisTitle = "Time";
                                                stationGraphDetail.yAxisTile = finalcolList[c].Trim();

                                                if (!finalcolList[c].ToLower().Contains("direction"))
                                                {
                                                    stationGraphDetail.MinDate = reportMin.Rows[c - 3][0].ToString();
                                                    stationGraphDetail.MinTime = reportMin.Rows[c - 3][1].ToString();
                                                    stationGraphDetail.MinValue = reportMin.Rows[c - 3][2].ToString();

                                                    stationGraphDetail.MaxDate = reportMax.Rows[c - 3][0].ToString();
                                                    stationGraphDetail.MaxTime = reportMax.Rows[c - 3][1].ToString();
                                                    stationGraphDetail.MaxValue = reportMax.Rows[c - 3][2].ToString();
                                                }
                                                else
                                                {
                                                    stationGraphDetail.MinDate = "";
                                                    stationGraphDetail.MinTime = "";
                                                    stationGraphDetail.MinValue = "";

                                                    stationGraphDetail.MaxDate = "";
                                                    stationGraphDetail.MaxTime = "";
                                                    stationGraphDetail.MaxValue = "";
                                                }


                                                List<clsGraphData> LstgraphData = new List<clsGraphData>();

                                                for (int r = 0; r < reportDT.Rows.Count; r++)
                                                {
                                                    clsGraphData graphData = new clsGraphData();
                                                    graphData.Date = reportDT.Rows[r]["Date"].ToString();
                                                    graphData.Time = reportDT.Rows[r]["Time"].ToString();
                                                    graphData.ParameterValue = reportDT.Rows[r][c].ToString();
                                                    graphData.ParameterUnit = NewUnit1[c] == "NA" ? "" : NewUnit1[c];
                                                    LstgraphData.Add(graphData);

                                                    stationGraphDetail.GraphData = LstgraphData;
                                                }

                                                graphDetailList.Add(stationGraphDetail);

                                            }
                                        }
                                    }

                                }
                            }
                            else if (ProfileName.Trim() == "VMC-NHP-GUJ")
                            {
                                string[] columnNames = dsSTData.Tables[0].Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                                List<string> finalcolList = columnNames.ToList();

                                clsStationGraphDetail stationGraphCummRain = new clsStationGraphDetail();
                                stationGraphCummRain.ParameterName = "Cummulative Rain";
                                stationGraphCummRain.Type = "rain";
                                stationGraphCummRain.GraphType = "both";

                                List<clsGraphData> LstgraphCummData = new List<clsGraphData>();
                                List<clsGraphData> LstgraphBarCummData = new List<clsGraphData>();

                                //string[] strCumRainFinal = new string[dsSTData.Tables[0].Rows.Count];


                                string CurrDate = DateTime.Now.ToString("yyyy-MM-dd");
                                DateTime time = DateTime.ParseExact("08:00", "HH:mm", CultureInfo.InvariantCulture);

                                for (int s = 0; s < dsSTData.Tables[0].Rows.Count; s++)
                                {
                                    string finalHR = "";

                                    string CurrTm = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                    //string PreTm = "";
                                    double DiffHR = 0;

                                    //16:00
                                    if (CurrTm != "00:00" && !CurrTm.Contains(":15"))
                                    {

                                        /*DateTime PreviousTm = Convert.ToDateTime(CurrTm).Subtract(new TimeSpan(0, 15, 0));
                                        PreTm = PreviousTm.ToString("HH:mm");

                                        string PreHRSelQry = "select Date,Time,[Hourly Rainfall] from tbl_StationData_" + StID.Trim() + " with (nolock) Where Date = '2023-03-16' and Time = '" + PreTm + "'";

                                        DataSet dsPreHR = null;
                                        for (int p = 0; p < 3; p++)
                                        {
                                            dsPreHR = ObjDB.FetchDataset(PreHRSelQry, "web");
                                            if (dsPreHR.Tables.Count > 0)
                                                break;
                                            Thread.Sleep(1000);
                                        }

                                        DiffHR = Convert.ToDouble(dsSTData.Tables[0].Rows[s]["Hourly Rainfall"]) - Convert.ToDouble(dsPreHR.Tables[0].Rows[0]["Hourly Rainfall"]);
                                        */

                                        if (s == 0)
                                        {
                                            DateTime DT = Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Date"].ToString());
                                            string finalDT = DT.ToString("yyyy-MM-dd");

                                            DateTime Tm = Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Time"].ToString()).AddMinutes(-15);
                                            string finalTm = Tm.ToString("HH:mm");

                                            string fetchQry = "select Top 1 Date,Time, [Hourly Rainfall] from tbl_StationData_" + StID.Trim() + " with(nolock) where Date = '" + finalDT.Trim() + "' and Time = '" + finalTm + "' order by Time desc";

                                            DataSet dsHR = null;

                                            for (int h = 0; h < 3; h++)
                                            {
                                                dsHR = ObjDB.FetchDataset(fetchQry, "web");
                                                if (dsHR.Tables.Count > 0)
                                                    break;
                                                Thread.Sleep(1000);
                                            }
                                        }
                                        else
                                            DiffHR = Convert.ToDouble(dsSTData.Tables[0].Rows[s]["Hourly Rainfall"]) - Convert.ToDouble(dsSTData.Tables[0].Rows[s - 1]["Hourly Rainfall"]);

                                        if (DiffHR <= 0)
                                            DiffHR = 000.0;

                                        finalHR = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                                    }
                                    else if (CurrTm.Contains(":15"))
                                    {
                                        DiffHR = Convert.ToDouble(dsSTData.Tables[0].Rows[s]["Hourly Rainfall"]);
                                        finalHR = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                                    }
                                    else if (CurrTm == "00:00")
                                    {
                                        DateTime PreDT = Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Date"].ToString()).AddDays(-1);
                                        string finalPreDT = PreDT.ToString("yyyy-MM-dd");

                                        string fetchQry = "select Top 1 Date,Time, [Hourly Rainfall] from tbl_StationData_" + StID.Trim() + " with(nolock) where Date = '" + finalPreDT + "' order by Time desc";

                                        DataSet dsHR = null;

                                        for (int h = 0; h < 3; h++)
                                        {
                                            dsHR = ObjDB.FetchDataset(fetchQry, "web");
                                            if (dsHR.Tables.Count > 0)
                                                break;
                                            Thread.Sleep(1000);
                                        }

                                        DiffHR = Convert.ToDouble(dsSTData.Tables[0].Rows[s]["Hourly Rainfall"]) - Convert.ToDouble(dsHR.Tables[0].Rows[0]["Hourly Rainfall"]);

                                        if (DiffHR <= 0)
                                            DiffHR = 000.0;

                                        finalHR = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                                    }

                                    //strCumRainFinal[s] = dsSTData.Tables[0].Rows[s]["Date"].ToString() + " , "  + dsSTData.Tables[0].Rows[s]["Time"].ToString() + " , " +  finalHR + " , " + dsSTData.Tables[0].Rows[s]["Daily Rain"].ToString();

                                    clsGraphData graphData = new clsGraphData();
                                    graphData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                    graphData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                    graphData.ParameterValue = dsSTData.Tables[0].Rows[s]["Daily Rain"].ToString(); //finalHR;
                                    graphData.ParameterUnit = "mm";
                                    LstgraphCummData.Add(graphData);
                                    stationGraphCummRain.GraphData = LstgraphCummData;

                                    // if (dsSTData.Tables[0].Rows[s]["Date"].ToString() == CurrDate && Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Time"]) >= time)
                                    // {

                                    clsGraphData graphBarData = new clsGraphData();
                                    graphBarData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                    graphBarData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                    //Cummulative Rain...
                                    graphBarData.ParameterValue = finalHR; //dsSTData.Tables[0].Rows[s]["Daily Rain"].ToString();
                                    graphBarData.ParameterUnit = "mm";
                                    LstgraphBarCummData.Add(graphBarData);
                                    stationGraphCummRain.GraphBarData = LstgraphBarCummData;

                                    // }
                                }


                                /*
                                for (int s = 0; s < dsSTData.Tables[0].Rows.Count; s++)
                                {
                                    clsGraphData graphData = new clsGraphData();
                                    graphData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                    graphData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();

                                    //REMAINING TASK - [B]....
                                    //Implement Logic of Remove 15min summation for converting [Hourly Rainfall] to (15minRainfall)...

                                    string RemoveSumHR = funRemoveSummationHR(dsSTData, StID, s);

                                    graphData.ParameterValue = RemoveSumHR;

                                    //graphData.ParameterValue = dsSTData.Tables[0].Rows[s]["Hourly Rainfall"].ToString();

                                    graphData.ParameterUnit = "mm";
                                    LstgraphCummData.Add(graphData);
                                    stationGraphCummRain.GraphData = LstgraphCummData;

                                    if (dsSTData.Tables[0].Rows[s]["Date"].ToString() == CurrDate && Convert.ToDateTime(dsSTData.Tables[0].Rows[s]["Time"]) >= time)
                                    {
                                        clsGraphData graphBarData = new clsGraphData();
                                        graphBarData.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                        graphBarData.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();

                                        //Cummulative Rain...
                                        graphBarData.ParameterValue = dsSTData.Tables[0].Rows[s]["Daily Rain"].ToString();
                                        graphBarData.ParameterUnit = "mm";
                                        LstgraphBarCummData.Add(graphBarData);
                                        stationGraphCummRain.GraphBarData = LstgraphBarCummData;
                                    }

                                }
                                */

                                graphDetailList.Add(stationGraphCummRain);

                                finalcolList.Add("Dew Point");
                                finalcolList.Add("Wind Run");
                                finalcolList.Add("Wind Chill");
                                finalcolList.Add("Heat Index");
                                finalcolList.Add("THW Index");

                                //"Dew Point","Wind Run","Wind Chill","Heat Index","THW Index"
                                stUnit += "°C,m,(kgcal/m2/h),°C,°C";

                                List<string> NewUnit1 = stUnit.Split(',').ToList();

                                DataTable reportDT = funVMCDataSet(StID, frDT, toDT, status);

                                DataSet reportMin = funMinVal(reportDT, StID, frDT, toDT, status);

                                DataSet reportMax = funMaxVal(reportDT, StID, frDT, toDT, status);

                                if (reportDT.Rows.Count > 0)
                                {
                                    for (int c = 3; c < reportDT.Columns.Count; c++)
                                    {

                                        if (finalcolList[c] != "Hourly Rainfall" && finalcolList[c] != "Daily Rain")
                                        {
                                            if (finalcolList[c].ToLower().Contains("speed"))
                                            {
                                                clsStationGraphDetail stationGraphDetail = new clsStationGraphDetail();

                                                stationGraphDetail.ParameterName = finalcolList[c];
                                                stationGraphDetail.GraphType = "Bar";

                                                string type = funSensorType(finalcolList[c]);
                                                stationGraphDetail.Type = type;

                                                stationGraphDetail.xAxisTitle = "Time";
                                                stationGraphDetail.yAxisTile = finalcolList[c].Trim();

                                                stationGraphDetail.MinDate = reportMin.Tables[0].Rows[c - 3][0].ToString();
                                                stationGraphDetail.MinTime = reportMin.Tables[0].Rows[c - 3][1].ToString();
                                                stationGraphDetail.MinValue = reportMin.Tables[0].Rows[c - 3][2].ToString();

                                                stationGraphDetail.MaxDate = reportMax.Tables[0].Rows[c - 3][0].ToString();
                                                stationGraphDetail.MaxTime = reportMax.Tables[0].Rows[c - 3][1].ToString();
                                                stationGraphDetail.MaxValue = reportMax.Tables[0].Rows[c - 3][2].ToString();

                                                List<clsGraphData> LstgraphBarData = new List<clsGraphData>();
                                                List<clsGraphData> LstgraphBarDataWG = new List<clsGraphData>();

                                                for (int r = 0; r < reportDT.Rows.Count; r++)
                                                {
                                                    clsGraphData graphBarData = new clsGraphData();
                                                    graphBarData.Date = reportDT.Rows[r]["Date"].ToString();
                                                    graphBarData.Time = reportDT.Rows[r]["Time"].ToString();
                                                    graphBarData.ParameterValue = reportDT.Rows[r][c].ToString();
                                                    graphBarData.ParameterUnit = NewUnit1[c] == "NA" ? "" : NewUnit1[c];
                                                    LstgraphBarData.Add(graphBarData);

                                                    stationGraphDetail.GraphBarData = LstgraphBarData;

                                                    clsGraphData graphBarDataWG = new clsGraphData();
                                                    graphBarDataWG.Date = reportDT.Rows[r]["Date"].ToString();
                                                    graphBarDataWG.Time = reportDT.Rows[r]["Time"].ToString();

                                                    //Change by vikas --> 10-Mar-2023
                                                    if (reportDT.Rows[r][c].ToString().Contains("--"))
                                                    {
                                                        reportDT.Rows[r][c] = "00.0";
                                                    }

                                                    graphBarDataWG.ParameterValue = (Convert.ToDouble(reportDT.Rows[r][c]) * 1.3).ToString();
                                                    graphBarDataWG.ParameterUnit = NewUnit1[c] == "NA" ? "" : NewUnit1[c];
                                                    LstgraphBarDataWG.Add(graphBarDataWG);

                                                    stationGraphDetail.GraphBarDataWG = LstgraphBarDataWG;
                                                }

                                                graphDetailList.Add(stationGraphDetail);

                                            }
                                            else
                                            {
                                                clsStationGraphDetail stationGraphDetail = new clsStationGraphDetail();
                                                stationGraphDetail.ParameterName = finalcolList[c];
                                                stationGraphDetail.GraphType = "Line";

                                                string type = funSensorType(finalcolList[c]);
                                                stationGraphDetail.Type = type;

                                                stationGraphDetail.xAxisTitle = "Time";
                                                stationGraphDetail.yAxisTile = finalcolList[c].Trim();

                                                if (!finalcolList[c].ToLower().Contains("direction"))
                                                {
                                                    stationGraphDetail.MinDate = reportMin.Tables[0].Rows[c - 3][0].ToString();
                                                    stationGraphDetail.MinTime = reportMin.Tables[0].Rows[c - 3][1].ToString();
                                                    stationGraphDetail.MinValue = reportMin.Tables[0].Rows[c - 3][2].ToString();

                                                    stationGraphDetail.MaxDate = reportMax.Tables[0].Rows[c - 3][0].ToString();
                                                    stationGraphDetail.MaxTime = reportMax.Tables[0].Rows[c - 3][1].ToString();
                                                    stationGraphDetail.MaxValue = reportMax.Tables[0].Rows[c - 3][2].ToString();
                                                }
                                                else
                                                {
                                                    stationGraphDetail.MinDate = "";
                                                    stationGraphDetail.MinTime = "";
                                                    stationGraphDetail.MinValue = "";

                                                    stationGraphDetail.MaxDate = "";
                                                    stationGraphDetail.MaxTime = "";
                                                    stationGraphDetail.MaxValue = "";
                                                }


                                                List<clsGraphData> LstgraphData = new List<clsGraphData>();

                                                //Change by Vikas ---> 15-Dec-2023
                                                //for (int r = 0; r < reportDT.Rows.Count - 4; r++)
                                                for (int r = 0; r < reportDT.Rows.Count; r++)
                                                {
                                                    clsGraphData graphData = new clsGraphData();
                                                    graphData.Date = reportDT.Rows[r]["Date"].ToString();
                                                    graphData.Time = reportDT.Rows[r]["Time"].ToString();
                                                    graphData.ParameterValue = reportDT.Rows[r][c].ToString();
                                                    graphData.ParameterUnit = NewUnit1[c] == "NA" ? "" : NewUnit1[c];
                                                    LstgraphData.Add(graphData);

                                                    stationGraphDetail.GraphData = LstgraphData;
                                                }

                                                graphDetailList.Add(stationGraphDetail);

                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, "No Data Exists");
                            }
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "No Data Exists");
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, graphDetailList);
            }
            catch (Exception Ex)
            {
                Ex.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, graphDetailList);
            }


        }

        public DataTable funVMCDataSetV2(DataSet dsSTData)
        {
            var dtAllData = new DataTable();
            var dtCaculativeData = new DataTable();
            try
            {
                if (dsSTData != null && dsSTData.Tables.Count > 0)
                {
                    dtAllData = dsSTData.Tables[0];
                    if (dsSTData.Tables.Count > 1)
                    {
                        dtCaculativeData = dsSTData.Tables[1];
                    }

                    dtAllData.Columns.Remove("ID");
                    dtAllData.Columns.Remove("Mobile");
                    dtAllData.Columns.Remove("PeripheralStatus");
                    dtAllData.Columns.Remove("CreatedDate");
                    dtAllData.Columns.Remove("InsertedDate");
                    dtAllData.Columns.Remove("UpdationDate");

                    DataRow dr = dtAllData.NewRow();
                    dtAllData.Rows.Add(dr);
                    dtAllData.Rows[dtAllData.Rows.Count - 1][2] = "Min";
                    DataRow dr1 = dtAllData.NewRow();
                    dtAllData.Rows.Add(dr1);
                    dtAllData.Rows[dtAllData.Rows.Count - 1][2] = "Max";

                    for (int i = 3; i < dtAllData.Columns.Count - 1; i++)
                    {
                        var headerName = dtAllData.Columns[i].ColumnName;
                        var minHeaderName = "Min" + dtAllData.Columns[i].ColumnName;

                        dtAllData.Rows[dtAllData.Rows.Count - 2][headerName] = Convert.ToString(dtCaculativeData.Rows[0][minHeaderName]);

                        var maxHeaderName = "Max" + dtAllData.Columns[i].ColumnName;
                        dtAllData.Rows[dtAllData.Rows.Count - 1][headerName] = Convert.ToString(dtCaculativeData.Rows[0][maxHeaderName]);
                    }

                    DataRow dr2 = dtAllData.NewRow();
                    dtAllData.Rows.Add(dr2);
                    dtAllData.Rows[dtAllData.Rows.Count - 1][2] = "Heat D-D";
                    DataRow dr3 = dtAllData.NewRow();
                    dtAllData.Rows.Add(dr3);
                    dtAllData.Rows[dtAllData.Rows.Count - 1][2] = "Cool D-D";

                    dtAllData.Rows[dtAllData.Rows.Count - 2][3] = Convert.ToString(dtCaculativeData.Rows[0]["HeatDD"]);
                    dtAllData.Rows[dtAllData.Rows.Count - 1][3] = Convert.ToString(dtCaculativeData.Rows[0]["CoolDD"]);

                    return dtAllData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x)
            {
                return null;
            }
        }

        public DataTable funGetMinValueV2(DataSet dsSTData)
        {
            var totalRowCount = dsSTData.Tables[0].Rows.Count;
            for (int i = dsSTData.Tables[0].Rows.Count; i > 0; i--)
            {
                var cellData = dsSTData.Tables[0].Rows[i - 1]["Time"].ToString();
                if (cellData == "Min" || cellData == "Max" || cellData == "Heat D-D" || cellData == "Cool D-D")
                {
                    dsSTData.Tables[0].Rows.RemoveAt(i - 1);
                }
            }

            var dtAllData = new DataTable();
            var dtCaculativeData = new DataTable();
            try
            {
                if (dsSTData != null && dsSTData.Tables.Count > 0)
                {
                    dtAllData = dsSTData.Tables[0];
                    if (dsSTData.Tables.Count > 1)
                    {
                        dtCaculativeData = dsSTData.Tables[1];
                    }
                    if (dtAllData.Columns.Contains("ID")) dtAllData.Columns.Remove("ID");
                    if (dtAllData.Columns.Contains("Mobile")) dtAllData.Columns.Remove("Mobile");
                    if (dtAllData.Columns.Contains("PeripheralStatus")) dtAllData.Columns.Remove("PeripheralStatus");
                    if (dtAllData.Columns.Contains("CreatedDate")) dtAllData.Columns.Remove("CreatedDate");
                    if (dtAllData.Columns.Contains("InsertedDate")) dtAllData.Columns.Remove("InsertedDate");
                    if (dtAllData.Columns.Contains("UpdationDate")) dtAllData.Columns.Remove("UpdationDate");

                    DataTable dtData = new DataTable();
                    dtData.Columns.Add("Date", typeof(string));
                    dtData.Columns.Add("Time", typeof(string));
                    dtData.Columns.Add("minVal", typeof(string));

                    for (int i = 3; i < dtAllData.Columns.Count; i++)
                    {
                        var columnName = dtAllData.Columns[i].ColumnName;
                        var minColumnName = "Min" + dtAllData.Columns[i].ColumnName;

                        var minValue = Convert.ToString(dtCaculativeData.Rows[0][minColumnName]);

                        var minData = dtAllData.AsEnumerable()
                            .Where(row => Convert.ToString(row.Field<string>("Time")) != "Min"
                            && Convert.ToString(row.Field<string>(columnName)) == minValue)
                            .Select(t => new
                            {
                                Date = Convert.ToString(t.Field<string>("Date")),
                                Time = Convert.ToString(t.Field<string>("Time")),
                                Value = Convert.ToString(t.Field<string>(columnName))
                            })
                            .OrderBy(t => t.Date)
                            .ThenBy(t => t.Time)
                            .FirstOrDefault();

                        if (minData != null)
                        {
                            DataRow drData = dtData.NewRow();
                            drData["Date"] = minData.Date;
                            drData["Time"] = minData.Time;
                            drData["minVal"] = minValue;
                            dtData.Rows.Add(drData);
                        }
                    }
                    return dtData;
                }
                else
                    return null;
            }
            catch (Exception x)
            {
                return null;
            }
        }

        public DataTable funGetMaxValueV2(DataSet dsSTData)
        {
            var totalRowCount = dsSTData.Tables[0].Rows.Count;
            for (int i = dsSTData.Tables[0].Rows.Count; i > 0; i--)
            {
                var cellData = dsSTData.Tables[0].Rows[i - 1]["Time"].ToString();
                if (cellData == "Min" || cellData == "Max" || cellData == "Heat D-D" || cellData == "Cool D-D")
                {
                    dsSTData.Tables[0].Rows.RemoveAt(i - 1);
                }
            }

            var dtAllData = new DataTable();
            var dtCaculativeData = new DataTable();
            try
            {
                if (dsSTData != null && dsSTData.Tables.Count > 0)
                {
                    dtAllData = dsSTData.Tables[0];
                    if (dsSTData.Tables.Count > 1)
                    {
                        dtCaculativeData = dsSTData.Tables[1];
                    }
                    if (dtAllData.Columns.Contains("ID")) dtAllData.Columns.Remove("ID");
                    if (dtAllData.Columns.Contains("Mobile")) dtAllData.Columns.Remove("Mobile");
                    if (dtAllData.Columns.Contains("PeripheralStatus")) dtAllData.Columns.Remove("PeripheralStatus");
                    if (dtAllData.Columns.Contains("CreatedDate")) dtAllData.Columns.Remove("CreatedDate");
                    if (dtAllData.Columns.Contains("InsertedDate")) dtAllData.Columns.Remove("InsertedDate");
                    if (dtAllData.Columns.Contains("UpdationDate")) dtAllData.Columns.Remove("UpdationDate");

                    DataTable dtData = new DataTable();
                    dtData.Columns.Add("Date", typeof(string));
                    dtData.Columns.Add("Time", typeof(string));
                    dtData.Columns.Add("maxVal", typeof(string));

                    for (int i = 3; i < dtAllData.Columns.Count; i++)
                    {
                        var columnName = dtAllData.Columns[i].ColumnName;
                        var maxColumnName = "Max" + dtAllData.Columns[i].ColumnName;

                        var maxValue = Convert.ToString(dtCaculativeData.Rows[0][maxColumnName]);

                        var maxData = dtAllData.AsEnumerable()
                            .Where(row => Convert.ToString(row.Field<string>("Time")) != "Max"
                            && Convert.ToString(row.Field<string>(columnName)) == maxValue)
                            .Select(t => new
                            {
                                Date = Convert.ToString(t.Field<string>("Date")),
                                Time = Convert.ToString(t.Field<string>("Time")),
                                Value = Convert.ToString(t.Field<string>(columnName))
                            })
                            .OrderBy(t => t.Date)
                            .ThenBy(t => t.Time)
                            .LastOrDefault();

                        if (maxData != null)
                        {
                            DataRow drData = dtData.NewRow();
                            drData["Date"] = maxData.Date;
                            drData["Time"] = maxData.Time;
                            drData["maxVal"] = maxValue;
                            dtData.Rows.Add(drData);
                        }
                    }
                    return dtData;
                }
                else
                    return null;
            }
            catch (Exception x)
            {
                return null;
            }
        }

        public DataSet funMinVal(DataTable reportDT, string StID, string frDT, string toDT, string status)
        {

            DataSet dsMinVal = null;

            try
            {
                for (int j = 0; j < 3; j++)
                {
                    dsMinVal = ObjDB.sp_MinMaxStationData_Status("rpt_DataReport_MinMaxUnit_New", StID, frDT, toDT, "MinVal", status, "Web");
                    if (dsMinVal.Tables.Count > 0)
                        break;
                    Thread.Sleep(1000);
                }

                if (dsMinVal.Tables.Count > 0)
                {
                    if (dsMinVal.Tables[0].Rows.Count > 0)
                    {
                        for (int e = 0; e < dsMinVal.Tables[0].Rows.Count; e++)
                        {
                            for (int f = 0; f < dsMinVal.Tables[0].Columns.Count; f++)
                            {
                                if (e == 3 && f == 2)
                                {
                                    double MinValue = Convert.ToDouble(dsMinVal.Tables[0].Rows[e][f].ToString());
                                    dsMinVal.Tables[0].Rows[e][f] = MinValue.ToString("F2");
                                }
                            }
                        }
                    }
                }

                int rowCnt = 0;
                for (int i = reportDT.Rows.Count - 1; i >= 0; i--)
                {
                    rowCnt++;
                    DataRow dr = reportDT.Rows[i];
                    if (rowCnt < 4)
                        dr.Delete();
                }

                reportDT.AcceptChanges();

                double minDP = reportDT.AsEnumerable().Where(row => row.Field<string>("Dew Point") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("Dew Point")))
                   .Min(x => x);

                var DP = (from data in reportDT.AsEnumerable()
                          where data.Field<string>("Dew Point") != "--" && Convert.ToDouble(data.Field<string>("Dew Point")) == minDP
                          select new
                          {
                              Date = data.Field<string>("Date"),
                              Time = data.Field<string>("Time"),

                          }).First();


                DataRow drDP = dsMinVal.Tables[0].NewRow();
                drDP["Date"] = DP.Date;
                drDP["Time"] = DP.Time;
                drDP["minVal"] = minDP;
                dsMinVal.Tables[0].Rows.Add(drDP);

                double minWR = reportDT.AsEnumerable().Where(row => row.Field<string>("Wind Run") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("Wind Run")))
                   .Min(x => x);

                var WR = (from data in reportDT.AsEnumerable()
                          where data.Field<string>("Wind Run") != "--" && Convert.ToDouble(data.Field<string>("Wind Run")) == minWR
                          select new
                          {
                              Date = data.Field<string>("Date"),
                              Time = data.Field<string>("Time"),

                          }).First();

                DataRow drWR = dsMinVal.Tables[0].NewRow();
                drWR["Date"] = WR.Date;
                drWR["Time"] = WR.Time;
                drWR["minVal"] = minWR;
                dsMinVal.Tables[0].Rows.Add(drWR);


                double minWC = reportDT.AsEnumerable().Where(row => row.Field<string>("Wind Chill") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("Wind Chill")))
                   .Min(x => x);

                var WC = (from data in reportDT.AsEnumerable()
                          where data.Field<string>("Wind Chill") != "--" && Convert.ToDouble(data.Field<string>("Wind Chill")) == minWC
                          select new
                          {
                              Date = data.Field<string>("Date"),
                              Time = data.Field<string>("Time"),

                          }).First();



                DataRow drWC = dsMinVal.Tables[0].NewRow();
                drWC["Date"] = WC.Date;
                drWC["Time"] = WC.Time;
                drWC["minVal"] = minWC;
                dsMinVal.Tables[0].Rows.Add(drWC);




                double minHI = reportDT.AsEnumerable().Where(row => row.Field<string>("Heat Index") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("Heat Index")))
                   .Min(x => x);

                var HI = (from data in reportDT.AsEnumerable()
                          where data.Field<string>("Heat Index") != "--" && Convert.ToDouble(data.Field<string>("Heat Index")) == minHI
                          select new
                          {
                              Date = data.Field<string>("Date"),
                              Time = data.Field<string>("Time"),

                          }).First();


                DataRow drHI = dsMinVal.Tables[0].NewRow();
                drHI["Date"] = HI.Date;
                drHI["Time"] = HI.Time;
                drHI["minVal"] = minHI;
                dsMinVal.Tables[0].Rows.Add(drHI);


                double minTHW = reportDT.AsEnumerable().Where(row => row.Field<string>("THW Index") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("THW Index")))
                   .Min(x => x);

                var THW = (from data in reportDT.AsEnumerable()
                           where data.Field<string>("THW Index") != "--" && Convert.ToDouble(data.Field<string>("THW Index")) == minTHW
                           select new
                           {
                               Date = data.Field<string>("Date"),
                               Time = data.Field<string>("Time"),

                           }).First();



                DataRow drTHW = dsMinVal.Tables[0].NewRow();
                drTHW["Date"] = THW.Date;
                drTHW["Time"] = THW.Time;
                drTHW["minVal"] = minTHW;
                dsMinVal.Tables[0].Rows.Add(drTHW);

                return dsMinVal;

            }
            catch (Exception Ex)
            {
                Ex.ToString();

                return dsMinVal;

            }
        }

        public DataSet funMaxVal(DataTable reportDT, string StID, string frDT, string toDT, string status)
        {

            DataSet dsMaxVal = null;

            try
            {

                for (int j = 0; j < 3; j++)
                {
                    dsMaxVal = ObjDB.sp_MinMaxStationData_Status("rpt_DataReport_MinMaxUnit_New", StID, frDT, toDT, "MaxVal", status, "Web");
                    if (dsMaxVal.Tables.Count > 0)
                        break;
                    Thread.Sleep(1000);
                }

                if (dsMaxVal.Tables.Count > 0)
                {
                    if (dsMaxVal.Tables[0].Rows.Count > 0)
                    {
                        for (int e = 0; e < dsMaxVal.Tables[0].Rows.Count; e++)
                        {
                            for (int f = 0; f < dsMaxVal.Tables[0].Columns.Count; f++)
                            {
                                if (e == 3 && f == 2)
                                {
                                    double MaxValue = Convert.ToDouble(dsMaxVal.Tables[0].Rows[e][f].ToString());
                                    dsMaxVal.Tables[0].Rows[e][f] = MaxValue.ToString("F2");
                                }

                            }
                        }
                    }
                }

                int rowCnt = 0;
                for (int i = reportDT.Rows.Count - 1; i >= 0; i--)
                {
                    rowCnt++;
                    DataRow dr = reportDT.Rows[i];
                    if (rowCnt < 2)
                        dr.Delete();

                }

                reportDT.AcceptChanges();


                double maxDP = reportDT.AsEnumerable().Where(row => row.Field<string>("Dew Point") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("Dew Point")))
                   .Max(x => x);

                var DPmax = (from data in reportDT.AsEnumerable()
                             where data.Field<string>("Dew Point") != "--" && Convert.ToDouble(data.Field<string>("Dew Point")) == maxDP
                             select new
                             {
                                 Date = data.Field<string>("Date"),
                                 Time = data.Field<string>("Time"),

                             }).First();

                DataRow drDP = dsMaxVal.Tables[0].NewRow();
                drDP["Date"] = DPmax.Date;
                drDP["Time"] = DPmax.Time;
                drDP["maxVal"] = maxDP;
                dsMaxVal.Tables[0].Rows.Add(drDP);


                double maxWR = reportDT.AsEnumerable().Where(row => row.Field<string>("Wind Run") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("Wind Run")))
                   .Max(x => x);

                var WRmax = (from data in reportDT.AsEnumerable()
                             where data.Field<string>("Wind Run") != "--" && Convert.ToDouble(data.Field<string>("Wind Run")) == maxWR
                             select new
                             {
                                 Date = data.Field<string>("Date"),
                                 Time = data.Field<string>("Time"),

                             }).First();


                DataRow drWR = dsMaxVal.Tables[0].NewRow();
                drWR["Date"] = WRmax.Date;
                drWR["Time"] = WRmax.Time;
                drWR["maxVal"] = maxWR;
                dsMaxVal.Tables[0].Rows.Add(drWR);


                double maxWC = reportDT.AsEnumerable().Where(row => row.Field<string>("Wind Chill") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("Wind Chill")))
                   .Max(x => x);

                var WCmax = (from data in reportDT.AsEnumerable()
                             where data.Field<string>("Wind Chill") != "--" && Convert.ToDouble(data.Field<string>("Wind Chill")) == maxWC
                             select new
                             {
                                 Date = data.Field<string>("Date"),
                                 Time = data.Field<string>("Time"),

                             }).First();


                DataRow drWC = dsMaxVal.Tables[0].NewRow();
                drWC["Date"] = WCmax.Date;
                drWC["Time"] = WCmax.Time;
                drWC["maxVal"] = maxWC;
                dsMaxVal.Tables[0].Rows.Add(drWC);

                double maxHI = reportDT.AsEnumerable().Where(row => row.Field<string>("Heat Index") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("Heat Index")))
                   .Max(x => x);

                var HImax = (from data in reportDT.AsEnumerable()
                             where data.Field<string>("Heat Index") != "--" && Convert.ToDouble(data.Field<string>("Heat Index")) == maxHI
                             select new
                             {
                                 Date = data.Field<string>("Date"),
                                 Time = data.Field<string>("Time"),

                             }).First();



                DataRow drHI = dsMaxVal.Tables[0].NewRow();
                drHI["Date"] = HImax.Date;
                drHI["Time"] = HImax.Time;
                drHI["maxVal"] = maxHI;
                dsMaxVal.Tables[0].Rows.Add(drHI);


                double maxTHW = reportDT.AsEnumerable().Where(row => row.Field<string>("THW Index") != "--")
                   .Select(x => Convert.ToDouble(x.Field<string>("THW Index")))
                   .Max(x => x);

                var THWmax = (from data in reportDT.AsEnumerable()
                              where data.Field<string>("THW Index") != "--" && Convert.ToDouble(data.Field<string>("THW Index")) == maxTHW
                              select new
                              {
                                  Date = data.Field<string>("Date"),
                                  Time = data.Field<string>("Time"),

                              }).First();



                DataRow drTHW = dsMaxVal.Tables[0].NewRow();
                drTHW["Date"] = THWmax.Date;
                drTHW["Time"] = THWmax.Time;
                drTHW["maxVal"] = maxTHW;
                dsMaxVal.Tables[0].Rows.Add(drTHW);



                return dsMaxVal;

            }
            catch (Exception Ex)
            {
                Ex.ToString();

                return dsMaxVal;

            }
        }

        public string funSensorType(string ParaName)
        {
            string type = "";

            if (ParaName.ToLower().Contains("battery"))
                type = "battery";
            else if (ParaName.ToLower().Contains("water"))
                type = "water";
            else if (ParaName.ToLower().Contains("hourly"))
                type = "rain";
            else if (ParaName.ToLower().Contains("daily"))
                type = "rain";
            else if (ParaName.ToLower().Contains("15mins"))
                type = "rain";
            else if (ParaName.ToLower().Contains("tempera"))
                type = "temperature";
            else if (ParaName.ToLower().Contains("snow"))
                type = "snow";
            else if (ParaName.ToLower().Contains("evapora"))
                type = "evaporation";
            else if (ParaName.Contains("WIND DIRECTION AT MAX WIND SPEED"))
                type = "highspeed";
            else if (ParaName.ToLower().Contains("wind spe") || ParaName.ToLower().Contains("windspe") || ParaName.ToLower().Contains("maxws"))
                type = "windspeed";
            else if (ParaName.ToLower().Contains("wind dir") || ParaName.ToLower().Contains("winddir"))
                type = "winddirection";
            else if (ParaName.ToLower().Contains("pressure"))
                type = "pressure";
            else if (ParaName.ToLower().Contains("humi") || ParaName.ToLower().Contains("soilmois"))
                type = "humidity";
            else if (ParaName.ToLower().Contains("radiation"))
                type = "radiation";

            else if (ParaName.ToLower().Contains("dew"))
                type = "dew point";
            else if (ParaName.ToLower().Contains("wind run"))
                type = "wind run";
            else if (ParaName.ToLower().Contains("wind chill"))
                type = "wind chill";
            else if (ParaName.ToLower().Contains("heat index"))
                type = "heat index";
            else if (ParaName.ToLower().Contains("thw index"))
                type = "thw index";
            else if (ParaName.ToLower().Contains("wind gust"))
                type = "wind gust";
            else if (ParaName.ToLower().Contains("rain rate"))
                type = "rain rate";

            return type;
        }

        public DataTable funVMCDataSet(string StID, string fromDate, string toDate, string status)
        {
            strFunctionName = "VMC Station DataSet";

            DataTable dt2 = new DataTable();
            DataSet reportset = null;

            try
            {
                //With Derived Parameter Values.....
                string Colname = ColumnName(StID);

                List<string> finalColname = Colname.Split(',').ToList();
                finalColname.RemoveAt(finalColname.Count - 1);
                strFunctionName = "VMC Station DataSet-1";

                for (int j = 0; j < 3; j++)
                {
                    reportset = ObjDB.sp_getTotalBurst_Status("GetStationDataLast24", StID, fromDate, toDate, status, "Web");
                    string prname = "select profile from [tbl_StationMaster] where StationID = '" + StID + "'";
                    DataTable Pname = ObjDB.FetchDataTable(prname, "Web");
                    if (Pname.Rows[0][0].ToString() == "VMC-AWS-GUJ")
                    {
                        reportset.Tables[0].Columns.Remove("Status");
                    }
                    if (reportset.Tables.Count > 0)
                        break;
                    Thread.Sleep(1000);
                }

                strFunctionName = "VMC Station DataSet-2";

                if (reportset.Tables.Count > 0)
                {
                    if (reportset.Tables[0].Rows.Count > 0)
                    {
                        var Rows = (from row in reportset.Tables[0].AsEnumerable()
                                    orderby row["Date"] ascending, row["Time"] ascending
                                    select row);
                        DataTable dt1 = Rows.AsDataView().ToTable();

                        //dt2 = funCalcWDAvg(dt1, StID);

                        dt2 = dt1;

                        strFunctionName = "VMC Station DataSet-3";
                        string Teststr = "";

                        if (dt2.Rows.Count > 0)
                        {
                            try
                            {
                                for (int i = 3; i < dt2.Columns.Count; i++)
                                {
                                    dt2.Columns[i].ColumnName = finalColname[i - 3];
                                    Teststr = "dt2 Count=" + dt2.Columns.Count.ToString() + "," + "final column count=" + finalColname.Count.ToString();
                                }
                            }
                            catch (Exception Ex)
                            {
                                strFunctionName = "VMC Station DataSet-3.1" + Teststr.ToString() + "," + Ex.Message.ToString();
                                string tmpExData = "";
                                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;
                                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);
                            }

                            strFunctionName = "VMC Station DataSet-4";

                            int basicColCount = dt2.Columns.Count;

                            dt2.Columns.Add("Dew Point", typeof(string));
                            dt2.Columns.Add("Wind Run", typeof(string));
                            dt2.Columns.Add("Wind Chill", typeof(string));
                            dt2.Columns.Add("Heat Index", typeof(string));
                            dt2.Columns.Add("THW Index", typeof(string));

                            strFunctionName = "VMC Station DataSet-5";

                            for (int j = 0; j < dt2.Rows.Count; j++)
                            {
                                double degreeTemp = 0.0;
                                //double FahrenheitTemp = 0.0;

                                if (!dt2.Rows[j]["Air Temperature(C)"].ToString().Contains("--"))
                                {
                                    degreeTemp = Convert.ToDouble(dt2.Rows[j]["Air Temperature(C)"].ToString());
                                    //FahrenheitTemp = Convert.ToDouble(dt2.Rows[j][6].ToString()) * 1.8 + 32;
                                    //dt2.Rows[j][6] = FahrenheitTemp.ToString();
                                    dt2.Rows[j]["Air Temperature(C)"] = degreeTemp.ToString("F2");
                                }
                                else
                                {
                                    dt2.Rows[j]["Humidity(%)"] = "--";
                                }

                                strFunctionName = "VMC Station DataSet-6";

                                if (dt2.Rows[j]["Humidity(%)"].ToString().Contains("--") == false && dt2.Rows[j]["Air Temperature(C)"].ToString().Contains("--") == false)
                                {
                                    double hum = Convert.ToDouble(dt2.Rows[j]["Humidity(%)"].ToString());

                                    //dt2.Rows[j]["Dew Point"] = ObjDP.funCalDewPoint(FahrenheitTemp, hum).ToString("F2");
                                    double DegDewPoint = ObjDP.funCalDewPoint(degreeTemp, hum);
                                    // dt2.Rows[j]["Dew Point"] = Convert.ToDouble(DegDewPoint.ToString("F2")) * 1.8 + 32;
                                    dt2.Rows[j]["Dew Point"] = DegDewPoint.ToString("F2");

                                    //dt2.Rows[j]["Heat Index"] = ObjDP.funCalHeatIndex(FahrenheitTemp, hum).ToString("F2");
                                    double HeatIndexPoint = ObjDP.funCalHeatIndex(degreeTemp, hum);
                                    //dt2.Rows[j]["Heat Index"] = Convert.ToDouble(HeatIndexPoint.ToString("F2")) * 1.8 + 32;
                                    dt2.Rows[j]["Heat Index"] = HeatIndexPoint.ToString("F2");

                                }
                                else
                                {
                                    dt2.Rows[j]["Dew Point"] = "--";
                                    dt2.Rows[j]["Heat Index"] = "--";
                                }
                                strFunctionName = "VMC Station DataSet-7";
                                if (dt2.Rows[j]["Wind Speed(m/s)"].ToString().Contains("--") == false)
                                {
                                    double ws = Convert.ToDouble(dt2.Rows[j]["Wind Speed(m/s)"].ToString());
                                    dt2.Rows[j]["Wind Run"] = ObjDP.funCalWindRun(ws).ToString("F2");
                                }
                                else
                                {
                                    dt2.Rows[j]["Wind Run"] = "--";
                                }
                                strFunctionName = "VMC Station DataSet-8";

                                if (dt2.Rows[j]["Wind Speed(m/s)"].ToString().Contains("--") == false && dt2.Rows[j]["Air Temperature(C)"].ToString().Contains("--") == false)
                                {
                                    double ws = Convert.ToDouble(dt2.Rows[j]["Wind Speed(m/s)"].ToString());

                                    // dt2.Rows[j]["Wind Chill"] = ObjDP.funCalWindChill(ws, FahrenheitTemp).ToString("F2");
                                    double WCPoint = ObjDP.funCalWindChill(ws, degreeTemp);
                                    //dt2.Rows[j]["Wind Chill"] = Convert.ToDouble(WCPoint.ToString("F2")) * 1.8 + 32;
                                    dt2.Rows[j]["Wind Chill"] = WCPoint.ToString("F2");
                                }
                                else
                                {
                                    dt2.Rows[j]["Wind Chill"] = "--";
                                }
                                strFunctionName = "VMC Station DataSet-9";

                                if (dt2.Rows[j][7].ToString().Contains("--") == false && dt2.Rows[j]["Humidity(%)"].ToString().Contains("--") == false && dt2.Rows[j]["Air Temperature(C)"].ToString().Contains("--") == false)
                                {
                                    double ws = Convert.ToDouble(dt2.Rows[j]["Wind Speed(m/s)"].ToString());
                                    double hum = Convert.ToDouble(dt2.Rows[j][10].ToString());

                                    //dt2.Rows[j]["THW Index"] = ObjDP.funCalTHWIndex(FahrenheitTemp, hum, ws).ToString("F2");
                                    double DegTHW = ObjDP.funCalTHWIndex(degreeTemp, hum, ws);
                                    //dt2.Rows[j]["THW Index"] = Convert.ToDouble(DegTHW.ToString("F2")) * 1.8 + 32; 
                                    dt2.Rows[j]["THW Index"] = DegTHW.ToString("F2");
                                }
                                else
                                {
                                    dt2.Rows[j]["THW Index"] = "--";
                                }

                                strFunctionName = "VMC Station DataSet-10";

                                //For windRose WindSpeed+WindDirection --> N,NNE
                                //if (!reportset.Tables[0].Rows[j]["Wind Speed(m/s)"].ToString().Contains("--") && !reportset.Tables[0].Rows[j]["Wind Direction(DEG)"].ToString().Contains("--"))
                                //{
                                //string WinDirDegree = ObjDP.WindRoseData(reportset.Tables[0].Rows[j]["Wind Direction(DEG)"].ToString(), reportset.Tables[0].Rows[j]["Wind Speed(m/s)"].ToString());
                                //reportset.Tables[0].Rows[j]["Wind Direction(DEG)"] = WinDirDegree;
                                //} 
                            }


                            double minDP = dt2.AsEnumerable().Where(r => r.Field<string>("Dew Point") != "--")
                                          .Select(x => Convert.ToDouble(x.Field<string>("Dew Point")))
                                          .Min(x => x);

                            strFunctionName = "VMC Station DataSet-11";

                            double minWR = dt2.AsEnumerable().Where(r => r.Field<string>("Wind Run") != "--")
                                          .Select(x => Convert.ToDouble(x.Field<string>("Wind Run")))
                                          .Min(x => x);

                            strFunctionName = "VMC Station DataSet-12";

                            double minWC = dt2.AsEnumerable().Where(r => r.Field<string>("Wind Chill") != "--")
                                          .Select(x => Convert.ToDouble(x.Field<string>("Wind Chill")))
                                          .Min(x => x);

                            strFunctionName = "VMC Station DataSet-13";

                            double minHI = dt2.AsEnumerable().Where(r => r.Field<string>("Heat Index") != "--")
                                           .Select(x => Convert.ToDouble(x.Field<string>("Heat Index")))
                                           .Min(x => x);

                            strFunctionName = "VMC Station DataSet-14";

                            double minTHW = dt2.AsEnumerable().Where(r => r.Field<string>("THW Index") != "--")
                                           .Select(x => Convert.ToDouble(x.Field<string>("THW Index")))
                                           .Min(x => x);

                            strFunctionName = "VMC Station DataSet-15";

                            double maxDP = dt2.AsEnumerable().Where(r => r.Field<string>("Dew Point") != "--")
                                          .Select(x => Convert.ToDouble(x.Field<string>("Dew Point")))
                                          .Max(x => x);

                            strFunctionName = "VMC Station DataSet-16";

                            double maxWR = dt2.AsEnumerable().Where(r => r.Field<string>("Wind Run") != "--")
                                          .Select(x => Convert.ToDouble(x.Field<string>("Wind Run")))
                                          .Max(x => x);

                            strFunctionName = "VMC Station DataSet-17";

                            double maxWC = dt2.AsEnumerable().Where(r => r.Field<string>("Wind Chill") != "--")
                                          .Select(x => Convert.ToDouble(x.Field<string>("Wind Chill")))
                                          .Max(x => x);

                            strFunctionName = "VMC Station DataSet-18";

                            double maxHI = dt2.AsEnumerable().Where(r => r.Field<string>("Heat Index") != "--")
                                          .Select(x => Convert.ToDouble(x.Field<string>("Heat Index")))
                                          .Max(x => x);

                            strFunctionName = "VMC Station DataSet-19";

                            double maxTHW = dt2.AsEnumerable().Where(r => r.Field<string>("THW Index") != "--")
                                           .Select(x => Convert.ToDouble(x.Field<string>("THW Index")))
                                           .Max(x => x);

                            strFunctionName = "VMC Station DataSet-20";

                            DataRow dr = dt2.NewRow();
                            dt2.Rows.Add(dr);
                            dt2.Rows[dt2.Rows.Count - 1][2] = "Min";
                            DataRow dr1 = dt2.NewRow();
                            dt2.Rows.Add(dr1);
                            dt2.Rows[dt2.Rows.Count - 1][2] = "Max";

                            strFunctionName = "VMC Station DataSet-21";

                            DataSet dsMinVal = null;
                            for (int m = 0; m < 3; m++)
                            {
                                dsMinVal = ObjDB.sp_getTotalBurst_Status("rpt_DataReport_MinMaxUnit", StID, fromDate, toDate, "MinVal", "Web");
                                if (dsMinVal.Tables.Count > 0)
                                    break;
                                Thread.Sleep(1000);
                            }

                            DataSet dsMaxVal = null;
                            for (int x = 0; x < 3; x++)
                            {
                                dsMaxVal = ObjDB.sp_getTotalBurst_Status("rpt_DataReport_MinMaxUnit", StID, fromDate, toDate, "MaxVal", "Web");
                                if (dsMaxVal.Tables.Count > 0)
                                    break;
                                Thread.Sleep(1000);

                            }

                            double HeatDD = 0.0;
                            double CoolDD = 0.0;

                            strFunctionName = "VMC Station DataSet-22";

                            if (dsMinVal.Tables.Count > 0 && dsMaxVal.Tables.Count > 0)
                            {
                                if (dsMinVal.Tables[0].Rows.Count > 0 && dsMaxVal.Tables[0].Rows.Count > 0)
                                {
                                    for (int c = 0; c < finalColname.Count; c++)
                                    {
                                        dt2.Rows[dt2.Rows.Count - 2][finalColname[c]] = dsMinVal.Tables[0].Rows[c]["minVal"];
                                        dt2.Rows[dt2.Rows.Count - 1][finalColname[c]] = dsMaxVal.Tables[0].Rows[c]["maxVal"];

                                        if (finalColname[c].ToLower().Contains("air temp"))
                                        {
                                            double minTemp = Convert.ToDouble(dt2.Rows[dt2.Rows.Count - 2][finalColname[c]]);
                                            double maxTemp = Convert.ToDouble(dt2.Rows[dt2.Rows.Count - 1][finalColname[c]]);
                                            HeatDD = ObjDP.funGetHeatDD(minTemp, maxTemp);
                                            CoolDD = ObjDP.funGetCoolDD(minTemp, maxTemp);
                                        }

                                    }
                                }
                            }

                            strFunctionName = "VMC Station DataSet-23";

                            dt2.Rows[dt2.Rows.Count - 2]["Dew Point"] = minDP.ToString("F2");

                            dt2.Rows[dt2.Rows.Count - 2]["Wind Run"] = minWR.ToString("F2");

                            dt2.Rows[dt2.Rows.Count - 2]["Wind Chill"] = minWC.ToString("F2");

                            dt2.Rows[dt2.Rows.Count - 2]["Heat Index"] = minHI.ToString("F2");

                            dt2.Rows[dt2.Rows.Count - 2]["THW Index"] = minTHW.ToString("F2");

                            strFunctionName = "VMC Station DataSet-24";

                            dt2.Rows[dt2.Rows.Count - 1]["Dew Point"] = maxDP.ToString("F2");

                            dt2.Rows[dt2.Rows.Count - 1]["Wind Run"] = maxWR.ToString("F2");

                            dt2.Rows[dt2.Rows.Count - 1]["Wind Chill"] = maxWC.ToString("F2");

                            dt2.Rows[dt2.Rows.Count - 1]["Heat Index"] = maxHI.ToString("F2");

                            dt2.Rows[dt2.Rows.Count - 1]["THW Index"] = maxTHW.ToString("F2");

                            strFunctionName = "VMC Station DataSet-25";

                            DataRow dr2 = dt2.NewRow();
                            dt2.Rows.Add(dr2);
                            dt2.Rows[dt2.Rows.Count - 1][2] = "Heat D-D";
                            DataRow dr3 = dt2.NewRow();
                            dt2.Rows.Add(dr3);
                            dt2.Rows[dt2.Rows.Count - 1][2] = "Cool D-D";

                            dt2.Rows[dt2.Rows.Count - 2][3] = HeatDD.ToString("F2") == null ? "--" : HeatDD.ToString("F2");
                            dt2.Rows[dt2.Rows.Count - 1][3] = CoolDD.ToString("F2") == null ? "--" : CoolDD.ToString("F2");

                            strFunctionName = "VMC Station DataSet-26";
                        }
                    }
                }
                return dt2;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                return dt2;
            }
        }

        public string ColumnName(string stationID)
        {
            string ReportcolumnName = string.Empty;

            var id = stationID;
            var tableName = "tbl_StationData_" + id;
            DataSet columnDataset = null;
            for (int c = 0; c < 3; c++)
            {
                columnDataset = ObjDB.FetchData_SP_columnName("getColumnName", tableName, "WEB");
                if (columnDataset.Tables.Count > 0)
                    break;
                Thread.Sleep(1000);
            }

            DataTable columnDataTable = new DataTable();

            if (columnDataset.Tables.Count != 0)
            {
                columnDataTable = columnDataset.Tables[0];

                for (int i = 0; i < columnDataTable.Rows.Count; i++)
                {
                    var sensorName = columnDataTable.Rows[i][0].ToString();
                    var SensorIDsql = db.tbl_SensorMaster.Where(x => x.Name == sensorName).FirstOrDefault();
                    int sensorID = Convert.ToInt32(SensorIDsql.ID);
                    var getUnit = db.tbl_ParameterMaster.Where(x => x.SensorID == sensorID).FirstOrDefault();

                    if (getUnit == null)
                    {
                        if (sensorName != "Status")
                        {
                            ReportcolumnName += columnDataTable.Rows[i][0].ToString() + ",";
                        }

                    }
                    else
                    {
                        ReportcolumnName += columnDataTable.Rows[i][0].ToString() + "(" + getUnit.Unit + ")" + ",";

                    }
                }
            }

            return ReportcolumnName.TrimEnd(',');
        }

        [HttpPost]
        [Route("finalStDataMinMax")]
        public async Task<HttpResponseMessage> GetStDataMinMax()
        {
            //Data insert by Console application.....
            //Called by/Consume by  mobile app....

            strFunctionName = "Get Station with  Data & MinMax Parameter details";

            try
            {
                var objStation = await Request.Content.ReadAsFormDataAsync();

                if (objStation != null)
                {
                    string StID = Regex.Unescape(objStation.Get("StationID"));

                    string SelQry = " select * from AWSAPI.tbl_StationDetail WITH (NOLOCK) where StationID = '" + StID.Trim() + "'";

                    DataSet dsData = null;

                    for (int j = 0; j < 3; j++)
                    {
                        dsData = ObjDB.FetchDataset(SelQry, "Web");
                        if (dsData.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsData.Tables.Count > 0)
                    {
                        if (dsData.Tables[0].Rows.Count > 0)
                        {
                            string jString = dsData.Tables[0].Rows[0]["JsonString"].ToString();

                            List<clsStationGraphDetail> finalStr = JsonConvert.DeserializeObject<List<clsStationGraphDetail>>(jString.Replace("?", "◦"));

                            clsJsonDataMinMax ObjJsonResp = new clsJsonDataMinMax();
                            ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                            ObjJsonResp.message = "success";
                            ObjJsonResp.status = true;
                            ObjJsonResp.data = finalStr;

                            return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                        }
                        else
                        {
                            clsJsonFail ObjJsonResp = new clsJsonFail();
                            ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                            ObjJsonResp.message = "data not found";
                            ObjJsonResp.status = false;
                            return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                        }
                    }
                    else
                    {
                        clsJsonFail ObjJsonResp = new clsJsonFail();
                        ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                        ObjJsonResp.message = "data not found";
                        ObjJsonResp.status = false;
                        return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                    }
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                    ObjJsonResp.message = "internal server error";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                }

            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "internal server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);

            }

        }

        #endregion Station DataMinMax Detail


        #region Station Forecast Data For VMC Only

        [HttpPost]
        [Route("StForecast")]
        public async Task<HttpResponseMessage> PostStForecast()
        {
            strFunctionName = "Get Station Forecast Data";

            try
            {
                var objStation = await Request.Content.ReadAsFormDataAsync();

                List<clsForecastData> LstforecastData = new List<clsForecastData>();

                if (objStation != null)
                {
                    string StID = Regex.Unescape(objStation.Get("StationID"));
                    string lat = Regex.Unescape(objStation.Get("Lat"));
                    string lng = Regex.Unescape(objStation.Get("Lng"));

                    using (HttpClient client = new HttpClient())
                    {
                        /* var values = new Dictionary<string, string>
                                 {
                                     { "StationID",  StID },
                                     { "apikey", "c8ebe26b3214521ac2cbd2561d1cca85" }
                                 };

                         var content = new FormUrlEncodedContent(values);
                         HttpResponseMessage response = await client.PostAsync("http://awsbackend.herokuapp.com/getforecast?stationID=" + StID + "&apikey=c8ebe26b3214521ac2cbd2561d1cca85", content);
                         var json = response.Content.ReadAsStringAsync().Result; */

                        //string lat = "22.380133939179974";
                        //string lng = "73.38287637777758";

                        //function check if data availiable 
                        //if availiable then
                        // return json from file
                        //else 
                        //do call api  key

                        var json = "";
                        var DT = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000;
                        var hourtime = DT - (DT % 3600);
                        var savefilefromhour = hourtime;
                        var savedir = "savedresponse" + '\\' + StID;
                        var fileNM = savefilefromhour + ".json";

                        //string filePath = Path.Combine("F://" , savedir);
                        string filePath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), savedir);

                        if (File.Exists(Path.Combine(filePath, fileNM)))
                        {
                            json = File.ReadAllText(Path.Combine(filePath, fileNM));
                        }
                        else
                        {
                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);

                            string apikey = "58acc1d1652dbc2ca1254123b62ebe8c";
                            HttpResponseMessage response = await client.GetAsync("https://api.openweathermap.org/data/3.0/onecall?lat=" + lat + "&lon=" + lng + "&exclude=current,minutely&units=metric&appid=" + apikey + "");
                            json = response.Content.ReadAsStringAsync().Result;

                            bool flgWrite = ObjFile.WriteIntoFile(filePath, fileNM, json);
                        }

                        Root objroot = new Root();
                        objroot = JsonConvert.DeserializeObject<Root>(json);

                        clsForecastData forecast = new clsForecastData();
                        List<clsDailyValues> LstdailyValues = new List<clsDailyValues>();
                        List<clsHourlyValues> LsthourlyValues = new List<clsHourlyValues>();

                        for (int j = 0; j < objroot.hourly.Count; j++)
                        {

                            if (j < objroot.daily.Count)
                            {
                                clsDailyValues dailyValues = new clsDailyValues();
                                List<clsDynamicDailyData> dynamicDailyData = new List<clsDynamicDailyData>();

                                clsDynamicDailyData dynamicDailyData1 = new clsDynamicDailyData();

                                double timestamp = double.Parse(objroot.daily[j].dt.ToString().ToString());
                                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);  //from start epoch time
                                DateTime Dtm1 = start.AddSeconds(timestamp); //add the seconds to the start DateTime

                                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                                DateTime Dtm = TimeZoneInfo.ConvertTimeFromUtc(Dtm1, INDIAN_ZONE);

                                string day = Dtm.Date.DayOfWeek.ToString();
                                dynamicDailyData1.dailyParaValue = day;
                                dynamicDailyData1.dailyParaUnit = "";
                                dynamicDailyData.Add(dynamicDailyData1);

                                clsDynamicDailyData dynamicDailyData2 = new clsDynamicDailyData();
                                string date = Dtm.Date.ToString("dd/MM");
                                dynamicDailyData2.dailyParaValue = date;
                                dynamicDailyData2.dailyParaUnit = "";
                                dynamicDailyData.Add(dynamicDailyData2);

                                clsDynamicDailyData dynamicDailyData3 = new clsDynamicDailyData();
                                double HT = objroot.daily[j].temp.max;
                                dynamicDailyData3.dailyParaValue = HT.ToString();
                                dynamicDailyData3.dailyParaUnit = "°C";
                                dynamicDailyData.Add(dynamicDailyData3);

                                clsDynamicDailyData dynamicDailyData4 = new clsDynamicDailyData();
                                double LT = objroot.daily[j].temp.min;
                                dynamicDailyData4.dailyParaValue = LT.ToString();
                                dynamicDailyData4.dailyParaUnit = "°C";
                                dynamicDailyData.Add(dynamicDailyData4);

                                clsDynamicDailyData dynamicDailyData5 = new clsDynamicDailyData();
                                string icon = objroot.daily[j].weather[0].icon;
                                string iconUrl = funIconType(icon);
                                dynamicDailyData5.dailyParaValue = iconUrl;
                                dynamicDailyData5.dailyParaUnit = "";
                                dynamicDailyData.Add(dynamicDailyData5);

                                clsDynamicDailyData dynamicDailyData6 = new clsDynamicDailyData();
                                double rain = objroot.daily[j].rain;
                                dynamicDailyData6.dailyParaValue = rain.ToString();
                                dynamicDailyData6.dailyParaUnit = "mm";
                                dynamicDailyData.Add(dynamicDailyData6);

                                clsDynamicDailyData dynamicDailyData7 = new clsDynamicDailyData();

                                //CHANGE BY VIKAS --> 26-Jul-2024                                 
                                //int pop = (int)objroot.daily[j].pop;
                                //dynamicDailyData7.dailyParaValue = pop.ToString();
                                decimal pop = (decimal)objroot.daily[j].pop;
                                dynamicDailyData7.dailyParaValue = (pop * 100).ToString("F0"); //string.Format("{0:0.#}", (pop * 100).ToString());
                                dynamicDailyData7.dailyParaUnit = "%";
                                dynamicDailyData.Add(dynamicDailyData7);

                                dailyValues.dynamicDailyData = dynamicDailyData;
                                LstdailyValues.Add(dailyValues);
                            }

                            if (j < objroot.hourly.Count)
                            {
                                clsHourlyValues hourlyValues = new clsHourlyValues();
                                List<clsDynamicHourlyData> LstdynamicHourlyData = new List<clsDynamicHourlyData>();

                                double timestamphr = double.Parse(objroot.hourly[j].dt.ToString().ToString());
                                DateTime starthr = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); //from start epoch time
                                DateTime Dtmhr1 = starthr.AddSeconds(timestamphr); //add the seconds to the start DateTime

                                TimeZoneInfo INDIAN_ZONE1 = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                                DateTime Dtmhr = TimeZoneInfo.ConvertTimeFromUtc(Dtmhr1, INDIAN_ZONE1);

                                clsDynamicHourlyData dynamicHourlyData1 = new clsDynamicHourlyData();
                                string hour = Dtmhr.Hour.ToString();
                                dynamicHourlyData1.hourlyParaValue = hour;
                                dynamicHourlyData1.hourlyParaUnit = "";
                                LstdynamicHourlyData.Add(dynamicHourlyData1);

                                clsDynamicHourlyData dynamicHourlyData2 = new clsDynamicHourlyData();
                                double temp = objroot.hourly[j].temp;
                                dynamicHourlyData2.hourlyParaValue = temp.ToString();
                                dynamicHourlyData2.hourlyParaUnit = "°C";
                                LstdynamicHourlyData.Add(dynamicHourlyData2);

                                clsDynamicHourlyData dynamicHourlyData3 = new clsDynamicHourlyData();
                                double feels_like = objroot.hourly[j].feels_like;
                                dynamicHourlyData3.hourlyParaValue = feels_like.ToString();
                                dynamicHourlyData3.hourlyParaUnit = "°C";
                                LstdynamicHourlyData.Add(dynamicHourlyData3);

                                clsDynamicHourlyData dynamicHourlyData4 = new clsDynamicHourlyData();
                                string icon = objroot.hourly[j].weather[0].icon;
                                //dynamicHourlyData4.hourlyParaValue = icon;
                                string iconUrl = funIconType(icon);
                                dynamicHourlyData4.hourlyParaValue = iconUrl;
                                dynamicHourlyData4.hourlyParaUnit = "";
                                LstdynamicHourlyData.Add(dynamicHourlyData4);

                                clsDynamicHourlyData dynamicHourlyData5 = new clsDynamicHourlyData();
                                //Change by vikas --> 29-07-2024
                                //double rain = objroot.hourly[j].rain != null ? objroot.hourly[j].rain.hr : 0;
                                double rain = objroot.hourly[j].rain != null ? objroot.hourly[j].rain.onehour : 0;
                                dynamicHourlyData5.hourlyParaValue = rain.ToString();
                                dynamicHourlyData5.hourlyParaUnit = "mm";
                                LstdynamicHourlyData.Add(dynamicHourlyData5);

                                clsDynamicHourlyData dynamicHourlyData6 = new clsDynamicHourlyData();
                                //CHANGE BY VIKAS --> 26-Jul-2024
                                //int pop = (int)objroot.hourly[j].pop;
                                //dynamicHourlyData6.hourlyParaValue = pop.ToString();
                                decimal pop = (decimal)objroot.hourly[j].pop;
                                dynamicHourlyData6.hourlyParaValue = (pop * 100).ToString("F0"); //string.Format("{0:0.#}",(pop * 100).ToString());
                                dynamicHourlyData6.hourlyParaUnit = "%";
                                LstdynamicHourlyData.Add(dynamicHourlyData6);
                                hourlyValues.dynamicHourlyData = LstdynamicHourlyData;
                                LsthourlyValues.Add(hourlyValues);

                            }


                        }

                        clsDailyData dailyData = new clsDailyData();
                        //Change by vikas -->< 03-05-2023
                        //dailyData.dailyParaTitles = new string[] { "day", "date", "HighTemp", "LowTemp", "icon", "rain", "pop" };
                        dailyData.dailyParaTitles = new string[] { "day", "date", "High Temp (°C)", "Low Temp (°C)", "icon", "rain (mm)", "Chances of Rainfall (%)" };
                        dailyData.dailyValues = LstdailyValues;
                        forecast.dailyData = dailyData;

                        clsHourlyData hourlyData = new clsHourlyData();
                        //Change by vikas -->< 03-05-2023
                        //hourlyData.hourlyParaTitles = new string[] { "hour", "temp", "feels_like", "icon", "rain", "pop" };
                        hourlyData.hourlyParaTitles = new string[] { "hour", "temp (°C)", "feels_like", "icon", "rain (mm)", "Chances of Rainfall (%)" };
                        hourlyData.hourlyValues = LsthourlyValues;
                        forecast.hourlyData = hourlyData;

                        LstforecastData.Add(forecast);
                    }

                }

                return Request.CreateResponse(HttpStatusCode.OK, LstforecastData);
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }

        }

        [HttpPost]
        [Route("finalStForecast")]
        public async Task<HttpResponseMessage> GetStForecast()
        {
            strFunctionName = "Get Station Forecast Data details";
            try
            {
                var objStation = await Request.Content.ReadAsFormDataAsync();

                if (objStation != null)
                {
                    string StID = Regex.Unescape(objStation.Get("StationID"));

                    string SelQry = " select * from AWSAPI.tbl_StationForecast WITH (NOLOCK) where StationID = '" + StID.Trim() + "'";

                    DataSet dsData = null;

                    for (int j = 0; j < 3; j++)
                    {
                        dsData = ObjDB.FetchDataset(SelQry, "Web");
                        if (dsData.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsData.Tables.Count > 0)
                    {
                        if (dsData.Tables[0].Rows.Count > 0)
                        {
                            string jString = dsData.Tables[0].Rows[0]["JsonString"].ToString();

                            List<clsForecastData> finalStr = JsonConvert.DeserializeObject<List<clsForecastData>>(jString.Replace("?", "◦"));

                            clsJsonForecast ObjJsonResp = new clsJsonForecast();
                            ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                            ObjJsonResp.message = "success";
                            ObjJsonResp.status = true;
                            ObjJsonResp.data = finalStr[0];

                            return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                        }
                        else
                        {
                            clsJsonFail ObjJsonResp = new clsJsonFail();
                            ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                            ObjJsonResp.message = "data not found";
                            ObjJsonResp.status = false;
                            return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                        }
                    }
                    else
                    {
                        clsJsonFail ObjJsonResp = new clsJsonFail();
                        ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                        ObjJsonResp.message = "data not found";
                        ObjJsonResp.status = false;
                        return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                    }
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                    ObjJsonResp.message = "internal server error";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                }

            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "internal server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
            }
        }

        public string funIconType(string Icon)
        {
            string iconUrl = string.Empty;

            if (Icon.ToLower().Contains("01d"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("01n"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("02d"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("02n"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("03d"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("03n"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("04d"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("04n"))
            {

                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("09d"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("09n"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("10d"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("10n"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("11d"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("11n"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("13d"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("13n"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("50d"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }
            else if (Icon.ToLower().Contains("50n"))
            {
                iconUrl = "~/WeatherIcon/" + Icon.Trim() + ".png";
                iconUrl = Url.Content(iconUrl);
            }

            return iconUrl;
        }

        #endregion Station Forecast Data For VMC Only


        #region Station Data History

        [HttpPost]
        [Route("historicaldata")]
        public async Task<HttpResponseMessage> PostHistoricalData()
        {
            //1. day wise high low...
            //2. month wise high low...
            //3. year wise high low...

            strFunctionName = "Get Historical Data";

            try
            {
                DataTable dtFinal = new DataTable();

                var objHistorical = await Request.Content.ReadAsFormDataAsync();

                if (objHistorical != null)
                {
                    string StID = Regex.Unescape(objHistorical.Get("StationID"));
                    string day = Regex.Unescape(objHistorical.Get("day"));
                    string month = Regex.Unescape(objHistorical.Get("month"));
                    string year = Regex.Unescape(objHistorical.Get("year"));

                    string fromDate = "";
                    string toDate = "";

                    if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                    {
                        int monthNumber = DateTime.ParseExact(month, "MMM", CultureInfo.CurrentCulture).Month;

                        if (monthNumber.ToString().Trim().Length == 1)
                        {
                            fromDate = year + "-" + "0" + monthNumber + "-" + day;
                            toDate = year + "-" + "0" + monthNumber + "-" + day;
                        }
                        else
                        {
                            fromDate = year + "-" + monthNumber + "-" + day;
                            toDate = year + "-" + monthNumber + "-" + day;

                        }
                    }
                    else if (string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                    {
                        int monthNumber = DateTime.ParseExact(month, "MMM", CultureInfo.CurrentCulture).Month;
                        int yr = Convert.ToInt32(year);
                        DateTime date = new DateTime(yr, monthNumber, 1);
                        var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                        fromDate = firstDayOfMonth.ToString("yyyy-MM-dd");
                        toDate = lastDayOfMonth.ToString("yyyy-MM-dd");
                    }
                    else if (string.IsNullOrEmpty(day) && string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                    {
                        int yr = Convert.ToInt32(year);
                        DateTime firstDay = new DateTime(yr, 1, 1);
                        DateTime lastDay = new DateTime(yr, 12, 31);
                        fromDate = firstDay.ToString("yyyy-MM-dd");
                        toDate = lastDay.ToString("yyyy-MM-dd");
                    }

                    //If VMC then [Battery Voltage],[Hourly Rainfall],[Daily Rain],[Air Temperature],[Wind Speed],[Wind Direction],[Atmospheric Pressure],[Humidity]
                    //If Not VMC then [Battery Voltage],[Hourly Rainfall],[Daily Rain],[Air Temperature],[Wind Speed],[Wind Direction],[Atmospheric Pressure],[Humidity],[Peripheral Status]
                    //For Forest GOA ---> 
                    string SelQry = @"select  pm.Name,pm.SensorName,srm.unit,srm.ValidationString,sm.ShowInGraph,sm.ShowInGrid from [tbl_StationMaster ] sm join tbl_StationRangeValidation srm on
                        sm.Profile=srm.ProfileName join tbl_ProfileMaster pm on sm.Profile = pm.Name where sm.StationID='" + StID + "'";

                    DataSet dtUnit = null;
                    for (int u = 0; u < 3; u++)
                    {
                        dtUnit = ObjDB.FetchDataset(SelQry, "Web");
                        if (dtUnit.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    string ProfileNM = dtUnit.Tables[0].Rows[0]["Name"].ToString();
                    if (ProfileNM == "VMC-AWS-GUJ")
                    {
                        DataSet dsMin = null;

                        for (int j = 0; j < 3; j++)
                        {
                            dsMin = ObjDB.sp_MinMaxStationData_Status("rpt_DemoDataReport_MinMaxUnit_NewVMCHistory", StID, fromDate, toDate, "MinVal", "", "Web");

                            if (dsMin.Tables.Count > 0)
                                break;
                            Thread.Sleep(1000);
                        }


                        DataSet dsMax = null;

                        for (int j = 0; j < 3; j++)
                        {
                            dsMax = ObjDB.sp_MinMaxStationData_Status("rpt_DemoDataReport_MinMaxUnit_NewVMCHistory", StID, fromDate, toDate, "MaxVal", "", "Web");

                            if (dsMax.Tables.Count > 0)
                                break;
                            Thread.Sleep(1000);
                        }

                        //Put condition over here ....
                        //Max WindSpeed Value,DateMax,TimeMax && Correspnding WindDirection Value....
                        DataRow windSpeedRow = dsMax.Tables[0].AsEnumerable()
                                               .FirstOrDefault(row => row.Field<string>("ColumnName") == "[Wind Speed]");
                        DataRow windDirectionRow = dsMax.Tables[0].AsEnumerable()
                                               .FirstOrDefault(row => row.Field<string>("ColumnName") == "[Wind Direction]");
                        DataRow windgustRow = dsMax.Tables[0].AsEnumerable()
                                               .FirstOrDefault(row => row.Field<string>("ColumnName") == "[WIND GUST]");
                        DataRow RainRow = dsMax.Tables[0].AsEnumerable()
                                               .FirstOrDefault(row => row.Field<string>("ColumnName") == "[15mins RAINFALL]");

                        string DateHighWS = windSpeedRow.Field<string>("Date");
                        string TimeHighWS = windSpeedRow.Field<string>("Time");
                        string HighWS = windSpeedRow.Field<string>("maxVal");

                        // Now you have DateHighWS, TimeHighWS, and HighWS for the "Wind Speed" column
                        // Use these variables as needed.


                        //string DominantWD = dsMax.Tables[0].Rows[5][2].ToString();
                        string DominantWD = windDirectionRow.Field<string>("maxVal");
                        double WindDirDegree = Convert.ToDouble(DominantWD);
                        string[] Sector = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
                        string DominantWindDir = Sector[Convert.ToInt32(Math.Round((WindDirDegree % 360) / 22.5))];

                        string WindGust = "";
                        WindGust = windgustRow.Field<string>("maxVal"); // + " mps / " + DominantWindDir;



                        string DateHighRain = null, TimeHighRain = null, HighRain = null;
                        string RainTotal = "";

                        if (ProfileNM == "VMC-AWS-GUJ")
                        {
                            #region For calculate VMC-RainTotal....Modify by vikas --> 2022-07-14

                            /*DataRow dr1 = dsMax.Tables[0].NewRow();
                            dr1[0] = DateHighWS;
                            dr1[1] = TimeHighWS;
                            dr1[2] = WindGust;
                            dsMax.Tables[0].Rows.Add(dr1);*/


                            toDate = Convert.ToDateTime(toDate).AddDays(1).ToString("yyyy-MM-dd");
                            //string SelRainQry = @"SELECT [StationID], SUM([MAXHOUR]) as CummulativeRain FROM (SELECT [StationID], DATEADD(second,DATEDIFF(second,'1970-01-01',CAST([Date] AS datetime) + CAST(DATEADD(minute,-15,CAST([Time] AS datetime)) AS datetime))/3600*3600 , '1970-01-01') AS [DATEHOUR]
                            //                           , MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS [MAXHOUR] FROM tbl_StationData_" + StID.Trim() + " with(nolock) where CAST([Date] AS datetime) + CAST([Time] AS datetime) >= '" + fromDate.Trim() + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) < '" + toDate.Trim() + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";
                            string SelRainQry = "select  [StationID],sum(try_cast(RTRIM(LTRIM(replace([15mins RAINFALL], ''' + + ''', ''''))) as decimal(18,2))) as CummulativeRain from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + fromDate.Trim() + "' and  CAST([Date] AS datetime) +CAST([Time] AS datetime) < '" + toDate.Trim() + "' group by StationID";
                            DataSet dsRain = null;
                            for (int r = 0; r < 3; r++)
                            {
                                dsRain = ObjDB.FetchDataset(SelRainQry, "Web");
                                if (dsRain.Tables.Count > 0)
                                    break;
                                Thread.Sleep(1000);
                            }


                            if (dsRain.Tables.Count > 0)
                            {
                                if (dsRain.Tables[0].Rows.Count > 0)
                                {
                                    for (int r = 0; r < dsRain.Tables[0].Rows.Count; r++)
                                    {
                                        RainTotal = dsRain.Tables[0].Rows[r]["CummulativeRain"].ToString();
                                    }
                                }
                            }

                            DateHighRain = RainRow.Field<string>("Date"); ;
                            TimeHighRain = RainRow.Field<string>("Time"); ;
                            HighRain = RainRow.Field<string>("maxVal");

                            #endregion For calculate VMC-RainTotal....Modify by vikas --> 2022-07-14
                        }

                        string[] Parameters = dtUnit.Tables[0].Rows[0]["SensorName"].ToString().Split(',');
                        string[] Units = dtUnit.Tables[0].Rows[0]["unit"].ToString().Split(',');

                        string strfinalUnits = string.Empty;
                        List<string> finalUnits = null;

                        string strfinalPara = string.Empty;
                        List<string> finalPara = null;

                        if (ProfileNM == "VMC-AWS-GUJ")
                        {
                            string EliminatePara = "Water Level,Snow Depth,Evaporation, Solar Radiation,Peripheral Status,WIND DIRECTION AT MAX WIND SPEED,RAIN RATE,15mins RAINFALL";
                            for (int p = 4; p < Parameters.Length - 1; p++)
                            {
                                if (!EliminatePara.Contains(Parameters[p]))
                                {
                                    if (Parameters[p] == "WIND GUST")
                                    {
                                        strfinalUnits = strfinalUnits + "," + Units[p] + " / " + DominantWindDir;
                                    }
                                    else
                                    {
                                        strfinalUnits = strfinalUnits + "," + Units[p];
                                    }



                                    strfinalPara = strfinalPara + "," + Parameters[p];
                                }
                            }


                            //strfinalUnits = strfinalUnits + "," + "mps";
                            strfinalUnits = strfinalUnits.Replace("Degree", "°").TrimStart(',');
                            finalUnits = strfinalUnits.Replace("C", "°C").Split(',').ToList();

                            //strfinalPara = strfinalPara + "," + "Wind Gust";
                            strfinalPara = strfinalPara.TrimStart(',');
                            finalPara = strfinalPara.Split(',').ToList();
                        }
                        else
                        {
                            string EliminatePara = "";


                            if (ProfileNM.ToLower().Contains("nhp-"))
                            {
                                if (ProfileNM.ToLower().Contains("awlr"))
                                    EliminatePara = "Snow Depth,Evaporation, Solar Radiation,Peripheral Status";
                                else if (ProfileNM.ToLower().Contains("aws"))
                                    EliminatePara = "Water Level,Snow Depth,Evaporation,Peripheral Status";
                                else if (ProfileNM.ToLower().Contains("aws+awlr"))
                                    EliminatePara = "Snow Depth,Evaporation, Solar Radiation,Peripheral Status";

                                for (int p = 4; p < Parameters.Length - 1; p++)
                                {
                                    if (!EliminatePara.Contains(Parameters[p]))
                                    {
                                        strfinalUnits = strfinalUnits + "," + Units[p];
                                        strfinalPara = strfinalPara + "," + Parameters[p];
                                    }
                                }

                            }
                            else
                            {
                                //StationID,Station Name,Latitude,Longitude,Altitude,Date,Time,Battery Voltage,GPS,GPRS SIGNALSTRENGTH,TypeOfSystem,NoOfParameters,RainFall HealthStatus,Hourly Rainfall,Daily Rain,Air Temperature HealthStatus,Air Temperature,Temperature Minimum,Temperature Maximum,Temperature DayMinMax,Humidity HealthStatus,Humidity,Humidity Minimum,Humidity Maximum,Humidity DayMinMax,WindDirection HealthStatus10m,WindDirection 10m,WindSpeed HealthStatus10m,WindSpeed 10m1minAvg,WindSpeed 10m3minAvg,WindSpeed 10m10minAvg,MaxWindSpeed 10m,WindSpeed DayMax10m,Pressure HealthStatus,Atmospheric Pressure,WindDirection HealthStatus3m,WindDirection 3m,WindSpeed HealthStatus3m,WindSpeed 3m1minAvg,WindSpeed 3m3minAvg,WindSpeed 3m10minAvg,MaxWindSpeed 3m,WindSpeed DayMax3m,SoilTemperature HealthStatus10cm,SoilTemperature 10cm,SoilMoisture HealthStatus10cm,SoilMoisture 10cm,SoilTemperature HealthStatus30cm,SoilTemperature 30cm,SoilMoisture HealthStatus30cm,SoilMoisture 30cm,SoilTemperature HealthStatus70cm,SoilTemperature 70cm,SoilMoisture HealthStatus70cm,SoilMoisture 70cm,SoilTemperature HealthStatus100cm,SoilTemperature 100cm,SoilMoisture HealthStatus100cm,SoilMoisture 100cm,SolarRadiation HealthStatus,Sunshine Duration,Global Radiation,Radiation HealthStatus,Par Data,Evaporameter HealthStatus,Evaporameter,UVRadiation_a HealthStatus,UV Radiation_a,UVRadiation_b HealthStatus,UV Radiation_b,UV Index,VisibilitySensor HealthStatus,Visibility,SnowDepth HealthStatus,Snow Depth,SnowWater HealthStatus,Snow WaterEquivalent,WaterLevel HealthStatus,WaterLevel Measurement
                                //EliminatePara = "StationID,Station Name,Latitude,Longitude,Altitude,Date,Time,Battery Voltage,GPS,GPRS SIGNALSTRENGTH,TypeOfSystem,NoOfParameters,RainFall HealthStatus,Hourly Rainfall,Daily Rain,AirTemperature HealthStatus,Temperature Minimum,Temperature Maximum,Temperature DayMinMax,Humidity HealthStatus,Humidity Minimum,Humidity Maximum,Humidity DayMinMax,WindDirection HealthStatus10m,WindSpeed HealthStatus10m,Pressure HealthStatus,WindDirection HealthStatus3m,WindDirection 3m,WindSpeed HealthStatus3m,WindSpeed 3m1minAvg,WindSpeed 3m3minAvg,WindSpeed 3m10minAvg,MaxWindSpeed 3m,WindSpeed DayMax3m,SoilTemperature HealthStatus10cm,SoilMoisture HealthStatus10cm,SoilTemperature HealthStatus30cm,SoilTemperature 30cm,SoilMoisture HealthStatus30cm,SoilMoisture 30cm,SoilTemperature HealthStatus70cm,SoilMoisture HealthStatus70cm,SoilTemperature HealthStatus100cm,SoilTemperature 100cm,SoilMoisture HealthStatus100cm,SoilMoisture 100cm,SolarRadiation HealthStatus,Sunshine Duration,Global Radiation,Radiation HealthStatus,Par Data,Evaporameter HealthStatus,Evaporameter,UVRadiation_a HealthStatus,UV Radiation_a,UVRadiation_b HealthStatus,UV Radiation_b,UV Index,VisibilitySensor HealthStatus,Visibility,SnowDepth HealthStatus,Snow Depth,SnowWater HealthStatus,Snow WaterEquivalent,WaterLevel HealthStatus,WaterLevel Measurement";

                                EliminatePara = "Battery Voltage,Hourly Rainfall,Daily Rain,Air Temperature,Temperature Minimum,Temperature Maximum,Temperature DayMinMax,Humidity,Humidity Minimum,Humidity Maximum,Humidity DayMinMax,WindDirection 10m,WindSpeed 10m1minAvg,WindSpeed 10m3minAvg,WindSpeed 10m10minAvg,MaxWindSpeed 10m,WindSpeed DayMax10m,Atmospheric Pressure,SoilTemperature 10cm,SoilMoisture 10cm,SoilTemperature 70cm,SoilMoisture 70cm";

                                //EliminatePara = "Battery Voltage,Hourly Rainfall,Daily Rain,Air Temperature,Humidity,WindDirection 10m,WindSpeed 10m1minAvg,WindSpeed 10m3minAvg,WindSpeed 10m10minAvg,Atmospheric Pressure,SoilTemperature 10cm,SoilMoisture 10cm,SoilTemperature 70cm,SoilMoisture 70cm";

                                string[] arrElimatePara = EliminatePara.Split(',');

                                for (int p = 0; p < Parameters.Length - 1; p++)
                                {
                                    for (int e = 0; e < arrElimatePara.Length; e++)
                                    {
                                        if (Parameters[p].ToLower().Trim().Equals(arrElimatePara[e].ToLower().Trim()) == true)
                                        {
                                            strfinalUnits = strfinalUnits + "," + Units[p];
                                            strfinalPara = strfinalPara + "," + Parameters[p];
                                            break;
                                        }
                                    }

                                }

                            }

                            strfinalUnits = strfinalUnits.TrimStart(',');
                            finalUnits = strfinalUnits.Split(',').ToList();

                            strfinalPara = strfinalPara.TrimStart(',');
                            finalPara = strfinalPara.Split(',').ToList();

                        }

                        dtFinal.Columns.Add("Parameter", typeof(string));
                        dtFinal.Columns.Add("DateMin", typeof(string));
                        dtFinal.Columns.Add("TimeMin", typeof(string));
                        dtFinal.Columns.Add("MinVal", typeof(string));
                        dtFinal.Columns.Add("DateMax", typeof(string));
                        dtFinal.Columns.Add("TimeMax", typeof(string));
                        dtFinal.Columns.Add("MaxVal", typeof(string));
                        dtFinal.Columns.Add("Unit", typeof(string));
                        var rowToRemovemin = dsMin.Tables[0].AsEnumerable()
                                 .FirstOrDefault(row => row.Field<string>("ColumnName") == "[15mins RAINFALL]");
                        dsMin.Tables[0].Rows.Remove(rowToRemovemin);

                        var rowToRemove = dsMax.Tables[0].AsEnumerable()
                                 .FirstOrDefault(row => row.Field<string>("ColumnName") == "[15mins RAINFALL]");
                        dsMax.Tables[0].Rows.Remove(rowToRemove);
                        foreach (DataRow maxRow in dsMax.Tables[0].Rows)
                        {

                            string parameter = maxRow[2].ToString(); // Assuming the parameter is in the first column of dsMax.Tables[0]

                            DataRow dr = dtFinal.NewRow();
                            dtFinal.Rows.Add(dr);

                            int m = dtFinal.Rows.Count - 1; // Index of the newly added row

                            if (ProfileNM == "VMC-AWS-GUJ")
                            {

                                dtFinal.Rows[m][0] = parameter.Replace("[", "").Replace("]", "");
                                if (dtFinal.Rows[m][0].ToString() == "WIND GUST")
                                {
                                    dtFinal.Rows[m][0] = "Wind Gust";
                                }

                                if (m < 11)
                                {
                                    DataRow minRows = dsMin.Tables[0].AsEnumerable()
                                           .FirstOrDefault(row => row.Field<string>("ColumnName") == "" + parameter + ""); ;
                                    if (minRows != null)
                                    {
                                        dtFinal.Rows[m][1] = minRows[0].ToString();
                                        dtFinal.Rows[m][2] = minRows[1].ToString();
                                        dtFinal.Rows[m][3] = minRows[3].ToString();
                                    }
                                }

                                dtFinal.Rows[m][4] = maxRow[0].ToString();
                                dtFinal.Rows[m][5] = maxRow[1].ToString();
                                dtFinal.Rows[m][6] = maxRow[3].ToString();
                                // Assuming finalUnits has the same order as finalPara
                                dtFinal.Rows[m][7] = finalUnits[finalPara.IndexOf(parameter.Trim().Replace("[", "").Replace("]", ""))];
                            }

                        }




                        if (ProfileNM == "VMC-AWS-GUJ")
                        {
                            //Rain Customized parameter.....
                            DataRow dr3 = dtFinal.NewRow();
                            dr3[0] = "Rain";
                            dr3[3] = RainTotal;
                            dr3[4] = DateHighRain;
                            dr3[5] = TimeHighRain;
                            dr3[6] = HighRain;
                            dr3[7] = "mm";
                            dtFinal.Rows.Add(dr3);
                        }
                        else if (ProfileNM == "Forest-GOA-AWS")
                        {
                            #region Remove Dynamic Column from Forest-GOA-AWS...
                            string[] arrRowsRemove = new string[] { "Temperature Minimum", "Temperature Maximum", "Temperature DayMinMax", "Humidity Minimum", "Humidity Maximum", "Humidity DayMinMax", "MaxWindSpeed 10m", "WindSpeed DayMax10m" };

                            for (int a = 0; a < arrRowsRemove.Length; a++)
                            {


                                //var table = dtFinal.AsEnumerable().Where(r => r.Field<string>("Parameter") == "Temperature Minimum" || r.Field<string>("Parameter") == "Temperature Maximum" || r.Field<string>("Parameter") == "Temperature DayMinMax").ToList();
                                var table = dtFinal.AsEnumerable().Where(r => r.Field<string>("Parameter") == arrRowsRemove[a]).ToList();
                                foreach (var row in table)
                                    dtFinal.Rows.Remove(row);


                            }
                            #endregion Remove Dynamic COlumn from Forest-GOA-AWS...
                        }
                    }

                    else if (ProfileNM == "VMC-NHP-GUJ")
                    {
                        DataSet dsMin = null;

                        for (int j = 0; j < 3; j++)
                        {
                            dsMin = ObjDB.sp_MinMaxStationData_Status("rpt_DataReport_MinMaxUnit_NewVMCHistory", StID, fromDate, toDate, "MinVal", "", "Web");
                            if (dsMin.Tables.Count > 0)
                                break;
                            Thread.Sleep(1000);
                        }


                        DataSet dsMax = null;

                        for (int j = 0; j < 3; j++)
                        {
                            dsMax = ObjDB.sp_MinMaxStationData_Status("rpt_DataReport_MinMaxUnit_NewVMCHistory", StID, fromDate, toDate, "MaxVal", "", "Web");
                            if (dsMax.Tables.Count > 0)
                                break;
                            Thread.Sleep(1000);
                        }

                        //Put condition over here ....
                        //Max WindSpeed Value,DateMax,TimeMax && Correspnding WindDirection Value....

                        string DateHighWS = dsMax.Tables[0].Rows[4][0].ToString();
                        string TimeHighWS = dsMax.Tables[0].Rows[4][1].ToString();
                        string HighWS = dsMax.Tables[0].Rows[4][2].ToString();

                        string DominantWD = dsMax.Tables[0].Rows[5][2].ToString();

                        double WindDirDegree = Convert.ToDouble(DominantWD);
                        string[] Sector = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
                        string DominantWindDir = Sector[Convert.ToInt32(Math.Round((WindDirDegree % 360) / 22.5))];

                        string WindGust = "";
                        WindGust = HighWS; // + " mps / " + DominantWindDir;


                        string DateHighRain = null, TimeHighRain = null, HighRain = null;
                        string RainTotal = "";

                        if (ProfileNM == "VMC-NHP-GUJ")
                        {
                            #region For calculate VMC-RainTotal....Modify by vikas --> 2022-07-14

                            DataRow dr1 = dsMax.Tables[0].NewRow();
                            dr1[0] = DateHighWS;
                            dr1[1] = TimeHighWS;
                            dr1[2] = WindGust;
                            dsMax.Tables[0].Rows.Add(dr1);


                            toDate = Convert.ToDateTime(toDate).AddDays(1).ToString("yyyy-MM-dd");
                            string SelRainQry = @"SELECT [StationID], SUM([MAXHOUR]) as CummulativeRain FROM (SELECT [StationID], DATEADD(second,DATEDIFF(second,'1970-01-01',CAST([Date] AS datetime) + CAST(DATEADD(minute,-15,CAST([Time] AS datetime)) AS datetime))/3600*3600 , '1970-01-01') AS [DATEHOUR]
                    , MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS [MAXHOUR] FROM tbl_StationData_" + StID.Trim() + " with(nolock) where CAST([Date] AS datetime) + CAST([Time] AS datetime) >= '" + fromDate.Trim() + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) < '" + toDate.Trim() + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";

                            DataSet dsRain = null;
                            for (int r = 0; r < 3; r++)
                            {
                                dsRain = ObjDB.FetchDataset(SelRainQry, "Web");
                                if (dsRain.Tables.Count > 0)
                                    break;
                                Thread.Sleep(1000);
                            }



                            if (dsRain.Tables.Count > 0)
                            {
                                if (dsRain.Tables[0].Rows.Count > 0)
                                {
                                    for (int r = 0; r < dsRain.Tables[0].Rows.Count; r++)
                                    {
                                        RainTotal = dsRain.Tables[0].Rows[r]["CummulativeRain"].ToString();
                                    }
                                }
                            }

                            DateHighRain = dsMax.Tables[0].Rows[1][0].ToString();
                            TimeHighRain = dsMax.Tables[0].Rows[1][1].ToString();
                            HighRain = dsMax.Tables[0].Rows[1][2].ToString();

                            #endregion For calculate VMC-RainTotal....Modify by vikas --> 2022-07-14
                        }

                        string[] Parameters = dtUnit.Tables[0].Rows[0]["SensorName"].ToString().Split(',');
                        string[] Units = dtUnit.Tables[0].Rows[0]["unit"].ToString().Split(',');

                        string strfinalUnits = string.Empty;
                        List<string> finalUnits = null;

                        string strfinalPara = string.Empty;
                        List<string> finalPara = null;

                        if (ProfileNM == "VMC-NHP-GUJ")
                        {
                            string EliminatePara = "Water Level,Snow Depth,Evaporation, Solar Radiation,Peripheral Status";
                            for (int p = 4; p < Parameters.Length - 1; p++)
                            {
                                if (!EliminatePara.Contains(Parameters[p]))
                                {
                                    strfinalUnits = strfinalUnits + "," + Units[p];
                                    strfinalPara = strfinalPara + "," + Parameters[p];
                                }
                            }

                            strfinalUnits = strfinalUnits + "," + " m/s / " + DominantWindDir;
                            //strfinalUnits = strfinalUnits + "," + "mps";
                            strfinalUnits = strfinalUnits.Replace("Degree", "°").TrimStart(',');
                            finalUnits = strfinalUnits.Replace("C", "°C").Split(',').ToList();

                            strfinalPara = strfinalPara + "," + "Wind Gust";
                            strfinalPara = strfinalPara.TrimStart(',');
                            finalPara = strfinalPara.Split(',').ToList();
                        }
                        else
                        {
                            string EliminatePara = "";


                            if (ProfileNM.ToLower().Contains("nhp-"))
                            {
                                if (ProfileNM.ToLower().Contains("awlr"))
                                    EliminatePara = "Snow Depth,Evaporation, Solar Radiation,Peripheral Status";
                                else if (ProfileNM.ToLower().Contains("aws"))
                                    EliminatePara = "Water Level,Snow Depth,Evaporation,Peripheral Status";
                                else if (ProfileNM.ToLower().Contains("aws+awlr"))
                                    EliminatePara = "Snow Depth,Evaporation, Solar Radiation,Peripheral Status";

                                for (int p = 4; p < Parameters.Length - 1; p++)
                                {
                                    if (!EliminatePara.Contains(Parameters[p]))
                                    {
                                        strfinalUnits = strfinalUnits + "," + Units[p];
                                        strfinalPara = strfinalPara + "," + Parameters[p];
                                    }
                                }

                            }
                            else
                            {
                                //StationID,Station Name,Latitude,Longitude,Altitude,Date,Time,Battery Voltage,GPS,GPRS SIGNALSTRENGTH,TypeOfSystem,NoOfParameters,RainFall HealthStatus,Hourly Rainfall,Daily Rain,Air Temperature HealthStatus,Air Temperature,Temperature Minimum,Temperature Maximum,Temperature DayMinMax,Humidity HealthStatus,Humidity,Humidity Minimum,Humidity Maximum,Humidity DayMinMax,WindDirection HealthStatus10m,WindDirection 10m,WindSpeed HealthStatus10m,WindSpeed 10m1minAvg,WindSpeed 10m3minAvg,WindSpeed 10m10minAvg,MaxWindSpeed 10m,WindSpeed DayMax10m,Pressure HealthStatus,Atmospheric Pressure,WindDirection HealthStatus3m,WindDirection 3m,WindSpeed HealthStatus3m,WindSpeed 3m1minAvg,WindSpeed 3m3minAvg,WindSpeed 3m10minAvg,MaxWindSpeed 3m,WindSpeed DayMax3m,SoilTemperature HealthStatus10cm,SoilTemperature 10cm,SoilMoisture HealthStatus10cm,SoilMoisture 10cm,SoilTemperature HealthStatus30cm,SoilTemperature 30cm,SoilMoisture HealthStatus30cm,SoilMoisture 30cm,SoilTemperature HealthStatus70cm,SoilTemperature 70cm,SoilMoisture HealthStatus70cm,SoilMoisture 70cm,SoilTemperature HealthStatus100cm,SoilTemperature 100cm,SoilMoisture HealthStatus100cm,SoilMoisture 100cm,SolarRadiation HealthStatus,Sunshine Duration,Global Radiation,Radiation HealthStatus,Par Data,Evaporameter HealthStatus,Evaporameter,UVRadiation_a HealthStatus,UV Radiation_a,UVRadiation_b HealthStatus,UV Radiation_b,UV Index,VisibilitySensor HealthStatus,Visibility,SnowDepth HealthStatus,Snow Depth,SnowWater HealthStatus,Snow WaterEquivalent,WaterLevel HealthStatus,WaterLevel Measurement
                                //EliminatePara = "StationID,Station Name,Latitude,Longitude,Altitude,Date,Time,Battery Voltage,GPS,GPRS SIGNALSTRENGTH,TypeOfSystem,NoOfParameters,RainFall HealthStatus,Hourly Rainfall,Daily Rain,AirTemperature HealthStatus,Temperature Minimum,Temperature Maximum,Temperature DayMinMax,Humidity HealthStatus,Humidity Minimum,Humidity Maximum,Humidity DayMinMax,WindDirection HealthStatus10m,WindSpeed HealthStatus10m,Pressure HealthStatus,WindDirection HealthStatus3m,WindDirection 3m,WindSpeed HealthStatus3m,WindSpeed 3m1minAvg,WindSpeed 3m3minAvg,WindSpeed 3m10minAvg,MaxWindSpeed 3m,WindSpeed DayMax3m,SoilTemperature HealthStatus10cm,SoilMoisture HealthStatus10cm,SoilTemperature HealthStatus30cm,SoilTemperature 30cm,SoilMoisture HealthStatus30cm,SoilMoisture 30cm,SoilTemperature HealthStatus70cm,SoilMoisture HealthStatus70cm,SoilTemperature HealthStatus100cm,SoilTemperature 100cm,SoilMoisture HealthStatus100cm,SoilMoisture 100cm,SolarRadiation HealthStatus,Sunshine Duration,Global Radiation,Radiation HealthStatus,Par Data,Evaporameter HealthStatus,Evaporameter,UVRadiation_a HealthStatus,UV Radiation_a,UVRadiation_b HealthStatus,UV Radiation_b,UV Index,VisibilitySensor HealthStatus,Visibility,SnowDepth HealthStatus,Snow Depth,SnowWater HealthStatus,Snow WaterEquivalent,WaterLevel HealthStatus,WaterLevel Measurement";

                                EliminatePara = "Battery Voltage,Hourly Rainfall,Daily Rain,Air Temperature,Temperature Minimum,Temperature Maximum,Temperature DayMinMax,Humidity,Humidity Minimum,Humidity Maximum,Humidity DayMinMax,WindDirection 10m,WindSpeed 10m1minAvg,WindSpeed 10m3minAvg,WindSpeed 10m10minAvg,MaxWindSpeed 10m,WindSpeed DayMax10m,Atmospheric Pressure,SoilTemperature 10cm,SoilMoisture 10cm,SoilTemperature 70cm,SoilMoisture 70cm";

                                //EliminatePara = "Battery Voltage,Hourly Rainfall,Daily Rain,Air Temperature,Humidity,WindDirection 10m,WindSpeed 10m1minAvg,WindSpeed 10m3minAvg,WindSpeed 10m10minAvg,Atmospheric Pressure,SoilTemperature 10cm,SoilMoisture 10cm,SoilTemperature 70cm,SoilMoisture 70cm";

                                string[] arrElimatePara = EliminatePara.Split(',');

                                for (int p = 0; p < Parameters.Length - 1; p++)
                                {
                                    for (int e = 0; e < arrElimatePara.Length; e++)
                                    {
                                        if (Parameters[p].ToLower().Trim().Equals(arrElimatePara[e].ToLower().Trim()) == true)
                                        {
                                            strfinalUnits = strfinalUnits + "," + Units[p];
                                            strfinalPara = strfinalPara + "," + Parameters[p];
                                            break;
                                        }
                                    }

                                }

                            }

                            strfinalUnits = strfinalUnits.TrimStart(',');
                            finalUnits = strfinalUnits.Split(',').ToList();

                            strfinalPara = strfinalPara.TrimStart(',');
                            finalPara = strfinalPara.Split(',').ToList();

                        }

                        dtFinal.Columns.Add("Parameter", typeof(string));
                        dtFinal.Columns.Add("DateMin", typeof(string));
                        dtFinal.Columns.Add("TimeMin", typeof(string));
                        dtFinal.Columns.Add("MinVal", typeof(string));
                        dtFinal.Columns.Add("DateMax", typeof(string));
                        dtFinal.Columns.Add("TimeMax", typeof(string));
                        dtFinal.Columns.Add("MaxVal", typeof(string));
                        dtFinal.Columns.Add("Unit", typeof(string));


                        for (int m = 0; m < dsMax.Tables[0].Rows.Count; m++)
                        {
                            DataRow dr = dtFinal.NewRow();
                            dtFinal.Rows.Add(dr);

                            if (ProfileNM == "VMC-NHP-GUJ")
                            {
                                dtFinal.Rows[m][0] = finalPara[m];

                                if (m < 8)
                                {
                                    dtFinal.Rows[m][1] = dsMin.Tables[0].Rows[m][0].ToString();
                                    dtFinal.Rows[m][2] = dsMin.Tables[0].Rows[m][1].ToString();
                                    dtFinal.Rows[m][3] = dsMin.Tables[0].Rows[m][2].ToString();
                                }
                                //else if(m == 8)
                                //{
                                //    dtFinal.Rows[m][3] = "";
                                //}

                                dtFinal.Rows[m][4] = dsMax.Tables[0].Rows[m][0].ToString();
                                dtFinal.Rows[m][5] = dsMax.Tables[0].Rows[m][1].ToString();
                                dtFinal.Rows[m][6] = dsMax.Tables[0].Rows[m][2].ToString();
                                dtFinal.Rows[m][7] = finalUnits[m];
                            }
                            else if (ProfileNM == "Forest-GOA-AWS")
                            {
                                dtFinal.Rows[m][0] = finalPara[m];

                                if (m < dsMax.Tables[0].Rows.Count)
                                {
                                    dtFinal.Rows[m][1] = dsMin.Tables[0].Rows[m][0].ToString();
                                    dtFinal.Rows[m][2] = dsMin.Tables[0].Rows[m][1].ToString();
                                    dtFinal.Rows[m][3] = dsMin.Tables[0].Rows[m][2].ToString();
                                }
                                dtFinal.Rows[m][4] = dsMax.Tables[0].Rows[m][0].ToString();
                                dtFinal.Rows[m][5] = dsMax.Tables[0].Rows[m][1].ToString();
                                dtFinal.Rows[m][6] = dsMax.Tables[0].Rows[m][2].ToString();


                                dtFinal.Rows[m][7] = finalUnits[m];
                            }
                            else
                            {

                                if (m >= finalPara.Count)
                                {
                                    finalPara.Add("Wind Gust");
                                    finalUnits.Add("m/s");
                                }

                                dtFinal.Rows[m][0] = finalPara[m];

                                if (m < dsMax.Tables[0].Rows.Count - 1)
                                {
                                    dtFinal.Rows[m][1] = dsMin.Tables[0].Rows[m][0].ToString();
                                    dtFinal.Rows[m][2] = dsMin.Tables[0].Rows[m][1].ToString();
                                    dtFinal.Rows[m][3] = dsMin.Tables[0].Rows[m][2].ToString();
                                }
                                dtFinal.Rows[m][4] = dsMax.Tables[0].Rows[m][0].ToString();
                                dtFinal.Rows[m][5] = dsMax.Tables[0].Rows[m][1].ToString();
                                dtFinal.Rows[m][6] = dsMax.Tables[0].Rows[m][2].ToString();

                                dtFinal.Rows[m][7] = finalUnits[m];
                            }

                        }


                        if (ProfileNM == "VMC-NHP-GUJ")
                        {
                            //Rain Customized parameter.....
                            DataRow dr3 = dtFinal.NewRow();
                            dr3[0] = "Rain";
                            dr3[3] = RainTotal;
                            dr3[4] = DateHighRain;
                            dr3[5] = TimeHighRain;
                            dr3[6] = HighRain;
                            dr3[7] = "mm";
                            dtFinal.Rows.Add(dr3);
                        }
                        else if (ProfileNM == "Forest-GOA-AWS")
                        {
                            #region Remove Dynamic Column from Forest-GOA-AWS...
                            string[] arrRowsRemove = new string[] { "Temperature Minimum", "Temperature Maximum", "Temperature DayMinMax", "Humidity Minimum", "Humidity Maximum", "Humidity DayMinMax", "MaxWindSpeed 10m", "WindSpeed DayMax10m" };

                            for (int a = 0; a < arrRowsRemove.Length; a++)
                            {


                                //var table = dtFinal.AsEnumerable().Where(r => r.Field<string>("Parameter") == "Temperature Minimum" || r.Field<string>("Parameter") == "Temperature Maximum" || r.Field<string>("Parameter") == "Temperature DayMinMax").ToList();
                                var table = dtFinal.AsEnumerable().Where(r => r.Field<string>("Parameter") == arrRowsRemove[a]).ToList();
                                foreach (var row in table)
                                    dtFinal.Rows.Remove(row);


                            }
                            #endregion Remove Dynamic COlumn from Forest-GOA-AWS...
                        }
                    }
                }


                return Request.CreateResponse(HttpStatusCode.OK, dtFinal);
            }

            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                return Request.CreateResponse(HttpStatusCode.OK, "Fail");

            }

        }

        [HttpPost]
        [Route("finalStHistoricaldata")]
        public async Task<HttpResponseMessage> GetStHistoricalData()
        {
            strFunctionName = "Get Station historical data as per selection criteria";

            try
            {
                var objStation = await Request.Content.ReadAsFormDataAsync();

                string fetchQry = string.Empty;
                string StID = string.Empty;

                if (objStation != null)
                {
                    StID = Regex.Unescape(objStation.Get("StationID"));
                    string Day = Regex.Unescape(objStation.Get("Day"));
                    string Month = Regex.Unescape(objStation.Get("Month"));
                    string Year = Regex.Unescape(objStation.Get("Year"));

                    if (string.IsNullOrEmpty(Day.Trim()) && string.IsNullOrEmpty(Month.Trim()) && !string.IsNullOrEmpty(Year.Trim()))
                        fetchQry = "select * from AWSAPI.tbl_Historyyear with(nolock) where StationId = '" + StID.Trim() + "' and Year = '" + Year.Trim() + "'";
                    else if (string.IsNullOrEmpty(Day.Trim()) && !string.IsNullOrEmpty(Month.Trim()) && !string.IsNullOrEmpty(Year.Trim()))
                        fetchQry = "select * from AWSAPI.tbl_Historymonth with(nolock) where StationId = '" + StID.Trim() + "' and Month  = '" + Month.Trim() + "' and Year = '" + Year.Trim() + "'";
                    else if (!string.IsNullOrEmpty(Day.Trim()) && !string.IsNullOrEmpty(Month.Trim()) && !string.IsNullOrEmpty(Year.Trim()))
                        fetchQry = "select * from AWSAPI.tbl_Historyday with(nolock) where StationId = '" + StID.Trim() + "' and Day= '" + Day.Trim() + "' and Month  = '" + Month.Trim() + "' and Year = '" + Year.Trim() + "'";
                }

                DataSet dsHistoryData = null;

                for (int c = 0; c < 3; c++)
                {
                    dsHistoryData = ObjDB.FetchDataset(fetchQry, "Web");
                    if (dsHistoryData.Tables.Count > 0)
                        break;
                    Thread.Sleep(1000);
                }

                if (dsHistoryData.Tables[0].Rows.Count > 0)
                {
                    string strJson = dsHistoryData.Tables[0].Rows[0]["Jsondata"].ToString();

                    List<clsHistoryHelper> Hdata = JsonConvert.DeserializeObject<List<clsHistoryHelper>>(strJson);

                    List<clsHistoricalData> LsthistoricalData = new List<clsHistoricalData>();

                    for (int h = 0; h < Hdata.Count; h++)
                    {
                        if (!Hdata[h].Parameter.ToLower().Contains("battery") && !Hdata[h].Parameter.ToLower().Contains("hourly") && !Hdata[h].Parameter.ToLower().Contains("daily") && !Hdata[h].Parameter.ToLower().Contains("wind spee") && !Hdata[h].Parameter.ToLower().Contains("wind dir"))
                        {
                            clsHistoricalData historicalData = new clsHistoricalData();

                            if (Hdata[h].Parameter.ToLower().Contains("air temp"))
                                historicalData.paraTitle = "temperature";
                            else if (Hdata[h].Parameter.ToLower().Contains("press"))
                                historicalData.paraTitle = "pressure";
                            else
                            {
                                string[] ParaTitle = Hdata[h].Parameter.Split(' ').ToArray();
                                historicalData.paraTitle = ParaTitle[0];
                            }

                            List<clsHistoryData> historyData = new List<clsHistoryData>();

                            for (int obj = 0; obj < 2; obj++)
                            {
                                if (obj == 0)
                                {
                                    clsHistoryData ObjhistoryData = new clsHistoryData();

                                    if (Hdata[h].Parameter.ToLower().Contains("wind"))
                                    {
                                        if (StID.StartsWith("BDC00") || StID.StartsWith("ABCDEF01"))
                                        {
                                            ObjhistoryData.title = "Dominant Wind Direction";
                                            string[] WD = Hdata[h].Unit.Split('/');
                                            ObjhistoryData.paraValue = WD[2].Trim();
                                            ObjhistoryData.paraUnit = "";
                                        }
                                        else
                                        {
                                            ObjhistoryData.title = "Low " + Hdata[h].Parameter;
                                            ObjhistoryData.paraValue = Hdata[h].MinVal;
                                            ObjhistoryData.dateTime = Hdata[h].DateMin + "@" + Hdata[h].TimeMin;
                                            ObjhistoryData.paraUnit = Hdata[h].Unit;
                                        }
                                    }
                                    else if (Hdata[h].Parameter.ToLower().Contains("rain"))
                                    {
                                        ObjhistoryData.title = "Rain Total";
                                        ObjhistoryData.paraValue = Hdata[h].MinVal; //Hdata[h].MaxVal;
                                        ObjhistoryData.paraUnit = Hdata[h].Unit;
                                    }
                                    else
                                    {
                                        ObjhistoryData.title = "Low " + Hdata[h].Parameter;
                                        ObjhistoryData.paraValue = Hdata[h].MinVal;
                                        ObjhistoryData.dateTime = Hdata[h].DateMin + "@" + Hdata[h].TimeMin;
                                        ObjhistoryData.paraUnit = Hdata[h].Unit;
                                    }

                                    historyData.Add(ObjhistoryData);
                                }
                                else
                                {
                                    clsHistoryData clsHistoryData = new clsHistoryData();
                                    if (!Hdata[h].Parameter.ToLower().Contains("wind dir"))
                                        clsHistoryData.title = "High " + Hdata[h].Parameter;
                                    else if (Hdata[h].Parameter.ToLower().Contains("wind dir"))
                                    {
                                        if (StID.StartsWith("BDC00"))
                                        {
                                            clsHistoryData.title = "Dominant Wind Direction";
                                            clsHistoryData.paraUnit = "";
                                        }
                                        else
                                        {
                                            clsHistoryData.title = "High " + Hdata[h].Parameter;
                                        }
                                    }
                                    clsHistoryData.paraValue = Hdata[h].MaxVal;
                                    clsHistoryData.dateTime = Hdata[h].DateMax + "@" + Hdata[h].TimeMax;
                                    string[] WD = Hdata[h].Unit.TrimStart(' ').Split(' ');
                                    clsHistoryData.paraUnit = WD[0];
                                    historyData.Add(clsHistoryData);
                                }
                            }

                            historicalData.historyData = historyData;

                            if (historicalData.paraTitle.ToLower().Contains("rain"))
                                LsthistoricalData.Insert(0, historicalData);
                            else
                                LsthistoricalData.Add(historicalData);
                        }
                    }

                    clsJsonHistory ObjJsonResp = new clsJsonHistory();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                    ObjJsonResp.message = "success";
                    ObjJsonResp.status = true;
                    ObjJsonResp.data = LsthistoricalData;

                    return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.OK);
                    ObjJsonResp.message = "data not found";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.OK, ObjJsonResp);
                }

            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "internal server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
            }

        }

        #endregion Station Data History


        #region RainSummary

        public List<clsRainDetail> RainSummary(string Profile, string StID)
        {
            strFunctionName = "Get Station Rain Summary Data";

            List<clsRainDetail> LstRainDetail = new List<clsRainDetail>();

            try
            {
                clsRainDetail Rdetail = new clsRainDetail();
                string CurrDT = string.Empty;

                //==============================Calculate 24 HR.....==================================

                string GellAllProfile = @"SELECT t2.Name , STUFF((SELECT ',' + t1.StationID FROM tbl_StationMaster t1 with(nolock) where t1.Profile = t2.Name FOR XML PATH('')), 1 ,1, '') AS StationID 
                                          FROM tbl_ProfileMaster t2  with(nolock) GROUP BY t2.Name having  t2.Name='" + Profile.Trim() + "'";

                DataSet dsSelect = null;

                for (int j = 0; j < 3; j++)
                {
                    dsSelect = ObjDB.FetchDataset(GellAllProfile, "web");

                    if (dsSelect.Tables.Count > 0)
                        break;
                    Thread.Sleep(1000);
                }

                string StIDs = "";

                if (dsSelect.Tables[0].Rows.Count > 0)
                {
                    StIDs = dsSelect.Tables[0].Rows[0]["StationID"].ToString();
                }

                DataSet dsRainDetail = null;

                if (Profile == "VMC-AWS-GUJ")
                {
                    CurrDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                    string yesterDayDT = string.Empty;

                    yesterDayDT = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm");

                    for (int j = 0; j < 3; j++)
                    {
                        dsRainDetail = ObjDB.FetchData_SP_StationData("DemoGetStationData_VMC", StIDs, CurrDT, CurrDT, "Web");

                        if (dsRainDetail.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    /*DataTable dtStationHR = new DataTable();

                    if (!string.IsNullOrEmpty(StID))
                    {
                        dtStationHR = dsRainDetail.Tables[0].AsEnumerable().Where(row => row.Field<string>("StationID") == StID.Trim()).CopyToDataTable();
                    }

                    Rdetail.Rain24HR = dtStationHR.Rows[0]["15mins RAINFALL"].ToString();*/

                    string strQry = @"select  [StationID],sum(try_cast(RTRIM(LTRIM(replace([15mins RAINFALL], ''' + + ''', ''''))) as decimal(18,2))) as CummulativeRain 
                                      from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + yesterDayDT.Trim() + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) <= '" + CurrDT.Trim() + "' group by StationID";

                    DataTable dtRain24HR = ObjDB.FetchDataTable(strQry, "web");

                    if (dtRain24HR.Rows.Count > 0)
                    {
                        Rdetail.Rain24HR = dtRain24HR.Rows[0]["CummulativeRain"].ToString();
                    }
                }
                else if (Profile == "VMC-NHP-GUJ")
                {
                    CurrDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                    for (int j = 0; j < 3; j++)
                    {
                        dsRainDetail = ObjDB.FetchData_SP_StationData("GetStationData_VMC", StIDs, CurrDT, CurrDT, "Web");
                        if (dsRainDetail.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    DataTable dtStationHR = new DataTable();

                    if (!string.IsNullOrEmpty(StID))
                    {
                        dtStationHR = dsRainDetail.Tables[0].AsEnumerable().Where(row => row.Field<string>("StationID") == StID.Trim()).CopyToDataTable();
                    }

                    Rdetail.Rain24HR = dtStationHR.Rows[0]["Hourly Rainfall"].ToString();
                }
                else
                {
                    CurrDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string LastDT = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm");
                    string CurrentLastDT = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM ( SELECT[StationID], Date,Time,SUM(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) + CAST([Time] AS datetime) <= '" + CurrDT.Trim() + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) >= '" + LastDT.Trim() + "' group by [StationID],Date,Time)A group by[StationID]";
                    DataSet dscurrentDT = ObjDB.FetchDataset(CurrentLastDT, "Web");
                    for (int j = 0; j < 3; j++)
                    {

                        if (dscurrentDT.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    Rdetail.Rain24HR = dscurrentDT.Tables[0].Rows[0]["CummulativeRain"].ToString();
                }




                //==============================END Calculate 24 HR.....==============================================================

                //================================Calculate Current Rate....=========================================================== 
                #region current Rain rate remove
                string CurrHRSelQry = "";
                if (Profile == "VMC-AWS-GUJ")
                {
                    CurrHRSelQry = "select top 1 Date,Time,[Rain Rate] from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) Where Date = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' order by Date desc,Time desc";
                }
                else
                {
                    CurrHRSelQry = "select top 1 Date,Time,[Hourly Rainfall] from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) Where Date = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' order by Date desc,Time desc";
                }

                if (Profile == "VMC-NHP-GUJ")
                {
                    //Current HR...
                    DataSet dsCurrHR = ObjDB.FetchDataset(CurrHRSelQry, "web");
                    string CurrTm = dsCurrHR.Tables[0].Rows[0]["Time"].ToString();

                    //Previous HR...
                    string PreTm = "";
                    double DiffHR = 0;

                    if (CurrTm != "00:00" && !CurrTm.Contains(":15"))
                    {
                        DateTime PreviousTm = Convert.ToDateTime(CurrTm).Subtract(new TimeSpan(0, 15, 0));
                        PreTm = PreviousTm.ToString("HH:mm");
                        string PreHRSelQry = "select Date,Time,[Hourly Rainfall] from tbl_StationData_" + StID.Trim() + " Where Date = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' and Time = '" + PreTm + "'";
                        DataSet dsPreHR = ObjDB.FetchDataset(PreHRSelQry, "web");

                        DiffHR = Convert.ToDouble(dsCurrHR.Tables[0].Rows[0]["Hourly Rainfall"]) - Convert.ToDouble(dsPreHR.Tables[0].Rows[0]["Hourly Rainfall"]);

                        if (DiffHR <= 0)
                            DiffHR = 000.0;

                        //Change by vikas --> 27-Apr-2023
                        //DiffHR = DiffHR * 4;
                        Rdetail.CurrentRainRate = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                    }
                    else if (CurrTm.Contains(":15"))
                    {
                        DiffHR = Convert.ToDouble(dsCurrHR.Tables[0].Rows[0]["Hourly Rainfall"]) * 4;
                        Rdetail.CurrentRainRate = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                    }
                    else if (CurrTm == "00:00")
                    {
                        DateTime PreDT = Convert.ToDateTime(dsCurrHR.Tables[0].Rows[0]["Date"].ToString()).AddDays(-1);
                        string finalPreDT = PreDT.ToString("yyyy-MM-dd");
                        string fetchQry = "select Top 1 Date,Time, [Hourly Rainfall] from tbl_StationData_" + StID.Trim() + " where Date = '" + finalPreDT + "' order by Time desc";
                        DataSet dsHR = ObjDB.FetchDataset(fetchQry, "web");

                        DiffHR = Convert.ToDouble(dsCurrHR.Tables[0].Rows[0]["Hourly Rainfall"]) - Convert.ToDouble(dsHR.Tables[0].Rows[0]["Hourly Rainfall"]);

                        if (DiffHR <= 0)
                            DiffHR = 000.0;

                        //Change by vikas --> 27-Apr-2023
                        //DiffHR = DiffHR * 4;
                        Rdetail.CurrentRainRate = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                    }
                }
                else if (Profile != "VMC-NHP-GUJ")
                {
                    DataSet dsCurrHR = ObjDB.FetchDataset(CurrHRSelQry, "web");
                    string CurrRate = "";

                    if (dsCurrHR.Tables.Count > 0)
                    {
                        if (dsCurrHR.Tables[0].Rows.Count > 0)
                        {
                            CurrRate = dsCurrHR.Tables[0].Rows[0]["Rain Rate"].ToString();
                        }

                    }
                    Rdetail.CurrentRainRate = CurrRate;
                }
                //================================END Calculate Current Rate....=========================================================== 
                #endregion
                //===============================Calculate CurrentDay,LastDay,LastHour,MonthTotal & YearTotal=============================================

                if (Profile == "VMC-AWS-GUJ")
                {
                    //Current Day...
                    string CurrDayStartDT = DateTime.Now.ToString("yyyy-MM-dd") + " " + "08:00";
                    string DT = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                    //string CurrDayTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM(SELECT[StationID], DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime)) / 3600 * 3600 , '1970-01-01') AS[DATEHOUR]
                    //,MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + CurrDayStartDT + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";
                    string CurrDayTotal = "select  [StationID],sum(try_cast(RTRIM(LTRIM(replace([15mins RAINFALL], ''' + + ''', ''''))) as decimal(18,2))) as CummulativeRain from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + CurrDayStartDT.Trim() + "' and  CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + DT.Trim() + "' group by StationID";
                    DataSet dsCurrDayTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsCurrDayTotal = ObjDB.FetchDataset(CurrDayTotal, "web");
                        if (dsCurrDayTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsCurrDayTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.CurrentDay = dsCurrDayTotal.Tables[0].Rows[0][1].ToString();
                    }

                    //Last Day...
                    string LastDayStartDT = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd") + " " + "08:15";
                    string LastCurrDT = DateTime.Now.ToString("yyyy-MM-dd") + " " + "08:00";

                    //string LastDayTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM(SELECT[StationID], DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime)) / 3600 * 3600 , '1970-01-01') AS[DATEHOUR]
                    //,MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + LastDayStartDT + "' and CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + LastCurrDT.Trim() + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";
                    string LastDayTotal = "select  [StationID],sum(try_cast(RTRIM(LTRIM(replace([15mins RAINFALL], ''' + + ''', ''''))) as decimal(18,2))) as CummulativeRain from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + LastDayStartDT.Trim() + "' and  CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + LastCurrDT.Trim() + "' group by StationID";
                    DataSet dsLastDayTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsLastDayTotal = ObjDB.FetchDataset(LastDayTotal, "web");
                        if (dsLastDayTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsLastDayTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.LastDay = dsLastDayTotal.Tables[0].Rows[0][1].ToString();
                    }


                    //Last Hour...
                    /*string LastHourDTm = Convert.ToDateTime(CurrDT).AddHours(-1).ToString("yyyy-MM-dd HH:mm");
                    string LastHourQry = @"select StationID,SUM(try_cast([Hourly Rainfall] as decimal(18,2))) as CummulativeRain from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) + CAST([Time] AS datetime) >= '" + LastHourDTm + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) <= '" + CurrDT + "' group by StationID";

                    DataSet dsLastHour = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsLastHour = ObjDB.FetchDataset(LastHourQry, "web");
                        if (dsLastHour.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsLastHour.Tables[0].Rows.Count > 0)
                    {

                        Rdetail.LastHour = dsLastHour.Tables[0].Rows[0][1].ToString();

                    }*/

                    var lastHRain = (from row in dsRainDetail.Tables[0].AsEnumerable()
                                     where row.Field<string>("StationID") == StID.Trim()
                                     select new
                                     {
                                         lastHour = row.Field<string>("15mins RainFALL"),
                                     }).FirstOrDefault();


                    //Month Total....
                    string MonthEndDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string MonthStartDT = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm");

                    //string SqlMonTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM(SELECT[StationID], DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime)) / 3600 * 3600 , '1970-01-01') AS[DATEHOUR]
                    //,MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + MonthStartDT + "' and CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + CurrDT.Trim() + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";
                    string SqlMonTotal = "select  [StationID],sum(try_cast(RTRIM(LTRIM(replace([15mins RAINFALL], ''' + + ''', ''''))) as decimal(18,2))) as CummulativeRain from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + MonthStartDT + "' and  CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + MonthEndDT + "' group by StationID";
                    DataSet dsMonTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsMonTotal = ObjDB.FetchDataset(SqlMonTotal, "web");
                        if (dsMonTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsMonTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.MonthTotal = dsMonTotal.Tables[0].Rows[0][1].ToString();
                    }

                    //Year Total...
                    string YearStartDT = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm");
                    string YearEndDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                    //string SqlYearTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM(SELECT[StationID], DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime)) / 3600 * 3600 , '1970-01-01') AS[DATEHOUR]
                    //,MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR] FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + YearStartDT + "' and CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + CurrDT.Trim() + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";
                    string SqlYearTotal = "select  [StationID],sum(try_cast(RTRIM(LTRIM(replace([15mins RAINFALL], ''' + + ''', ''''))) as decimal(18,2))) as CummulativeRain from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + YearStartDT.Trim() + "' and  CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + YearEndDT.Trim() + "' group by StationID";
                    DataSet dsYearTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsYearTotal = ObjDB.FetchDataset(SqlYearTotal, "web");
                        if (dsYearTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsYearTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.YearTotal = dsYearTotal.Tables[0].Rows[0][1].ToString();
                    }

                }
                else if (Profile == "VMC-NHP-GUJ")
                {
                    //Current Day...
                    string CurrDayStartDT = DateTime.Now.ToString("yyyy-MM-dd") + " " + "08:00";

                    string CurrDayTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM(SELECT[StationID], DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime)) / 3600 * 3600 , '1970-01-01') AS[DATEHOUR]
                        ,MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + CurrDayStartDT + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";

                    DataSet dsCurrDayTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsCurrDayTotal = ObjDB.FetchDataset(CurrDayTotal, "web");
                        if (dsCurrDayTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsCurrDayTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.CurrentDay = dsCurrDayTotal.Tables[0].Rows[0][1].ToString();
                    }

                    //Last Day...
                    string LastDayStartDT = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd") + " " + "08:15";
                    string LastCurrDT = DateTime.Now.ToString("yyyy-MM-dd") + " " + "08:00";

                    string LastDayTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM(SELECT[StationID], DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime)) / 3600 * 3600 , '1970-01-01') AS[DATEHOUR]
                               ,MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + LastDayStartDT + "' and CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + LastCurrDT.Trim() + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";

                    DataSet dsLastDayTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsLastDayTotal = ObjDB.FetchDataset(LastDayTotal, "web");
                        if (dsLastDayTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsLastDayTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.LastDay = dsLastDayTotal.Tables[0].Rows[0][1].ToString();
                    }


                    //Last Hour...
                    /*string LastHourDTm = Convert.ToDateTime(CurrDT).AddHours(-1).ToString("yyyy-MM-dd HH:mm");
                    string LastHourQry = @"select StationID,SUM(try_cast([Hourly Rainfall] as decimal(18,2))) as CummulativeRain from tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) + CAST([Time] AS datetime) >= '" + LastHourDTm + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) <= '" + CurrDT + "' group by StationID";

                    DataSet dsLastHour = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsLastHour = ObjDB.FetchDataset(LastHourQry, "web");
                        if (dsLastHour.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsLastHour.Tables[0].Rows.Count > 0)
                    {

                        Rdetail.LastHour = dsLastHour.Tables[0].Rows[0][1].ToString();

                    }*/

                    var lastHRain = (from row in dsRainDetail.Tables[0].AsEnumerable()
                                     where row.Field<string>("StationID") == StID.Trim()
                                     select new
                                     {
                                         lastHour = row.Field<string>("Hourly Rainfall"),
                                     }).FirstOrDefault();

                    //Month Total....
                    string MonthStartDT = Convert.ToDateTime(CurrDT).Year + "-" + Convert.ToDateTime(CurrDT).Month + "-" + "01" + " " + DateTime.Now.ToString("HH:mm");

                    string SqlMonTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM(SELECT[StationID], DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime)) / 3600 * 3600 , '1970-01-01') AS[DATEHOUR]
                        ,MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + MonthStartDT + "' and CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + CurrDT.Trim() + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";

                    DataSet dsMonTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsMonTotal = ObjDB.FetchDataset(SqlMonTotal, "web");
                        if (dsMonTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsMonTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.MonthTotal = dsMonTotal.Tables[0].Rows[0][1].ToString();
                    }

                    //Year Total...
                    string YearStartDT = Convert.ToDateTime(CurrDT).Year + "-" + "01" + "-" + "01" + " " + DateTime.Now.ToString("HH:mm");

                    string SqlYearTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM(SELECT[StationID], DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime)) / 3600 * 3600 , '1970-01-01') AS[DATEHOUR]
                        ,MAX(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR] FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + YearStartDT + "' and CAST([Date] AS datetime) +CAST([Time] AS datetime) <= '" + CurrDT.Trim() + "' group by DATEADD(second, DATEDIFF(second, '1970-01-01', CAST([Date] AS datetime) + CAST(DATEADD(minute, -15, CAST([Time] AS datetime)) AS datetime))/ 3600 * 3600 , '1970-01-01'),[StationID])A group by[StationID]";

                    DataSet dsYearTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsYearTotal = ObjDB.FetchDataset(SqlYearTotal, "web");
                        if (dsYearTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsYearTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.YearTotal = dsYearTotal.Tables[0].Rows[0][1].ToString();
                    }

                }
                else
                {
                    //Current Day...
                    string CurrDayStartDT = DateTime.Now.ToString("yyyy-MM-dd") + " " + "08:00";

                    string CurrDayTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM( SELECT[StationID], Date,Time 
                    ,SUM(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) +CAST([Time] AS datetime) >= '" + CurrDayStartDT + "' group by [StationID],Date,Time)A group by[StationID]";

                    DataSet dsCurrDayTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsCurrDayTotal = ObjDB.FetchDataset(CurrDayTotal, "web");
                        if (dsCurrDayTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsCurrDayTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.CurrentDay = dsCurrDayTotal.Tables[0].Rows[0][1].ToString();
                    }


                    //Last Day...
                    string LastDayStartDT = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd") + " " + "08:15";
                    string LastCurrDT = DateTime.Now.ToString("yyyy-MM-dd") + " " + "08:00";

                    string LastDayTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM (
                     SELECT[StationID], Date,Time,SUM(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) + CAST([Time] AS datetime) >= '" + LastDayStartDT.Trim() + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) <= '" + LastCurrDT.Trim() + "' group by [StationID],Date,Time )A group by[StationID]";

                    DataSet dsLastDayTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsLastDayTotal = ObjDB.FetchDataset(LastDayTotal, "web");
                        if (dsLastDayTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsLastDayTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.LastDay = dsLastDayTotal.Tables[0].Rows[0][1].ToString();
                    }

                    //Last Hour
                    if (Profile != "Forest-GOA-AWS")
                    {

                        var lastHRain = (from row in dsRainDetail.Tables[0].AsEnumerable()
                                         where row.Field<string>("StationID") == StID.Trim()
                                         select new
                                         {
                                             lastHour = row.Field<string>("Hourly Rainfall"),
                                         }).FirstOrDefault();
                    }
                    else
                    {
                        string getCurrDT = "select top 1 Date,Time from tbl_StationData_" + StID.Trim() + " order by Date desc,Time desc";
                        DataTable dtCurrDT = ObjDB.FetchDataTable(getCurrDT, "web");

                        if (dtCurrDT.Rows.Count > 0)
                        {

                            DateTime dTm = Convert.ToDateTime(dtCurrDT.Rows[0][0] + " " + dtCurrDT.Rows[0][1]);
                            string frHRDT = dTm.ToString("yyyy-MM-dd HH:mm");
                            string toHRDT = dTm.AddMinutes(-60).ToString("yyyy-MM-dd HH:mm");

                            string lastHRain = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM ( SELECT[StationID], Date,Time,SUM(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) + CAST([Time] AS datetime) <= '" + frHRDT.Trim() + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) >= '" + toHRDT.Trim() + "' group by [StationID],Date,Time)A group by[StationID]";
                            DataTable dtlastHR = ObjDB.FetchDataTable(lastHRain, "web");

                            if (dtlastHR.Rows.Count > 0)
                            {
                                var lastHR = dtlastHR.Rows[0]["CummulativeRain"].ToString();
                                Rdetail.LastHour = lastHR;
                            }

                        }
                    }

                    //Month Total....
                    string MonthStartDT = Convert.ToDateTime(CurrDT).Year + "-" + Convert.ToDateTime(CurrDT).Month + "-" + "01";

                    string MonthSDT = Convert.ToDateTime(MonthStartDT).ToString("yyyy-MM-dd");

                    string SqlMonTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM (
                     SELECT[StationID], Date,Time,SUM(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) + CAST([Time] AS datetime) >= '" + MonthSDT.Trim() + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) <= '" + CurrDT.Trim() + "' group by [StationID],Date,Time )A group by[StationID]";

                    DataSet dsMonTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsMonTotal = ObjDB.FetchDataset(SqlMonTotal, "web");
                        if (dsMonTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsMonTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.MonthTotal = dsMonTotal.Tables[0].Rows[0][1].ToString();
                    }

                    //Year Total...
                    string YearStartDT = Convert.ToDateTime(CurrDT).Year + "-" + "01" + "-" + "01";

                    string SqlYearTotal = @"SELECT[StationID], SUM([MAXHOUR]) as CummulativeRain FROM (
                     SELECT[StationID], Date,Time,SUM(try_cast(RTRIM(LTRIM(replace([Hourly Rainfall], ''' + + ''', ''''))) as decimal(18,2))) AS[MAXHOUR]  FROM tbl_StationData_" + StID.Trim() + " WITH (NOLOCK) where CAST([Date] AS datetime) + CAST([Time] AS datetime) >= '" + YearStartDT.Trim() + "' and CAST([Date] AS datetime) + CAST([Time] AS datetime) <= '" + CurrDT.Trim() + "' group by [StationID],Date,Time )A group by[StationID]";

                    DataSet dsYearTotal = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsYearTotal = ObjDB.FetchDataset(SqlYearTotal, "web");
                        if (dsYearTotal.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsYearTotal.Tables[0].Rows.Count > 0)
                    {
                        Rdetail.YearTotal = dsYearTotal.Tables[0].Rows[0][1].ToString();
                    }


                }

                //===============================END Calculate Month Total & Year Total========================================= 


                #region RAIN STORM
                var CRDate = DateTime.Now.ToString("yyyy-MM-dd");
                string SelectDB = "";
                if (Profile == "VMC-AWS-GUJ")
                {
                    SelectDB = "Select StationID,Date,Time,[15mins RainFALL] From tbl_StationData_" + StID.Trim() + " Where Date = '" + CRDate.Trim() + "' order by Date asc, Time asc";
                }
                else if (Profile == "VMC-NHP-GUJ")
                {
                    SelectDB = "Select StationID,Date,Time,[Hourly Rainfall] From tbl_StationData_" + StID.Trim() + " Where Date = '" + CRDate.Trim() + "' order by Date asc, Time asc";
                }
                var dsHRain = ObjDB.FetchDataset(SelectDB, "web");

                if (dsHRain.Tables[0].Rows.Count > 0)
                {
                    double finalHRVal = 0.0;
                    string StromVal = string.Empty;
                    string stromDuration = string.Empty;
                    string startDTStrom = "";
                    string endDTStorm = "";

                    bool flgStartDT = false;

                    int CntZeroHRain = 0;

                    DateTime dTmStartStom = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

                    for (int r = 0; r < dsHRain.Tables[0].Rows.Count; r++)
                    {
                        #region 15 min summation remove logic is delete
                        double HRain = 000.0;
                        if (Profile == "VMC-NHP-GUJ")
                        {
                            string stHRain = funRemoveSummationHR(dsHRain, StID, r); // <-- dt.Rows[r][HR] -- 15 Min summation remove

                            if (stHRain.Length != 0)
                            {
                                HRain = Convert.ToDouble(stHRain);
                            }
                        }
                        //string stHRain = funRemoveSummationHR(dsHRain, StID, r); // <-- dt.Rows[r][HR] -- 15 Min summation remove
                        else
                        {
                            HRain = Convert.ToDouble(dsHRain.Tables[0].Rows[r]["15mins RainFALL"]);
                        }
                        #endregion

                        if (HRain >= 000.5)
                        {
                            if (flgStartDT == false)
                            {
                                startDTStrom = dsHRain.Tables[0].Rows[r][1] + " " + dsHRain.Tables[0].Rows[r][2];
                                dTmStartStom = Convert.ToDateTime(startDTStrom).AddMinutes(-15);

                                flgStartDT = true;
                            }
                            //else
                            //endDTStorm = dt.Rows[r][Date] + " " +  dt.Rows[r][Time];


                        }
                        else
                        {
                            CntZeroHRain++;
                        }

                        endDTStorm = dsHRain.Tables[0].Rows[r][1] + " " + dsHRain.Tables[0].Rows[r][2];
                        finalHRVal = finalHRVal + HRain;

                        if (CntZeroHRain == 96)
                        {

                            dTmStartStom = Convert.ToDateTime(DateTime.Now.ToString("0000-00-00"));
                            StromVal = "--";
                            stromDuration = "--";
                        }
                        else
                        {

                            TimeSpan timeDifference = new TimeSpan(0, 00, 0);

                            timeDifference = Convert.ToDateTime(endDTStorm) - dTmStartStom;


                            int days = timeDifference.Days;
                            double hours = timeDifference.TotalHours;
                            //double StromDuration = timeDifference.TotalDays;
                            //stromDurastion   = diff (0 Days 0 Hours)
                            StromVal = finalHRVal.ToString("F2");
                            stromDuration = days.ToString() + " Days " + hours.ToString() + " Hours";

                        }
                    }

                    Rdetail.StormValue = StromVal;
                    Rdetail.StormStartDateTime = dTmStartStom.ToString("dd-MM-yyyy HH:mm");
                    Rdetail.StormDuration = stromDuration;
                }

                LstRainDetail.Add(Rdetail);

                //========================END Calculate RainStorm==================================================================


                #endregion RAIN STROM



                return LstRainDetail;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                return LstRainDetail;
            }


        }
        public string funRemoveSummationHR(DataSet dsRainSR, string StationID, int RowIndex)
        {
            strFunctionName = "Remove 15 min Summation";
            string finalHR = "";

            try
            {
                string CurrTm = dsRainSR.Tables[0].Rows[RowIndex]["Time"].ToString();
                string PreTm = "";
                double DiffHR = 0;

                if (CurrTm != "00:00" && !CurrTm.Contains(":15"))
                {
                    DateTime PreviousTm = Convert.ToDateTime(CurrTm).Subtract(new TimeSpan(0, 15, 0));
                    PreTm = PreviousTm.ToString("HH:mm");

                    string PreHRSelQry = "select Date,Time,[Hourly Rainfall] from tbl_StationData_" + StationID.Trim() + " with (nolock) Where Date = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' and Time = '" + PreTm + "'";
                    //string PreHRSelQry = "select Date,Time,[Hourly Rainfall] from tbl_StationData_" + StationID.Trim() + " with (nolock) Where Date = '2023-03-16' and Time = '" + PreTm + "'";

                    DataSet dsPreHR = null;
                    for (int p = 0; p < 3; p++)
                    {
                        dsPreHR = ObjDB.FetchDataset(PreHRSelQry, "web");
                        if (dsPreHR.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    DiffHR = Convert.ToDouble(dsRainSR.Tables[0].Rows[RowIndex]["Hourly Rainfall"]) - Convert.ToDouble(dsPreHR.Tables[0].Rows[0]["Hourly Rainfall"]);

                    if (DiffHR <= 0)
                        DiffHR = 000.0;

                    finalHR = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                }
                else if (CurrTm.Contains(":15"))
                {
                    DiffHR = Convert.ToDouble(dsRainSR.Tables[0].Rows[RowIndex]["Hourly Rainfall"]);
                    finalHR = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                }
                else if (CurrTm == "00:00")
                {
                    DateTime PreDT = Convert.ToDateTime(dsRainSR.Tables[0].Rows[RowIndex]["Date"].ToString());
                    string finalPreDT = PreDT.ToString("yyyy-MM-dd");

                    //string finalPreDT = "2023-03-15";
                    string fetchQry = "select Top 1 Date,Time, [Hourly Rainfall] from tbl_StationData_" + StationID.Trim() + " with(nolock) where Date = '" + finalPreDT + "' order by Time desc";

                    DataSet dsHR = null;

                    for (int h = 0; h < 3; h++)
                    {
                        dsHR = ObjDB.FetchDataset(fetchQry, "web");
                        if (dsHR.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    DiffHR = Convert.ToDouble(dsRainSR.Tables[0].Rows[RowIndex]["Hourly Rainfall"]) - Convert.ToDouble(dsHR.Tables[0].Rows[0]["Hourly Rainfall"]);

                    if (DiffHR <= 0)
                        DiffHR = 000.0;

                    finalHR = DiffHR == 0 ? "000.0" : String.Format("{0:000.0}", DiffHR);
                }
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);
            }

            return finalHR;
        }

        #endregion  RainSummary


        [HttpPost]
        [Route("signup")]
        public async Task<HttpResponseMessage> SignUp()
        {
            bool FlgIns = false;

            strFunctionName = "Get User informatiom";

            try
            {
                var objStation = await Request.Content.ReadAsFormDataAsync();

                string Unm = Regex.Unescape(objStation.Get("Username"));
                string Fnm = Regex.Unescape(objStation.Get("FirstName"));
                string Lnm = Regex.Unescape(objStation.Get("LastName"));
                string OrganisationName = Regex.Unescape(objStation.Get("OrganisationName"));
                string MobileNumber = Regex.Unescape(objStation.Get("MobileNumber"));

                if (objStation != null)
                {
                    string InsQry = @"INSERT INTO [dbo].[tbl_User]
                                   ([FirstName]
                                   ,[LastName]
                                   ,[EmailID]
                                   ,[PhoneNumber]
                                   ,[Address]
                                   ,[Username]
                                   ,[Password]
                                   ,[RoleID]
                                   ,[IsActive]
                                   ,[IsAdmin]
                                   ,[CreatedDate]
                                   ,[CreatedBy]
                                   ,[UpdatedDate]
                                   ,[UpdatedBy]
                                   ,[IsDeleted]
                                   ,[IsSuperAdmin]
                                   ,[RoleName]
                                   ,[Remarks]
                                   ,[Admin]
                                   ,[VendorLogin]
                                   ,[OrganisationName])
                                    VALUES  ('" + Fnm.Trim() + "','" + Lnm.Trim() + "','" + Unm.Trim() + "','" + null + "','" + null + "','" + Unm.Trim() + "','" + null + "','" + null + "',0,0,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + null + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + null + "',0,0,'" + null + "','" + null + "','" + null + "','" + null + "','" + OrganisationName.Trim() + "')";

                    FlgIns = ObjDB.InsertIntoDb(InsQry, "web");

                }

                if (FlgIns == true)
                {
                    clsJsonSignUp clsJsonSignUp = new clsJsonSignUp();
                    clsJsonSignUp.code = Convert.ToInt32(HttpStatusCode.OK);
                    clsJsonSignUp.message = "success";
                    clsJsonSignUp.status = true;
                    clsJsonSignUp.data = "Registration request received, you will get the registration confirmation soon over email";

                    sendEmailViaWebAPi(Fnm, Lnm, Unm);

                    return Request.CreateResponse(HttpStatusCode.OK, clsJsonSignUp);
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                    ObjJsonResp.message = "internal server error";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                }
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "internal server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
            }
        }

        public void sendEmailViaWebAPi(string Fnm, string Lnm, string Unm)
        {
            using (MailMessage mm = new MailMessage("hydrometregistration@azistaaerospace.com", "hydrometregistration@azistaaerospace.com"))
            {
                string textBody = "User wants to register into hydrometlink app which having Username like " + Unm.Trim() + " and FirstName like " + Fnm.Trim() + " and LastName like " + Lnm.Trim() + "";

                mm.Subject = "Regarding New Hydrometlink App Registration Request";
                mm.Body = textBody;
                mm.IsBodyHtml = true;
                mm.CC.Add("vikas.dwivedi@azistaaerospace.com");
                //mm.Bcc.Add("niraj.shah@azistaaerospace.com");

                using (SmtpClient smtp = new SmtpClient("smtphhc.Logix.in", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("hydrometregistration@azistaaerospace.com", "Az1@azista");
                    smtp.EnableSsl = true;
                    smtp.Send(mm);
                }
            }
        }

        [HttpPost]
        [Route("signin")]
        public async Task<HttpResponseMessage> SignIn()
        {
            strFunctionName = "Sign In - Login User";


            bool FlgUpd = false;

            try
            {
                var objUser = await Request.Content.ReadAsFormDataAsync();
                string Unm = Regex.Unescape(objUser.Get("Username"));
                string Pwd = Regex.Unescape(objUser.Get("Password"));

                if (objUser != null)
                {
                    DataTable dtEmailID = ObjDB.FetchDataTable("select * from tbl_User with(nolock) where Username = '" + Unm.Trim() + "'", "web");

                    if (dtEmailID.Rows.Count > 0)
                    {
                        FlgUpd = ObjDB.UpdateIntoDb("update tbl_User set Password = '" + Pwd.Trim() + "' where Username = '" + Unm.Trim() + "'", "web");

                        if (FlgUpd == true)
                        {
                            clsJsonSignUp clsJsonSignUp = new clsJsonSignUp();
                            clsJsonSignUp.code = Convert.ToInt32(HttpStatusCode.OK);
                            clsJsonSignUp.message = "success";
                            clsJsonSignUp.status = true;
                            clsJsonSignUp.data = "Sign in successfully....";
                            return Request.CreateResponse(HttpStatusCode.OK, clsJsonSignUp);
                        }
                        else
                        {
                            clsJsonFail ObjJsonResp = new clsJsonFail();
                            ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                            ObjJsonResp.message = "Sign in fail....";
                            ObjJsonResp.status = false;
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                        }

                    }
                    else
                    {
                        clsJsonFail ObjJsonResp = new clsJsonFail();
                        ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                        ObjJsonResp.message = "User not exists...please register first";
                        ObjJsonResp.status = false;
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                    }
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                    ObjJsonResp.message = "internal server error";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                }
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "internal server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
            }
        }

        [HttpPost]
        [Route("verifyuser")]
        public async Task<HttpResponseMessage> UserVerification()
        {
            try
            {
                var objUser = await Request.Content.ReadAsFormDataAsync();

                if (objUser != null)
                {
                    string Unm = Regex.Unescape(objUser.Get("Username"));

                    DataTable dtEmailID = ObjDB.FetchDataTable("select * from tbl_User with(nolock) where Username = '" + Unm.Trim() + "'", "web");

                    if (dtEmailID.Rows.Count > 0)
                    {
                        clsJsonSignUp clsJsonSignUp = new clsJsonSignUp();
                        clsJsonSignUp.code = Convert.ToInt32(HttpStatusCode.OK);
                        clsJsonSignUp.message = "success";
                        clsJsonSignUp.status = true;
                        clsJsonSignUp.data = "User Exists...";
                        return Request.CreateResponse(HttpStatusCode.OK, clsJsonSignUp);
                    }
                    else
                    {
                        clsJsonFail ObjJsonResp = new clsJsonFail();
                        ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                        ObjJsonResp.message = "User not exists...";
                        ObjJsonResp.status = false;
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                    }
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                    ObjJsonResp.message = "internal server error";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                }
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "internal server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
            }

        }

        [HttpPost]
        [Route("forgotpassword")]
        public async Task<HttpResponseMessage> ForgotPassword()
        {
            try
            {
                var objPassword = await Request.Content.ReadAsFormDataAsync();

                if (objPassword != null)
                {
                    string Unm = Regex.Unescape(objPassword.Get("Username"));
                    string pwd = Regex.Unescape(objPassword.Get("Password"));

                    string UpdQry = "update tbl_User set Password = '" + pwd.Trim() + "' where Username = '" + Unm.Trim() + "'";
                    bool FlgUpd = ObjDB.UpdateIntoDb(UpdQry, "web");

                    if (FlgUpd == true)
                    {
                        clsJsonSignUp clsJsonSignUp = new clsJsonSignUp();
                        clsJsonSignUp.code = Convert.ToInt32(HttpStatusCode.OK);
                        clsJsonSignUp.message = "success";
                        clsJsonSignUp.status = true;
                        clsJsonSignUp.data = "Password Reset Successfully...";
                        return Request.CreateResponse(HttpStatusCode.OK, clsJsonSignUp);
                    }
                    else
                    {
                        clsJsonFail ObjJsonResp = new clsJsonFail();
                        ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                        ObjJsonResp.message = "internal server error";
                        ObjJsonResp.status = false;
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                    }
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                    ObjJsonResp.message = "internal server error";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                }
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "internal server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
            }

        }

        [HttpPost]
        [Route("resetpassword")]
        public async Task<HttpResponseMessage> ResetPassword()
        {
            strFunctionName = "Verify User - Reset Password";

            try
            {
                var objUser = await Request.Content.ReadAsFormDataAsync();

                if (objUser != null)
                {
                    string Unm = Regex.Unescape(objUser.Get("Username"));
                    string pwd = Regex.Unescape(objUser.Get("Password"));

                    DataTable dtEmailID = null;
                    for (int i = 0; i < 3; i++)
                    {
                        dtEmailID = ObjDB.FetchDataTable("select * from tbl_User with(nolock) where Username = '" + Unm.Trim() + "'", "web");
                        if (dtEmailID != null)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dtEmailID.Rows.Count > 0)
                    {
                        //User Exists.....
                        string UpdQry = "update tbl_User set Password = '" + pwd.Trim() + "' where Username = '" + Unm.Trim() + "'";

                        bool FlgUpd = ObjDB.UpdateIntoDb(UpdQry, "web");

                        if (FlgUpd == true)
                        {
                            clsJsonSignUp clsJsonSignUp = new clsJsonSignUp();
                            clsJsonSignUp.code = Convert.ToInt32(HttpStatusCode.OK);
                            clsJsonSignUp.message = "success";
                            clsJsonSignUp.status = true;
                            clsJsonSignUp.data = "Password Reset Successfully...";
                            return Request.CreateResponse(HttpStatusCode.OK, clsJsonSignUp);
                        }
                        else
                        {
                            clsJsonFail ObjJsonResp = new clsJsonFail();
                            ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                            ObjJsonResp.message = "internal server error";
                            ObjJsonResp.status = false;
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                        }
                    }
                    else
                    {
                        clsJsonFail ObjJsonResp = new clsJsonFail();
                        ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                        ObjJsonResp.message = "User not exists...";
                        ObjJsonResp.status = false;
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                    }
                }
                else
                {
                    clsJsonFail ObjJsonResp = new clsJsonFail();
                    ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                    ObjJsonResp.message = "internal server error";
                    ObjJsonResp.status = false;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
                }
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                clsJsonFail ObjJsonResp = new clsJsonFail();
                ObjJsonResp.code = Convert.ToInt32(HttpStatusCode.InternalServerError);
                ObjJsonResp.message = "internal server error";
                ObjJsonResp.status = false;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ObjJsonResp);
            }
        }

        [HttpGet]
        [Route("signout")]
        public async Task<HttpResponseMessage> SignOut()
        {
            clsJsonSignUp clsJsonSignUp = new clsJsonSignUp();
            clsJsonSignUp.code = Convert.ToInt32(HttpStatusCode.OK);
            clsJsonSignUp.message = "success";
            clsJsonSignUp.status = true;
            clsJsonSignUp.data = "success";
            return Request.CreateResponse(HttpStatusCode.OK, clsJsonSignUp);
        }

        public static GeoCoordinate GetCentralGeoCoordinate(IList<GeoCoordinate> geoCoordinates)
        {
            if (geoCoordinates.Count == 1)
            {
                return geoCoordinates.Single();
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var geoCoordinate in geoCoordinates)
            {
                var latitude = geoCoordinate.Latitude * Math.PI / 180;
                var longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = geoCoordinates.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new GeoCoordinate(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
        }

        [HttpPost]
        [Route("webuistationdata")]
        public async Task<HttpResponseMessage> WebUIStationData()
        {

            strFunctionName = "WEB UI - Fetch Station Data a/c to selected duration";

            var objStation = await Request.Content.ReadAsFormDataAsync();

            List<clsStationDetail> lstStationData = new List<clsStationDetail>();

            try
            {
                if (objStation != null)
                {
                    string StID = Regex.Unescape(objStation.Get("StationID"));
                    string frDT = (Regex.Unescape(objStation.Get("fromDate")) == null || Regex.Unescape(objStation.Get("fromDate")) == "") ? "" : Regex.Unescape(objStation.Get("fromDate"));
                    string toDT = (Regex.Unescape(objStation.Get("toDate")) == null || Regex.Unescape(objStation.Get("toDate")) == "") ? "" : Regex.Unescape(objStation.Get("toDate"));
                    string status = Regex.Unescape(objStation.Get("Status"));

                    string selqry = @"select pm.Name,pm.SensorName,srm.unit,srm.ValidationString,sm.Name as StationName,sm.District,sm.ShowInGraph,sm.ShowInGrid from [tbl_StationMaster ] sm join tbl_StationRangeValidation srm on
                    sm.Profile=srm.ProfileName join tbl_ProfileMaster pm on sm.Profile = pm.Name where sm.StationID='" + StID.Trim() + "'";

                    string stUnit = string.Empty;
                    string ProfileName = string.Empty;
                    string StationName = string.Empty;
                    string Location = string.Empty;
                    List<string> units = new List<string>();

                    DataSet dsUnit = null;
                    for (int j = 0; j < 3; j++)
                    {
                        dsUnit = ObjDB.FetchDataset(selqry, "Web");
                        if (dsUnit.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsUnit.Tables[0].Rows.Count > 0)
                    {
                        ProfileName = dsUnit.Tables[0].Rows[0]["Name"].ToString();
                        StationName = dsUnit.Tables[0].Rows[0]["StationName"].ToString();
                        Location = dsUnit.Tables[0].Rows[0]["District"].ToString();
                        string[] displaypara = dsUnit.Tables[0].Rows[0]["ShowInGraph"].ToString().Split(',');
                        string[] sname = dsUnit.Tables[0].Rows[0]["SensorName"].ToString().Split(',');
                        units = dsUnit.Tables[0].Rows[0]["unit"].ToString().Split(',').ToList();

                        for (int u = 0; u < displaypara.Length; u++)
                        {
                            if (units[u].Trim() == "C")
                            {
                                units[u] = "°C";
                            }
                            else if (units[u].Trim().ToLower().Contains("deg"))
                                units[u] = "◦";

                            if (displaypara[u].Trim() == "1")
                            {
                                stUnit += units[u] + ",";
                            }
                        }
                    }

                    //Last24Hour Data.....
                    DataSet dsSTData = null;

                    for (int j = 0; j < 3; j++)
                    {
                        dsSTData = ObjDB.sp_getTotalBurst("rpt_DataReport", StID, frDT, toDT, "Web");

                        if (dsSTData.Tables[0].Columns.Contains("Peripheral Status"))
                            dsSTData.Tables[0].Columns.Remove("Peripheral Status");

                        if (dsSTData.Tables[0].Columns.Contains("Created Date"))
                            dsSTData.Tables[0].Columns.Remove("Created Date");


                        if (dsSTData.Tables.Count > 0)
                            break;
                        Thread.Sleep(1000);
                    }

                    if (dsSTData.Tables.Count > 0)
                    {

                        if (dsSTData.Tables[0].Rows.Count > 0)
                        {
                            string[] finalUnit = stUnit.Split(',');

                            List<string> columnNameList = dsSTData.Tables[0].Columns.Cast<DataColumn>().Where(x => x.ColumnName != "Status" && x.ColumnName != "Created Date")
                                            .Select(x => x.ColumnName)
                                            .ToList();

                            string[] NewUnit = stUnit.TrimEnd(',').Split(',');

                            if (ProfileName.Trim() != "VMC-NHP-GUJ")
                            {
                                for (int s = 0; s < dsSTData.Tables[0].Rows.Count; s++)
                                {
                                    clsStationDetail stationDetail = new clsStationDetail();

                                    stationDetail.StationID = dsSTData.Tables[0].Rows[s]["StationID"].ToString();
                                    stationDetail.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                    stationDetail.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                    stationDetail.StationType = ProfileName;
                                    stationDetail.StationName = StationName;
                                    stationDetail.Location = Location;
                                    stationDetail.Status = "";

                                    List<clsParaDetail> lstparaDetails = new List<clsParaDetail>();

                                    for (int c = 3; c < dsSTData.Tables[0].Columns.Count; c++)
                                    {
                                        clsParaDetail paraDetail = new clsParaDetail();
                                        paraDetail.ParameterName = dsSTData.Tables[0].Columns[c].ToString();
                                        paraDetail.ParameterValue = dsSTData.Tables[0].Rows[s][c].ToString();
                                        paraDetail.ParameterUnit = (NewUnit[c] == "NA" ? "" : NewUnit[c]);
                                        lstparaDetails.Add(paraDetail);

                                    }

                                    stationDetail.ParaDetails = lstparaDetails;
                                    lstStationData.Add(stationDetail);
                                }


                            }

                            else if (ProfileName.Trim() == "VMC-NHP-GUJ")
                            {
                                string[] columnNames = dsSTData.Tables[0].Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                                List<string> finalcolList = columnNames.ToList();

                                finalcolList.Add("Dew Point");
                                finalcolList.Add("Wind Run");
                                finalcolList.Add("Wind Chill");
                                finalcolList.Add("Heat Index");
                                finalcolList.Add("THW Index");

                                //"Dew Point","Wind Run","Wind Chill","Heat Index","THW Index"
                                stUnit += "°C,m,(kgcal/m2/h),°C,°C";

                                List<string> NewUnit1 = stUnit.Split(',').ToList();

                                DataTable reportDT = funVMCDataSet(StID, frDT, toDT, status);

                                if (reportDT.Rows.Count > 0)
                                {
                                    for (int s = 0; s < reportDT.Rows.Count - 4; s++)
                                    {
                                        clsStationDetail stationDetail = new clsStationDetail();

                                        stationDetail.StationID = reportDT.Rows[s]["StationID"].ToString();
                                        stationDetail.Date = reportDT.Rows[s]["Date"].ToString();
                                        stationDetail.Time = reportDT.Rows[s]["Time"].ToString();
                                        stationDetail.StationType = ProfileName;
                                        stationDetail.StationName = StationName;
                                        stationDetail.Location = Location;
                                        stationDetail.Status = "";

                                        List<clsParaDetail> lstparaDetails = new List<clsParaDetail>();

                                        for (int c = 3; c < reportDT.Columns.Count; c++)
                                        {
                                            clsParaDetail paraDetail = new clsParaDetail();
                                            paraDetail.ParameterName = reportDT.Columns[c].ToString();
                                            paraDetail.ParameterValue = reportDT.Rows[s][c].ToString();
                                            paraDetail.ParameterUnit = (NewUnit1[c] == "NA" ? "" : NewUnit1[c]);
                                            lstparaDetails.Add(paraDetail);
                                        }

                                        stationDetail.ParaDetails = lstparaDetails;
                                        lstStationData.Add(stationDetail);
                                    }
                                }

                            }

                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, "No Data Exists");
                            }
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "No Data Exists");
                    }

                }

                return Request.CreateResponse(HttpStatusCode.OK, lstStationData);
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                return Request.CreateResponse(HttpStatusCode.OK, lstStationData);
            }

        }

        [HttpPost]
        [Route("webuistationdataap")]

        public async Task<HttpResponseMessage> WebUIStationDataAP()
        {

            strFunctionName = "WEB UI - Fetch Stations Data a/c to selected duration";

            var objStation = await Request.Content.ReadAsFormDataAsync();

            List<clsStationDetail> lstStationData = new List<clsStationDetail>();

            try
            {
                if (objStation != null)
                {
                    string StID = Regex.Unescape(objStation.Get("StationID"));
                    string frDT = (Regex.Unescape(objStation.Get("fromDate")) == null || Regex.Unescape(objStation.Get("fromDate")) == "") ? "" : Regex.Unescape(objStation.Get("fromDate"));
                    string toDT = (Regex.Unescape(objStation.Get("toDate")) == null || Regex.Unescape(objStation.Get("toDate")) == "") ? "" : Regex.Unescape(objStation.Get("toDate"));
                    string status = Regex.Unescape(objStation.Get("Status"));
                    List<string> stringList = new List<string>();


                    string[] array = StID.Split(',');


                    stringList.AddRange(array);

                    for (int i = 0; i < stringList.Count; i++)
                    {
                        string selqry = @"select pm.Name,pm.SensorName,srm.unit,srm.ValidationString,sm.Name as StationName,sm.District,sm.ShowInGraph,sm.ShowInGrid from [tbl_StationMaster ] sm join tbl_StationRangeValidation srm on
                    sm.Profile=srm.ProfileName join tbl_ProfileMaster pm on sm.Profile = pm.Name where sm.StationID='" + stringList[i].Trim() + "'";

                        string stUnit = string.Empty;
                        string ProfileName = string.Empty;
                        string StationName = string.Empty;
                        string Location = string.Empty;
                        List<string> units = new List<string>();

                        DataSet dsUnit = null;
                        for (int j = 0; j < 3; j++)
                        {
                            dsUnit = ObjDB.FetchDataset(selqry, "Web");
                            if (dsUnit.Tables.Count > 0)
                                break;
                            Thread.Sleep(1000);
                        }

                        if (dsUnit.Tables[0].Rows.Count > 0)
                        {
                            ProfileName = dsUnit.Tables[0].Rows[0]["Name"].ToString();
                            StationName = dsUnit.Tables[0].Rows[0]["StationName"].ToString();
                            Location = dsUnit.Tables[0].Rows[0]["District"].ToString();
                            string[] displaypara = dsUnit.Tables[0].Rows[0]["ShowInGraph"].ToString().Split(',');
                            string[] sname = dsUnit.Tables[0].Rows[0]["SensorName"].ToString().Split(',');
                            units = dsUnit.Tables[0].Rows[0]["unit"].ToString().Split(',').ToList();

                            for (int u = 0; u < displaypara.Length; u++)
                            {
                                if (units[u].Trim() == "C")
                                {
                                    units[u] = "°C";
                                }
                                else if (units[u].Trim().ToLower().Contains("deg"))
                                    units[u] = "◦";

                                if (displaypara[u].Trim() == "1")
                                {
                                    stUnit += units[u] + ",";
                                }
                            }
                        }

                        //Last24Hour Data.....
                        DataSet dsSTData = null;

                        for (int j = 0; j < 3; j++)
                        {
                            dsSTData = ObjDB.FetchData_GenericStation("[AWSAPI].[GenericPastStationDataAP]", stringList[i].Trim(), frDT, toDT, status, "Web");
                            if (dsSTData.Tables.Count > 0)
                                break;
                            Thread.Sleep(1000);
                        }
                        for (int j = dsSTData.Tables[0].Rows.Count - 1; j >= 0; j--)
                        {
                            DataRow row = dsSTData.Tables[0].Rows[j];
                            if (row["Status"].Equals("Delhi"))
                            {
                                dsSTData.Tables[0].Rows.RemoveAt(j);
                            }
                        }
                        if (dsSTData.Tables.Count > 0)
                        {

                            if (dsSTData.Tables[0].Rows.Count > 0)
                            {
                                string[] finalUnit = stUnit.Split(',');

                                List<string> columnNameList = dsSTData.Tables[0].Columns.Cast<DataColumn>().Where(x => x.ColumnName != "Status" && x.ColumnName != "Created Date")
                                                .Select(x => x.ColumnName)
                                                .ToList();

                                string[] NewUnit = stUnit.TrimEnd(',').Split(',');

                                if (ProfileName.Contains("NHPAP"))
                                {
                                    for (int s = 0; s < dsSTData.Tables[0].Rows.Count; s++)
                                    {

                                        clsStationDetail stationDetail = new clsStationDetail();

                                        stationDetail.StationID = dsSTData.Tables[0].Rows[s]["StationID"].ToString();
                                        stationDetail.Date = dsSTData.Tables[0].Rows[s]["Date"].ToString();
                                        stationDetail.Time = dsSTData.Tables[0].Rows[s]["Time"].ToString();
                                        stationDetail.StationType = ProfileName;
                                        stationDetail.StationName = StationName;
                                        stationDetail.Location = Location;
                                        stationDetail.Status = "";

                                        List<clsParaDetail> lstparaDetails = new List<clsParaDetail>();

                                        for (int c = 3; c < columnNameList.Count; c++)
                                        {
                                            clsParaDetail paraDetail = new clsParaDetail();
                                            paraDetail.ParameterName = dsSTData.Tables[0].Columns[c].ToString();
                                            paraDetail.ParameterValue = dsSTData.Tables[0].Rows[s][c].ToString();
                                            paraDetail.ParameterUnit = (NewUnit[c] == "NA" ? "" : NewUnit[c]);
                                            lstparaDetails.Add(paraDetail);

                                        }

                                        stationDetail.ParaDetails = lstparaDetails;
                                        lstStationData.Add(stationDetail);
                                    }


                                }



                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.OK, "No Data Exists");
                                }
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "No Data Exists");
                        }
                    }


                }

                return Request.CreateResponse(HttpStatusCode.OK, lstStationData);
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);

                return Request.CreateResponse(HttpStatusCode.OK, lstStationData);
            }

        }
    }
}
