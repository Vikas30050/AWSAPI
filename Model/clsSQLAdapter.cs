using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Data.SqlClient;
using AWSAPI.HelperClass;

namespace AWS
{
    public class clsSQLAdapter
    {
        DataSet ds;
        SqlConnection con;
        SqlDataAdapter sda;
        SqlCommand cmd;
        SqlDataReader dr;
        clsExceptionDataRoutine clsEDR= new clsExceptionDataRoutine();

        public clsSQLAdapter()
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWSDatabaseContext"].ConnectionString);
            con.Open();
        }
       // public static clsExceptionDataRoutine clsEDR = new clsExceptionDataRoutine();
       // public static ClsFileOperationRoutines clsFile = new ClsFileOperationRoutines();
        public static string strModuleName = "SQL Adapter";
        public static string strFunctionName = "";
        public static string strExceptionMessage = "";
        public static string strLogMessage = "";
        public SqlDataReader selectlogin(string username, string password)
        {
            cmd = new SqlCommand("selectlogin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            dr = cmd.ExecuteReader();
            return dr;
        }
        public SqlDataReader getColumnName(string tableName)
        {
            strFunctionName = "getColumnName";
            try
            {
                cmd = new SqlCommand("getColumnName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableName", tableName);
                dr = cmd.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                strExceptionMessage = ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;
                //bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                throw;

            }
        }
        public DataSet sp_getTotalBurst(string spname, string StationID, string fromDate, string toDate)
        {
            strFunctionName = "getColumnName";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            try
            {
                cmd = new SqlCommand(spname, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Profile", StationID);
                cmd.Parameters.AddWithValue("@fromDate", fromDate);
                cmd.Parameters.AddWithValue("@toDate", toDate);
                dr = cmd.ExecuteReader();
                dt.Load(dr);
                ds.Tables.Add(dt);
                return ds;
            }
            catch (Exception ex)
            {
                strExceptionMessage = ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;
                //bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                //throw;
                return ds;

            }
        }
        public SqlDataReader selectfrontlogin(string username, string password)
        {
            cmd = new SqlCommand("selectfrontuser", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            dr = cmd.ExecuteReader();
            return dr;
        }
        public  DataSet sp_DataReport(string spname, string StationID, string FromDate, string ToDate)
        {
            DataSet ds = new DataSet();
            try
            {
                cmd = new SqlCommand(spname, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StationID", StationID);
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
                cmd.Parameters.AddWithValue("@ToDate", ToDate);

                sda = new SqlDataAdapter(cmd);
                ds = new DataSet();
                sda.Fill(ds);
                processcommand(cmd);
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                return ds;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;
                //bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                return ds;
            }
        }
        public SqlDataReader getMenuList(int userID)
        {
            strFunctionName = "getMenuList";
            try
            {
                cmd = new SqlCommand("sp_getMenu", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userID);
                dr = cmd.ExecuteReader();
                return dr;
            }
            catch(Exception ex)
            {
                strExceptionMessage = ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;
                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                throw;

            }
        }
        public DataSet selectdata(string str)
        {
            ds = new DataSet();
            try
            {
                sda = new SqlDataAdapter(str, con);

                sda.Fill(ds);
                return ds;
            }
            catch(Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;
                //bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                return ds;
            }
        }
      
        public DataSet fetchData(string spname,string TableName, string ColumnName, string UserID, string toDate, string fromDate)
        {
            strFunctionName = "Fetch Data From Database-1";
            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;
                try
                {

                    cmd = new SqlCommand(spname, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@TableName", TableName);
                    cmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@toDate", toDate);
                    cmd.Parameters.AddWithValue("@fromDate", fromDate);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                    processcommand(cmd);
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                        con.Close();
                       
                    }


                }
            }
        }
        public void UDI(string str)
        {
            cmd = new SqlCommand(str, con);
            cmd.CommandType = CommandType.Text;
            processcommand(cmd);
        }

        public void processcommand(SqlCommand cmd)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            cmd.CommandTimeout = 600;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
        public SqlDataReader checkavailability(string username)
        {
            cmd = new SqlCommand("SELECT * FROM tbl_user WHERE username = '" + username + "' ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@username", username);
            dr = cmd.ExecuteReader();
            return dr;
        }

        public SqlDataReader VehicleNumber(string number)
        {
            cmd = new SqlCommand("SELECT * FROM tbl_car_registration WHERE reg_no = '" + number + "' ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@reg_no", number);
            dr = cmd.ExecuteReader();
            return dr;
        }
    }
}
