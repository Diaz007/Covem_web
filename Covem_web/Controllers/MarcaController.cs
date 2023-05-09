using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covem_web.Controllers
{
    public class MarcaController : Controller
    {
        vtreact_Covume2Entities db = new vtreact_Covume2Entities();
        // GET: Marca
        public ActionResult Index()
        {
            List<Marca_Completa> lista = new List<Marca_Completa>();
            foreach (var item in db.Marca.Where(x=>x.activo==true).ToList().OrderBy(x => x.Marca1))
            {
                Marca_Completa marca = new Marca_Completa();
                marca.Marca = item;
                marca.modelos = db.Modelo.Where(x => x.idMarca == item.id).Count();
                lista.Add(marca);
            }
            return View(lista);
        }

        // GET: Marca/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Marca/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Marca/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, HttpPostedFileBase image)
        {
            try
            {

                if (image != null)
                {
                    var ImageMimeType = image.ContentType;
                    var ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(ImageData, 0, image.ContentLength);
                    Image image1 = MiClase.ConvertByteArrayToImage(ImageData);

                    image1 = MiClase.BajarCalidad(image1);
                    ImageData = MiClase.ConvertImageToByteArray(image1);

                    MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/Logos/", image.FileName, "Covemu", "2M5^cuo86", ImageData);

                }
                var marca = collection["marca1"];
                var activo = collection["activo"];

                var imagen = collection["image"];
                // TODO: Add insert logic here
                Marca _marca = new Marca();
                _marca.foto = "https://covemu.vtreact.com/archivos/Logos/" + image.FileName;
                _marca.activo = true;
                _marca.Marca1 = marca;
                db.Marca.Add(_marca);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Marca/Edit/5
        public ActionResult Edit(int id)
        {
            Marca marca = db.Marca.Find(id);
            if (marca == null)
                return View();

            marca.activo = false;
            return View(marca);
        }

        // POST: Marca/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection, HttpPostedFileBase image)
        {
            try
            {
                // TODO: Add update logic here
                Marca Marca = db.Marca.Find(id);
                if (Marca == null)
                    return View(Marca);
                string img = Marca.foto;
                if (image != null)
                {
                    try
                    {
                        var ImageMimeType = image.ContentType;
                        var ImageData = new byte[image.ContentLength];
                        image.InputStream.Read(ImageData, 0, image.ContentLength);
                        Image image1 = MiClase.ConvertByteArrayToImage(ImageData);

                        image1 = MiClase.BajarCalidad(image1);
                        ImageData = MiClase.ConvertImageToByteArray(image1);
                        img= "https://covemu.vtreact.com/archivos/Logos/" + image.FileName;
                        MiClase.UploadFile("ftp://Covemu@vtreact.com/archivos/Logos/", image.FileName, "Covemu", "2M5^cuo86", ImageData);
                    }
                    catch { }
                }
                var marca = collection["marca1"];
                var activo = collection["activo"];

                var imagen = collection["image"];
                // TODO: Add insert logic here
               
                Marca.foto = img;

                Marca.Marca1 = marca;

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Marca/Delete/5
        public ActionResult Delete(int id)
        {
            Marca marca = db.Marca.Find(id);
            if (marca == null)
                return View();

            marca.activo = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Marca/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                Marca marca = db.Marca.Find(id);
                if (marca == null)
                    return View();

                marca.activo = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
