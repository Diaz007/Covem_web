using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covem_web.Controllers
{
    public class AutosController : Controller
    {
        vtreact_Covume2Entities db = new vtreact_Covume2Entities();
        // GET: Autos
        public ActionResult PDF(int id)
        {


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "Asistencia.rpt"));


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

            return File(stream, "application/pdf", "Asistencia" + id + ".pdf");
        }

        public ActionResult Index()
        {
            var autos = db.Auto.Where(x=>x.activo==true).ToList();
            Automoviles automoviles = new Automoviles();
            List<Automoviles> Lautomoviles = new List<Automoviles>();
            foreach (var item in autos)
            {
                automoviles = new Automoviles();
                automoviles.auto = item;
                automoviles.marca = db.Marca.Where(x => x.id == item.idmarca).FirstOrDefault();
                automoviles.modelo = db.Modelo.Where(x => x.id == item.idmodelo).FirstOrDefault();
                automoviles.persona = db.Persona.Where(z => z.idAuto == item.id).FirstOrDefault();
                Lautomoviles.Add(automoviles);
            }

            return View(Lautomoviles.ToList());
        }
        [HttpPost]
        public ActionResult NuevoDocumento(FormCollection collection, HttpPostedFileBase image)
        {
            Auto_Archivo archivo = new Auto_Archivo();
            archivo.activo = true;
            archivo.archivo = "";
            archivo.estatus= collection["Vigencia"];
            archivo.idAuto =Convert.ToInt32( collection["auto.id"]);
            archivo.nombre = collection["Usuario"];
            archivo.notas = collection["notas"];
            archivo.fecha = DateTime.Now;
            archivo.Vigencia =Convert.ToDateTime( collection["fechaEntrada"]);
            if (image != null)
            {
                var ImageMimeType = image.ContentType;
                var ImageData = new byte[image.ContentLength];

                string path = Server.MapPath("~/Archivos/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string nombre = "ARA" + DateTime.Now.ToString("yyyyMMddhhmmss");
                string Extension = System.IO.Path.GetExtension(image.FileName);
                image.SaveAs(path + Path.GetFileName(nombre+ Extension));
                ViewBag.Message = "File uploaded successfully.";
                //MiClase.UploadFile("ftp://Covemu.vtreact.com/", image.FileName, "Covemu", "2M5^cuo86", ImageData);



                archivo.archivo = "Archivos/" + nombre + Extension;//"https://covemu.vtreact.com/Autos/" + image.FileName;
                db.Auto_Archivo.Add(archivo);
                db.SaveChanges();
            }

            return RedirectToAction("Documentacion/" + archivo.idAuto);
        }
        public ActionResult Documentacion(int id, int  bandera = 0,int iddocumento=0)
        {



            try
            {
                if (iddocumento > 0)
                {
                    Auto_Archivo archivo = db.Auto_Archivo.Find(iddocumento);
                    archivo.activo = false;
                    db.SaveChanges();
                }


                Auto auto = db.Auto.Find(id);


                Automoviles automoviles = new Automoviles();
                automoviles.auto = auto;
                automoviles.Fotos = db.auto_Fotos.ToList();
                automoviles.marca = db.Marca.Where(x => x.id == auto.idmarca).FirstOrDefault();
                automoviles.modelo = db.Modelo.Where(x => x.id == auto.idmodelo).FirstOrDefault();
                automoviles.persona = db.Persona.Where(x => x.idAuto == id).FirstOrDefault();
                automoviles.Larchivo = db.Auto_Archivo.Where(x=>x.activo==true).Where(x => x.idAuto == id).ToList();
                automoviles.bandera = 0;
                if (automoviles.Larchivo == null)
                    automoviles.Larchivo = new List<Auto_Archivo>();
                if (bandera>0)
                    automoviles.bandera = 1;

                automoviles.archivo = new Auto_Archivo();
                automoviles.EntregaRecepcion = new EntregaRecepcion_Vehiculo();
                return View(automoviles);
            }
            catch { return View(); }
        }
        // GET: Autos/Asignar/5
        public ActionResult Asignar(int id,int idpersona=0,int opcion =0)
        {
             Auto auto = db.Auto.Find(id);
           

            Automoviles automoviles = new Automoviles();
            automoviles.auto = auto;
            automoviles.Fotos = db.auto_Fotos.Where(x=>x.idauto==id).ToList();
            automoviles.marca = db.Marca.Where(x => x.id == auto.idmarca).FirstOrDefault();
            automoviles.modelo = db.Modelo.Where(x => x.id == auto.idmodelo).FirstOrDefault();
            if (idpersona > 0)
            {
                automoviles.persona = db.Persona.Find(idpersona);
                Persona persona = db.Persona.Find(idpersona);
                if (opcion == 0)
                    persona.idAuto = id;
                else
                    persona.idAuto = 0;
                db.SaveChanges();
            }
            automoviles.P_disponibles = db.Persona.Where(p => p.idAuto == 0).ToList();
            automoviles.persona = db.Persona.Where(x => x.idAuto == id).FirstOrDefault();
            return View(automoviles);
        }
        public ActionResult estacionamiento(int id)
        {
            Auto auto = db.Auto.Find(id);
            auto.NumeroEstacionamiento = null;
            db.SaveChanges();

           
            return RedirectToAction("asignar/" + id);
        }
        [HttpPost]
        public ActionResult estacionamiento(Automoviles automovil, FormCollection collection) {

            
            Auto auto = db.Auto.Find(automovil.auto.id);
            if (automovil.auto.NumeroEstacionamiento > 0)
            {
                string d = collection["tipolist"];
                auto.NumeroEstacionamiento = automovil.auto.NumeroEstacionamiento;
                auto.TipoEstacionamiento = d;
                db.SaveChanges();
            }
            else
            {
                auto.NumeroEstacionamiento = null;
                db.SaveChanges();
            }

            return RedirectToAction("asignar/" + auto.id);
        }

        // GET: Autos/Details/5
        public ActionResult Details(int id)
        {
            Auto auto = db.Auto.Find(id);
            ViewBag.Marca = db.Marca.Find(auto.idmarca).Marca1.ToString();
            ViewBag.Modelo = db.Modelo.Find(auto.idmodelo).modelo1;
            ViewBag.Imagenes = db.auto_Fotos.ToList();


            Automoviles automoviles = new Automoviles();
            automoviles.auto = auto;
            automoviles.Fotos = db.auto_Fotos.ToList();
            return View(automoviles);
        }
        [HttpPost]
        public ActionResult vImagen(FormCollection collection, HttpPostedFileBase image)
        {
            string id= collection["auto.id"];
            if (image != null)
            {
                var ImageMimeType = image.ContentType;
                var ImageData = new byte[image.ContentLength];
                image.InputStream.Read(ImageData, 0, image.ContentLength);
                Image image1 = MiClase.ConvertByteArrayToImage(ImageData);

                image1 = MiClase.BajarCalidad(image1);
                ImageData = MiClase.ConvertImageToByteArray(image1);

                MiClase.UploadFile("ftp://Covemu.vtreact.com/", image.FileName, "Covemu", "2M5^cuo86", ImageData);
               

                auto_Fotos _Fotos = new auto_Fotos();
                _Fotos.activo = true;
                _Fotos.idauto = Convert.ToInt32(id);
                _Fotos.url= "https://covemu.vtreact.com/Autos/" + image.FileName;
                db.auto_Fotos.Add(_Fotos);
                db.SaveChanges();
            }
            return RedirectToAction("Fotos/" + id);
        }
        [HttpPost]
        public string vImagen2( HttpPostedFileBase image)
        {
           
            if (image != null)
            {
                var ImageMimeType = image.ContentType;
                var ImageData = new byte[image.ContentLength];
                image.InputStream.Read(ImageData, 0, image.ContentLength);
                Image image1 = MiClase.ConvertByteArrayToImage(ImageData);

                image1 = MiClase.BajarCalidad(image1);
                ImageData = MiClase.ConvertImageToByteArray(image1);

                MiClase.UploadFile("ftp://Covemu@vtreact.com/Autos/", image.FileName, "Covemu", "2M5^cuo86", ImageData);


                return "https://covemu.vtreact.com/Autos/" + image.FileName;
            }
            // return RedirectToAction("Details/" + id);
            return "";
        }
        private List<SelectListItem> _regionsItems;
        // GET: Autos/Create
        public JsonResult GetStates(string id)
        {
            List<SelectListItem> states = new List<SelectListItem>();
            switch (id)
            {
                case "1":
                    states.Add(new SelectListItem { Text = "Select", Value = "0" });
                    states.Add(new SelectListItem { Text = "ANDAMAN & NIKOBAR ISLANDS", Value = "1" });
                    states.Add(new SelectListItem { Text = "ANDHRA PRADESH", Value = "2" });
                    states.Add(new SelectListItem { Text = "ARUNACHAL PRADESH", Value = "3" });
                    states.Add(new SelectListItem { Text = "ASSAM", Value = "4" });
                    states.Add(new SelectListItem { Text = "BIHAR", Value = "5" });
                    states.Add(new SelectListItem { Text = "CHANDIGARH", Value = "6" });
                    states.Add(new SelectListItem { Text = "CHHATTISGARH", Value = "7" });
                    states.Add(new SelectListItem { Text = "DADRA & NAGAR HAVELI", Value = "8" });
                    states.Add(new SelectListItem { Text = "DAMAN & DIU", Value = "9" });
                    states.Add(new SelectListItem { Text = "GOA", Value = "10" });
                    states.Add(new SelectListItem { Text = "GUJARAT", Value = "11" });
                    states.Add(new SelectListItem { Text = "HARYANA", Value = "12" });
                    states.Add(new SelectListItem { Text = "HIMACHAL PRADESH", Value = "13" });
                    states.Add(new SelectListItem { Text = "JAMMU & KASHMIR", Value = "14" });
                    states.Add(new SelectListItem { Text = "JHARKHAND", Value = "15" });
                    states.Add(new SelectListItem { Text = "KARNATAKA", Value = "16" });
                    states.Add(new SelectListItem { Text = "KERALA", Value = "17" });
                    states.Add(new SelectListItem { Text = "LAKSHADWEEP", Value = "18" });
                    states.Add(new SelectListItem { Text = "MADHYA PRADESH", Value = "19" });
                    states.Add(new SelectListItem { Text = "MAHARASHTRA", Value = "20" });
                    states.Add(new SelectListItem { Text = "MANIPUR", Value = "21" });
                    states.Add(new SelectListItem { Text = "MEGHALAYA", Value = "22" });
                    states.Add(new SelectListItem { Text = "MIZORAM", Value = "23" });
                    states.Add(new SelectListItem { Text = "NAGALAND", Value = "24" });
                    states.Add(new SelectListItem { Text = "NCT OF DELHI", Value = "25" });
                    states.Add(new SelectListItem { Text = "ORISSA", Value = "26" });
                    states.Add(new SelectListItem { Text = "PUDUCHERRY", Value = "27" });
                    states.Add(new SelectListItem { Text = "PUNJAB", Value = "28" });
                    states.Add(new SelectListItem { Text = "RAJASTHAN", Value = "29" });
                    states.Add(new SelectListItem { Text = "SIKKIM", Value = "30" });
                    states.Add(new SelectListItem { Text = "TAMIL NADU", Value = "31" });
                    states.Add(new SelectListItem { Text = "TRIPURA", Value = "32" });
                    states.Add(new SelectListItem { Text = "UTTAR PRADESH", Value = "33" });
                    states.Add(new SelectListItem { Text = "UTTARAKHAND", Value = "34" });
                    states.Add(new SelectListItem { Text = "WEST BENGAL", Value = "35" });
                    break;
                case "UK":
                    break;
                case "India":
                    break;
            }
            return Json(new SelectList(states, "Value", "Text"));
        }
        public ActionResult Create()
        {

            CascadingModel model = new CascadingModel();

            model.Modelos= db.Modelo.Where(x => x.idMarca == 1).ToList();
            model.Marcas= db.Marca.ToList();
            _regionsItems = new List<SelectListItem>();
            foreach (var item in db.Marca)
            {
                _regionsItems.Add(new SelectListItem
                {
                    Text = item.Marca1,
                    Value = item.id.ToString()
                }); 
            }
          //  ViewData["country"]    = _regionsItems;
            ViewBag.Modelos = db.Modelo.Where(x => x.idMarca == 1).ToList();
            //  CascadingModel model = new CascadingModel();
            //model.Marca = MiClase.PopulateDropDown("SELECT id, Marca FROM Marca", "Marca", "id");
            //return View(model);
            return View(model);
        }
        public JsonResult GetCity(string id)
        {
            List<SelectListItem> City = new List<SelectListItem>();
            switch (id)
            {
                case "20":
                    City.Add(new SelectListItem { Text = "Select", Value = "0" });
                    City.Add(new SelectListItem { Text = "MUMBAI", Value = "1" });
                    City.Add(new SelectListItem { Text = "PUNE", Value = "2" });
                    City.Add(new SelectListItem { Text = "KOLHAPUR", Value = "3" });
                    City.Add(new SelectListItem { Text = "RATNAGIRI", Value = "4" });
                    City.Add(new SelectListItem { Text = "NAGPUR", Value = "5" });
                    City.Add(new SelectListItem { Text = "JALGAON", Value = "6" });
                    break;

            }

            return Json(new SelectList(City, "Value", "Text"));
        }

        public JsonResult GetModelos(int id)
        {

            List<SelectListItem> ModelosList = new List<SelectListItem>();
            var modelos = db.Modelo.Where(x => x.idMarca == id).ToList();
            foreach (var item in modelos) {
                ModelosList.Add(new SelectListItem { Text = item.modelo1, Value = item.id.ToString() }) ;
            }
            return Json(new SelectList(ModelosList, "Value", "Text"));
            //var territories = db.Modelo.Where(x => x.idMarca == id).ToList();
            //return Json(territories, JsonRequestBehavior.AllowGet);

        }


        // POST: Autos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Auto auto = new Auto();
                auto.año = Convert.ToInt32(collection["txtProduccion"]);
                auto.Color = collection["txtColor"];
                auto.FechaAdquisicion= Convert.ToDateTime(collection["txtFechaN"]);
                auto.idmarca=Convert.ToInt32(collection["SlMarca"]);
                auto.idmodelo = Convert.ToInt32(collection["SlModelo"]);
                auto.placas = collection["txtPlacas"];
                auto.vin=collection["txtMotor"];
                auto.activo = true;
                auto.Estatus = collection["Estatus"];
                auto.estadoFisico = collection["EstatusFisico"];
                auto.NotasFisicas = collection["txtNotaE"];
                auto.Propietario = collection["SlPropietario"];
                db.Auto.Add(auto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Autos/Edit/5
        public ActionResult Edit(int id)
        {
            //    Auto auto = db.Auto.Find(id);
            //         ViewBag.Marca = db.Marca.ToList();
            //   // ViewBag.Modelo = db.Modelo.ToList();
            //    ViewBag.Modelo = db.Modelo.Where(x => x.id == auto.idmodelo).ToList();
            //    //ViewBag.Imagenes = db.auto_Fotos.ToList();


            //    Automoviles automoviles = new Automoviles();
            //    automoviles.auto = auto;
            //    automoviles.Fotos = db.auto_Fotos.Where(x=>x.idauto==id).ToList();
            //    return View(automoviles);
            //}
            CascadingModel model = new CascadingModel();
            model.auto = db.Auto.Find(id);
           
            model.Marcas = db.Marca.ToList();

            model.MarcaId = (int)db.Modelo.Find(model.auto.idmodelo).idMarca;
            model.Modelos = db.Modelo.Where(x => x.idMarca == model.MarcaId).ToList();

            //  ViewData["country"]    = _regionsItems;
            ViewBag.Modelos = db.Modelo.Where(x => x.idMarca == model.MarcaId).ToList();
            return View(model);
        }
        // POST: Autos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                Auto auto = db.Auto.Find(id);
                //auto.año = Convert.ToInt32(collection["txtProduccion"]);
                //auto.Color = collection["txtColor"];
                //auto.FechaAdquisicion = Convert.ToDateTime(collection["txtFechaN"]);
                //auto.idmarca = Convert.ToInt32(collection["SlMarca"]);
                //auto.idmodelo = Convert.ToInt32(collection["SlModelo"]);
                //auto.placas = collection["txtPlacas"];
                // db.Auto.Add(auto);
              //  Auto auto = new Auto();
                auto.año = Convert.ToInt32(collection["txtProduccion"]);
                auto.Color = collection["txtColor"];
                auto.FechaAdquisicion = Convert.ToDateTime(collection["txtFechaN"]);
                auto.idmarca = Convert.ToInt32(collection["SlMarca"]);
                auto.idmodelo = Convert.ToInt32(collection["SlModelo"]);
                auto.placas = collection["txtPlacas"];
                auto.vin = collection["txtMotor"];
                auto.activo = true;
                auto.Estatus = collection["Estatus"];
                auto.estadoFisico = collection["EstatusFisico"];
                auto.NotasFisicas = collection["txtNotaE"];
                auto.Propietario = collection["SlPropietario"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Fotos(int id)
        {
            Auto auto = db.Auto.Find(id);
            ViewBag.Marca = db.Marca.Find(auto.idmarca).Marca1.ToString();
            ViewBag.Modelo = db.Modelo.Find(auto.idmodelo).modelo1;
            ViewBag.Imagenes = db.auto_Fotos.Where(x=>x.idauto==id);


            Automoviles automoviles = new Automoviles();
            automoviles.auto = auto;
            automoviles.Fotos = db.auto_Fotos.Where(x => x.idauto == id).ToList();
            return View(automoviles);
        }

        // GET: Autos/Delete/5
        public ActionResult Delete(int id)
        {
            auto_Fotos auto = db.auto_Fotos.Find(id);
            db.auto_Fotos.Remove(auto);
            db.SaveChanges();

            return RedirectToAction("Fotos/" + auto.idauto);
        }
        // GET: Autos/Delete/5
        public ActionResult DeleteAuto(int id)
        {
            Auto auto = db.Auto.Find(id);
            auto.activo = false;
            db.SaveChanges();
            return RedirectToAction("index");
        }

        // POST: Autos/Delete/5
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
