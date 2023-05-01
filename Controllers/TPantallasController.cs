using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using gestionDiversidad.Constantes;
using gestionDiversidad.ViewModels.TPantallas;

namespace gestionDiversidad.Controllers
{
    public class TPantallasController : Controller
    {
        private readonly TfgContext _context;

        public TPantallasController(TfgContext context)
        {
            _context = context;
        }

        //Función para recuperar el rol del usuario que ha iniciado sesión
        public int giveSesionRol()
        {
            int? rolRaw = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            int rol = rolRaw ?? 0;
            return rol;
        }

        //Función para recuperar el nif del usuario que ha iniciado sesión 
        public string giveSesionNif()
        {
            string sesionNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            return sesionNif;
        }

        // GET: TPantallas/listaPantallas
        public async Task<IActionResult> listaPantallas()
        {
            List<TPantalla> listaPantallas = (await _context.TPantallas.ToListAsync());
            ListaPantallasView vistaListaPantallas = new ListaPantallasView();
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();

            vistaListaPantallas.ListaPantallas = listaPantallas;
            vistaListaPantallas.SesionNif = sesionNif;
            vistaListaPantallas.SesionRol = sesionRol;

            return View(vistaListaPantallas);

        }
    }
}
