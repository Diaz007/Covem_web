using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covem_web.Controllers
{
    public class ModeloController : Controller
    {
        vtreact_Covume2Entities db = new vtreact_Covume2Entities();
        // GET: Modelo
        public ActionResult Index()
        {
            List<modelo_Completa> lista = new List<modelo_Completa>();
            foreach (var item in db.Modelo.ToList())
            {
                modelo_Completa modelo = new modelo_Completa();
                modelo.modelo = item;
                modelo.Marca = db.Marca.Find(item.idMarca);
                modelo.autos = db.Auto.Where(x => x.activo == true).Where(x => x.idmodelo == item.id).Count();
                lista.Add(modelo);
            }
            return View(lista.OrderByDescending(x=>x.autos));
        }

        // GET: Modelo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Modelo/Create
        public ActionResult Create()
        {
            ViewBag.Marca = db.Marca.ToList();
            return View();
        }

        // POST: Modelo/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                var marca = collection["idmarca"];
                var activo = collection["activo"];

                var _modelo = collection["modelo1"];
                Modelo modelo = new Modelo();
                modelo.activo = true;
                modelo.idMarca = Convert.ToInt16(marca);
                modelo.modelo1 = _modelo;
                db.Modelo.Add(modelo);
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Modelo/Edit/5
        public ActionResult Edit(int id)
        {
            modelo_Completa modelo = new modelo_Completa();
            modelo.listamarcas= db.Marca.ToList();
            modelo.modelo = db.Modelo.Find(id);
            return View(modelo);
        }

        // POST: Modelo/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var marca = collection["SlMarca"];
                var activo = collection["activo"];

                var _modelo = collection["modelo.modelo1"];
                Modelo modelo = db.Modelo.Find(id);
               modelo.idMarca = Convert.ToInt16(marca);
                modelo.modelo1 = _modelo;
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Modelo/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Modelo/Delete/5
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
