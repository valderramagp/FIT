using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIT.BLL;
using FIT.Models;

namespace FIT.Controllers
{
    public class InscripcionController : Controller
    {
        // GET: Inscripcion
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult DatosCompetidor(int numCorredores = 1)
        {
            ViewBag.Corredores = numCorredores;
            return PartialView();
        }

        [HttpPost]
        public void Inscribir(List<Corredor> corredor)
        {
            Session.Timeout = 50;
            Session["corredores"] = corredor;
        }

        public ActionResult Confirmation(string tx = "", string st = "", double amt = 0)
        {
            if (st == "Completed")
            {
                Manager m = new Manager();
                var numCorredores = amt / 360;
                if (numCorredores < 1)
                    return RedirectToAction("Error", new { message = 3 });

                var corredores = Session["corredores"] as List<Corredor>;
                if (corredores == null)
                    return RedirectToAction("Error", new { message = 1 });

                while(corredores.Count != numCorredores)
                {
                    var i = corredores.Count - 1;
                    corredores.RemoveAt(i);
                }
                var totalInscritos = m.CreateCorredores(corredores, tx);
                ViewBag.TotalInscritos = totalInscritos;
                ViewBag.Confirmation = tx;
                Session.Abandon();
                return View();
            }
            else return RedirectToAction("Error", new { message = 2 });
        }

        public ActionResult Error(int message = 0)
        {
            switch (message)
            {
                case 1:
                    ViewBag.Error = "¡La sesión ha expirado!";
                    ViewBag.Cod = 1010;
                    break;
                case 2:
                    ViewBag.Error = "¡El pago no pudo ser completado!";
                    ViewBag.Cod = 1011;
                    break;
                case 3:
                    ViewBag.Error = "¡El pago no ha sido procesado correctamente!";
                    ViewBag.Cod = 1012;
                    break;
                default:
                    ViewBag.Error = "¡Ha ocurrido un error inesperado!";
                    ViewBag.Cod = 1010;
                    break;
            }
            return View();
        }
    }
}