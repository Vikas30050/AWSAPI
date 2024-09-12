using AWSAPI.DB;
using AWSAPI.Model;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using static AWSAPI.Model.SiteVisit2AWLR;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace AWSAPI.Controller
{

   
    public class ReportControllerold : ApiController
    {
        AWSEntities db = new AWSEntities();

        public string strModuleName = "Report API Data Module";
        public string strFunctionName = "";
        public string strExceptionMessage = "";
        public string strLogMessage = "";


        [Route("api/report/vmcdatareport")]
        [HttpPost]
        public async Task<IHttpActionResult> VMCDataReport()
        {
            var objStation = await Request.Content.ReadAsFormDataAsync();
            List<clsVMCDataReport> jsonData = new List<clsVMCDataReport>();
            List<clsResponseDataReport> jsonResp = new List<clsResponseDataReport>();

            try
            {
                if (objStation != null)
                {
                    string stationName = Regex.Unescape(objStation.Get("stationName"));
                    string fromDate = (Regex.Unescape(objStation.Get("fromDate")) == null || Regex.Unescape(objStation.Get("fromDate")) == "") ? "" : Regex.Unescape(objStation.Get("fromDate"));
                    string toDate = (Regex.Unescape(objStation.Get("toDate")) == null || Regex.Unescape(objStation.Get("toDate")) == "") ? "" : Regex.Unescape(objStation.Get("toDate"));

                    var toDateArray = DateTime.Parse(new string(toDate.Take(24).ToArray()));
                    var toDateString = toDateArray.ToString("yyyy-MM-dd");
                    var fromDateArray = DateTime.Parse(new string(fromDate.Take(24).ToArray()));
                    string fromDateString = fromDateArray.ToString("yyyy-MM-dd");

                    string fprofile = string.Empty;
                    string stationID = string.Empty;
                    string Colname = string.Empty;

                    var StationSql = db.tbl_StationMaster_.Where(x => x.Name == stationName.Trim()).FirstOrDefault();
                    if (StationSql != null)
                    {

                        stationID = StationSql.StationID;
                        fprofile = StationSql.Profile;
                    }

                    if (fprofile == "VMC-NHP-GUJ")
                    {

                        DateTime parsedFromDate = DateTime.Parse(fromDate);
                        DateTime parsedToDate = DateTime.Parse(toDate);

                        var result = db.tbl_VMCDataReport
                            .AsNoTracking()
                            .Where(x => x.StationID == stationID
                                        && Convert.ToDateTime(x.Date) >= parsedFromDate
                                        && Convert.ToDateTime(x.Date) <= parsedToDate)
                            .ToList();


                    }
                    else if (fprofile == "VMC-AWS-GUJ")
                    {

                    }


                  
                }
            }
            catch (Exception Ex)
            {

                Ex.ToString();
            }
            return Ok();
        }


        public string ColumnName(string stationID)
        {
            string ReportcolumnName = string.Empty;

            var id = stationID;
            var tableName = "tbl_StationData_" + id;
            DataSet columnDataset = null;
            //for (int c = 0; c < 3; c++)
            //{
            //    columnDataset = ObjDB.FetchData_SP_columnName("getColumnName", tableName, "WEB");
            //    if (columnDataset.Tables.Count > 0)
            //        break;
            //    Thread.Sleep(1000);
            //}

            db.getColumnName(tableName);

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
    }
}
