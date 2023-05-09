using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covem_web.Controllers
{
    public class HomeController : Controller
    {
        vtreact_Covume2Entities db = new Covem_web.vtreact_Covume2Entities();
        [Authorize]
        public ActionResult Index(int? id)
        {
            Vista_Principal vista = new Vista_Principal();
            DateTime fechaA = DateTime.Now;
            fechaA= fechaA.AddHours (-(fechaA.Hour));
            DateTime fechaB = fechaA.AddDays(1);

            vista.persona = db.Persona.Where(x => x.Correo == User.Identity.Name).FirstOrDefault();
            try
            {
                //vista.auto = db.Auto.Find(vista.persona.idAuto);
                //vista.marca = db.Marca.Find(vista.auto.idmarca);
                //vista.modelo = db.Modelo.Find(vista.auto.idmodelo);
            }
            catch { }
            
         
            vista.cheking = db.Cheking.Where(x=>x.idPersona==vista.persona.Id).Where( x=> x.fecha.Value >= fechaA).Where(x => x.fecha.Value <= fechaB).FirstOrDefault();

            if (!User.Identity.IsAuthenticated)
                return View("Account/Login");
            return View(vista);
        }
        public ActionResult opcion(int id)
        {
            switch (id)
            {
                case 1:
                    return RedirectToAction("Nuevo", "Cheking");
                    //return View("Cheking/Nuevo");

            }
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}