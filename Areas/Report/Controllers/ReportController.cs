
using AWSAPI.Areas.Report.Models;
using AWSAPI.HelperClass;
using AWSAPI.Model;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.UI.WebControls;
using System.Threading;


namespace AWSAPI.Areas.Report.Controllers
{
    [Route("api/report/")]
    public class ReportController : ApiController
    {
        AWSDatabaseContext dbContext = new AWSDatabaseContext();
        static clsDatabase ObjDB = new clsDatabase();
        static clsExceptionDataRoutine ObjEx = new clsExceptionDataRoutine();

        public string strModuleName = "Report API Data Module";
        public string strFunctionName = "";
        public string strExceptionMessage = "";
        public string strLogMessage = "";

        /*
        [HttpGet]
        public async Task<HttpResponseMessage> VMCDataReport1()
        {
            return Request.CreateResponse(HttpStatusCode.NoContent, "");
        }*/

        /*
        [Route("api/report/vmcdatareport")]
        [HttpGet]
        public async Task<HttpResponseMessage> VMCDataReportEF(VMCStationDataModel data)
        {

            List<clsVMCDataReport> jsonData = new List<clsVMCDataReport>();
            List<clsResponseDataReport> jsonResp = new List<clsResponseDataReport>();

            try
            {
                if (data != null)
                {
                    string stationName = data.stationName;
                    string fromDate = data.fromDate;
                    string toDate = data.toDate;

                    var toDateArray = DateTime.Parse(new string(toDate.Take(24).ToArray()));
                    var toDateString = toDateArray.ToString("yyyy-MM-dd");
                    var fromDateArray = DateTime.Parse(new string(fromDate.Take(24).ToArray()));
                    string fromDateString = fromDateArray.ToString("yyyy-MM-dd");

                    string fprofile = string.Empty;
                    string stationID = string.Empty;

                    var StationSql = await dbContext.tbl_StationMaster.Where(x => x.Name == stationName.Trim()).FirstOrDefaultAsync();
                    if (StationSql != null)
                    {
                        stationID = StationSql.StationID;
                        fprofile = StationSql.Profile;
                    }

                    if (fprofile == "VMC-NHP-GUJ")
                    {
                        DateTime parsedFromDate = DateTime.Parse(fromDate);
                        DateTime parsedToDate = DateTime.Parse(toDate);

                        var result = await dbContext.tbl_VMCDataReport
                            .AsNoTracking()
                            .Where(x => x.StationID == stationID
                                        && Convert.ToDateTime(x.Date) >= parsedFromDate
                                        && Convert.ToDateTime(x.Date) <= parsedToDate)

                            .Select(item => new clsResponseDataReport
                            {
                                StationID = item.StationID,
                                Date = item.Date,
                                Time = item.Time.Trim(),
                                Rainfall15mins = Convert.ToDouble(item.Hourly_Rainfall_mm_),
                                DailyRain = Convert.ToDouble(item.Daily_Rain_mm_),
                            })
                            .ToListAsync();

                        jsonResp.AddRange(result);

                    }
                    else if (fprofile == "VMC-AWS-GUJ")
                    {

                        DateTime parsedFromDate = DateTime.Parse(fromDate);
                        DateTime parsedToDate = DateTime.Parse(toDate);

                        var result = await dbContext.tbl_VMCAjwaDataReport
                          .AsNoTracking()
                          .Where(x => x.StationID == stationID
                                      && Convert.ToDateTime(x.Date) >= parsedFromDate
                                      && Convert.ToDateTime(x.Date) <= parsedToDate)

                          .Select(item => new clsResponseDataReport
                          {
                              StationID = item.StationID,
                              Date = item.Date,
                              Time = item.Time.Trim(),
                              Rainfall15mins = Convert.ToDouble(item.C15mins_RAINFALL_mm_),
                              DailyRain = Convert.ToDouble(item.Daily_Rain_mm_),
                          })
                          .ToListAsync();

                        jsonResp.AddRange(result);

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, jsonResp);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, jsonResp);
                }
            }
            catch (Exception Ex)
            {
                Ex.ToString();
                return Request.CreateResponse(HttpStatusCode.BadRequest, jsonResp);
            }

        }
        */


        [Route("api/report/vmcdatareport")]
        [HttpGet]
        public async Task<IHttpActionResult> VMCDataReport(string stationName, string fromDate, string toDate)
        {
            strFunctionName = "Fetch - VMC Data Report ";

            string json = string.Empty;
            List<clsVMCDataReport> jsonData = new List<clsVMCDataReport>();
            List<clsResponseDataReport> jsonResp = new List<clsResponseDataReport>();

            try
            {
                if (!string.IsNullOrEmpty(stationName) && !string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    var fromDateArray = DateTime.Parse(new string(fromDate.Trim().Take(24).ToArray()));
                    string fromDateString = fromDateArray.ToString("yyyy-MM-dd");
                    var toDateArray = DateTime.Parse(new string(toDate.Trim().Take(24).ToArray()));
                    var toDateString = toDateArray.ToString("yyyy-MM-dd");
                   
                    string fprofile = string.Empty;
                    string stationID = string.Empty;

                    string selQry = "select StationID,profile from tbl_StationMaster (nolock) where Name = '" + stationName.Trim() + "'";

                    DataTable dtStation = null;

                    for (int a = 0; a < 3; a++)
                    {
                        dtStation = ObjDB.FetchDataTable(selQry, "web");
                        if (dtStation != null &&  dtStation.Rows.Count > 0)
                                break;
                        Thread.Sleep(1000);
                    }

                    if (dtStation.Rows.Count > 0)
                    {
                        stationID = dtStation.Rows[0]["StationID"].ToString();
                        fprofile = dtStation.Rows[0]["profile"].ToString();

                        DataSet reportset = null;

                        if (fprofile == "VMC-NHP-GUJ")
                        {
                            string SelQry = "select StationID,Date,Time,[Hourly RainFall(mm)],[Daily Rain(mm)] from tbl_VMCDataReport (nolock) where Date between '" + fromDateString.Trim() + "' and '" + toDateString.Trim() + "' and StationID='" + stationID.Trim() + "' order by Date,Time";

                            for (int a = 0; a < 3; a++)
                            {
                                reportset = ObjDB.FetchDataset(SelQry, "Web");
                                if (reportset != null && reportset.Tables.Count > 0)
                                    break;
                                Thread.Sleep(1000);
                            }
                        }

                        else if (fprofile == "VMC-AWS-GUJ")
                        {
                            string SelQry = "select StationID,Date,Time,[15mins RAINFALL(mm)],[Daily Rain(mm)] from tbl_VMCAjwaDataReport (nolock) where Date between '" + fromDateString.Trim() + "' and '" + toDateString.Trim() + "' and StationID='" + stationID.Trim() + "' order by Date,Time";

                            for (int a = 0; a < 3; a++)
                            {
                                reportset = ObjDB.FetchDataset(SelQry, "Web");
                                if (reportset != null && reportset.Tables.Count > 0)
                                    break;
                                Thread.Sleep(1000);
                            }
                        }

                        if (reportset.Tables.Count > 0)
                        {
                            if (reportset.Tables[0].Columns.Contains("Hourly RainFall(mm)"))
                            {
                                reportset.Tables[0].Columns["Hourly RainFall(mm)"].ColumnName = "15mins RAINFALL(mm)";
                            }

                            if (reportset.Tables[0].Rows.Count > 0)
                            {

                                //Remove Space from Time Column....Efficient direct approach without LINQ
                                DataColumn timeColumn = reportset.Tables[0].Columns["Time"];

                                for (int i = 0; i < reportset.Tables[0].Rows.Count; i++)
                                {
                                    DataRow row = reportset.Tables[0].Rows[i];
                                    if (row[timeColumn] != DBNull.Value)
                                    {
                                        row[timeColumn] = row[timeColumn].ToString().Trim();
                                    }
                                }

                                DataTable reportdata = new DataTable();

                                if (reportset.Tables[0].Columns.Count != 0)
                                {
                                    reportdata = reportset.Tables[0];
                                    json = JsonConvert.SerializeObject(reportdata, new IsoDateTimeConverter() { DateTimeFormat = "dd-MMM-yyy" });
                                    jsonData = JsonConvert.DeserializeObject<List<clsVMCDataReport>>(json);

                                    foreach (var item in jsonData)
                                    {
                                        clsResponseDataReport modelData = new clsResponseDataReport();
                                        modelData.StationID = item.StationID;
                                        modelData.Date = item.Date;
                                        modelData.Time = item.Time.Trim();
                                        //modelData.BatteryVoltage = item.BatteryVoltage;
                                        modelData.Rainfall15mins = item.Rainfall15mins == null ? 0 : item.Rainfall15mins;
                                        modelData.DailyRain = item.DailyRain;
                                        /*modelData.AirTemperature = item.AirTemperature;
                                        modelData.WindSpeed = item.WindSpeed;
                                        modelData.WindDirection = item.WindDirection;
                                        modelData.AtmosphericPressure = item.AtmosphericPressure;
                                        modelData.Humidity = item.Humidity;
                                        modelData.DewPoint = item.DewPoint;
                                        modelData.WindRun = item.WindRun;
                                        modelData.WindChill = item.WindChill;
                                        modelData.HeatIndex = item.HeatIndex;
                                        modelData.THWIndex = item.THWIndex;
                                        modelData.RainRate = item.RainRate;
                                        modelData.HighSpeed = item.HighSpeed;
                                        modelData.HighDirection = item.HighDirection;*/
                                        jsonResp.Add(modelData);
                                    }
                                }

                            }

                        }
                    }
                }

                await Task.CompletedTask;

                string tmpExData = "";
                
                if (jsonResp != null && jsonResp.Count > 0)
                {
                    tmpExData = " Station : " + stationName + " From Date : " + fromDate + " To Date : " + toDate + " Data : " + jsonResp.Count;
                    bool ExcpMesg = ObjEx.WriteIntoExceptionFile(strModuleName, strFunctionName,tmpExData,clsGlobalData.strLogFileName, clsGlobalData.strLoc_LogFile);

                    return Ok(jsonResp);
                }
                else
                {
                    tmpExData = " Station : " + stationName + " From Date : " + fromDate + " To Date : " + toDate + " Data : No data found";
                    bool ExcpMesg = ObjEx.WriteIntoExceptionFile(strModuleName, strFunctionName,tmpExData, clsGlobalData.strLogFileName, clsGlobalData.strLoc_LogFile);
                    return BadRequest(tmpExData);
                }
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                string tmpExData = "";
                tmpExData = " Station : " + stationName + " From Date : " + fromDate + " To Date : " + toDate + Environment.NewLine;
                tmpExData += strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                bool ExcpMesg = ObjEx.WriteIntoExceptionFile(tmpExData);
                return Ok(jsonResp);
            }
        }








    }
}
