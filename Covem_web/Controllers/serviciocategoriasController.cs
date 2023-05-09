using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Covem_web;

namespace Covem_web
{
    public class serviciocategoriasController : Controller
    {
        private vtreact_Covume2Entities db = new vtreact_Covume2Entities();

        // GET: serviciocategorias
        public ActionResult Index()
        {
            return View(db.serviciocategoria.ToList());
        }

        // GET: serviciocategorias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            serviciocategoria serviciocategoria = db.serviciocategoria.Find(id);
            if (serviciocategoria == null)
            {
                return HttpNotFound();
            }
            return View(serviciocategoria);
        }

        // GET: serviciocategorias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: serviciocategorias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,categoria,activo")] serviciocategoria serviciocategoria)
        {
            if (ModelState.IsValid)
            {
                db.serviciocategoria.Add(serviciocategoria);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(serviciocategoria);
        }

        // GET: serviciocategorias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            serviciocategoria serviciocategoria = db.serviciocategoria.Find(id);
            if (serviciocategoria == null)
            {
                return HttpNotFound();
            }
            return View(serviciocategoria);
        }

        // POST: serviciocategorias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,categoria,activo")] serviciocategoria serviciocategoria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviciocategoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serviciocategoria);
        }

        // GET: serviciocategorias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            serviciocategoria serviciocategoria = db.serviciocategoria.Find(id);
            if (serviciocategoria == null)
            {
                return HttpNotFound();
            }
            return View(serviciocategoria);
        }

        // POST: serviciocategorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            serviciocategoria serviciocategoria = db.serviciocategoria.Find(id);
            db.serviciocategoria.Remove(serviciocategoria);
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
