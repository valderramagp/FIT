using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace FIT.Filters
{
    public class SessionHelpers
    {
        public static void CerrarSesion()
        {
            FormsAuthentication.SignOut();
        }

        public static bool IsAutenticado()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public static int GetIdUsuario()
        {
            var IdUsuario = 0;

            if (HttpContext.Current.User == null || !(HttpContext.Current.User.Identity is FormsIdentity))
                return IdUsuario;

            FormsAuthenticationTicket ticket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;

            if (ticket != null)
                IdUsuario = Convert.ToInt32(ticket.UserData);

            return IdUsuario;

        }

        public static void IniciarSesion(string nombreUsuario, string idUsuario)
        {
            var cookieUsuario = FormsAuthentication.GetAuthCookie(nombreUsuario, true);
            cookieUsuario.Name = FormsAuthentication.FormsCookieName;
            cookieUsuario.Expires = DateTime.Now.AddDays(7);

            var ticket = FormsAuthentication.Decrypt(cookieUsuario.Value);
            var nuevoTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, idUsuario);
            cookieUsuario.Value = FormsAuthentication.Encrypt(nuevoTicket);
            HttpContext.Current.Response.Cookies.Add(cookieUsuario);
        }
    }
}