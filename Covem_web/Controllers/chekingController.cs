using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covem_web.Controllers
{
    public class chekingController : Controller
    {
        vtreact_Covume2Entities db = new Covem_web.vtreact_Covume2Entities();
        // GET: cheking
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PDF(int id)
        {


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "rptCheking.rpt"));


            // var obras = db.Obras.ToList();
            MiClase.Reporteconexionportabla(rd);
            //  rd.SetDataSource(obras);
            rd.SetParameterValue(0, id);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Reporte" + id + ".pdf");
        }

        public ActionResult Admin()
        {
            List<Vista_Cheking> lista = new List<Vista_Cheking>();


            foreach (var item in db.Cheking.Where(x=>x.activo==true))
            {
                Vista_Cheking vista = new Vista_Cheking();

                vista.cheking = item;
                vista.auto = db.Auto.Find(item.idAuto);

                vista.persona = db.Persona.Find(item.idPersona);
                
                vista.Marca = db.Marca.Find(vista.auto.idmarca);
                vista.modelo = db.Modelo.Find(vista.auto.idmodelo);

                lista.Add(vista);
            }
            return View(lista);
        }


        // GET: cheking/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: cheking/Create
        public ActionResult Nuevo()
        {
            Persona persona = db.Persona.Where(x => x.Correo == User.Identity.Name).FirstOrDefault();
            Vista_Cheking vista = new Vista_Cheking();
            vista.persona = persona;
            vista.auto = db.Auto.Find(persona.idAuto);
            vista.Marca = db.Marca.Find(vista.auto.idmarca);
            vista.modelo = db.Modelo.Find(vista.auto.idmodelo);
            vista.lista = db.Persona_Archivo.Where(x => x.idPersona == persona.Id).ToList();
            return View(vista);
        }

        // POST: cheking/Create
        [HttpPost]
        public ActionResult Nuevo(Vista_Cheking vista,FormCollection collection, HttpPostedFileBase image, HttpPostedFileBase image2, HttpPostedFileBase image3, HttpPostedFileBase image4)
        {
            try
            {
                // TODO: Add insert logic here

                Cheking cheking = new Cheking();

                cheking.idAuto = vista.auto.id;
                cheking.idPersona =vista.persona.Id;
                try { cheking.aceite = Convert.ToInt32(collection["rating"]); }
                catch { cheking.aceite = 0; }

                try { cheking.gasolina = Convert.ToInt32(collection["rating2"]); }
                catch { cheking.gasolina = 0; }
               
                cheking.activo = true;
                cheking.fecha = DateTime.Now;
                
                cheking.kilometrajeActual = vista.cheking.kilometrajeActual;

                Auto auto = db.Auto.Find(vista.auto.id);
                auto.KilometrajeActual = vista.cheking.kilometrajeActual;

                cheking.notas = vista.cheking.notas;
                cheking.valordoc1 =Convert.ToBoolean( collection["cv1"]);
                cheking.valordoc2 = Convert.ToBoolean(collection["cv2"]);
                cheking.valordoc3 = Convert.ToBoolean(collection["cv3"]);
                cheking.valordoc4 = Convert.ToBoolean(collection["cv4"]);
                //interior
                cheking.valorint1 = Convert.ToBoolean(collection["cv5"]);
                cheking.valorint2 = Convert.ToBoolean(collection["cv6"]);
                cheking.valorint3 = Convert.ToBoolean(collection["cv7"]);
                cheking.valorint4 = Convert.ToBoolean(collection["cv8"]);
                cheking.valorint5 = Convert.ToBoolean(collection["cv9"]);
                cheking.valorint6 = Convert.ToBoolean(collection["cv10"]);
                cheking.valorint7 = Convert.ToBoolean(collection["cv11"]);
                cheking.valorint8 = Convert.ToBoolean(collection["cv12"]);
                //0                                                                            
                cheking.valormot1 = Convert.ToBoolean(collection["cv13"]);
                cheking.valormot2 = Convert.ToBoolean(collection["cv14"]);
                cheking.valormot3 = Convert.ToBoolean(collection["cv15"]);
                cheking.valormot4 = Convert.ToBoolean(collection["cv16"]);
                //partes
                cheking.valorpart1 = Convert.ToBoolean(collection["cv17"]);
                cheking.valorpart2 = Convert.ToBoolean(collection["cv18"]);
                cheking.valorpart3 = Convert.ToBoolean(collection["cv19"]);
                cheking.valorpart4 = Convert.ToBoolean(collection["cv20"]);
                //
                cheking.valorex1 = Convert.ToBoolean(collection["cv21"]);
                cheking.valorex2 = Convert.ToBoolean(collection["cv22"]);
                cheking.valorex3 = Convert.ToBoolean(collection["cv23"]);
                cheking.valorex4 = Convert.ToBoolean(collection["cv24"]);

                if (image != null)
                {
                    var ImageMimeType = image.ContentType;
                    var ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(ImageData, 0, image.ContentLength);
                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image.FileName, "Covemu", "2M5^cuo86", ImageData);

                   
                    cheking.FotoA = "https://covemu.vtreact.com/archivos/" + image.FileName;
                   
                }
                if (image2 != null)
                {
                    var ImageMimeType = image2.ContentType;
                    var ImageData = new byte[image2.ContentLength];
                    image2.InputStream.Read(ImageData, 0, image2.ContentLength);
                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image2.FileName, "Covemu", "2M5^cuo86", ImageData);
                    cheking.FotoB = "https://covemu.vtreact.com/archivos/" + image2.FileName;
                }
                if (image3 != null)
                {
                    var ImageMimeType = image3.ContentType;
                    var ImageData = new byte[image3.ContentLength];
                    image3.InputStream.Read(ImageData, 0, image3.ContentLength);
                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image3.FileName, "Covemu", "2M5^cuo86", ImageData);
                    cheking.FotoC = "https://covemu.vtreact.com/archivos/" + image3.FileName;
                }
                if (image4 != null)
                {
                    var ImageMimeType = image4.ContentType;
                    var ImageData = new byte[image4.ContentLength];
                    image4.InputStream.Read(ImageData, 0, image4.ContentLength);
                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image4.FileName, "Covemu", "2M5^cuo86", ImageData);
                    cheking.FotoD = "https://covemu.vtreact.com/archivos/" + image4.FileName;
                }




                db.Cheking.Add(cheking);
                db.SaveChanges();

                return RedirectToAction("index", "Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: cheking/Edit/5
        public ActionResult Edit(int id)
        {
            
            Vista_Cheking vista = new Vista_Cheking();
            vista.cheking= db.Cheking.Find(id);
            Persona persona = db.Persona.Find(vista.cheking.idPersona);
            vista.persona = persona;
            vista.auto = db.Auto.Find(persona.idAuto);
            vista.Marca = db.Marca.Find(vista.auto.idmarca);
            vista.modelo = db.Modelo.Find(vista.auto.idmodelo);
            vista.lista = db.Persona_Archivo.Where(x => x.idPersona == persona.Id).ToList();
            return View(vista);
        }

        // POST: cheking/Edit/5
        [HttpPost]
        public ActionResult Edit(Vista_Cheking vista, FormCollection collection, HttpPostedFileBase image, HttpPostedFileBase image2, HttpPostedFileBase image3, HttpPostedFileBase image4)
        {
            try
            {
                // TODO: Add insert logic here

                Cheking cheking = db.Cheking.Find(vista.cheking.id);

                cheking.idAuto = vista.auto.id;
                cheking.idPersona = vista.persona.Id;
                try { cheking.aceite = Convert.ToInt32(collection["rating"]); }
                catch { cheking.aceite = 0; }

                try { cheking.gasolina = Convert.ToInt32(collection["rating2"]); }
                catch { cheking.gasolina = 0; }

                cheking.activo = true;
                cheking.fecha = DateTime.Now;

                cheking.kilometrajeActual = vista.cheking.kilometrajeActual;

                Auto auto = db.Auto.Find(vista.auto.id);
                auto.KilometrajeActual = vista.cheking.kilometrajeActual;

                cheking.notas = vista.cheking.notas;
                cheking.valordoc1 = Convert.ToBoolean(collection["cv1"]);
                cheking.valordoc2 = Convert.ToBoolean(collection["cv2"]);
                cheking.valordoc3 = Convert.ToBoolean(collection["cv3"]);
                cheking.valordoc4 = Convert.ToBoolean(collection["cv4"]);
                //interior
                cheking.valorint1 = Convert.ToBoolean(collection["cv5"]);
                cheking.valorint2 = Convert.ToBoolean(collection["cv6"]);
                cheking.valorint3 = Convert.ToBoolean(collection["cv7"]);
                cheking.valorint4 = Convert.ToBoolean(collection["cv8"]);
                cheking.valorint5 = Convert.ToBoolean(collection["cv9"]);
                cheking.valorint6 = Convert.ToBoolean(collection["cv10"]);
                cheking.valorint7 = Convert.ToBoolean(collection["cv11"]);
                cheking.valorint8 = Convert.ToBoolean(collection["cv12"]);
                //0                                                                            
                cheking.valormot1 = Convert.ToBoolean(collection["cv13"]);
                cheking.valormot2 = Convert.ToBoolean(collection["cv14"]);
                cheking.valormot3 = Convert.ToBoolean(collection["cv15"]);
                cheking.valormot4 = Convert.ToBoolean(collection["cv16"]);
                //partes
                cheking.valorpart1 = Convert.ToBoolean(collection["cv17"]);
                cheking.valorpart2 = Convert.ToBoolean(collection["cv18"]);
                cheking.valorpart3 = Convert.ToBoolean(collection["cv19"]);
                cheking.valorpart4 = Convert.ToBoolean(collection["cv20"]);
                //
                cheking.valorex1 = Convert.ToBoolean(collection["cv21"]);
                cheking.valorex2 = Convert.ToBoolean(collection["cv22"]);
                cheking.valorex3 = Convert.ToBoolean(collection["cv23"]);
                cheking.valorex4 = Convert.ToBoolean(collection["cv24"]);
                cheking.Revicion = "REVISADO";
                if (image != null)
                {
                    var ImageMimeType = image.ContentType;
                    var ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(ImageData, 0, image.ContentLength);
                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image.FileName, "Covemu", "2M5^cuo86", ImageData);


                    cheking.FotoA = "https://covemu.vtreact.com/archivos/" + image.FileName;

                }
                if (image2 != null)
                {
                    var ImageMimeType = image2.ContentType;
                    var ImageData = new byte[image2.ContentLength];
                    image2.InputStream.Read(ImageData, 0, image2.ContentLength);
                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image2.FileName, "Covemu", "2M5^cuo86", ImageData);
                    cheking.FotoB = "https://covemu.vtreact.com/archivos/" + image2.FileName;
                }
                if (image3 != null)
                {
                    var ImageMimeType = image3.ContentType;
                    var ImageData = new byte[image3.ContentLength];
                    image3.InputStream.Read(ImageData, 0, image3.ContentLength);
                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image3.FileName, "Covemu", "2M5^cuo86", ImageData);
                    cheking.FotoC = "https://covemu.vtreact.com/archivos/" + image3.FileName;
                }
                if (image4 != null)
                {
                    var ImageMimeType = image4.ContentType;
                    var ImageData = new byte[image4.ContentLength];
                    image4.InputStream.Read(ImageData, 0, image4.ContentLength);
                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image4.FileName, "Covemu", "2M5^cuo86", ImageData);
                    cheking.FotoD = "https://covemu.vtreact.com/archivos/" + image4.FileName;
                }




              //  db.Cheking.Add(cheking);
                db.SaveChanges();

                return RedirectToAction("admin", "cheking");
            }
            catch
            {
                return View();
            }
        }

        // GET: cheking/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: cheking/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
