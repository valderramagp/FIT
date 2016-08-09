using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIT.BLL;
using FIT.Models;
using System.Web.UI;
using System.IO;
using System.Web.Routing;

namespace FIT.Controllers
{
    public class InscripcionController : Controller
    {
        // GET: Inscripcion
        public ActionResult Index()
        {
            Response.Cookies["AcceptCookies"].Value = "ok";
            Response.Cookies["AcceptCookies"].Expires = DateTime.Now.AddDays(1);

            var status = TestCookies("AcceptCookies");
            if (!status)
                return RedirectToAction("Error", new { message = 4 });
            Response.Cookies["AcceptCookies"].Expires = DateTime.Now.AddDays(-1);
            if(TestCookies("codigo"))
            {
                Response.Cookies["codigo"].Expires = DateTime.Now.AddDays(-1);
            } 
            return View();
        }

        public PartialViewResult DatosCompetidor(int numCorredores = 1)
        {
            ViewBag.Corredores = numCorredores;
            return PartialView();
        }

        [HttpPost]
        public JsonResult Inscribir(List<Temporal> corredor)
        {
            Manager m = new Manager();
            m.CreateCorredores(corredor);
            Response.Cookies["codigo"].Value = corredor[0].Cookie;
            Response.Cookies["codigo"].Expires = DateTime.Now.AddDays(1);
            var status = TestCookies("codigo");
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public string RenderViewToString(object viewData)
        {
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", "Inscripcion");
                var inscripcionController = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new InscripcionController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(inscripcionController, "SuccessEmail", "", false);

                var viewContext = new ViewContext(inscripcionController, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        public ActionResult SuccessEmail(Corredor corredor)
        {
            return View(corredor);
        }

        private bool TestCookies(string cookie)
        {
            return Request.Cookies[cookie] != null ? true : false;
        }

        public bool TestSession()
        {
            var corredores = Session["corredores"] as List<Corredor>;
            return corredores == null ? false : true;
        }

        public ActionResult Confirmation(string tx = "", string st = "", double amt = 0)
        {
            if (st == "Completed")
            {
                Manager m = new Manager();
                var numCorredores = amt / 360;
                if (numCorredores < 1)
                    return RedirectToAction("Error", new { message = 3 });

                HttpCookie codigo = new HttpCookie("codigo");
                codigo = Request.Cookies["codigo"];
                var exists = TestCookies("codigo");
                if (!exists)
                    return RedirectToAction("Error", new { message = 1 });
                
                var totalInscritos = m.CreateCorredores(codigo.Value, tx);
                if (totalInscritos.Count > 0)
                {
                    foreach(var item in totalInscritos)
                    {
                        item.Carrera = m.GetCarreraById(item.IdCarrera);
                        var body = RenderViewToString(item);
                        m.SendMail(item, body);
                    }

                    ViewBag.TotalInscritos = totalInscritos.Count;
                    ViewBag.Confirmation = tx;
                    Response.Cookies["codigo"].Expires = DateTime.Now.AddDays(-1);
                    return View();
                }
                else return RedirectToAction("Error", new { message = 1 });
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
                case 4:
                    ViewBag.Error = "Por favor configura tu navegador para aceptar cookies!";
                    ViewBag.Cod = 1013;
                    break;
                case 404:
                    ViewBag.Error = "La página que buscas no existe!";
                    ViewBag.Cod = 404;
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