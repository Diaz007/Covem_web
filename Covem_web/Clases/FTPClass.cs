using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading.Tasks;


namespace Covem_web
{
   


     /// <summary>
        /// FTPServer file is used for Files operation on FTP Server
        /// ftp Server instance should be created inside the calling function...
        /// </summary>
        public class FTPServer
        {
            private string ftpPath;
            private string ftpUserID;
            private string ftpPassword;
            private string ftpDownloadPath;

            public FTPServer()
            {
                ftpPath = "ftp://Covemu@vtreact.com/";
                ftpUserID = "Covemu";
                ftpPassword = "2M5^cuo86";
                ftpDownloadPath = "";
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="folderNameWithCompletePath"></param>
            /// <returns></returns>
            public async Task CreateDirectory(string folderNameWithCompletePath)
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpPath + folderNameWithCompletePath);

                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = true;

                FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
                int statuscode = (int)response.StatusCode;
                response.Close();
            }

            /// <summary>
            /// Delete File from FTP 
            /// </summary>
            /// <param name="fileNameWithDestinationPath">File name that will delete from FTP</param>
            /// <returns></returns>
            public async Task DeleteFile(string fileNameWithDestinationPath)
            {
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpPath + fileNameWithDestinationPath);
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.KeepAlive = true;
                ftp.UseBinary = true;
                ftp.UsePassive = true;
                ftp.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)await ftp.GetResponseAsync();
                response.Close();
            }

            /// <summary>
            /// Featch the name of all file names
            /// </summary>
            /// <returns>File Names</returns>
            public async Task<IList<string>> ListOfFiles()
            {
                List<string> files = new List<string>();
                //Create FTP request
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpPath);

                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                while (!reader.EndOfStream)
                {
                    files.Add(await reader.ReadLineAsync());
                }
                reader.Close();
                responseStream.Close();
                response.Close();
                return files;
            }




            /// <summary>
            /// Recommended for reading small file 
            /// Because we are creating in Memory Buffer and that buffer transfer to the Client...
            /// </summary>
            /// <param name="fileNameWithPath"></param>
            /// <returns></returns>
            public async Task<byte[]> DownloadFile(string fileNameWithPath)
            {
                int bytesRead = 0;
                byte[] buffer = new byte[2048];
                byte[] fileData = null;
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpDownloadPath + fileNameWithPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                Stream reader = request.GetResponse().GetResponseStream();
                using (MemoryStream ms = new MemoryStream())
                {
                    while (true)
                    {
                        bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;
                        ms.Write(buffer, 0, bytesRead);
                    }
                    fileData = ms.ToArray();
                }
                //reader.Close();
                return fileData;
            }

            /// <summary>
            /// This method will return the Stream object 
            /// and we can download a large file
            /// </summary>
            /// <param name="fileNameWithPath"></param>
            /// <returns></returns>
            public async Task<Stream> DownloadLargeFile(string fileNameWithPath)
            {
                return await Task.Run(() =>
                {
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpPath + fileNameWithPath);
                    request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    Stream reader = request.GetResponse().GetResponseStream();
                    return reader;
                });
            }



        /// <summary>
        /// Upload File On FTP Server
        /// </summary>
        /// <param name="fileName">New File name </param>
        /// <param name="source">Byte Array for new File</param>
        /// <returns></returns>
        public async Task UploadFile(string fileName, byte[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Soruce");
            }
            else if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("FileName");
            }
            try
            {
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpPath + fileName);
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.KeepAlive = true;
                ftp.UseBinary = true;
                ftp.UsePassive = true;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;
                Stream ftpstream = ftp.GetRequestStream();
                int bufferSize = 2048; // Byte on single go
                int pointer = 0;
                while (source.Length > (bufferSize * pointer))
                {
                    bufferSize = (source.Length < (bufferSize * (pointer + 1))) ? source.Length - (bufferSize * (pointer)) : bufferSize;
                    await ftpstream.WriteAsync(source, pointer, bufferSize);
                    pointer++; // Pointing to next Byte blocks
                }
                ftpstream.Close();
            }
            catch (Exception e)
            { }
            }

            /// <summary>
            /// Upload File on FTP Server
            /// 
            /// </summary>
            /// <param name="fileName">New Uploaded file name</param>
            /// <param name="stream">File Stream </param>
            /// <returns>Task Instance </returns>
            public async Task UploadFile(string fileName, Stream stream)
            {
                if (stream == null)
                {
                    throw new ArgumentNullException("Stream");
                }
                else if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new ArgumentNullException("FileName");
                }
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpPath + fileName);
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.KeepAlive = true;
                ftp.UseBinary = true;
                ftp.UsePassive = true;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;
                Stream ftpstream = ftp.GetRequestStream();

                byte[] buffer = new byte[2048];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await ftpstream.WriteAsync(buffer, 0, bytesRead);
                }
                ftpstream.Close();
            }
        }
    
}