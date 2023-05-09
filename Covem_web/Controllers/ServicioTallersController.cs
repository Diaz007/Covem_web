using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Covem_web;

namespace Covem_web.Controllers
{
    public class ServicioTallersController : Controller
    {
        private vtreact_Covume2Entities db = new vtreact_Covume2Entities();

        // GET: ServicioTallers
        public ActionResult Index()
        {
            return View(db.ServicioTaller.ToList());
        }

        // GET: ServicioTallers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServicioTaller servicioTaller = db.ServicioTaller.Find(id);
            if (servicioTaller == null)
            {
                return HttpNotFound();
            }
            return View(servicioTaller);
        }

        public ActionResult Asig_Serv(int? id) 
        {


            Automoviles automovil = (Automoviles)Session["automovil"];
            if (automovil == null)
                automovil = new Automoviles();


            if (id != null)
            {
                servicios servicio = db.servicios.Find(id);


                automovil.Lservicios.Remove(servicio);

                automovil.LserviciosA.Add(servicio);
            }
            else
            {
                automovil.bandera = 2;
            }
            Session["automovil"] = automovil;
            return RedirectToAction("nuevo/" + automovil.auto.id);
        }
        // GET: ServicioTallers/Details/5
        public ActionResult Nuevo(int? id, int idTaller=0)
        {
            Automoviles automovil;
            try
            {
                 automovil = (Automoviles)Session["automovil"];
                if(automovil==null)
                    automovil = new Automoviles();
            }
            catch
            {
                 automovil = new Automoviles();
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (idTaller > 0)
            {
                automovil.bandera = 1;
                automovil.Taller = db.tallerMecanico.Find(idTaller);
            }
            else
            {
              //  automovil.bandera = 0;
            }
            if (automovil.LserviciosA.Count > 0)
            {
                if (automovil.bandera != 2)
                automovil.bandera = 1;
            }

           
           
            automovil.auto = db.Auto.Find(id);
            automovil.Fotos = db.auto_Fotos.Where(x=>x.idauto==id).ToList();
           
            automovil.EntregaRecepcion = new EntregaRecepcion_Vehiculo();
            automovil.marca = db.Marca.Where(x => x.id == automovil.auto.idmarca).FirstOrDefault();
            automovil.modelo = db.Modelo.Where(x => x.id == automovil.auto.idmodelo).FirstOrDefault();
            automovil.persona = db.Persona.Where(x => x.idAuto == id).FirstOrDefault();
            automovil.Talleres_Disponibles = db.tallerMecanico.Where(a => a.activo == true).ToList();


            if (automovil.LserviciosA == null || automovil.LserviciosA.Count==0)
            {
                automovil.Lservicios = db.servicios.ToList();
                automovil.LserviciosA = new List<servicios>();
            }
            Session["automovil"] = automovil;
            if (automovil.auto == null)
            {
                return HttpNotFound();
            }
            return View(automovil);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Nuevo (FormCollection collection)
        {
            Automoviles automovil = (Automoviles)Session["automovil"];


            EntregaRecepcion_Vehiculo entrega = new EntregaRecepcion_Vehiculo();
            entrega.activo = true;
            entrega.fechaEntrega = Convert.ToDateTime(collection["fechaEntrada"]);
            entrega.fechallegada = null;
            entrega.idAuto = automovil.auto.id;
            entrega.idPersonaEntrega =automovil.persona.Id; 
            entrega.idPersonallegada = null;
            entrega.idtaller = automovil.Taller.id;
            entrega.kilometrajeEntrega = Convert.ToInt32(collection["kmEntrada"]);
            entrega.kilometrajellegada =  0;
            entrega.ObservecionesEntrega =  collection["notas"];

            db.EntregaRecepcion_Vehiculo.Add(entrega);
            db.SaveChanges();
            entrega = db.EntregaRecepcion_Vehiculo.Where(x => x.activo == true).Where(x => x.idAuto == automovil.auto.id).Where(x => x.idPersonaEntrega == automovil.persona.Id).FirstOrDefault();

            foreach (var item in automovil.LserviciosA)
            {
                Entrega_Sevicios addservicio = new Entrega_Sevicios();
                addservicio.activo = true;
                addservicio.diasRealizarServicio = item.diasRealizarServicio;
                addservicio.idServicio = item.id;
                addservicio.idorden = entrega.id;
                addservicio.Precio = item.Precio;
                addservicio.semanasGarantiaservicio = item.semanasGarantiaservicio;
                db.Entrega_Sevicios.Add(addservicio);

            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        // GET: ServicioTallers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ServicioTallers/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idtaller,idServicio,activo")] ServicioTaller servicioTaller)
        {
            if (ModelState.IsValid)
            {
                db.ServicioTaller.Add(servicioTaller);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(servicioTaller);
        }

        // GET: ServicioTallers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServicioTaller servicioTaller = db.ServicioTaller.Find(id);
            if (servicioTaller == null)
            {
                return HttpNotFound();
            }
            return View(servicioTaller);
        }

        // POST: ServicioTallers/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idtaller,idServicio,activo")] ServicioTaller servicioTaller)
        {
            if (ModelState.IsValid)
            {
                db.Entry(servicioTaller).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(servicioTaller);
        }

        // GET: ServicioTallers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServicioTaller servicioTaller = db.ServicioTaller.Find(id);
            if (servicioTaller == null)
            {
                return HttpNotFound();
            }
            return View(servicioTaller);
        }

        // POST: ServicioTallers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServicioTaller servicioTaller = db.ServicioTaller.Find(id);
            db.ServicioTaller.Remove(servicioTaller);
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
