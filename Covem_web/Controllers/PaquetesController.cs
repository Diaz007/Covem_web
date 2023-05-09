using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covem_web.Controllers
{
    public class PaquetesController : Controller
    {
        private vtreact_Covume2Entities db = new vtreact_Covume2Entities();

        // GET: Paquetes
        public ActionResult Index()
        {
            return View(db.GrupoServicio.Where(x=>x.actio==true));
        }

        // GET: Paquetes/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        public ActionResult Servicios(int id)
        {
            Vista_Paquete vista = new Vista_Paquete();

            try
            {
                vista = (Vista_Paquete)Session["automovil"];
                if (vista.grupo.id != id)
                {
                    vista = null;
                }

                if (vista == null)
                {
                    vista = new Vista_Paquete(); ;
                    vista.grupo = db.GrupoServicio.Find(id);
                    vista.listaRelacionada = db.Grupo_Relacion_Servicio.Where(x => x.idGrupo == id).ToList();
                    vista.listaCompleta = db.servicios.Where(x => x.activo == true).ToList();
                    vista.listaAgregar = new List<servicios>();
                    foreach (var item in vista.listaRelacionada)
                    {
                        servicios servicio = db.servicios.Find(item.idServicio);
                        vista.listaCompleta.Remove(servicio);
                        vista.listaAgregar.Add(servicio);
                    }
                }
            }
            catch
            {
                vista = new Vista_Paquete(); ;
                vista.grupo = db.GrupoServicio.Find(id);
                vista.listaRelacionada = db.Grupo_Relacion_Servicio.Where(x => x.idGrupo == id).ToList();
                vista.listaCompleta = db.servicios.Where(x => x.activo == true).ToList();
                vista.listaAgregar = new List<servicios>();
                foreach (var item in vista.listaRelacionada)
                {
                    servicios servicio = db.servicios.Find(item.idServicio);
                    vista.listaCompleta.Remove(servicio);
                    vista.listaAgregar.Add(servicio);
                }

            }


            Session["automovil"] = vista;
            return View(vista);
        }

        public ActionResult Agregar(int id)
        {
            Vista_Paquete vista = new Vista_Paquete();

            vista = (Vista_Paquete)Session["automovil"];
            servicios servicio = db.servicios.Find(id);
            List<servicios> lista = new List<servicios>();
            foreach (var item in vista.listaCompleta)
            {
                if (item.nombreServicio == servicio.nombreServicio)
                {
                    vista.listaAgregar.Add(servicio);
                }
                else
                {
                    lista.Add(item);
                }
            }
            vista.listaCompleta = lista;

            Session["automovil"] = vista;
            return RedirectToAction("servicios/"+vista.grupo.id);
        }
        public ActionResult Quitar(int id)
        {
            Vista_Paquete vista = new Vista_Paquete();

            vista = (Vista_Paquete)Session["automovil"];
            servicios servicio = db.servicios.Find(id);
            List<servicios> lista = new List<servicios>();
            foreach (var item in vista.listaAgregar)
            {
                if (item.nombreServicio == servicio.nombreServicio)
                {
                    vista.listaCompleta.Add(servicio);
                }
                else
                {
                    lista.Add(item);
                }
            }
            vista.listaAgregar = lista;

            Session["automovil"] = vista;
            return RedirectToAction("servicios/" + vista.grupo.id);
        }

        [HttpPost]
        public ActionResult Servicios(FormCollection collection)
        {
            Vista_Paquete vista = new Vista_Paquete();

            vista = (Vista_Paquete)Session["automovil"];

            foreach (var item in vista.listaAgregar)
            {
                Grupo_Relacion_Servicio relacion = new Grupo_Relacion_Servicio();
                relacion.activo = true;
                relacion.idGrupo = vista.grupo.id;
                relacion.idServicio = item.id;
                db.Grupo_Relacion_Servicio.Add(relacion);
                db.SaveChanges();
            }
            Session["automovil"] = null;
            return RedirectToAction("Index");
        }

        // GET: Paquetes/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: Paquetes/Create
        [HttpPost]
        public ActionResult Create(GrupoServicio grupo)
        {
            try
            {
                grupo.actio = true;
                db.GrupoServicio.Add(grupo);
                db.SaveChanges();
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Paquetes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Paquetes/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Paquetes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Paquetes/Delete/5
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
