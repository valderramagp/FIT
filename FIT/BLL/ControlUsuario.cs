using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FIT.Models;
namespace FIT.BLL
{
    public class ControlUsuario
    {
        readonly FITEntities _ctx;
        public ControlUsuario()
        {
            _ctx = new FITEntities();
        }

        public Usuarios GetUsuarioByCuenta(string cuenta, string password)
        {
            return _ctx.Usuarios.FirstOrDefault(x => x.Nombre.Equals(cuenta) && x.Password.Equals(password));
        }

        public Usuarios GetUsuarioById(int IdUser)
        {
            return _ctx.Usuarios.FirstOrDefault(x => x.IdUsuario == IdUser);
        }
    }
}