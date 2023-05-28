using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AWSAPI.HelperClass
{
    class clsExceptionDataRoutine
    {
        public string strModuleName = "Exception Data Routines";
        public string strFunctionName = "";
        public string strExceptionMessage = "";

        public bool WriteIntoExceptionFile(string tmpData)
        {
            strFunctionName = "Write Into Exception File-1";

            lock (this)
            {
                try
                {
                    string tmpModName = "";
                    string tmpFunName = "";
                    string tmpExMsg = "";
                    string tmpLoc = "";
                    string tmpFilename = "";

                    if (!tmpData.Contains("#"))
                    {
                        return false;
                    }
                    String[] arrData = tmpData.Split('#');
                    if (arrData.Length >= 2)
                    {
                        tmpModName = arrData[0];
                        tmpFunName = arrData[1];
                        tmpExMsg = arrData[2];
                        tmpFilename = arrData[3];
                        tmpLoc = arrData[4];
                    }

                    String tmpDate = "";
                    String tmpTime = "";

                    tmpDate = String.Format("{0:dd-MMM-yyyy}", System.Convert.ToDateTime(DateTime.Now).Date);
                    tmpTime = String.Format("{0:HH:mm:ss.fff}", System.Convert.ToDateTime(DateTime.Now));

                    //string strWriteData = "Module Name ->" + tmpModName + System.Environment.NewLine + "Function Name ->" + tmpFunName + System.Environment.NewLine + "Exception Message ->" + tmpExMsg + System.Environment.NewLine + "-----------------------------------------------------------------------------------------------------" + System.Environment.NewLine;
                    string strWriteData = "";
                    strWriteData = strWriteData + "Date -> " + tmpDate + "     " + "Time -> " + tmpTime + System.Environment.NewLine;
                    strWriteData = strWriteData + "Module Name -> " + tmpModName + System.Environment.NewLine;
                    strWriteData = strWriteData + "Function Name -> " + tmpFunName + System.Environment.NewLine;
                    strWriteData = strWriteData + "Exception Message -> " + tmpExMsg + System.Environment.NewLine;
                    strWriteData = strWriteData + "-----------------------------------------------------------------------------------------------------" + System.Environment.NewLine;
                    strWriteData = strWriteData + System.Environment.NewLine;

                    clsFileOperationRoutines clsFOR = new clsFileOperationRoutines();

                    bool WriteIntoFile = clsFOR.WriteIntoFile(tmpLoc, tmpFilename, strWriteData);

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    ////bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public bool WriteIntoExceptionFile(string ModuleName, string FunctionName, string ExceptionMessage, string Filename, string Location)
        {
            strFunctionName = "Write Into Exception File-2";

            lock (this)
            {
                try
                {
                    //string strWriteData = "Module Name ->" + ModuleName + System.Environment.NewLine + "Function Name ->" + FunctionName + System.Environment.NewLine + "Exception Message ->" + ExceptionMessage + System.Environment.NewLine + "-----------------------------------------------------------------------------------------------------" + System.Environment.NewLine;

                    String tmpDate = "";
                    String tmpTime = "";

                    tmpDate = String.Format("{0:dd-MMM-yyyy}", System.Convert.ToDateTime(DateTime.Now).Date);
                    tmpTime = String.Format("{0:HH:mm:ss.fff}", System.Convert.ToDateTime(DateTime.Now));

                    string strWriteData = "";
                    strWriteData = strWriteData + "Date -> " + tmpDate + "     " + "Time -> " + tmpTime + System.Environment.NewLine;
                    strWriteData = strWriteData + "Module Name -> " + ModuleName + System.Environment.NewLine;
                    strWriteData = strWriteData + "Function Name -> " + FunctionName + System.Environment.NewLine;
                    strWriteData = strWriteData + "Exception Message -> " + ExceptionMessage + System.Environment.NewLine;
                    strWriteData = strWriteData + "-----------------------------------------------------------------------------------------------------" + System.Environment.NewLine;
                    strWriteData = strWriteData + System.Environment.NewLine;

                    clsFileOperationRoutines clsFOR = new clsFileOperationRoutines();

                    clsGlobalData clsGD = new clsGlobalData();

                    if (Filename.Trim() == "")
                    {
                        String tmpDate1 = "";
                        tmpDate1 = String.Format("{0:dd-MMM-yyyy}", System.Convert.ToDateTime(DateTime.Now).Date);

                        clsGlobalData.strExceptionFileName = "";
                        clsGlobalData.strExceptionFileName = clsGlobalData.myProjName + "_ExceptionFile_" + tmpDate1 + ".txt";

                        Filename = clsGlobalData.strExceptionFileName;
                    }

                    bool WriteIntoFile = clsFOR.WriteIntoFile(Location, Filename, strWriteData);

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    ////bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public static bool static_WriteIntoExceptionFile(string ModuleName, string FunctionName, string ExceptionMessage, string Filename, string Location)
        {
            //string tmpFunctionName = "Write Into Static Exception File";

            try
            {
                //string strWriteData = "Module Name ->" + ModuleName + System.Environment.NewLine + "Function Name ->" + FunctionName + System.Environment.NewLine + "Exception Message ->" + ExceptionMessage + System.Environment.NewLine + "-----------------------------------------------------------------------------------------------------" + System.Environment.NewLine;

                String tmpDate = "";
                String tmpTime = "";

                tmpDate = String.Format("{0:dd-MMM-yyyy}", System.Convert.ToDateTime(DateTime.Now).Date);
                tmpTime = String.Format("{0:HH:mm:ss.fff}", System.Convert.ToDateTime(DateTime.Now));

                string strWriteData = "";
                strWriteData = strWriteData + "Date -> " + tmpDate + "     " + "Time -> " + tmpTime + System.Environment.NewLine;
                strWriteData = strWriteData + "Module Name -> " + ModuleName + System.Environment.NewLine;
                strWriteData = strWriteData + "Function Name -> " + FunctionName + System.Environment.NewLine;
                strWriteData = strWriteData + "Exception Message -> " + ExceptionMessage + System.Environment.NewLine;
                strWriteData = strWriteData + "-----------------------------------------------------------------------------------------------------" + System.Environment.NewLine;
                strWriteData = strWriteData + System.Environment.NewLine;

                clsFileOperationRoutines clsFOR = new clsFileOperationRoutines();

                clsGlobalData clsGD = new clsGlobalData();

                if (Filename.Trim() == "")
                {
                    String tmpDate1 = "";
                    tmpDate1 = String.Format("{0:dd-MMM-yyyy}", System.Convert.ToDateTime(DateTime.Now).Date);

                    clsGlobalData.strExceptionFileName = "";
                    clsGlobalData.strExceptionFileName = clsGlobalData.myProjName + "_ExceptionFile_" + tmpDate1 + ".txt";

                    Filename = clsGlobalData.strExceptionFileName;
                }

                bool WriteIntoFile = clsFOR.WriteIntoFile(Location, Filename, strWriteData);

                return true;
            }
            catch (Exception Ex)
            {
                string strExceptionMessage = Ex.Message;

                ////bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return false;
            }
            
        }

        public bool WriteIntoLogFile(string tmpData, int intoNoOfSpace)
        {
            strFunctionName = "Write Into Log File-1";

            lock (this)
            {
                try
                {
                    string tmpModName = "";
                    string tmpFunName = "";
                    string tmpExMsg = "";
                    string tmpLoc = "";
                    string tmpFilename = "";

                    if (!tmpData.Contains("#"))
                    {
                        return false;
                    }

                    String[] arrData = tmpData.Split('#');
                    if (arrData.Length >= 2)
                    {
                        tmpModName = arrData[0];
                        tmpFunName = arrData[1];
                        tmpExMsg = arrData[2];
                        tmpFilename = arrData[3];
                        tmpLoc = arrData[4];
                    }

                    String tmpDate = "";
                    String tmpTime = "";

                    tmpDate = String.Format("{0:dd-MMM-yyyy}", System.Convert.ToDateTime(DateTime.Now).Date);
                    tmpTime = String.Format("{0:HH:mm:ss.fff}", System.Convert.ToDateTime(DateTime.Now));

                    //Radio Sonde Module Start
                    //Balloon Burst............
                    //RS232 - Receiver-1 - Data Acquisition Process Start
                    //RS232 - Receiver-2 - Data Acquisition Process Start
                    //RS232 - Ground Station - Data Acquisition Process Start.
                    //Flight Duration Process Start.
                    //Radio Sonde - Start Button Process.
                    //Radio Sonde - Stop Button Process.
                    //Radio Sonde Module Stop

                    //Report Module Start
                    //Report Module Stop
                    //View Data Report Process Start.
                    //View Data Report Process Completed.

                    string strWriteData = "";
                    //strWriteData = tmpExMsg + " - " + tmpDate + " - " + tmpTime + System.Environment.NewLine;

                    if (tmpExMsg == "Radio Sonde Module Start")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + System.Environment.NewLine;
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "Balloon Burst............")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + System.Environment.NewLine;
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "RS232 - Receiver-1 - Data Acquisition Process Start")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "RS232 - Receiver-2 - Data Acquisition Process Start")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "RS232 - Ground Station - Data Acquisition Process Start")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    //else if (tmpExMsg == "Flight Duration Process Start")
                    else if (tmpExMsg == "Ascent Duration Process Start")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + System.Environment.NewLine;
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "Radio Sonde - Start Button Process")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "Radio Sonde - Stop Button Process")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "Radio Sonde Module Stop")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + System.Environment.NewLine;
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "Report Module Start")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + System.Environment.NewLine;
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "Report Module Stop")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + System.Environment.NewLine;
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "View Data Report Process Start")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else if (tmpExMsg == "View Data Report Process Completed")
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }
                    else
                    {
                        strWriteData = "";
                        strWriteData = strWriteData + tmpExMsg + " - " + tmpDate + " - " + tmpTime;
                        strWriteData = strWriteData + System.Environment.NewLine;
                    }


                    clsFileOperationRoutines clsFOR = new clsFileOperationRoutines();

                    bool WriteIntoFile = clsFOR.WriteIntoFile(tmpLoc, tmpFilename, strWriteData);

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public bool WriteIntoLogFile_OLD(string tmpData, int intoNoOfSpace)
        {
            strFunctionName = "Write Into Log File-1- old";

            lock (this)
            {
                try
                {
                    string tmpModName = "";
                    string tmpFunName = "";
                    string tmpExMsg = "";
                    string tmpLoc = "";
                    string tmpFilename = "";

                    if (!tmpData.Contains("#"))
                    {
                        return false;
                    }

                    String[] arrData = tmpData.Split('#');
                    if (arrData.Length >= 2)
                    {
                        tmpModName = arrData[0];
                        tmpFunName = arrData[1];
                        tmpExMsg = arrData[2];
                        tmpFilename = arrData[3];
                        tmpLoc = arrData[4];
                    }

                    String tmpDate = "";
                    String tmpTime = "";

                    tmpDate = String.Format("{0:dd-MMM-yyyy}", System.Convert.ToDateTime(DateTime.Now).Date);
                    tmpTime = String.Format("{0:HH:mm:ss.fff}", System.Convert.ToDateTime(DateTime.Now));

                    string strWriteData = "";
                    strWriteData = strWriteData + "Date -> " + tmpDate + "     " + "Time -> " + tmpTime + System.Environment.NewLine;
                    strWriteData = strWriteData + "Module Name -> " + tmpModName + System.Environment.NewLine;
                    strWriteData = strWriteData + "Function Name -> " + tmpFunName + System.Environment.NewLine;
                    strWriteData = strWriteData + "Log Message -> " + tmpExMsg + System.Environment.NewLine;
                    strWriteData = strWriteData + "-----------------------------------------------------------------------------------------------------" + System.Environment.NewLine;
                    strWriteData = strWriteData + System.Environment.NewLine;

                    clsFileOperationRoutines clsFOR = new clsFileOperationRoutines();

                    bool WriteIntoFile = clsFOR.WriteIntoFile(tmpLoc, tmpFilename, strWriteData);

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public bool WriteIntoLogFile(string ModuleName, string FunctionName, string ExceptionMessage, string Filename, string Location)
        {
            strFunctionName = "Write Into Log File-2";

            lock (this)
            {
                try
                {
                    String tmpDate = "";
                    String tmpTime = "";

                    tmpDate = String.Format("{0:dd-MMM-yyyy}", System.Convert.ToDateTime(DateTime.Now).Date);
                    tmpTime = String.Format("{0:HH:mm:ss.fff}", System.Convert.ToDateTime(DateTime.Now));

                    string strWriteData = "";
                    strWriteData = strWriteData + "Date -> " + tmpDate + "     " + "Time -> " + tmpTime + System.Environment.NewLine;
                    strWriteData = strWriteData + "Module Name -> " + ModuleName + System.Environment.NewLine;
                    strWriteData = strWriteData + "Function Name -> " + FunctionName + System.Environment.NewLine;
                    strWriteData = strWriteData + "Log Message -> " + ExceptionMessage + System.Environment.NewLine;
                    strWriteData = strWriteData + "-----------------------------------------------------------------------------------------------------" + System.Environment.NewLine;
                    strWriteData = strWriteData + System.Environment.NewLine;

                    clsFileOperationRoutines clsFOR = new clsFileOperationRoutines();

                    clsGlobalData clsGD = new clsGlobalData();

                    if (Filename.Trim() == "")
                    {
                        String tmpDate1 = "";
                        tmpDate1 = String.Format("{0:dd-MMM-yyyy}", System.Convert.ToDateTime(DateTime.Now).Date);

                        clsGlobalData.strLogFileName = "";
                        clsGlobalData.strLogFileName = clsGlobalData.myProjName + "_LogFile_" + tmpDate1 + ".txt";

                        Filename = clsGlobalData.strLogFileName;
                    }

                    bool WriteIntoFile = clsFOR.WriteIntoFile(Location, Filename, strWriteData);

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }
    }
}
