using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIT.Filters;
using FIT.ViewModels;
using FIT.BLL;

namespace FIT.Controllers
{
    public class AccesoController : Controller
    {
        [NoLogin]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LogIn(Login model)
        {
            ControlUsuario cu = new ControlUsuario();
            
            var usuario = cu.GetUsuarioByCuenta(model.Username, model.Password);
            if (usuario != null)
            {
                SessionHelpers.IniciarSesion(usuario.Nombre, usuario.IdUsuario.ToString());
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            SessionHelpers.CerrarSesion();
            return RedirectToAction("Index");
        }
    }
}