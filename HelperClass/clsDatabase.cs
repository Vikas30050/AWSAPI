using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace AWSAPI.HelperClass
{
    public class clsDatabase
    {
        public string strModuleName = "Database Routines";

        public string strFunctionName = "";

        public string strExceptionMessage = "";

        string appath = Path.GetDirectoryName(HttpRuntime.AppDomainAppPath); //(Application.ExecutablePath);

        public SqlConnectionStringBuilder Sbu = new SqlConnectionStringBuilder();

        //public static SqlConnection Con = new SqlConnection(); 
        //public String GlobalConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DL_CNFG_SFT_DB;Integrated Security=True;Asynchronous Processing=true;MultipleActiveResultSets=True";
        //public String GlobalConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog="++"DL_CNFG_SFT_DB;Integrated Security=True;Asynchronous Processing=true;MultipleActiveResultSets=True";

        public String GlobalConnectionString = "";

        public static SqlConnection GlobalConnection = new SqlConnection();

        public static SqlConnection GlobalConnection_Web = new SqlConnection();

        public SqlConnectionStringBuilder Sbu_Web = new SqlConnectionStringBuilder();

        public bool OpenConnection()
        {
            strFunctionName = "Open Database Connection";

            lock (this)
            {
                try
                {
                    appath = Path.GetDirectoryName(HttpRuntime.AppDomainAppPath);  //(Application.ExecutablePath);
                    appath = @appath + "\\" + "App_Data\\ARSDB.mdf";

                    if (GlobalConnection.State.ToString().ToUpper() == "OPEN")
                    {
                        GlobalConnection.Close();
                    }

                    Sbu.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + appath + ";Integrated Security=True;Asynchronous Processing=true;MultipleActiveResultSets=True; Connect Timeout=300;";

                    //Sbu.ConnectionString = "Data Source=184.168.194.70;Initial Catalog=ph19324034461_;Persist Security Info=True;User ID=smbdb;Password=***********";

                    //Sbu.ConnectionString = "Data Source=azistatst.c7cwycb3wgvd.us-east-2.rds.amazonaws.com;Initial Catalog=myDb;Persist Security Info=True;User ID=awsazistaindustries;Password=azista123";

                    GlobalConnection.ConnectionString = Sbu.ConnectionString;

                    GlobalConnection.Open();

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;


                    return false;
                }
            }
        }

        public bool Connection_Web()
        {
            strFunctionName = "Web Database Connection";

            lock (this)
            {
                try
                {
                    if (GlobalConnection_Web.State.ToString().ToUpper() == "OPEN")
                    {
                        GlobalConnection_Web.Close();
                    }
                    //Sbu_Web.DataSource = @"AZISTAAERO040\SQLEXPRESS";
                    //Sbu_Web.DataSource = @"AZISTAAERO072";
                    //Sbu_Web.DataSource = @"3.13.189.69";
                    //Sbu_Web.DataSource = @"13.76.192.12,1433";
                    //Sbu_Web.DataSource = @"tcp:azistaproduction.database.windows.net,1433";
                    //Sbu_Web.DataSource = @"172.22.1.88,1433";
                    //Sbu_Web.DataSource = @"18.219.8.229";
                    //Sbu_Web.InitialCatalog = "AWS_Stage";
                    //Sbu_Web.InitialCatalog = "AWSTEMP2";

                    //Sbu_Web.DataSource = @"20.205.238.253,1433";

                    Sbu_Web.DataSource = @"20.204.8.104,1433";
                    Sbu_Web.InitialCatalog = "AWS";
                    Sbu_Web.IntegratedSecurity = false;
                    //Sbu_Web.IntegratedSecurity = true;
                    Sbu_Web.MultipleActiveResultSets = true;
                    //Sbu_Web.PersistSecurityInfo = false;
                    //Sbu_Web.ConnectTimeout = 0;
                    Sbu_Web.UserID = "sa";
                    //Sbu_Web.Password = "azista@1";
                    Sbu_Web.Password = "H3ter0Cloud@2020";

                    GlobalConnection_Web.ConnectionString = Sbu_Web.ConnectionString;
                    GlobalConnection_Web.Open();

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;


                    return false;
                }
            }
        }
        public bool OpenConnection_Web()
        {
            strFunctionName = "Open Web Database Connection";

            lock (this)
            {
                try
                {
                    if (GlobalConnection_Web.State.ToString().ToUpper() == "OPEN")
                    {
                        GlobalConnection_Web.Close();
                    }

                    //Sbu_Web.DataSource = @"AZISTAAERO040\SQLEXPRESS";
                    //Sbu_Web.DataSource = @"AZISTAAERO011\SQLEXPRESS";
                    //Sbu_Web.DataSource = @"SERVER2\SQLEXPRESS";
                    //Sbu_Web.DataSource = @"172.22.1.88,1433";
                    //Sbu_Web.DataSource = @"18.219.8.229";
                    //Sbu_Web.InitialCatalog = "myDb";
                    //Sbu_Web.InitialCatalog = "ATS";
                    //Sbu_Web.DataSource = @"13.76.192.12,1433";
                    //Sbu_Web.DataSource = @"3.13.189.69";
                    //Sbu_Web.DataSource = @"tcp:azistaproduction.database.windows.net,1433";
                    //Sbu_Web.InitialCatalog = "AWS_Stage";

                    //Sbu_Web.DataSource = @"20.205.238.253,1433";

                    Sbu_Web.DataSource = @"20.204.8.104,1433";
                    Sbu_Web.InitialCatalog = "AWS";

                    //Sbu_Web.IntegratedSecurity = false;
                    Sbu_Web.IntegratedSecurity = false;
                    //Sbu_Web.InitialCatalog = "Remote_AITS_DB";
                    //Sbu_Web.IntegratedSecurity = true;
                    Sbu_Web.MultipleActiveResultSets = true;
                    //Sbu_Web.PersistSecurityInfo = false;
                    //Sbu_Web.ConnectTimeout = 0;

                    Sbu_Web.UserID = "sa";
                    Sbu_Web.Password = "H3ter0Cloud@2020";
                    //Sbu_Web.Password = "azista@1";

                    GlobalConnection_Web.ConnectionString = Sbu_Web.ConnectionString;
                    GlobalConnection_Web.Open();

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;


                    return false;
                }
            }
        }

        public bool CreateTable(string query, string strConnType)
        {
            strFunctionName = "Update Into Database-1";

            lock (this)
            {
                SqlCommand CreateCmd = new SqlCommand();

                try
                {
                    //UpdtCmd.Connection = GlobalConnection;

                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSE")
                        {
                            OpenConnection();
                        }

                        CreateCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        CreateCmd.Connection = GlobalConnection_Web;
                    }

                    CreateCmd.CommandText = query;

                    CreateCmd.ExecuteNonQuery();

                    if (CreateCmd != null)
                    {
                        CreateCmd.Dispose();
                    }

                    return true;
                }
                catch (Exception Ex)
                {
                    if (CreateCmd != null)
                    {
                        CreateCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;



                    return false;
                }
                finally
                {
                    if (CreateCmd != null)
                    {
                        CreateCmd.Dispose();
                    }
                }
            }
        }
        public DataSet FetchData_Table(string spname, string strConnType)
        {
            strFunctionName = "Fetch Data From Store Procedure - I";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;

                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }
                    SelCmd.CommandText = spname;


                    da = new SqlDataAdapter(SelCmd);
                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    strExceptionMessage = Ex.Message;


                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                }
            }
        }
        public DataSet FetchData_SP(string spname, string strConnType)
        {
            strFunctionName = "Fetch Data From Store Procedure - I";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;

                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }
                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    da = new SqlDataAdapter(SelCmd);
                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    strExceptionMessage = Ex.Message;


                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                }
            }
        }

        public SqlDataReader FetchData(string query, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataReader SelRdr = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    //SelCmd.Connection = GlobalConnection;

                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection();
                        }

                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection_Web();
                        }

                        SelCmd.Connection = GlobalConnection_Web;
                    }

                    SelCmd.CommandText = query;

                    SelRdr = SelCmd.ExecuteReader();

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    /////As return "Reader" value
                    //if (SelRdr != null)
                    //{
                    //    SelRdr.Dispose();
                    //}

                    return SelRdr;
                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    /////As return "Reader" value
                    //if (SelRdr != null)
                    //{
                    //    SelRdr.Dispose();
                    //}

                    strExceptionMessage = Ex.Message;

                    // bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return SelRdr;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    /////As return "Reader" value
                    //if (SelRdr != null)
                    //{
                    //    SelRdr.Dispose();
                    //}
                }
            }
        }

        public SqlDataReader FetchData_Sdr(string spname, string tblname, string strConnType)
        {
            SqlDataReader dr = null;
            SqlCommand SelCmd = new SqlCommand();

            try
            {
                if (strConnType.Trim().ToUpper() == "LOCAL")
                {
                    Connection_Web();
                    SelCmd.Connection = GlobalConnection;
                }
                else if (strConnType.Trim().ToUpper() == "WEB")
                {
                    if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                    {
                        Connection_Web();
                    }
                    SelCmd.Connection = GlobalConnection_Web;

                    //Connection_Web();
                    //SelCmd.Connection = GlobalConnection_Web;
                }

                SelCmd.CommandText = spname;
                SelCmd.CommandType = CommandType.StoredProcedure;
                SelCmd.Parameters.AddWithValue("@TableName", tblname);
                dr = SelCmd.ExecuteReader();

                if (SelCmd != null)
                {
                    SelCmd.Dispose();
                }

                return dr;
            }
            catch (Exception Ex)
            {
                if (SelCmd != null)
                {
                    SelCmd.Dispose();
                }

                strExceptionMessage = Ex.Message;

                return dr;
            }
            finally
            {
                if (SelCmd != null)
                {
                    SelCmd.Dispose();
                }

            }
        }

        public DataSet FetchDataset(string query, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-2";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;
                    }

                    SelCmd.CommandText = query;

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    return ds;
                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                }
            }
        }

        public DataTable FetchDataTable(string query, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-2";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataTable dt = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;
                    }

                    SelCmd.CommandText = query;

                    da = new SqlDataAdapter(SelCmd);

                    dt = new DataTable();

                    da.Fill(dt);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    return dt;
                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    return dt;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                }
            }
        }

        public DataSet FetchData_SP_columnName(string spname, string tblname, int uid, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;

                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@TableName", tblname);
                    SelCmd.Parameters.AddWithValue("@UserID", uid);


                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }
        public DataSet FetchData_SP_columnName(string spname, string tblname, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;


                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@TableName", tblname);


                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }
        public DataSet FetchData_SP_WithoutPara(string spname, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    //SelCmd.Connection = GlobalConnection;

                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        //if (GlobalConnection.State.ToString().ToUpper() == "CLOSE")
                        //{
                        //    OpenConnection();
                        //}
                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection_Web();
                        }
                        //Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }
                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    da = new SqlDataAdapter(SelCmd);
                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    /////As return "Reader" value
                    //if (SelRdr != null)
                    //{
                    //    SelRdr.Dispose();
                    //}
                    return ds;
                    //return SelRdr;
                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    /////As return "Reader" value
                    //if (SelRdr != null)
                    //{
                    //    SelRdr.Dispose();
                    //}

                    strExceptionMessage = Ex.Message;

                    //bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    /////As return "Reader" value
                    //if (SelRdr != null)
                    //{
                    //    SelRdr.Dispose();
                    //}
                }
            }
        }
        public DataSet FetchData_SP_MonthlyWeeklyData(string spname, string tblname, string ColumnName, int uid, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;


                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@tb", tblname);
                    SelCmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    SelCmd.Parameters.AddWithValue("@UserID", uid);





                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }
        public DataSet FetchData_SP_MonthlyWeeklyData(string spname, string tblname, string ColumnName, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;

                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@tb", tblname);
                    SelCmd.Parameters.AddWithValue("@ColumnName", ColumnName);




                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }


        public bool InsertIntoDb(string query, string strConnType)
        {
            strFunctionName = "Insert From Database - I";

            lock (this)
            {
                SqlCommand InsCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection();
                        }

                        InsCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection_Web();
                        }

                        InsCmd.Connection = GlobalConnection_Web;
                    }

                    InsCmd.CommandText = query;

                    InsCmd.ExecuteNonQuery();

                    if (InsCmd != null)
                    {
                        InsCmd.Dispose();
                    }

                    return true;
                }
                catch (Exception Ex)
                {
                    if (InsCmd != null)
                    {
                        InsCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;


                    return false;
                }
                finally
                {
                    if (InsCmd != null)
                    {
                        InsCmd.Dispose();
                    }
                }
            }
        }



        public bool InsertIntoDb(string query, string tblname, string strConnType)
        {
            strFunctionName = "Insert Into Database-II";

            lock (this)
            {
                SqlCommand InsCmd = new SqlCommand();
                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSE")
                        {
                            OpenConnection();
                        }
                        InsCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection_Web();
                        }

                        InsCmd.Connection = GlobalConnection_Web;
                    }


                    string InsrtQry = "Insert into " + tblname + " values(" + query + ")";

                    InsCmd.CommandText = InsrtQry;

                    InsCmd.ExecuteNonQuery();

                    if (InsCmd != null)
                    {
                        InsCmd.Dispose();
                    }

                    return true;
                }
                catch (Exception Ex)
                {
                    if (InsCmd != null)
                    {
                        InsCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    //An explicit value for the identity column in table 'tbl_SondeData' can only be specified when a column list is used and IDENTITY_INSERT is ON."

                    //  bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
                finally
                {
                    if (InsCmd != null)
                    {
                        InsCmd.Dispose();
                    }
                }
            }
        }

        public bool DeleteFromDb(string query, string strConnType)
        {
            strFunctionName = "Delete From Database";

            lock (this)
            {
                SqlCommand DeltCmd = new SqlCommand();

                try
                {
                    //DeltCmd.Connection = GlobalConnection;

                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection();
                        }

                        DeltCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection_Web();
                        }

                        DeltCmd.Connection = GlobalConnection_Web;
                    }

                    DeltCmd.CommandText = query;

                    DeltCmd.ExecuteNonQuery();

                    if (DeltCmd != null)
                    {
                        DeltCmd.Dispose();
                    }

                    return true;
                }
                catch (Exception Ex)
                {
                    if (DeltCmd != null)
                    {
                        DeltCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    // bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
                finally
                {
                    if (DeltCmd != null)
                    {
                        DeltCmd.Dispose();
                    }
                }
            }
        }

        public bool UpdateIntoDb(string query, string strConnType)
        {
            strFunctionName = "Update Into Database-1";

            lock (this)
            {
                SqlCommand UpdtCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection();
                        }

                        UpdtCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection_Web();
                        }

                        UpdtCmd.Connection = GlobalConnection_Web;
                    }

                    UpdtCmd.CommandText = query;

                    UpdtCmd.ExecuteNonQuery();

                    if (UpdtCmd != null)
                    {
                        UpdtCmd.Dispose();
                    }

                    return true;
                }
                catch (Exception Ex)
                {
                    if (UpdtCmd != null)
                    {
                        UpdtCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    //bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
                finally
                {
                    if (UpdtCmd != null)
                    {
                        UpdtCmd.Dispose();
                    }
                }
            }
        }

        public bool UpdateIntoDb(string query, string tblname, string strConnType)
        {
            strFunctionName = "Update Into Database-2";

            lock (this)
            {
                SqlCommand UpdtCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection();
                        }

                        UpdtCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection_Web();
                        }

                        UpdtCmd.Connection = GlobalConnection_Web;
                    }

                    UpdtCmd.CommandText = "Update " + tblname + " " + query;

                    UpdtCmd.ExecuteNonQuery();

                    if (UpdtCmd != null)
                    {
                        UpdtCmd.Dispose();
                    }

                    return true;
                }
                catch (Exception Ex)
                {
                    if (UpdtCmd != null)
                    {
                        UpdtCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    //bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
                finally
                {
                    if (UpdtCmd != null)
                    {
                        UpdtCmd.Dispose();
                    }
                }
            }
        }


        public DataSet SP_ColName(string spname, string tblname, string ColumnName, int uid, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@TableName", tblname);
                    SelCmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    SelCmd.Parameters.AddWithValue("@UserID", uid);





                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }
                }
            }
        }

        public DataSet SP_ColName_Datareport(string spname, string tblname, string ColumnName, string fromdate, string todate, int uid, string strConnType)
        {
            strFunctionName = "Fetch Data Sp_ColName_Datareport From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@TableName", tblname);
                    SelCmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    SelCmd.Parameters.AddWithValue("@Fromdate", fromdate);
                    SelCmd.Parameters.AddWithValue("@todate", todate);
                    SelCmd.Parameters.AddWithValue("@UserID", uid);

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }
                }
            }
        }
        public DataSet SP_ColName_Alias(string spname, string tblname, string ColumnName, string AliasColumnName, int uid, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@TableName", tblname);
                    SelCmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    SelCmd.Parameters.AddWithValue("@AliasColumnName", AliasColumnName);
                    SelCmd.Parameters.AddWithValue("@UserID", uid);





                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }
        public DataSet SP_CustomRange(string spname, string tblname, string ColumnName, string AliasColumnName, int uid, string fromdate, string todate, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@TableName", tblname);
                    SelCmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    SelCmd.Parameters.AddWithValue("@AliasColumnName", AliasColumnName);
                    SelCmd.Parameters.AddWithValue("@UserID", uid);
                    SelCmd.Parameters.AddWithValue("@fromdate", fromdate);
                    SelCmd.Parameters.AddWithValue("@todate", todate);
                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }
        public DataSet SP_CustomRangGraph(string spname, string tblname, string ColumnName, int uid, string fromdate, string todate, string strConnType)
        {
            strFunctionName = "Fetch Data SP_CustomRangGraph From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@TableName", tblname);
                    SelCmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    SelCmd.Parameters.AddWithValue("@UserID", uid);
                    SelCmd.Parameters.AddWithValue("@fromdate", fromdate);
                    SelCmd.Parameters.AddWithValue("@todate", todate);
                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }
        public DataSet sp_getTotalBurst(string spname, string StationID, string fromDate, string toDate, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@StationID", StationID);
                    SelCmd.Parameters.AddWithValue("@fromDate", fromDate);
                    SelCmd.Parameters.AddWithValue("@toDate", toDate);

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }
        public DataSet sp_dashboardSP(string spname, string fromDate, string toDate, string profile, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;


                    SelCmd.Parameters.AddWithValue("@fromDate", fromDate);
                    SelCmd.Parameters.AddWithValue("@toDate", toDate);
                    SelCmd.Parameters.AddWithValue("@Profile", profile);

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }
        public DataSet FetchData_SP_getDisplayStation(string spname, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;




                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }
        public bool CheckIfTableExists(string query, string strConnType)
        {
            strFunctionName = "Update Into Database-1";

            lock (this)
            {
                SqlCommand CheckTblCmd = new SqlCommand();

                try
                {
                    //UpdtCmd.Connection = GlobalConnection;

                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection();
                        }

                        CheckTblCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            OpenConnection_Web();
                        }
                        CheckTblCmd.Connection = GlobalConnection_Web;
                    }

                    CheckTblCmd.CommandText = query;
                    int x = Convert.ToInt32(CheckTblCmd.ExecuteScalar());

                    if (x == 0)
                        return false;

                    if (CheckTblCmd != null)
                    {
                        CheckTblCmd.Dispose();
                    }
                    return true;
                }
                catch (Exception Ex)
                {
                    if (CheckTblCmd != null)
                    {
                        CheckTblCmd.Dispose();
                    }
                    strExceptionMessage = Ex.Message;

                    //bool ExcpMesg = clsFile.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
                finally
                {
                    if (CheckTblCmd != null)
                    {
                        CheckTblCmd.Dispose();
                    }
                }
            }
        }
        public DataSet FetchData_HourlyReport(string spname, string hour, string Date, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@hour", hour);
                    SelCmd.Parameters.AddWithValue("@Date", Date);


                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }

        public DataSet SP_FetchTodayNewData(string spname, string tblname, string ColumnName, string uid, string fromdate, string todate, string strConnType)
        {
            strFunctionName = "Fetch DataSP_FetchTodayNewData From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@TableName", tblname);
                    SelCmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    SelCmd.Parameters.AddWithValue("@UserID", uid);
                    SelCmd.Parameters.AddWithValue("@toDate", todate);
                    SelCmd.Parameters.AddWithValue("@fromDate", fromdate);


                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }

        public DataSet SP_FetchTodayNewData(string spname, string tblname, string ColumnName, string uid, string fromdate, string todate, string WindDirAvg, string strConnType)
        {
            strFunctionName = "Fetch DataSP_FetchTodayNewData From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }

                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@TableName", tblname);
                    SelCmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    SelCmd.Parameters.AddWithValue("@UserID", uid);
                    SelCmd.Parameters.AddWithValue("@toDate", todate);
                    SelCmd.Parameters.AddWithValue("@fromDate", fromdate);
                    SelCmd.Parameters.AddWithValue("@WindDirAvg", WindDirAvg);

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                }
            }
        }

        public DataSet FetchData_SP_AuthenticateUser(string spname, string Uname, string Pwd, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }


                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@Uname", Uname);
                    SelCmd.Parameters.AddWithValue("@Pwd", Pwd);


                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }
                    return ds;
                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }

        public DataSet FetchData_SP_StationData(string spname, string StID, string fromDt, string toDt, string strConnType)
        {
            strFunctionName = "Fetch Data From  SP-StationData Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSE")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection;
                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;
                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }

                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@StationIDList", StID);
                    SelCmd.Parameters.AddWithValue("@fromDate", fromDt);
                    SelCmd.Parameters.AddWithValue("@toDate", toDt);

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }

        public DataSet FetchData_GenericSummary(string spname, string StID, string fromDt, string toDt, string Status, string strConnType)
        {
            strFunctionName = "Fetch Data From  SP-StationData Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection;
                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;
                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }

                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    if (spname == "[AWSAPI].[GenericStationData]")
                    {
                        SelCmd.Parameters.AddWithValue("@Profile", StID);
                        SelCmd.Parameters.AddWithValue("@StationIDList", fromDt);
                        SelCmd.Parameters.AddWithValue("@fromDate", toDt);
                        SelCmd.Parameters.AddWithValue("@toDate", Status);
                    }
                    else if (spname == "[AWSAPI].[GenericPastStationData]")
                    {
                        SelCmd.Parameters.AddWithValue("@StationID", StID);
                        SelCmd.Parameters.AddWithValue("@fromDate", fromDt);
                        SelCmd.Parameters.AddWithValue("@toDate", toDt);
                        SelCmd.Parameters.AddWithValue("@Status", Status);
                    }
                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }

        public DataSet FetchData_GenericStation(string spname, string StID, string fromDt, string toDt, string Status, string strConnType)
        {
            strFunctionName = "Fetch Data From  SP-StationData Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        if (GlobalConnection.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection;
                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;
                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }

                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@StationID", StID);
                    SelCmd.Parameters.AddWithValue("@fromDate", fromDt);
                    SelCmd.Parameters.AddWithValue("@toDate", toDt);
                    SelCmd.Parameters.AddWithValue("@Status", Status);

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    strExceptionMessage = Ex.Message;



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }

        public DataSet FetchData_SP_LatestSTRec(string spname, string StationID, string strConnType)
        {
            strFunctionName = "Fetch Latest ST Record";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        Connection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }

                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;
                    SelCmd.Parameters.AddWithValue("@Stid", StationID);

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }
                }
            }
        }

        public DataSet sp_getTotalBurst_Status(string spname, string StationID, string fromDate, string toDate, string Status, string strConnType)
        {
            strFunctionName = "Fetch Data From Database-1";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {


                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;


                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@StationID", StationID);
                    SelCmd.Parameters.AddWithValue("@fromDate", fromDate);
                    SelCmd.Parameters.AddWithValue("@toDate", toDate);
                    SelCmd.Parameters.AddWithValue("@Status", Status);

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }

        public DataSet sp_MinMaxStationData_Status(string spname, string StationID, string fromDate, string toDate, string Status, string Last24Status, string strConnType)
        {
            strFunctionName = "Get Min Max  Values of StationData sp_MinMaxStationData_Status";

            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {
                        Connection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {
                        if (GlobalConnection_Web.State.ToString().ToUpper() == "CLOSED")
                        {
                            Connection_Web();
                        }
                        SelCmd.Connection = GlobalConnection_Web;

                        //Connection_Web();
                        //SelCmd.Connection = GlobalConnection_Web;
                    }

                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;

                    SelCmd.Parameters.AddWithValue("@StationID", StationID);
                    SelCmd.Parameters.AddWithValue("@fromDate", fromDate);
                    SelCmd.Parameters.AddWithValue("@toDate", toDate);
                    SelCmd.Parameters.AddWithValue("@Status", Status);
                    SelCmd.Parameters.AddWithValue("@Last24Status", Last24Status == null ? (object)DBNull.Value : Last24Status);

                    da = new SqlDataAdapter(SelCmd);

                    ds = new DataSet();

                    da.Fill(ds);

                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    strExceptionMessage = Ex.Message;

                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }
                }
            }
        }
    }
}
