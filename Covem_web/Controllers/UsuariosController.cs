using Covem_web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Covem_web.Controllers
{

    public class UsuariosController : Controller
    {
        vtreact_Covume2Entities db = new vtreact_Covume2Entities();
        // GET: Usuarios
        public ActionResult Index(string txtNombre="",string SlDepartamento ="")
        {
            var lista = db.Persona.ToList();
            ViewBag.data = db.Departamento.ToList();
            if (txtNombre != "")
            { lista = lista.Where(x => x.nombre.Contains(txtNombre)).ToList(); }
            if (SlDepartamento != "")
            { lista = lista.Where(x => x.idDepartamento == Convert.ToInt32( SlDepartamento)).ToList(); }

            return View(lista);
        }
// GET: Usuarios/Details/5
        public ActionResult miPerfil()
        {

            return View(db.Persona.Where(x => x.Correo == User.Identity.Name).FirstOrDefault()) ;
        }

        // GET: Usuarios/Details/5
        public ActionResult Credenciales(string  correo)
        {
            ViewBag.data = db.Puesto.ToList(); 
           RegisterViewModel model = new RegisterViewModel();
            model.Email = correo;
            return View(model);
        }
        // GET: Usuarios/Details/5
        public ActionResult Archivo(int id)
        {
            Persona_Archivos _Archivos = new Persona_Archivos();
            _Archivos.persona = db.Persona.Find(id);
            _Archivos.archivos = db.Persona_Archivo.Where(x=>x.idPersona==id).ToList();
            return View(_Archivos);
        }
        // GET: Usuarios/Details/5
        [HttpPost]
        public ActionResult Archivo(FormCollection collection, Persona persona, HttpPostedFileBase image)
        {
            try {
                if (image != null)
                {
                    var ImageMimeType = image.ContentType;
                    var ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(ImageData, 0, image.ContentLength);
                    //Image image1 = MiClase.ConvertByteArrayToImage(ImageData);

                    //image1 = MiClase.BajarCalidad(image1);
                    //ImageData = MiClase.ConvertImageToByteArray(image1);

                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image.FileName, "Covemu", "2M5^cuo86", ImageData);

                    Persona_Archivo archivo = new Persona_Archivo();
                    archivo.activo = true;
                    archivo.archivo= "https://covemu.vtreact.com/archivos/" + image.FileName;
                    archivo.estatus = "Activo";
                    archivo.idPersona = persona.Id;
                    archivo.nombre= collection["txtNombre"];
                    archivo.Vigencia =Convert.ToDateTime( collection["txtFechaN"]);
                    db.Persona_Archivo.Add(archivo);
                    db.SaveChanges();
                }

                return RedirectToAction("Archivo/"+persona.Id);
            }
            catch {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Calificar(FormCollection collection, Persona persona, HttpPostedFileBase image)
        {
            try
            {
                string d = collection["rating"];
                string c = collection["persona.observacionCalificacion"];
                persona = db.Persona.Find(persona.Id);
                persona.calificacion = Convert.ToInt32(collection["rating"]);
                persona.observacionCalificacion = c;

                db.SaveChanges();
                return RedirectToAction("Archivo/" + persona.Id);
            }
            catch
            {
              
                return View();
            }
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            ViewBag.data = db.Departamento.ToList();
            return View();
        }
       
        // POST: Usuarios/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, Persona persona, HttpPostedFileBase image)
        {
            try
            {

                if (image != null)
                {
                    var ImageMimeType = image.ContentType;
                    var ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(ImageData, 0, image.ContentLength);
                    Image image1 =MiClase.ConvertByteArrayToImage(ImageData);

                    image1 = MiClase.BajarCalidad(image1);
                    ImageData = MiClase.ConvertImageToByteArray(image1);

                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image.FileName, "Covemu", "2M5^cuo86", ImageData);
                   
                }
                persona = new Persona();
                persona.Imagen = "https://covemu.vtreact.com/archivos/" + image.FileName;

                var Ixmagen = collection["__RequestVerificationToken"];
                //var Imagen = collection["TxtImagen"];
                var nombre = collection["txtNombre"];
                var Apellidos = collection["txtApellidos"];
                var Genero = collection["SlcGenero"];
                var FechaN = collection["txtFechaN"];
                var Telefono = collection["txttelefono"];
                var Licencia = collection["txtLicencia"];
                var Correo = collection["txtcorreo"];
                var referenciac = collection["txttelefono2"];
                var referencianombre = collection["txtPersona2"];
                var departamento = collection["SlDepartamento"];
                var tipoLicencia = collection["txtTipoLicencia"];
                var extencion = collection["txtExtencion"];

                // var Imagen = collection["TxtImagen"];
                persona.nombre = nombre;
                persona.Apellidos = Apellidos;
                persona.Correo = Correo;
                persona.estatus = "Conducor";
                persona.FechaNac =Convert.ToDateTime( FechaN);
                persona.FechaRegistro = DateTime.Now;
                persona.Licencia = Licencia;
                persona.Telefono = Telefono;
                persona.activo = true;
                
                persona.Genero = Genero;
                persona.Referencia = referencianombre;
                persona.ReferenciaNumero = referenciac;
                persona.idDepartamento = Convert.ToInt32(departamento);
                persona.Extencion = extencion;
                persona.TipoLicncia = tipoLicencia; 
                persona.Departamento = db.Departamento.Find(persona.idDepartamento).Departamento1;
                db.Persona.Add(persona);
                db.SaveChanges();

                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                    file.SaveAs(_path);
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }
        // GET: Usuarios/Edit/5
        public ActionResult Editar(int id)
        {
            ViewBag.data = db.Departamento.ToList();
            return View(db.Persona.Find(id));
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        public ActionResult Editar(int id,FormCollection collection, Persona persona, HttpPostedFileBase image)
        {
            try
            {
               // if (persona == null)
               var  aa = collection["Image"];
                persona = db.Persona.Find(id);
                if (image != null)
                {
                    var ImageMimeType = image.ContentType;
                    var ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(ImageData, 0, image.ContentLength);
                    Image image1 = MiClase.ConvertByteArrayToImage(ImageData);

                    image1 = MiClase.BajarCalidad(image1);
                    ImageData = MiClase.ConvertImageToByteArray(image1);

                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/", image.FileName, "Covemu", "2M5^cuo86", ImageData);
                    persona.Imagen = "https://covemu.vtreact.com/archivos/" + image.FileName;
                }
              ///  persona = new Persona();
               

                var Ixmagen = collection["__RequestVerificationToken"];
                //var Imagen = collection["TxtImagen"];
                var nombre = collection["txtNombre"];
                var Apellidos = collection["txtApellidos"];
                var Genero = collection["SlcGenero"];
                var FechaN = collection["txtFechaN"];
                var Telefono = collection["txttelefono"];
                var Licencia = collection["txtLicencia"];
                var Correo = collection["txtcorreo"];
                var referenciac = collection["txttelefono2"];
                var referencianombre = collection["txtPersona2"];
                var departamento = collection["SlDepartamento"];
                var tipoLicencia = collection["txtTipoLicencia"];
                var extencion = collection["txtExtencion"];

                // var Imagen = collection["TxtImagen"];
                persona.nombre = nombre;
                persona.Apellidos = Apellidos;
                persona.Correo = Correo;
                persona.estatus = "Conducor";
                persona.FechaNac = Convert.ToDateTime(FechaN);
                persona.FechaRegistro = DateTime.Now;
                persona.Licencia = Licencia;
                persona.Telefono = Telefono;
                persona.activo = true;
               
                persona.Genero = Genero;
                persona.Referencia = referencianombre;
                persona.ReferenciaNumero = referenciac;
                persona.idDepartamento = Convert.ToInt32(departamento);
                persona.Extencion = extencion;
                persona.TipoLicncia = tipoLicencia;
                persona.Departamento = db.Departamento.Find(persona.idDepartamento).Departamento1;

                //  db.Persona.Add(persona);
                db.SaveChanges();


                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Usuarios/Delete/5
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
