using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;

namespace AWSAPI.HelperClass
{
    class clsFileOperationRoutines
    {
        clsGlobalData clsGD = new clsGlobalData();
        
        clsExceptionDataRoutine clsEDR = new clsExceptionDataRoutine();

        public string strModuleName = "File Operation Routines";
        public string strFunctionName = "";
        public string strExceptionMessage = "";

        public object HttpCacheability { get; private set; }

        public bool WriteIntoFile(string Location, string FileName, string WriteData)
        {
            strFunctionName = "Write Into File-1";

            lock (this)
            {
                try
                {
                    string NewFile = Location + @"\" + FileName;

                    if (WriteData.Trim().ToUpper() == "REPORT")
                    {
                        //FileStream fs = new FileStream(NewFile, FileMode.Create, FileAccess.Write, FileShare.None);
                        FileStream fs = new FileStream(NewFile, FileMode.Append, FileAccess.Write, FileShare.None);

                        fs.Close();
                    }
                    else
                    {

                        //FileStream fs = new FileStream(NewFile, FileMode.Create, FileAccess.Write, FileShare.None);
                        FileStream fs = new FileStream(NewFile, FileMode.Append, FileAccess.Write, FileShare.None);

                        byte[] bytetext = System.Text.Encoding.ASCII.GetBytes(WriteData);

                        fs.Write(bytetext, 0, bytetext.Length);

                        fs.Close();
                    }

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public bool WriteIntoFile(string Location, string FileName, string WriteData, string FileHeader)
        {
            strFunctionName = "Write Into File-2";

            lock (this)
            {
                try
                {
                    string NewFile = Location + @"\" + FileName;

                    if (File.Exists(NewFile))
                    {
                        FileStream fs = new FileStream(NewFile, FileMode.Append, FileAccess.Write, FileShare.None);

                        byte[] bytetext = System.Text.Encoding.ASCII.GetBytes(WriteData);

                        fs.Write(bytetext, 0, bytetext.Length);

                        fs.Close();
                    }
                    else
                    {
                        FileStream fs = new FileStream(NewFile, FileMode.Append, FileAccess.Write, FileShare.None);

                        String HdrData = FileHeader + System.Environment.NewLine + WriteData;

                        byte[] bytetext = System.Text.Encoding.ASCII.GetBytes(HdrData);

                        fs.Write(bytetext, 0, bytetext.Length);

                        fs.Close();
                    }
                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public string ReadFromFile(string Location, string FileName)
        {
            strFunctionName = "Read From File";

            lock (this)
            {
                string result = "";
                try
                {
                    // string NewFile = Location + @"\" + FileName;

                    string NewFile = Location + "\\" + FileName;

                    FileStream fs = new FileStream(NewFile, FileMode.Open, FileAccess.Read, FileShare.None);

                    byte[] bytetext = new byte[fs.Length];

                    fs.Read(bytetext, 0, bytetext.Length);

                    result = System.Text.Encoding.ASCII.GetString(bytetext);

                    fs.Close();

                    return result;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return result;
                }
            }
        }
        
        public string  ReadFromFTP(string Location,string FileName,string Uname, string Pwd)   
        {
            strFunctionName = "Read File From FTP Server";

            lock (this)
            {
                string result = string.Empty;
                try
                {
                  
                        StringBuilder sb = new StringBuilder();

                        string url = string.Empty;

                        WebClient request = new WebClient();
                        url = Location + "/" + FileName;
                        request.Credentials = new NetworkCredential(Uname, Pwd);

                        //byte[] newFileData = request.DownloadData(url);
                        //result = System.Text.Encoding.UTF8.GetString(newFileData);

                        using (Stream myStream = request.OpenRead(url))
                        {
                            using (StreamReader sr = new StreamReader(myStream))
                            {
                                String line;
                                // Read and display lines from the file until the end of 
                                // the file is reached.
                                while ((line = sr.ReadLine()) != null)
                                {
                                    sb.AppendLine(line);
                                }
                            }

                            result = sb.ToString();
                        }
                    
                    return result;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return result;
                }
            }
        }

        //public string ReadFromFTPReqObj(FtpWebRequest request)
        //{
        //    strFunctionName = "Read File From FTP Server";

        //    lock (this)
        //    {
        //        string result = string.Empty;
        //        try
        //        {
        //            using (WebResponse tmpRes = request.GetResponse())
        //            {
        //                //GET THE STREAM TO READ THE RESPONSE FROM
        //                using (Stream tmpStream = tmpRes.GetResponseStream())
        //                {
        //                    //CREATE A TXT READER (COULD BE BINARY OR ANY OTHER TYPE YOU NEED)
        //                    using (TextReader tmpReader = new System.IO.StreamReader(tmpStream))
        //                    {
        //                        //STORE THE FILE CONTENTS INTO A STRING
        //                        result = tmpReader.ReadToEnd();

        //                        return result;
        //                        //DO SOMETHING WITH SAID FILE CONTENTS
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception Ex)
        //        {
        //            strExceptionMessage = Ex.Message;

        //            bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

        //            return result;
        //        }
        //    }
        //}

        public string ReadFromFTPReqObj(FtpWebRequest request, string Location, string FileName)
        {

            string filerows = string.Empty;
            string url = string.Empty;
            url = Location + "/" + FileName;
            request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UsePassive = true;
            request.UseBinary = true;
            request.EnableSsl = false;
            FtpWebResponse FileResponse = (FtpWebResponse)request.GetResponse();
            Stream fileStream = FileResponse.GetResponseStream();
            StreamReader filereader = new StreamReader(fileStream);
            filerows = filereader.ReadToEnd();

            return filerows;
                
        }

        public bool CopyFile(string SourceFile, string DestinationFile)
        {
            strFunctionName = "Copy File";

            lock (this)
            {
                try
                {
                    bool FileExists = File.Exists(SourceFile);

                    if (FileExists == true)
                    {
                        File.Copy(SourceFile, DestinationFile);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public bool DeleteFile(string FileName)
        {
            strFunctionName = "Delete File";

            lock (this)
            {
                try
                {
                    bool FileExists = File.Exists(FileName);

                    if (FileExists == true)
                    {
                        File.Delete(FileName);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public bool MoveFile(string SourceFile, string DestinationFile)
        {
            strFunctionName = "Move File";

            lock (this)
            {
                try
                {
                    bool FileExists = File.Exists(SourceFile);

                    if (FileExists == true)
                    {
                        File.Move(SourceFile, DestinationFile);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }


        public string DeleteFileFromFTP(string Location, string fileName, string Uname, string Pwd)
        {
            strFunctionName = "Delete Files from FTP";

            string reponseStatus = string.Empty;
            string url = string.Empty;

            try
            {
                url = Location + "//" + fileName;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(Uname, Pwd);
               
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    reponseStatus = response.StatusDescription;
                    
                    return reponseStatus;
                }
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return reponseStatus;
            }

        }

       public bool DownloadFTPFile(string Uname,string Pwd,string Location,string FileNm,string DownloadPath)
        {

            strFunctionName = "Download  Files from FTP";

            try
            {
                WebClient request = new WebClient();

                request.Credentials = new NetworkCredential(Uname, Pwd);

                byte[] fileData = request.DownloadData(Location + "/" + FileNm);

                FileStream file = File.Create(DownloadPath + "\\" + FileNm);

                file.Write(fileData, 0, fileData.Length);

                file.Close();

                return true;
            }
            catch(Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return false;
            }
        }

        public bool UploadFileToFTP(string destinationFilePath,string sourceFilePath,string Uname,string Pwd)
        {
            strFunctionName = "Upload Files from FTP";

            try
            {
                using (WebClient ftpClient = new WebClient())
                {
                    ftpClient.Credentials = new NetworkCredential(Uname, Pwd);

                    ftpClient.UploadFile(destinationFilePath, sourceFilePath);

                    string[] Filenm = null;
                    string Loc = string.Empty;

                    Filenm = sourceFilePath.Split(new[] { "\\" }, StringSplitOptions.None);

                    //Loc is the folder path....

                    for (int l = 0; l < Filenm.Length - 1; l++)
                        if (l < Filenm.Length)
                            Loc += Filenm[l] + "\\";

                    bool FlgDel =  DeleteFile(sourceFilePath);


                    /* -----------------Upload the File  &  Delete The Uploaded File from local download Folder---------------- */
                    /*
                    byte[] byteArr = ftpClient.UploadFile(destinationFilePath, sourceFilePath);

                    string[] Filenm = null;
                    string Loc = string.Empty;

                    if (byteArr.Length > 0)
                    {
                        Filenm = sourceFilePath.Split(new[] { "\\" }, StringSplitOptions.None);

                        //Loc is the folder path....

                        for (int l = 0; l < Filenm.Length - 1; l++)
                            if (l < Filenm.Length)
                                Loc += Filenm[l] + "\\";
                       
                        //DeleteFileFromFTP(Loc,Filenm[Filenm.Length - 1],Uname,Pwd);
                    }
                    */

                }
                return true;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return false;
            }

        }

        public bool Send_File_HTTP(string HTTP_address, string SendData)
        {
            try
            {                
                using (var client = new HttpClient())
                {
                    //var res = client.PostAsync("http://aws.azistaaerospace.com/receiverpost", new StringContent(output3[1] + "\n"));
                    var res = client.PostAsync(HTTP_address, new StringContent(SendData+ "\n"));
                    try
                    {
                        res.Result.EnsureSuccessStatusCode();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        return false;
                    }
                }

            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;
                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);
                return false;
            }

        }

        public bool CheckIfFileExistsOnServer(string ftpPath ,string fileName,string Uname, string Pwd)
        {
            var request = (FtpWebRequest)WebRequest.Create(ftpPath + "//" +fileName);
            request.Credentials = new NetworkCredential(Uname,Pwd);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch(WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                     return false;
            }
            catch(Exception Ex)
            {
                Ex.ToString();
                return false;
            }
            return false;
        }

        public bool RenameFileInFTP(string SourceFilePath,string DestinationFilePath,string Uname,string Password)
        {
            strFunctionName = "Rename Files";

            //FtpWebRequest ftpRequest = null;
            //FtpWebResponse ftpResponse = null;
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(SourceFilePath);  //ftp://mysite.com/folder1/fileName.ext
                ftpRequest.Credentials = new NetworkCredential(Uname,Password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpRequest.RenameTo = DestinationFilePath; //"/folder2/fileName.ext";
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
                return true;
            }
            catch (Exception Ex)
            {
                // String status = ((FtpWebResponse)e.Response).StatusDescription;
                //strExceptionMessage = status;

                strExceptionMessage = Ex.ToString();

                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return false;
            }
        }

        public bool RenameFileInFTPReqObj(FtpWebRequest ftpRequest ,string SourceFilePath,string DestinationFilePath)
        {
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(SourceFilePath);  //ftp://mysite.com/folder1/fileName.ext
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpRequest.RenameTo = DestinationFilePath;            //"/folder2/fileName.ext";
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
                return true;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return false;
            }

        }

        public bool RenameFile(string OldFileName, string NewFileName)
        {
            strFunctionName = "Rename File";

            lock (this)
            {
                try
                {
                    bool FileExists = File.Exists(OldFileName);

                    if (FileExists == true)
                    {
                        File.Move(OldFileName, NewFileName);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public string FileLocation()
        {
            strFunctionName = "File Location";

            lock (this)
            {
                try
                {
                    return "C:\\";
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return "";
                }
            }
        }

        public string CreateDataFile(string tmpPath, string tmpFileName, string tmpModule)
        {
            strFunctionName = "Create Data File Process";

            lock (this)
            {
                try
                {
                    String DirName = tmpPath.Trim().ToString() + "\\";
                    DirectoryInfo Dir = new DirectoryInfo(DirName);

                    if (!Dir.Exists)
                    {
                        Dir.Create();
                    }

                    if (tmpModule.Trim().ToUpper() == "VALID DATA REPORT")
                    {
                        string tmpHead = "";
                        //tmpHead = tmpHead + "Record No,Ref. Sonde ID,UTC Date,UTC Time,Temperature (C),Humidity (%),Pressure (mbar),Int. Temp. (C),Latitude, Latitude Unit,Longitude,Longitude Unit,Status,Satellite,Altitude (m),Heading (Deg),Speed (knot),RSSI,TRFK,Receiver No,Sonde ID,Batt. Volt. (V),Heading (GPS) (Deg),Speed (GPS) (knot),Pressure Altitude (m),Vertical Speed (m/s),Ground Distance (Km),Slant Range (Km),Azimuth (Deg),Elevation (Deg),Launch (Sec),Res-3,Res-4,System Date,Data Interface";
                        tmpHead = tmpHead + "Record No,Ref. Sonde ID,UTC Date,UTC Time,Temperature (C),Humidity (%),Pressure (mbar),Int. Temp. (C),Latitude, Latitude Unit,Longitude,Longitude Unit,Status,Satellite,Altitude (m),Wind Direction (Deg),Speed (knot),RSSI,TRFK,Receiver No,Sonde ID,Batt. Volt. (V),Heading (GPS) (Deg),Speed (GPS) (knot),Pressure Altitude (m),Vertical Speed (m/s),Ground Distance (Km),Slant Range (Km),Azimuth (Deg),Elevation (Deg),Launch (Sec),Res-3,Res-4,System Date,Data Interface";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "RECEIVED RAW DATA REPORT")
                    {
                        string tmpHead = "";
                        //tmpHead = tmpHead + "Record No,Ref. Sonde ID,SOT,UTC Date,UTC Time,Temperature (C),Humidity (%),Pressure (mbar),Int. Temp. (C),Latitude, Latitude Unit,Longitude,Longitude Unit,Status,Satellite,Altitude (m),Heading (Deg),Speed (knot),RSSI,TRFK,Receiver No,Sonde ID,Batt. Volt. (V),Heading (GPS) (Deg),Speed (GPS) (knot),Res-3,Res-4,EOT,CRC";
                        tmpHead = tmpHead + "Record No,Ref. Sonde ID,SOT,UTC Date,UTC Time,Temperature (C),Humidity (%),Pressure (mbar),Int. Temp. (C),Latitude, Latitude Unit,Longitude,Longitude Unit,Status,Satellite,Altitude (m),Wind Direction (Deg),Speed (knot),RSSI,TRFK,Receiver No,Sonde ID,Batt. Volt. (V),Heading (GPS) (Deg),Speed (GPS) (knot),Res-3,Res-4,EOT,CRC";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "GROUND STATION DATA REPORT")
                    {
                        string tmpHead = "";
                        //tmpHead = tmpHead + "Record No,Ref. Sonde ID,UTC Date,UTC Time,Temperature (C),Humidity (%),Wind Speed(m/s),Wind Direction (Deg),Pressure (mbar),Res-1,Res-2,Res-3";
                        tmpHead = tmpHead + "Record No,Ref. Sonde ID,UTC Date,UTC Time,Temperature (C),Humidity (%),Wind Speed(knot),Wind Direction (Deg),Pressure (mbar),Res-1,Res-2,Res-3";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "INVALID DATA REPORT")
                    {
                        string tmpHead = "";
                        tmpHead = tmpHead + "Record No,Ref. Sonde ID,Table Name,Remarks,Data Interface,Received Data";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "RAOB DATA REPORT")
                    {
                        //string tmpHead = "";
                        //tmpHead = tmpHead + "UTC Date, UTC Time,Temperature( °C ),RH( % ), Pressure(millibar),Internal Temperature( °C ),Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude(Metre), Heading(° Due True North), Speed(Knot)";
                        //tmpHead = tmpHead + System.Environment.NewLine;

                        WriteIntoFile(tmpPath, tmpFileName, "");
                        //WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "STANAG DATA REPORT")
                    {
                        string tmpHead = "";
                        tmpHead = tmpHead + "UTC Date, UTC Time,Temperature,Humidity, Pressure,Internal Temperature,Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude,Altitude Unit, Heading, Heading Unit, Speed, Speed Unit,RSSI,TRkreg.,Receiver No.";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "RAW DATA REPORT")
                    {
                        string tmpHead = "";
                        //tmpHead = tmpHead + "UTC Date, UTC Time,Temperature,Humidity, Pressure,Internal Temperature,Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude,Altitude Unit, Heading, Heading Unit, Speed, Speed Unit,RSSI,TRkreg.,Receiver No.,Dew Point";
                        //tmpHead = tmpHead + "UTC Date, UTC Time,Temperature,Humidity, Pressure,Internal Temperature,Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude,Altitude Unit, Heading, Heading Unit, Speed, Speed Unit,Dew Point";
                        tmpHead = tmpHead + "UTC Date, UTC Time,Temperature,Humidity, Pressure,Internal Temperature,Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude,Altitude Unit, Heading, Heading Unit, Speed, Speed Unit,Dew Point, Derieved Pressure";  ////change on 27-Feb-2016 (as per discussion with Nirajbhai)
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }

                    return "OK";
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return strExceptionMessage;
                }
            }
        }

        public List<string> ConnectionWithFTP(string HostNm ,string Uname,string Pwd)
        {
            strFunctionName = "Connection Status With FTP Server";

            List<string> directories = null;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(HostNm);
                request.Credentials = new NetworkCredential(Uname, Pwd);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                 directories = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                        directories.Add(line);
                        line = streamReader.ReadLine();
                    
                }

                streamReader.Close();

                //  request.GetResponse();
                 return directories;
            }
            catch (WebException Ex)
            {
                strExceptionMessage = Ex.Message;

                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return directories;
            }
         
        }

        public FtpWebRequest ConnectionWithFTP_ReqObj(string HostNm, string Uname, string Pwd)
        {
            strFunctionName = "Connection Status With FTP Server";

            FtpWebRequest request = null;

            try
            {
                request = (FtpWebRequest)WebRequest.Create(HostNm);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(Uname, Pwd);
                request.GetResponse();
                return request;
            }
            catch (WebException Ex)
            {
                strExceptionMessage = Ex.Message;

                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return request;
            }

        }

        public List<string> ListFiles(string HostNm, string Uname, string Pwd)
        {
            strFunctionName = " Get List of Files From FTP Server";

            List<string> fileList = null;

            try
            {
                //string Url = string.Empty;
                //Url = HostNm + FolderNm;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(HostNm);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential(Uname, Pwd);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                string[] stringSeparators = new string[] { "\r\n" };
                string[] fileArr = names.Split(stringSeparators, StringSplitOptions.None);

                fileList = new List<string>();
                for (int i = 0; i < fileArr.Length - 1; i++)
                    fileList.Add(fileArr[i]);

                return fileList;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return fileList;
            }
        }

        public string ListFilesWithReqObj(FtpWebRequest request,string Location,string Uname,string Pwd)
        {
            strFunctionName = " Get List of Files From FTP Server";

            List<string> fileList = null;

            string filerows = string.Empty;

            try
            {
                //string Url = string.Empty;
                //Url = HostNm + FolderNm;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                string[] stringSeparators = new string[] { "\r\n" };
                string[] fileArr = names.Split(stringSeparators, StringSplitOptions.None);

                fileList = new List<string>();
                for (int i = 0; i < fileArr.Length - 1; i++)
                    fileList.Add(fileArr[i]);

                for (int f = 0; f < fileList.Count; f++)
                {
                    request = (FtpWebRequest)WebRequest.Create(Location + fileList[f]);
                   // request.Credentials = new NetworkCredential(Uname,Pwd);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.EnableSsl = false;
                    FtpWebResponse FileResponse = (FtpWebResponse)request.GetResponse();
                    Stream fileStream = response.GetResponseStream();
                    StreamReader filereader = new StreamReader(responseStream);
                    filerows = reader.ReadToEnd();
                }

                return filerows;
            }
            catch (Exception Ex)
            {
                strExceptionMessage = Ex.Message;

                bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                return filerows;
            }
        }


        public bool CreateDataFile_FTPServer(string textContent, string ftpUrl, string userName, string password)
        {
            try
            {
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(userName, password);

                // convert contents to byte.
                byte[] fileContents = Encoding.ASCII.GetBytes(textContent); ;
                request.ContentLength = fileContents.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }

                return true;
            }
            catch(Exception Ex)
            {
                Ex.ToString();

                return false;
            }

        }

    }
}
