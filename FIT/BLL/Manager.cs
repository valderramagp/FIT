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
            return ctx.Corredor.ToList();
        }

        public void CreateCorredor(Corredor model)
        {
            ctx.Corredor.Add(model);
            var status = ctx.SaveChanges();
            if(status > 0)
            {
                SendMail(model);
            }
        }

        public void SendMail(Corredor corredor)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("gustavoavalderrama@gmail.com", "ntjmeaqaepvuxkce"),
                EnableSsl = true
            };

            client.Send("stephaniiebass@gmail.com", corredor.Correo, "test", "testbody");
        }

        public List<Carrera> GetCarrerasList()
        {
            return ctx.Carrera.ToList();
        }

        public IEnumerable<SelectListItem> GetCarreras()
        {
            return (from c in ctx.Carrera select new SelectListItem { Text = c.Descripcion, Value = c.IdCarrera.ToString() });
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