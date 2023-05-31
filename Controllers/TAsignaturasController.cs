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
using gestionDiversidad.Navigation;
using Newtonsoft.Json;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using gestionDiversidad.ViewModels.TAsignaturas;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace gestionDiversidad.Controllers
{
    public class TAsignaturasController : Controller
    {
        private readonly TfgContext _context;
        private readonly IServiceController _serviceController;

        public TAsignaturasController(TfgContext context, IServiceController sc)
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

        //GET: TAsignaturas/listaAsignaturas
        public async Task<IActionResult> listaAsignaturas()
        {
            ListaAsignaturasView vistaListaAsignaturas= new ListaAsignaturasView();
            int sesionRol = giveSesionRol();
            UserNavigation actualUser = giveActualUser();
            string actualName = (await _serviceController.giveActualNombre(actualUser.nif, actualUser.rol));
            List<TAsignatura> asignaturas = await _serviceController
                .listaAsignaturas(actualUser.nif, actualUser.rol);

            vistaListaAsignaturas.ListaAsignaturas = asignaturas;
            vistaListaAsignaturas.Permiso = await _serviceController.
                permisoPantalla(constDefinidas.screenListaAsignaturas, sesionRol);
            vistaListaAsignaturas.ActualRol = actualUser.rol;
            vistaListaAsignaturas.ActualNif = actualUser.nif;
            vistaListaAsignaturas.ActualName = actualName;

            return View(vistaListaAsignaturas);

        }

        //GET: TAsignaturas/modificarAsignatura
        public async Task<IActionResult> modificarAsignatura(int id)
        {
            TAsignatura asignatura = (await _context.TAsignaturas
                .FirstOrDefaultAsync(a => a.Id == id))!;
            ModificarAsignatura modificarAsignaturaView = new ModificarAsignatura();
            modificarAsignaturaView.Id = asignatura.Id;
            modificarAsignaturaView.Nombre = asignatura.Nombre;

            return View(modificarAsignaturaView);
        }

        //Funcion que te devuelve los nombres de las asignaturas
        public async Task<List<string>> nombresAsignatura()
        {
            List<string> nombres = new List<string>();
            nombres = (await _context.TAsignaturas
                .Select(a => a.Nombre)
                .ToListAsync());
            return nombres;

        }

        //GET: TAsignaturas/insertarAsignatura
        public IActionResult insertarAsignatura()
        {
            UserNavigation actualUser = giveActualUser();
            CrearAsignaturaView vistaCrearAsignatura = new CrearAsignaturaView();
            vistaCrearAsignatura.ActualNif = actualUser.nif;
            vistaCrearAsignatura.ActualRol = actualUser.rol;


            return View(vistaCrearAsignatura);
        }

        //POST: TAsignaturas/crearAsignatura
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> crearAsignatura(CrearAsignaturaView model)
        {
            string sesionNif = giveSesionNif();
            string nombre = _serviceController.quitarEspacios(model.Nombre);


            if (ModelState.IsValid)
            {
                var asignatura = new TAsignatura
                {
                    Nombre = nombre

            };
                _context.Add(asignatura);
                await _serviceController
                    .guardarCrearBorrarAsignaturaAuditoria(sesionNif, constDefinidas.screenListaAsignaturas, constDefinidas.accionCrearElemento, nombre);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction("listaAsignaturas", "TAsignaturas");
        }

        //[Remote] para ver que la estructura está bien
        public async Task<IActionResult> verificarAsignatura(string idAsignatura)
        {
            if (_serviceController.confirmarEstructura(idAsignatura) && await _serviceController.existeDistintaAsignatura(idAsignatura))
            {
                return Json(true);
            }

            return Json(false);

        }

        //[Remote] para que no se repita nombre en la creación de la asignatura
        public async Task<IActionResult> verificarNombreAsignatura(string nombre)
        {
            if (nombre == null)
            {
                //ModelState.AddModelError("Nif", "El campo no puede estar vacío o contener solo espacios en blanco");
                return Json("El nombre no puede ser solo espacios en blanco");
            }
            nombre = _serviceController.quitarEspacios(nombre);
            if (await _context.TAsignaturas.AnyAsync(a => a.Nombre == nombre))
            {
                return Json(false);
            }
            return Json(true);

        }

        //[Remote] para que no se repita nombre de asignatura mientras lo modificas
        public async Task<IActionResult> verificarModificarAsignatura(int id, string nombre)
        {
            if (nombre == null)
            {
                return Json("El nombre no puede ser solo espacios en blanco");
            }
            nombre = _serviceController.quitarEspacios(nombre);
            TAsignatura asignatura = (await _context.TAsignaturas.FirstOrDefaultAsync(a => a.Id == id))!;
            var TAsignatura = await _context.TAsignaturas
                .AnyAsync(a => a.Nombre == nombre);
            if (asignatura!.Nombre == nombre)
            {
                return Json(TAsignatura);
            }
            return Json(!TAsignatura);

        }

        // POST: TUsuarios/confirmarCambiosAsignatura
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarCambioAsignatura(ModificarAsignatura model)
        {
            string nuevoNombre = _serviceController.quitarEspacios(model.Nombre);
            TAsignatura asignatura = (await _context.TAsignaturas
                .FirstOrDefaultAsync(a => a.Id == model.Id))!;

            if (ModelState.IsValid && (asignatura.Nombre != nuevoNombre))
            {
                string sesionNif = giveSesionNif();

                await _serviceController
                    .guardarModificarAsignaturaAuditoria(sesionNif, constDefinidas.screenListaAsignaturas, asignatura.Nombre, nuevoNombre, model.Id);
                asignatura.Nombre = _serviceController.quitarEspacios(model.Nombre);
                await _context.SaveChangesAsync();

                return RedirectToAction("listaAsignaturas", "TAsignaturas");
            }

            TempData["SinCambiosAsignatura"] = "No se ha modificado ningún parámetro de la asignatura";

            return RedirectToAction("modificarAsignatura", "TAsignaturas", new
            {
                id = model.Id
            });
        }


        // GET: TAsignaturas/borrarAsignatura/5
        public async Task<IActionResult> borrarAsignatura(string actualNif, int actualRol,int? id)
        {
            AsignaturaView vistaAsignatura = new AsignaturaView();

            if (id == null || _context.TAsignaturas == null)
            {
                return NotFound();
            }
            var tAsignatura = await _context.TAsignaturas
                .Include(a => a.NifAlumnos)
                .Include(a => a.NifProfesors)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (tAsignatura == null)
            {
                return NotFound();
            }

            vistaAsignatura.Asignatura = tAsignatura;
            vistaAsignatura.ActualRol = actualRol;
            vistaAsignatura.ActualNif = actualNif;

            return View(vistaAsignatura);
        }

        // POST: TAsignaturas/confirmarBorrado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarBorrado(int id)
        {
            string sesionNif = giveSesionNif();
            if (_context.TAsignaturas == null)
            {
                return Problem("Entity set 'TfgContext.TAsignaturas'  is null.");
            }
            var tAsignatura = await _context.TAsignaturas
                .Include(a => a.NifAlumnos)
                .Include(a => a.NifProfesors)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (tAsignatura != null)
            {
                List<TProfesor> profesores = tAsignatura.NifProfesors.ToList();
                List<TAlumno> alumnos = tAsignatura.NifAlumnos.ToList();    

                foreach(var profesor in profesores)
                {
                    profesor.IdAsignaturas.Remove(tAsignatura);
                }
                foreach(var alumno in alumnos)
                {
                    alumno.IdAsignaturas.Remove(tAsignatura);
                }
                await _serviceController
                    .guardarCrearBorrarAsignaturaAuditoria(sesionNif, constDefinidas.screenListaAsignaturas, constDefinidas.accionBorrarElemento, tAsignatura.Nombre);

                _context.TAsignaturas.Remove(tAsignatura);

            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("listaAsignaturas", "TAsignaturas");
        }
    }
}
