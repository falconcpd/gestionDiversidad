using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using gestionDiversidad.ViewModels;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing.Text;
using gestionDiversidad.Interfaces;


namespace gestionDiversidad.Controllers
{
    public class ServiceController : Controller, IServiceController
    {
        private readonly TfgContext _context;

        public ServiceController(TfgContext context)
        {
            _context = context;
        }

        //Filtra Listas de permisos por roles
        public List<TPermiso> permisosRol(int rol)
        {
            List<TPermiso> permisos;
            permisos = _context.TPermisos.Where(p => p.IdRol == rol).ToList();
            return permisos;

        }

        //Filtra Listas de permisos por roles y permisos
        public TPermiso permisoPantalla(int pantalla, int rol)
        {
            List<TPermiso> r_permisos = permisosRol(rol);
            TPermiso permiso = r_permisos.FirstOrDefault(p => p.IdPantalla == pantalla && p.IdRol == rol);

            return permiso;
        }

    }
}
