using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Covem_web
{
    public class MiClase
    {

        public static void Reporteconexionportabla(ReportDocument Documento, string NombreConexion = "InmobiliariaEntities", string Base = "")
        {
            //rd.SetDatabaseLogon("Usuario", "Contraseña", , );

            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[NombreConexion].ToString());
            ConnectionInfo connectionInfo = new ConnectionInfo();
            connectionInfo.DatabaseName = "vtreact_Inmobiliaria";
            connectionInfo.UserID = "vtreact_vtreact";
            connectionInfo.Password = "Gacom1983@";
            connectionInfo.ServerName = "database.negox.com";

            if (Base != "")
                connectionInfo.DatabaseName = Base;

            Tables tables = Documento.Database.Tables;

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(tableLogonInfo);
            }
        }


        public static List<SelectListItem> PopulateDropDown(string query, string textColumn, string valueColumn)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr[textColumn].ToString(),
                                Value = sdr[valueColumn].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
        public static string UploadFile(string FtpUrl, string fileName, string userName, string password, byte[] data, string UploadDirectory = "")
        {
            try
            {
                string PureFileName = new FileInfo(fileName).Name;
                String uploadUrl = String.Format("{0}{1}/{2}", FtpUrl, UploadDirectory, PureFileName);
                FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
                req.Proxy = null;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(userName, password);
                req.UseBinary = true;
                req.UsePassive = true;
                // byte[] data =;
                req.ContentLength = data.Length;
                Stream stream = req.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                FtpWebResponse res = (FtpWebResponse)req.GetResponse();
                return res.StatusDescription;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static Image BajarCalidad(Image source)
        {
            using (var target = new Bitmap(source.Width, source.Height))
            {
                using (var g = System.Drawing.Graphics.FromImage(target))
                {
                    g.DrawImage(source, 0, 0, source.Width, source.Height);
                }

                return (Image)target.Clone();
            }


        }
        public static Image ConvertByteArrayToImage(byte[] byteArrayIn)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
            {
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
        }
        public static byte[] ConvertImageToByteArray(System.Drawing.Image imageIn)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}