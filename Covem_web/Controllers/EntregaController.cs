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
    public class EntregaController : Controller
    {
        private vtreact_Covume2Entities db = new vtreact_Covume2Entities();

        // GET: Entrega
        public ActionResult Index()
        {
            Session["automovil"] = null;
            List<Automoviles> lista = new List<Automoviles>();
            foreach (var item in db.EntregaRecepcion_Vehiculo.Where(x => x.activo == true).ToList())
            {
                Automoviles automovil = new Automoviles();
                automovil.EntregaRecepcion = item;
                automovil.auto = db.Auto.Find(item.idAuto);
                if (automovil.auto != null)
                {
                    automovil.modelo = db.Modelo.Find(automovil.auto.idmodelo);
                    automovil.marca = db.Marca.Find(automovil.modelo.idMarca);

                    automovil.persona = db.Persona.Find(item.idPersonaEntrega);
                    automovil.Taller = db.tallerMecanico.Find(item.idtaller);
                    automovil.LEServicios = db.Entrega_Sevicios.Where(x => x.idorden == item.id).ToList();
                    lista.Add(automovil);
                }
                
            }
            return View(lista);
        }

        // GET: Entrega/Details/5
        public ActionResult Terminar(int? id,int opcion=0,decimal km=0)
        {
            Automoviles _movil = new Automoviles();

            try
            {
                _movil = (Automoviles)Session["automovil"];
                if (_movil == null)
                {
                    _movil = new Automoviles();
                    _movil.EntregaRecepcion = db.EntregaRecepcion_Vehiculo.Find(id);
                    _movil.auto = db.Auto.Find(_movil.EntregaRecepcion.idAuto);
                    _movil.bandera = 0;
                    _movil.Fotos = db.auto_Fotos.Where(x => x.idauto == _movil.EntregaRecepcion.idAuto).ToList();
                    _movil.Lservicios = db.servicios.ToList();
                    _movil.LserviciosA = new List<servicios>();
                    foreach (var item in db.Entrega_Sevicios.Where(x => x.idorden == id).Where(x => x.activo == true).ToList())
                    {
                        servicios servicio = db.servicios.Find(item.idServicio);
                        _movil.LserviciosA.Add(servicio);
                        _movil.LEServicios.Add(item);
                    }
                    _movil.marca = db.Marca.Find(_movil.auto.idmarca);
                    _movil.modelo = db.Modelo.Find(_movil.auto.idmodelo);
                    _movil.persona = db.Persona.Where(x => x.idAuto == _movil.EntregaRecepcion.idAuto).FirstOrDefault();
                    _movil.Taller = db.tallerMecanico.Find(_movil.EntregaRecepcion.idtaller);


                }
            }
            catch (Exception ex)
            {
                _movil = new Automoviles();
                _movil.bandera = 0;
            }


    
            _movil.Lservicios = db.servicios.ToList();
           
            Session["automovil"] = _movil;
            if (opcion > 1)
            {
              EntregaRecepcion_Vehiculo entrega=  db.EntregaRecepcion_Vehiculo.Find(id);
                entrega.estatus = "Finalizado";
                entrega.fechallegada = DateTime.Now;
                entrega.kilometrajeEntrega = 0;
                db.SaveChanges();

                
                foreach (var item in _movil.LserviciosA)
                {
                    ServicioAuto Sauto = new ServicioAuto();
                    Sauto.activo = true;
                    Sauto.fecha = DateTime.Now;
                    Sauto.idauto = _movil.auto.id;
                    Sauto.idorder = _movil.EntregaRecepcion.id;
                    Sauto.idservicio = item.id;
                    
                    Sauto.fechaPservicio = DateTime.Now.AddMonths((int)item.semanasGarantiaservicio);
                    db.ServicioAuto.Add(Sauto);
                    db.SaveChanges();
                }

            }
            return View(_movil);
        }
        [HttpPost]
        public ActionResult Busqueda(FormCollection collection)
        {
            string clave= collection["CustomerName"];
            Automoviles _movil = new Automoviles();

            try
            {
                _movil = (Automoviles)Session["automovil"];
                if (_movil == null)
                {
                    _movil = new Automoviles();
                    _movil.bandera = 0;
                }
            }
            catch
            {
                _movil = new Automoviles();
                _movil.bandera = 0;
            }

            GrupoServicio grupo = db.GrupoServicio.Where(x => x.clave == clave).FirstOrDefault();

            List<Grupo_Relacion_Servicio> lista = db.Grupo_Relacion_Servicio.Where(x => x.idGrupo == grupo.id).ToList();

            foreach (var item in lista)
            {
                _movil.LserviciosA.Add(db.servicios.Find(item.idServicio));
            }
            
            return RedirectToAction("Create/" );
        }
        // GET: Entrega/Create
        public ActionResult Create(int id = 0, int opcion = 0)
        {
            Automoviles _movil = new Automoviles();

            try
            {
                _movil = (Automoviles)Session["automovil"];
                if (_movil == null)
                {
                    _movil = new Automoviles();
                    _movil.bandera = 0;
                }
            }
            catch
            {
                _movil = new Automoviles();
                _movil.bandera = 0;
            }


            List<Automoviles> lista = new List<Automoviles>();

            foreach (var item in db.Auto.Where(x => x.activo == true))
            {
                Automoviles automovil = new Automoviles();
                automovil.auto = item;
                automovil.marca = db.Marca.Find(item.idmarca);
                automovil.modelo = db.Modelo.Find(item.idmodelo);
                automovil.persona = db.Persona.Where(x => x.idAuto == item.id).FirstOrDefault();
              //  automovil.marca.Marca1 += " " + automovil.modelo.modelo1 + " " + automovil.auto.año;
                lista.Add(automovil);
            }



            ViewBag.Autos = new System.Web.Mvc.SelectList(lista, "auto.id", "marca.marca1");
            ViewBag.Talleres = new System.Web.Mvc.SelectList(db.tallerMecanico.ToList(), "id", "nombreTaller");


            _movil.Lservicios = db.servicios.ToList();
            if (id > 0)
                _movil.LserviciosA.Add(db.servicios.Find(id));
            if (opcion > 0)
                _movil.bandera = opcion;

            Session["automovil"] = _movil;

            return View(_movil);
        }
        public JsonResult GetPersona(int id)
        {
            Automoviles _movil = new Automoviles();

            try
            {
                _movil = (Automoviles)Session["automovil"];
                if (_movil == null)
                {
                    _movil = new Automoviles();
                    _movil.bandera = 0;
                }
            }
            catch
            {
                _movil = new Automoviles();
                _movil.bandera = 0;
            }

            List<SelectListItem> ModelosList = new List<SelectListItem>();
            var modelos = db.Persona.Where(x => x.idAuto == id).ToList();
            foreach (var item in modelos)
            {
                ModelosList.Add(new SelectListItem { Text = item.nombre + " " + item.Apellidos, Value = item.Id.ToString() });
            }

            try
            {
                _movil.auto = db.Auto.Find(id);
                _movil.persona = db.Persona.Where(x => x.idAuto == id).FirstOrDefault();
            }
            catch { }
            Session["automovil"] = _movil;
            return Json(new SelectList(ModelosList, "Value", "Text"));
            //var territories = db.Modelo.Where(x => x.idMarca == id).ToList();
            //return Json(territories, JsonRequestBehavior.AllowGet);

        }

        // POST: Entrega/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idAuto,idPersonaEntrega,fechaEntrega,ObservecionesEntrega,activo,kilometrajeEntrega,idPersonallegada,fechallegada,Observecionesllegada,kilometrajellegada,idtaller")] EntregaRecepcion_Vehiculo entregaRecepcion_Vehiculo, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                Automoviles _movil = new Automoviles();

                try
                {
                    _movil = (Automoviles)Session["automovil"];
                    if (_movil == null)
                    {
                        _movil = new Automoviles();
                        _movil.bandera = 0;
                    }
                }
                catch
                {
                    _movil = new Automoviles();
                    _movil.bandera = 0;
                }
                entregaRecepcion_Vehiculo.activo = true;
                entregaRecepcion_Vehiculo.estatus = "En Servicio";
                entregaRecepcion_Vehiculo.fechaEntrega = Convert.ToDateTime(collection["txtFechaN"]);
                entregaRecepcion_Vehiculo.fechallegada = null;
                entregaRecepcion_Vehiculo.idAuto = _movil.auto.id;
                entregaRecepcion_Vehiculo.idPersonaEntrega = _movil.persona.Id;
                entregaRecepcion_Vehiculo.idPersonallegada = null;
                entregaRecepcion_Vehiculo.idtaller = Convert.ToInt32(collection["EntregaRecepcion.idtaller"]);
                entregaRecepcion_Vehiculo.kilometrajeEntrega = Convert.ToInt32(collection["txtKm"]);
                entregaRecepcion_Vehiculo.kilometrajellegada = 0;
                entregaRecepcion_Vehiculo.ObservecionesEntrega = collection["txtNotas"];
                entregaRecepcion_Vehiculo.Observecionesllegada = "";



                db.EntregaRecepcion_Vehiculo.Add(entregaRecepcion_Vehiculo);
                db.SaveChanges();

                _movil.EntregaRecepcion = db.EntregaRecepcion_Vehiculo.Where(x => x.idAuto == _movil.auto.id).Where(x => x.idPersonaEntrega == _movil.persona.Id).FirstOrDefault();

                foreach (var item in _movil.LserviciosA)
                {
                    Entrega_Sevicios _Sevicios = new Entrega_Sevicios();
                    _Sevicios.activo = true;
                    _Sevicios.diasRealizarServicio = item.diasRealizarServicio;
                    _Sevicios.idorden = _movil.EntregaRecepcion.id;
                    _Sevicios.idServicio = item.id;
                    _Sevicios.Precio = item.Precio;
                    _Sevicios.semanasGarantiaservicio = item.semanasGarantiaservicio;
                    db.Entrega_Sevicios.Add(_Sevicios);
                    db.SaveChanges();
                }
                Session["automovil"] = null;
                return RedirectToAction("Index");
            }

            return View(entregaRecepcion_Vehiculo);
        }

        // GET: Entrega/Edit/5
        public ActionResult Edit(int? id, int opcion = 0, int idservicio = 0)
        {
            Automoviles _movil = new Automoviles();

            try
            {
                _movil = (Automoviles)Session["automovil"];
                if (_movil == null)
                {
                    _movil = new Automoviles();
                    _movil.EntregaRecepcion = db.EntregaRecepcion_Vehiculo.Find(id);
                    _movil.auto = db.Auto.Find(_movil.EntregaRecepcion.idAuto);
                    _movil.bandera = 0;
                    _movil.Lservicios = db.servicios.ToList();
                    _movil.LserviciosA = new List<servicios>();
                    foreach (var item in db.Entrega_Sevicios.Where(x => x.idorden == id).Where(x => x.activo == true).ToList())
                    {
                        servicios servicio = db.servicios.Find(item.idServicio);
                        _movil.LserviciosA.Add(servicio);
                        _movil.LEServicios.Add(item);
                    }
                    _movil.marca = db.Marca.Find(_movil.auto.idmarca);
                    _movil.modelo = db.Modelo.Find(_movil.auto.idmodelo);
                    _movil.persona = db.Persona.Where(x => x.idAuto == _movil.EntregaRecepcion.idAuto).FirstOrDefault();
                    _movil.Taller = db.tallerMecanico.Find(_movil.EntregaRecepcion.idtaller);


                }
            }
            catch (Exception ex)
            {
                _movil = new Automoviles();
                _movil.bandera = 0;
            }


            List<Automoviles> lista = new List<Automoviles>();

            foreach (var item in db.Auto.Where(x => x.activo == true))
            {
                Automoviles automovil = new Automoviles();
                automovil.auto = item;
                automovil.marca = db.Marca.Find(item.idmarca);
                automovil.modelo = db.Modelo.Find(item.idmodelo);
                automovil.persona = db.Persona.Where(x => x.idAuto == item.id).FirstOrDefault();
                automovil.marca.Marca1 += " " + automovil.modelo.modelo1 + " " + automovil.auto.año;
                lista.Add(automovil);
            }



            ViewBag.Autos = new System.Web.Mvc.SelectList(lista, "auto.id", "marca.marca1");
            ViewBag.Talleres = new System.Web.Mvc.SelectList(db.tallerMecanico.ToList(), "id", "nombreTaller");


            _movil.Lservicios = db.servicios.ToList();
            if (opcion == -1)
                _movil.LserviciosA.Add(db.servicios.Find(idservicio));
            else if (opcion > 0)
            {
                _movil.bandera = opcion;
            }
            else if (opcion == -2)
            {
                List<servicios> l = _movil.LserviciosA;
                servicios serv = (db.servicios.Find(idservicio));

                _ = l.Remove(serv);

                _movil.LserviciosA = new List<servicios>();
                foreach (var item in l)
                {
                    if (item.id != serv.id)
                    {
                        _movil.LserviciosA.Add(item);
                    }

                }
            }
            Session["automovil"] = _movil;

            return View(_movil);
        }

        // POST: Entrega/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idAuto,idPersonaEntrega,fechaEntrega,ObservecionesEntrega,activo,kilometrajeEntrega,idPersonallegada,fechallegada,Observecionesllegada,kilometrajellegada,idtaller")] EntregaRecepcion_Vehiculo entregaRecepcion_Vehiculo, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                Automoviles _movil = new Automoviles();

                try
                {
                    _movil = (Automoviles)Session["automovil"];

                    if (_movil == null)
                    {
                        _movil = new Automoviles();
                        _movil.bandera = 0;
                    }
                    _movil.EntregaRecepcion = db.EntregaRecepcion_Vehiculo.Find(entregaRecepcion_Vehiculo.id);
                }
                catch
                {
                    _movil = new Automoviles();
                    _movil.bandera = 0;
                }
                entregaRecepcion_Vehiculo = db.EntregaRecepcion_Vehiculo.Find(entregaRecepcion_Vehiculo.id);
                entregaRecepcion_Vehiculo.fechallegada = null;
                entregaRecepcion_Vehiculo.idAuto = _movil.auto.id;
                entregaRecepcion_Vehiculo.idPersonaEntrega = _movil.persona.Id;
                entregaRecepcion_Vehiculo.idPersonallegada = null;
                entregaRecepcion_Vehiculo.idtaller = Convert.ToInt32(collection["EntregaRecepcion.idtaller"]);
                entregaRecepcion_Vehiculo.kilometrajeEntrega = Convert.ToInt32(collection["txtKm"]);
                entregaRecepcion_Vehiculo.kilometrajellegada = 0;
                entregaRecepcion_Vehiculo.ObservecionesEntrega = collection["txtNotas"];
                entregaRecepcion_Vehiculo.Observecionesllegada = "";



                // db.EntregaRecepcion_Vehiculo.Add(entregaRecepcion_Vehiculo);
                db.SaveChanges();

                // _movil.EntregaRecepcion = db.EntregaRecepcion_Vehiculo.Where(x => x.idAuto == _movil.auto.id).Where(x => x.idPersonaEntrega == _movil.persona.Id).FirstOrDefault();
                foreach (var item in db.Entrega_Sevicios.Where(x => x.idorden == entregaRecepcion_Vehiculo.id)) 
                {
                    Entrega_Sevicios entrega = new Entrega_Sevicios();
                    entrega = db.Entrega_Sevicios.Find(item.id);
                    entrega.activo = false;
                   
                }
                db.SaveChanges();
                foreach (var item in _movil.LserviciosA)
                {
                    Entrega_Sevicios _Sevicios = db.Entrega_Sevicios.Where(x => x.idorden == entregaRecepcion_Vehiculo.id).Where(x => x.idServicio == item.id).FirstOrDefault();
                    if (_Sevicios == null)
                        _Sevicios = new Entrega_Sevicios();
                    if((_Sevicios.activo==false)&&(_Sevicios.idServicio==item.id))
                    _Sevicios.activo = true;
                    _Sevicios.diasRealizarServicio = item.diasRealizarServicio;
                    _Sevicios.idorden = _movil.EntregaRecepcion.id;
                    _Sevicios.idServicio = item.id;
                    _Sevicios.Precio = item.Precio;
                    _Sevicios.semanasGarantiaservicio = item.semanasGarantiaservicio;

                    if (_Sevicios.id == 0)
                    {
                        db.Entrega_Sevicios.Add(_Sevicios);
                    }
                    db.SaveChanges();
                }
                Session["automovil"] = null;
                return RedirectToAction("Index");
            }

            return View(entregaRecepcion_Vehiculo);
        }



        // GET: Entrega/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EntregaRecepcion_Vehiculo entregaRecepcion_Vehiculo = db.EntregaRecepcion_Vehiculo.Find(id);
            if (entregaRecepcion_Vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(entregaRecepcion_Vehiculo);
        }

        // POST: Entrega/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EntregaRecepcion_Vehiculo entregaRecepcion_Vehiculo = db.EntregaRecepcion_Vehiculo.Find(id);
            db.EntregaRecepcion_Vehiculo.Remove(entregaRecepcion_Vehiculo);
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
