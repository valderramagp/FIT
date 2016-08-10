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

            var status = CookieExists("AcceptCookies");
            if (!status)
                return RedirectToAction("Error", new { message = 4 });
            Response.Cookies["AcceptCookies"].Expires = DateTime.Now.AddDays(-1);
            if(CookieExists("codigo"))
            {
                Response.Cookies["codigo"].Expires = DateTime.Now.AddDays(-1);
            } 
            return View();
        }

        public ActionResult Offline()
        {
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
            var code = m.RandomString(10);
            Response.Cookies["codigo"].Value = code;
            Response.Cookies["codigo"].Expires = DateTime.Now.AddDays(1);
            m.CreateCorredores(corredor, code);

            if (!CookieExists("codigo"))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(code, JsonRequestBehavior.AllowGet);
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

        public ActionResult SuccessEmail(DatosCorreo corredor)
        {
            return View(corredor);
        }

        private bool CookieExists(string cookie)
        {
            return Request.Cookies[cookie] != null ? true : false;
        }

        public bool TestSession()
        {
            var corredores = Session["corredores"] as List<Corredor>;
            return corredores == null ? false : true;
        }

        [HttpPost]
        public void Notification()
        {
            var cookie = Request["custom"];
            Manager m = new Manager();
            var inscritos = m.CreateCorredores(cookie);
            foreach (var item in inscritos)
            {
                DatosCorreo data = new DatosCorreo();
                data.NumCorredor = item.Folio;
                data.Nombre = item.Nombres;
                data.Paterno = item.Paterno;
                data.Materno = item.Materno;
                data.Edad = item.Edad;
                data.Telefono = item.Telefono;
                data.Celular = item.Celular;
                data.Correo = item.Correo;
                data.Sexo = item.Sexo;
                data.Talla = item.Talla;
                data.Carrera = m.GetCarreraById(item.IdCarrera).Descripcion;
                data.ConfirmacionPago = item.ConfirmacionPago;
                var body = RenderViewToString(data);
                m.SendMail(item, body);
            }
        }

        public ActionResult Confirmation(string st = "")
        {
            if (st == "Completed")
            {
                Manager m = new Manager();

                if (!CookieExists("codigo"))
                    return RedirectToAction("message", new { message = 1 });

                HttpCookie codigo = new HttpCookie("codigo");
                codigo = Request.Cookies["codigo"];
                
                ViewBag.Confirmation = codigo.Value;
                Response.Cookies["codigo"].Expires = DateTime.Now.AddDays(-1);
                return View();
            }
            return RedirectToAction("Error", new { message = 2 });
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