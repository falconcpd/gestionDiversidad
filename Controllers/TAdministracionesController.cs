using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using gestionDiversidad.Interfaces;
using gestionDiversidad.Constantes;
using gestionDiversidad.ViewModels;
using gestionDiversidad.Navigation;
using Newtonsoft.Json;

namespace gestionDiversidad.Controllers
{
    public class TAdministracionesController : Controller
    {
        private readonly TfgContext _context;
        private readonly IServiceController _serviceController;

        public TAdministracionesController(TfgContext context, IServiceController sc)
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

        //Función que devuelve el usuario en el que nos encontramos
        public UserNavigation giveActualUser()
        {
            string userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
            UserNavigation actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;
            return actualUser;
        }

        // GET: TAdministraciones/infoBasica/5
        public async Task<IActionResult> infoBasica(string id)
        {
            AdminView vistaAdmin = new AdminView();
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();
            UserNavigation actualUser = giveActualUser();
            string nif = id;
            vistaAdmin = new AdminView();


            if (id == null || _context.TAlumnos == null || sesionRol == 0)
            {
                return NotFound();
            }

            var tAdmin = await _context.TAdministracions
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tAdmin == null)
            {
                return NotFound();
            }

            vistaAdmin.Admin = tAdmin;
            vistaAdmin.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenAdministracion, sesionRol);
            vistaAdmin.LAsignaturas = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAsignaturas, sesionRol);
            vistaAdmin.LMatriculas = await _serviceController
                .permisoPantalla(constDefinidas.screenListaMatriculas, sesionRol);
            vistaAdmin.LMedicos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaMedicos, sesionRol);
            vistaAdmin.LProfesores = await _serviceController
                .permisoPantalla(constDefinidas.screenListaProfesores, sesionRol);
            vistaAdmin.LAlumnos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, sesionRol);
            vistaAdmin.LInformes = await _serviceController
                .permisoPantalla(constDefinidas.screenListalInformes, sesionRol);
            vistaAdmin.LDocencias = await _serviceController
                .permisoPantalla(constDefinidas.screenListaDocencias, sesionRol);
            vistaAdmin.LMatriculas = await _serviceController
                .permisoPantalla(constDefinidas.screenListaMatriculas, sesionRol);
            vistaAdmin.Rol = constDefinidas.rolAdmin;
            vistaAdmin.Nif = nif;
            vistaAdmin.SesionRol = sesionRol;
            vistaAdmin.SesionNif = sesionNif;
            return View(vistaAdmin);
        }
    }
}
