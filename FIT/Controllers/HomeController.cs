using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIT.BLL;
using FIT.Models;
using FIT.Filters;

namespace FIT.Controllers
{
    [LoginFilter]
    public class HomeController : Controller
    {
        Manager m = new Manager();
        public ActionResult Index()
        {
            var model = m.GetCorredores();
            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.Carreras = m.GetCarreras();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult CreateRunner(Corredor corredor)
        {
            m.CreateCorredor(corredor);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var model = m.GetCorredorById(id);
            ViewBag.Carreras = m.GetCarrerasList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult Modificar(Corredor model)
        {
            m.EditCorredor(model);
            return RedirectToAction("Index");
        }

        
        public RedirectToRouteResult Delete(int id)
        {
            m.DeleteCorredor(id);
            return RedirectToAction("Index");
        }
    }
}