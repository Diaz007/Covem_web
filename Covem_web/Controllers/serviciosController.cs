using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Covem_web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Covem_web
{
    public class serviciosController : Controller
    {
        private vtreact_Covume2Entities db = new vtreact_Covume2Entities();

        // GET: servicios
        public ActionResult Index()
        {
            List<Vista_Servicio> vistas = new List<Vista_Servicio>();
            foreach (var item in db.servicios.Where(x => x.activo == true).ToList())
            {
                Vista_Servicio vista = new Vista_Servicio();
                vista.servicio = item;
                vista.categoria = db.serviciocategoria.Find(item.idcategoria);
                vistas.Add(vista);
            }
            return View(vistas);
        }

        // GET: servicios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            servicios servicios = db.servicios.Find(id);
            if (servicios == null)
            {
                return HttpNotFound();
            }
            return View(servicios);
        }

        // GET: servicios/Create
        public ActionResult Create()
        {
            ViewBag.Categorias = new System.Web.Mvc.SelectList(db.serviciocategoria, "id", "categoria");
            return View();
        }

        // POST: servicios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idcategoria,semanasGarantiaservicio,diasRealizarServicio,nombreServicio,KilometrosProximoServicio")] servicios servicios)
        {
            if (ModelState.IsValid)
            {
                servicios.activo = true;
                db.servicios.Add(servicios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(servicios);
        }

        // GET: servicios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            servicios servicios = db.servicios.Find(id);
            if (servicios == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categorias = new System.Web.Mvc.SelectList(db.serviciocategoria, "id", "categoria");

            return View(servicios);
        }

        // POST: servicios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idcategoria,semanasGarantiaservicio,diasRealizarServicio,activo,nombreServicio")] servicios servicios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(servicios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(servicios);
        }

        // GET: servicios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            servicios servicios = db.servicios.Find(id);
            if (servicios == null)
            {
                return HttpNotFound();
            }
            return View(servicios);
        }

        // POST: servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            servicios servicios = db.servicios.Find(id);
            db.servicios.Remove(servicios);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
