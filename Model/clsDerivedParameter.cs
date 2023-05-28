using AWS;
using AWSAPI.HelperClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AWSAPI.Model
{
    public class clsDerivedParameter
    {
        static clsDatabase objDB = new clsDatabase();
        static clsExceptionDataRoutine clsEDR = new clsExceptionDataRoutine();

        public string strModuleName = "Calculate Derived Parameter";
        public string strFunctionName = "";
        public string strExceptionMessage = "";

        public double funCalDewPoint(double Temp, double RH)
        {
            strFunctionName = "Calculate DewPoint";
            //string TdString = "";
            double Td = 0.0;
            try
            {
                ////Online Dew Point calculation link
                ////http://andrew.rsmas.miami.edu/bmcnoldy/Humidity.html

               
                double a = 17.62;
                double b = 243.12;
                double GamaVal;
                double Gmval1, Gmval2;
                double Tdval1, Tdval2;

                Gmval1 = (a * Temp) / (b + Temp);
                Gmval2 = 2.303 * Math.Log10(RH / 100);
                //Gmval2 = Math.Log(RH / 100);

                GamaVal = Gmval1 + Gmval2;

                Tdval1 = b * GamaVal;
                Tdval2 = a - GamaVal;
                Td = Tdval1 / Tdval2;
                //TdString = Td.ToString("N2");

                //return TdString;

                return Td;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strLogFileName + "#" + clsGlobalData.strLoc_LogFile;
                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);

                return Td;
            }

        }

        public double funCalWindRun(double WS)
        {
            strFunctionName = "Calculate Wind Run";
            double  WindRun = 0.0;

            try
            {
                WindRun = WS * 0.25;
                return WindRun;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strLogFileName + "#" + clsGlobalData.strLoc_LogFile;
                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                return WindRun;
            }

        }

        public double funCalWindChill(double WS,double Temp)
        {
            strFunctionName = "Calculate Wind Chill";
            double WindChill = 0.0;
            try
            {
                //(10 * √ Parameter - 8 + 9 - Parameter - 8) *(33 - Parameter - 5);
                //unit kg*ca/m²/h
                //WindChill = (10 * Math.Sqrt(WS) + 9 - WS) * (33 - Temp);
                //WindChill = (12.1452 + 11.6222 * Math.Sqrt(WS) - 1.16222 * WS) * (33 - Temp);

                WindChill = (10 * Math.Sqrt(WS) - WS + 10.5) * (33-Temp);
                return WindChill;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strLogFileName + "#" + clsGlobalData.strLoc_LogFile;
                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                return WindChill;
            }

        }

        public double funCalHeatIndex(double Temp, double RH)
        {
            strFunctionName = "Calculate Heat Index";
            double HeatIndex = 0.0;
           
            const double c1 = -8.78469475556;
            const double c2 =  1.61139411;
            const double c3 =  2.33854883889;
            const double c4 =  -0.14611605;
            const double c5 =  -0.012308094;
            const double c6 =  -0.0164248277778;
            const double c7 =  0.002211732;
            const double c8 =  0.00072546;
            const double c9 =  -0.000003582;

            try
            {
                HeatIndex  =  c1 + (c2 * Temp) + (c3 * RH) +  (c4 * Temp * RH) + (c5 * Math.Pow(Temp,2)) + (c6 * Math.Pow(RH,2))  + (c7 * Math.Pow(Temp, 2) * RH) + (c8  * Temp * Math.Pow(RH, 2)) + (c9 * Math.Pow(Temp, 2) * Math.Pow(RH, 2));
                return HeatIndex;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strLogFileName + "#" + clsGlobalData.strLoc_LogFile;
                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                return HeatIndex;
            }

        }

        public double funCalTHWIndex(double Temp, double RH, double WS)
        {
            strFunctionName = "Calculate THW Index";
            double THW_Index = 0.0;

            try
            {
               //  e = (Parameter-11 / 100) * 6.105 * exp (17.27 * Parameter-5 / (237.7 + Parameter-5)) 
                double e = (RH / 100) * Math.Exp(17.27 * Temp / (237.7 + Temp));
                THW_Index = Temp + (0.33 * e) - (0.70 * WS) - 4.00;
                return THW_Index;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strLogFileName + "#" + clsGlobalData.strLoc_LogFile;
                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                return THW_Index;
            }

        }

        public double funGetHeatDD(double MaxTemp, double MinTemp)
        {
            strFunctionName = "Get Heat Degree Day";
            double HeatDD = 0.0;

            try
            {
                //'HEAT D-D = ((Parameter-15 + Parameter-16)/2)*9/5 - 65      if HEAT D-D < 0, HEAT D-D = 0  
                HeatDD = ((MaxTemp + MinTemp) / 2) * 9 / 5 - 65;
                if (HeatDD < 0)
                {
                    HeatDD = 0;
                    return HeatDD;
                }
                else
                    return HeatDD;
                
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strLogFileName + "#" + clsGlobalData.strLoc_LogFile;
                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                return HeatDD;
            }

        }

        public double funGetCoolDD(double MaxTemp, double MinTemp)
        {
            strFunctionName = "Get Cool Degree Day";
            double CoolDD = 0.0;

            try
            {
                //'COOL D-D =  65 - ((Parameter-15 + Parameter-16)/2)*9/5    if COOL D-D < 0, COOL D-D = 0
                CoolDD = 65 - ((MaxTemp + MinTemp) / 2) * 9 / 5;
                if (CoolDD < 0)
                {
                    CoolDD = 0;
                    return CoolDD;
                }
                else
                    return CoolDD;

            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                string tmpExData = "";
                tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strLogFileName + "#" + clsGlobalData.strLoc_LogFile;
                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(tmpExData);
                return CoolDD;
            }

        }

        public  double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        public double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }

        public double funAvgWindDirection(int totalRec,List<string> winddirection, List<string> windspeed)
        {
            double EW_Vector = 0.0, NS_Vector = 0.0;
            double EW_Average = 0.0, NS_Average = 0.0;

            for (int i = 0; i < totalRec; i++)
            {
                EW_Vector += Math.Sin(ConvertDegreesToRadians(Convert.ToDouble(winddirection[i]))) * Convert.ToDouble(windspeed[i]);
                NS_Vector += Math.Cos(ConvertDegreesToRadians(Convert.ToDouble(winddirection[i]))) * Convert.ToDouble(windspeed[i]);
            }

            EW_Average = (EW_Vector / totalRec) * -1; //Average in Radians
            NS_Average = (NS_Vector / totalRec) * -1; //Average in Radians

            double Atan2Direction = Math.Atan2(EW_Average, NS_Average); //can be found in any math library
            double AvgDirectionInDeg = ConvertRadiansToDegrees(Atan2Direction);
            if (AvgDirectionInDeg > 180)
                AvgDirectionInDeg -= 180;
            else if (AvgDirectionInDeg < 180)
                AvgDirectionInDeg += 180;

            return AvgDirectionInDeg;
        }

        public string WindRoseData(string winddirection,string windspeed)
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

        //public void  WindRoseData(List<string> winddirection, List<string> windspeed)
        //{   

        //    if (winddirection != null && windspeed != null)
        //    {

        //        List<windDirClass> lstWDir = (List<windDirClass>)Session["WindDir"];
        //        List<windSpdClass> lstWSpd = (List<windSpdClass>)Session["WindSpd"];

        //        if (lstWDir.Count != 0 && lstWSpd.Count != 0)
        //        {
        //            string StrN = "", StrNNE = "", StrNE = "", StrENE = "", StrE = "", StrESE = "", StrSE = "", StrSSE = "", StrS = "", StrSSW = "", StrSW = "", StrWSW = "", StrW = "", StrWNW = "", StrNW = "", StrNNW = "";
        //            string JsonStrN = "", JsonStrSE = "", JsonStrSSE = "", JsonStrS = "", JsonStrSSW = "", JsonStrSW = "", JsonStrW = "", JsonStrNNW = "", JsonStrNNE = "", JsonStrNE = "", JsonStrENE = "", JsonStrWSW = "", JsonStrE = "", JsonStrESE = "", JsonStrWNW = "", JsonStrNW = "";



        //            for (int d = 0; d < lstWDir.Count; d++)
        //            {
        //                if (!string.IsNullOrEmpty(lstWDir[d].WD))
        //                {
        //                    double WindDirDegree = Convert.ToDouble(lstWDir[d].WD);
        //                    string[] Sector = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
        //                    string WindDir = Sector[Convert.ToInt32(Math.Round((WindDirDegree % 360) / 22.5))];

        //                    if (lstWSpd[d].WS.ToString().Contains("."))
        //                    {
        //                        string[] arr = lstWSpd[d].WS.Split('.');
        //                        if (arr[0].Length > 1 && arr[0].StartsWith("0"))
        //                        {
        //                            lstWSpd[d].WS = arr[0].Remove(0, 1) + "." + arr[1];
        //                        }
        //                    }

        //                    if (WindDir == "N")
        //                    {
        //                        JsonStrN = "";

        //                        StrN += lstWSpd[d].WS + ",";
        //                        JsonStrN = JsonStrN + "{arg : '" + WindDir + "',";

        //                        string[] StrNArr = StrN.TrimEnd(',').Split(',');
        //                        for (int a = 0; a < StrNArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrN += value + ":" + StrNArr[a] + ",";
        //                        }

        //                        JsonStrN = JsonStrN.TrimEnd(',') + "},";
        //                    }
        //                    else if (WindDir == "NNE")
        //                    {
        //                        JsonStrNNE = "";

        //                        StrNNE += lstWSpd[d].WS + ",";
        //                        JsonStrNNE = JsonStrNNE + "{arg : '" + WindDir + "',";

        //                        string[] StrNNEArr = StrNNE.TrimEnd(',').Split(',');
        //                        for (int a = 0; a < StrNNEArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrNNE += value + ":" + StrNNEArr[a] + ",";
        //                        }

        //                        JsonStrNNE = JsonStrNNE.TrimEnd(',') + "},";

        //                    }
        //                    else if (WindDir == "NE")
        //                    {
        //                        JsonStrNE = "";

        //                        StrNE += lstWSpd[d].WS + ",";
        //                        JsonStrNE = JsonStrNE + "{arg : '" + WindDir + "',";

        //                        string[] StrNEArr = StrNE.TrimEnd(',').Split(',');
        //                        for (int a = 0; a < StrNEArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrNE += value + ":" + StrNEArr[a] + ",";
        //                        }

        //                        JsonStrNE = JsonStrNE.TrimEnd(',') + "},";

        //                    }
        //                    else if (WindDir == "ENE")
        //                    {

        //                        JsonStrENE = "";

        //                        StrENE += lstWSpd[d].WS + ",";
        //                        JsonStrENE = JsonStrENE + "{arg : '" + WindDir + "',";

        //                        string[] StrENEArr = StrENE.TrimEnd(',').Split(',');
        //                        for (int a = 0; a < StrENEArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrENE += value + ":" + StrENEArr[a] + ",";
        //                        }

        //                        JsonStrENE = JsonStrENE.TrimEnd(',') + "},";
        //                    }
        //                    else if (WindDir == "E")
        //                    {

        //                        JsonStrE = "";

        //                        StrE += lstWSpd[d].WS + ",";
        //                        JsonStrE = JsonStrE + "{arg : '" + WindDir + "',";

        //                        string[] StrEArr = StrE.TrimEnd(',').Split(',');
        //                        for (int a = 0; a < StrEArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrE += value + ":" + StrEArr[a] + ",";
        //                        }

        //                        JsonStrE = JsonStrE.TrimEnd(',') + "},";

        //                    }
        //                    else if (WindDir == "ESE")
        //                    {

        //                        JsonStrESE = "";

        //                        StrESE += lstWSpd[d].WS + ",";
        //                        JsonStrESE = JsonStrESE + "{arg : '" + WindDir + "',";

        //                        string[] StrESEArr = StrESE.TrimEnd(',').Split(',');
        //                        for (int a = 0; a < StrESEArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrESE += value + ":" + StrESEArr[a] + ",";
        //                        }

        //                        JsonStrESE = JsonStrESE.TrimEnd(',') + "},";


        //                    }
        //                    else if (WindDir == "SE")
        //                    {
        //                        JsonStrSE = "";

        //                        StrSE += lstWSpd[d].WS + ",";
        //                        JsonStrSE = JsonStrSE + "{arg : '" + WindDir + "',";

        //                        string[] StrSEArr = StrSE.TrimEnd(',').Split(',');
        //                        for (int a = 0; a < StrSEArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrSE += value + ":" + StrSEArr[a] + ",";
        //                        }

        //                        JsonStrSE = JsonStrSE.TrimEnd(',') + "},";

        //                    }
        //                    else if (WindDir == "SSE")
        //                    {
        //                        JsonStrSSE = "";

        //                        StrSSE += lstWSpd[d].WS + ",";
        //                        JsonStrSSE = JsonStrSSE + "{arg : '" + WindDir + "',";

        //                        string[] StrSSEArr = StrSSE.TrimEnd(',').Split(',');
        //                        for (int a = 0; a < StrSSEArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrSSE += value + ":" + StrSSEArr[a] + ",";
        //                        }

        //                        JsonStrSSE = JsonStrSSE.TrimEnd(',') + "},";
        //                    }
        //                    else if (WindDir == "S")
        //                    {
        //                        JsonStrS = "";

        //                        StrS += lstWSpd[d].WS + ",";
        //                        JsonStrS = JsonStrS + "{arg : '" + WindDir + "',";

        //                        string[] StrSArr = StrS.TrimEnd(',').Split(',');

        //                        for (int a = 0; a < StrSArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrS += value + ":" + StrSArr[a] + ",";
        //                        }

        //                        JsonStrS = JsonStrS.TrimEnd(',') + "},";

        //                    }
        //                    else if (WindDir == "SSW")
        //                    {
        //                        JsonStrSSW = "";

        //                        StrSSW += lstWSpd[d].WS + ",";
        //                        JsonStrSSW = JsonStrSSW + "{arg : '" + WindDir + "',";

        //                        string[] StrSSWArr = StrSSW.TrimEnd(',').Split(',');

        //                        for (int a = 0; a < StrSSWArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrSSW += value + ":" + StrSSWArr[a] + ",";
        //                        }

        //                        JsonStrSSW = JsonStrSSW.TrimEnd(',') + "},";

        //                    }
        //                    else if (WindDir == "SW")
        //                    {
        //                        JsonStrSW = "";

        //                        StrSW += lstWSpd[d].WS + ",";
        //                        JsonStrSW = JsonStrSW + "{arg : '" + WindDir + "',";

        //                        string[] StrSWArr = StrSW.TrimEnd(',').Split(',');

        //                        for (int a = 0; a < StrSWArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrSW += value + ":" + StrSWArr[a] + ",";
        //                        }

        //                        JsonStrSW = JsonStrSW.TrimEnd(',') + "},";

        //                    }
        //                    else if (WindDir == "WSW")
        //                    {
        //                        JsonStrWSW = "";

        //                        StrWSW += lstWSpd[d].WS + ",";
        //                        JsonStrWSW = JsonStrWSW + "{arg : '" + WindDir + "',";

        //                        string[] StrWSWArr = StrWSW.TrimEnd(',').Split(',');

        //                        for (int a = 0; a < StrWSWArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrWSW += value + ":" + StrWSWArr[a] + ",";
        //                        }

        //                        JsonStrWSW = JsonStrWSW.TrimEnd(',') + "},";
        //                    }
        //                    else if (WindDir == "W")
        //                    {
        //                        JsonStrW = "";

        //                        StrW += lstWSpd[d].WS + ",";
        //                        JsonStrW = JsonStrW + "{arg : '" + WindDir + "',";

        //                        string[] StrWArr = StrW.TrimEnd(',').Split(',');

        //                        for (int a = 0; a < StrWArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrW += value + ":" + StrWArr[a] + ",";
        //                        }

        //                        JsonStrW = JsonStrW.TrimEnd(',') + "},";

        //                    }
        //                    else if (WindDir == "WNW")
        //                    {

        //                        JsonStrWNW = "";

        //                        StrWNW += lstWSpd[d].WS + ",";
        //                        JsonStrWNW = JsonStrWNW + "{arg : '" + WindDir + "',";

        //                        string[] StrWNWArr = StrWNW.TrimEnd(',').Split(',');

        //                        for (int a = 0; a < StrWNWArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrWNW += value + ":" + StrWNWArr[a] + ",";
        //                        }

        //                        JsonStrWNW = JsonStrWNW.TrimEnd(',') + "},";
        //                    }
        //                    else if (WindDir == "NW")
        //                    {

        //                        JsonStrNW = "";

        //                        StrNW += lstWSpd[d].WS + ",";
        //                        JsonStrNW = JsonStrNW + "{arg : '" + WindDir + "',";

        //                        string[] StrNWArr = StrNW.TrimEnd(',').Split(',');

        //                        for (int a = 0; a < StrNWArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrNW += value + ":" + StrNWArr[a] + ",";
        //                        }

        //                        JsonStrNW = JsonStrNW.TrimEnd(',') + "},";
        //                    }
        //                    else if (WindDir == "NNW")
        //                    {

        //                        JsonStrNNW = "";

        //                        StrNNW += lstWSpd[d].WS + ",";
        //                        JsonStrNNW = JsonStrNNW + "{arg : '" + WindDir + "',";

        //                        string[] StrNNWArr = StrNNW.TrimEnd(',').Split(',');

        //                        for (int a = 0; a < StrNNWArr.Length; a++)
        //                        {
        //                            int SrNo = a + 1;
        //                            string value = "val" + SrNo;
        //                            JsonStrNNW += value + ":" + StrNNWArr[a] + ",";
        //                        }

        //                        JsonStrNNW = JsonStrNNW.TrimEnd(',') + "},";
        //                    }


        //                }
        //            }



        //        }

        //    }

        //}



    }
}
