using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Covem_web;
using CrystalDecisions.CrystalReports.Engine;

namespace Covem_web
{
    public class tallerMecanicoController : Controller
    {
        private vtreact_Covume2Entities db = new vtreact_Covume2Entities();
        public ActionResult Download_PDF()
        {
            //empEntities context = new empEntities();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "ListaAutos.rpt"));


            // var obras = db.Obras.ToList();
            MiClase.Reporteconexionportabla(rd);
            //  rd.SetDataSource(obras);
           // rd.SetParameterValue(0, id);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "ListaAutos" + ".pdf");
        }

    
    // GET: tallerMecanicoes
    public ActionResult Index()
        {
            return View(db.tallerMecanico.ToList());
        }

        // GET: tallerMecanicoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tallerMecanico tallerMecanico = db.tallerMecanico.Find(id);
            ViewBag.Valores= VerificarServiciosTaller(tallerMecanico);
            if (tallerMecanico == null)
            {
                return HttpNotFound();
            }
            return View(tallerMecanico);
        }
        // GET: tallerMecanicoes/Details/5
        public ActionResult Asignar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServicioTaller taller = db.ServicioTaller.Find(id);
            taller.activo = !taller.activo;
            db.SaveChanges();
            if (taller == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Details/" + taller.idtaller);
        }




        private List<Servicios_Taller> VerificarServiciosTaller(tallerMecanico tallerMecanico)
        {
            List <Servicios_Taller>  lista = new List<Servicios_Taller>();
            Servicios_Taller servicios_ = new Servicios_Taller();
            foreach (var item in db.servicios.Where(x => x.activo == true).ToList())
            {
                servicios_ = new Servicios_Taller();
                ServicioTaller servicioTaller = db.ServicioTaller.Where(x => x.idServicio == item.id).Where(x => x.idtaller == tallerMecanico.id).FirstOrDefault();
                if (servicioTaller == null) 
                {
                    servicioTaller = new ServicioTaller();
                    servicioTaller.activo = false;
                    servicioTaller.idServicio = item.id;
                    servicioTaller.idtaller = tallerMecanico.id;
                    db.ServicioTaller.Add(servicioTaller);
                    db.SaveChanges();
                }
                servicios_.Taller = servicioTaller;
                servicios_.categoria = db.serviciocategoria.Find(item.idcategoria);
                servicios_.servicio = item;
                lista.Add(servicios_);
            }
            return lista;
        }

        // GET: tallerMecanicoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tallerMecanicoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nombreTaller,ubicacion,activo")] tallerMecanico tallerMecanico)
        {
            if (ModelState.IsValid)
            {
                db.tallerMecanico.Add(tallerMecanico);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tallerMecanico);
        }

        // GET: tallerMecanicoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tallerMecanico tallerMecanico = db.tallerMecanico.Find(id);
            db.SaveChanges();
            if (tallerMecanico == null)
            {
                return HttpNotFound();
            }
            return View(tallerMecanico);
        }

        // POST: tallerMecanicoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nombreTaller,ubicacion,activo")] tallerMecanico tallerMecanico)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tallerMecanico).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tallerMecanico);
        }

        // GET: tallerMecanicoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tallerMecanico tallerMecanico = db.tallerMecanico.Find(id);
            if (tallerMecanico == null)
            {
                return HttpNotFound();
            }
            return View(tallerMecanico);
        }

        // POST: tallerMecanicoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tallerMecanico tallerMecanico = db.tallerMecanico.Find(id);
            db.tallerMecanico.Remove(tallerMecanico);
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
