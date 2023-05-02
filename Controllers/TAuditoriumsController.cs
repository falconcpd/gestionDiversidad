using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using gestionDiversidad.Constantes;
using gestionDiversidad.ViewModels.TAuditoriums;
using gestionDiversidad.Interfaces;

namespace gestionDiversidad.Controllers
{
    public class TAuditoriumsController : Controller
    {
        private readonly TfgContext _context;
        private readonly IServiceController _serviceController;

        public TAuditoriumsController(TfgContext context, IServiceController sc)
        {
            _context = context;
            _serviceController = sc;
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

        //GET: TAuditoriums/listaAuditoriums
        public async Task<IActionResult> listaAuditorias()
        {
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();
            ListaAuditoriasView vistaListaAuditorias = new ListaAuditoriasView();

            vistaListaAuditorias.SesionNif = sesionNif;
            vistaListaAuditorias.SesionRol = sesionRol;
            vistaListaAuditorias.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenAuditoria, sesionRol);
            vistaListaAuditorias.ListaAuditorias = await _serviceController.listaAuditorias();

            return View(vistaListaAuditorias);
        }
    }
}
