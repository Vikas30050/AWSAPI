using System;
using System.IO;

namespace AWSAPI.HelperClass
{
    class clsGlobalData
    {
        public static string myProjName = "";
        public static string myProjTitle = "";
        public static string myVersion = "";

        public static string strLogFileName = "AWSAPI_StationLog_Data.txt";
        public static string strLoc_LogFile = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

        public static string strExceptionFileName = "AWSAPI_StationData_Excpetion.txt";
        public static string strLoc_ExceptionFile = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

    }
}
