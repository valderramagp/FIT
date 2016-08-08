using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FIT.Models;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace FIT.BLL
{
    public class Manager
    {
        FITEntities ctx = null;
        public Manager()
        {
            ctx = new FITEntities();
        }

        public List<Corredor> GetCorredores()
        {
            return ctx.Corredor.Where(x => x.Status == true).ToList();
        }

        public void CreateCorredor(Corredor model)
        {
            model.Status = true;
            model.ConfirmacionPago = "Manager";
            ctx.Corredor.Add(model);
            var status = ctx.SaveChanges();
            if(status > 0)
            {
                SendMail(model, "Administrador");
            }
        }

        /// <summary>
        /// Crea los corredores temporalmente en la base de datos con status false
        /// </summary>
        /// <param name="corredores"></param>
        /// <returns></returns>
        public int CreateCorredores(List<Corredor> corredores)
        {
            var code = RandomString(10);
            foreach(var corredor in corredores)
            {
                corredor.Status = false;
                corredor.ConfirmacionPago = code;
                ctx.Corredor.Add(corredor);
            }
            return ctx.SaveChanges();
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public int CreateCorredores(string codigo, string cp)
        {
            var exists = ctx.Corredor.Where(x => x.ConfirmacionPago == cp).ToList();
            var status = 0;
            if (exists.Count == 0)
            {
                var corredores = ctx.Corredor.Where(x => x.ConfirmacionPago == codigo).ToList();
                if (corredores != null)
                {
                    foreach (var corredor in corredores)
                    {
                        corredor.Status = true;
                        corredor.ConfirmacionPago = cp;
                    
                        SendMail(corredor, "Paypal");
                    }
                    status = ctx.SaveChanges();
                }
            }
            return status;
        }

        public void SendMail(Corredor corredor, string tipoPago)
        {
            string from = "g316polanco@gmail.com";
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(from, "duwugsrufumdlgyx"),
                EnableSsl = true,
                
            };
            var cuerpo = FormarCuerpo(corredor, tipoPago);
            MailMessage mail = new MailMessage(from, corredor.Correo, "FIT: ¡INSCRIPCIÓN EXITOSA!", cuerpo);
            mail.IsBodyHtml = true;
            client.Send(mail);
        }

        public string FormarCuerpo(Corredor corredor, string tipoPago)
        {
            var carrera = GetCarreraById(corredor.IdCarrera);
            string cuerpo = "Estimado " + corredor.Nombres + ", agradecemos su inscripción a la CARRERA FIT y anexamos su comprobante de inscripción:<br/><b> Información Personal </b><br/>";
            cuerpo += "Confirmación: " + corredor.ConfirmacionPago + "<br/>";
            cuerpo += "Nombre: " + corredor.Nombres + "<br/>";
            cuerpo += "Apellido Paterno: " + corredor.Paterno + "<br/>";
            cuerpo += "Apellido Materno: " + corredor.Materno + "<br/>";
            cuerpo += "Sexo: " + corredor.Sexo + "<br />";
            cuerpo += "Fecha de Nacimiento: " + corredor.Edad + "<br/>";
            cuerpo += "# Corredor: " + corredor.Folio + "<br/>";
            cuerpo += "<b>Información de la Carrera</b><br/>";
            cuerpo += "Carrera: FIT<br/>";
            cuerpo += "Fecha: SÁBADO 3 DE SEPTIEMBRE DEL 2016 A LAS 07:00<br/>";
            cuerpo += "Dirección: PARQUE BICENTENARIO<br/>";
            cuerpo += "<b>Información de la Inscripción</b><br/>";
            cuerpo += "Tipo de Pago: " + tipoPago + "<br/>";
            cuerpo += "Precio: $360.00<br/>";
            cuerpo += "Categoría: + " +  carrera.Descripcion + "<br/>";
            cuerpo += "Talla: " + corredor.Talla + "<br/><br/>";
            cuerpo += "Te recordamos que...<br/>";
            cuerpo += "<ul><li>Para recoger tu kit de corredor deberás llevar: confirmación de inscripción, ";
            cuerpo += "identificación oficial y exoneración de responsabilidades firmada la cual podrán descargar en los siguientes enlaces:";
            cuerpo += "FIT_ADULTO.pdf [G316POLANCO.ORG/CARRERAFIT/FIT_ADULTO.PDF]FIT_MENOR.pdf [G316POLANCO.ORG/CARRERAFIT/FIT_MENOR.PDF]</li>";
            cuerpo += "<li>La entrega de paquetes será el día 2 de SEPTIEMBRE en AUDITORIO G316 POLANCO DE 10M A 6PM </li>";
            cuerpo += "<li>Te recordamos que no hay cambios de datos, talla de playera ni distancia</li>";
            cuerpo += "<li>Llega una hora antes de tu salida</li>";
            cuerpo += "</ul>";
            return cuerpo;
        }

        public List<Carrera> GetCarrerasList()
        {
            return ctx.Carrera.ToList();
        }

        public IEnumerable<SelectListItem> GetCarreras()
        {
            return (from c in ctx.Carrera select new SelectListItem { Text = c.Descripcion, Value = c.IdCarrera.ToString() });
        }

        public Carrera GetCarreraById(int id)
        {
            return ctx.Carrera.FirstOrDefault(x => x.IdCarrera == id);
        }

        public Corredor GetCorredorById(int id)
        {
            return ctx.Corredor.Where(x => x.Folio == id).FirstOrDefault();
        }

        public void EditCorredor(Corredor corredor)
        {
            var original = GetCorredorById(corredor.Folio);
            if(original != null)
            {
                ctx.Entry(original).CurrentValues.SetValues(corredor);
                ctx.SaveChanges();
            }
        }

        public void DeleteCorredor(int id)
        {
            var corredor = GetCorredorById(id);
            ctx.Corredor.Remove(corredor);
            ctx.SaveChanges();
        }
    }
}