﻿using System;
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
            var cp = RandomString(10);
            model.Status = true;
            model.ConfirmacionPago = cp;
            ctx.Corredor.Add(model);
            ctx.SaveChanges();
        }

        /// <summary>
        /// Crea los corredores temporalmente en la base de datos con status false
        /// </summary>
        /// <param name="corredores"></param>
        /// <returns></returns>
        public int CreateCorredores(List<Temporal> temporales, string code)
        {
            foreach(var corredor in temporales)
            {
                corredor.FechaRegistro = DateTime.Now;
                corredor.Cookie = code;
                ctx.Temporal.Add(corredor);
            }
            return ctx.SaveChanges();
        }

       

        

        public string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public List<Corredor> CreateCorredores(string codigo, string cp)
        {
            var temporales = ctx.Temporal.Where(x => x.Cookie == codigo).ToList();
            if (temporales != null)
            {
                var corredores = new List<Corredor>();
                foreach (var temporal in temporales)
                {
                    Corredor corredor = new Corredor();
                    corredor.Nombres = temporal.Nombres;
                    corredor.Paterno = temporal.Paterno;
                    corredor.Materno = temporal.Materno;
                    corredor.Edad = temporal.Edad;
                    corredor.Telefono = temporal.Telefono;
                    corredor.Celular = temporal.Celular;
                    corredor.Correo = temporal.Correo;
                    corredor.Sexo = temporal.Sexo;
                    corredor.Talla = temporal.Talla;
                    corredor.IdCarrera = temporal.IdCarrera;
                    corredor.Status = true;
                    corredor.ConfirmacionPago = cp;
                    corredores.Add(corredor);
                    ctx.Corredor.Add(corredor);
                }
                ctx.SaveChanges();
                return corredores;
            }
            return null;
        }

        public List<Temporal> GetTemporales(string cookie)
        {
            return ctx.Temporal.Where(x => x.Cookie == cookie).ToList();
        }

        public List<Corredor> GetInscritos(string cookie)
        {
            return ctx.Corredor.Where(x => x.ConfirmacionPago == cookie).ToList();
        }

        public List<Corredor> CreateCorredores(string cookie)
        {
            var temporales = ctx.Temporal.Where(x => x.Cookie == cookie).ToList();
            var corredores = new List<Corredor>();
            foreach (var temporal in temporales)
            {
                Corredor corredor = new Corredor();
                corredor.Nombres = temporal.Nombres;
                corredor.Paterno = temporal.Paterno;
                corredor.Materno = temporal.Materno;
                corredor.Edad = temporal.Edad;
                corredor.Telefono = temporal.Telefono;
                corredor.Celular = temporal.Celular;
                corredor.Correo = temporal.Correo;
                corredor.Sexo = temporal.Sexo;
                corredor.Talla = temporal.Talla;
                corredor.IdCarrera = temporal.IdCarrera;
                corredor.Status = true;
                corredor.ConfirmacionPago = cookie;
                corredores.Add(corredor);
                ctx.Corredor.Add(corredor);
            }
            ctx.SaveChanges();
            return corredores;
        }

        public void EliminarTemporales(string cookie)
        {
            var temporales = ctx.Temporal.Where(x => x.Cookie == cookie);
            ctx.Temporal.RemoveRange(temporales);
            ctx.SaveChanges();
        }

        public void SendMail(Corredor corredor, string body)
        {
            string from = "g316polanco@gmail.com";
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(from, "duwugsrufumdlgyx"),
                EnableSsl = true,
            };
            MailMessage mail = new MailMessage(from, corredor.Correo, "FIT: ¡INSCRIPCIÓN EXITOSA!", body);
            mail.IsBodyHtml = true;
            client.Send(mail);

            MailMessage mailAdmin = new MailMessage(from, "g316polanco@jme.mx", "FIT: Confirmación de Pago de " + corredor.Nombres, body);
            mailAdmin.To.Add("gustavoavalderrama@gmail.com");
            mailAdmin.IsBodyHtml = true;
            client.Send(mailAdmin);
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
                corredor.Status = original.Status;
                corredor.ConfirmacionPago = original.ConfirmacionPago;
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