using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using gestionDiversidad.Interfaces;
using gestionDiversidad.ViewModels;
using gestionDiversidad.ViewModels.TProfesores;
using gestionDiversidad.Constantes;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.ComponentModel;
using gestionDiversidad.Navigation;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp;

namespace gestionDiversidad.Controllers
{
    public class TProfesoresController : Controller
    {
        private readonly TfgContext _context;
        private readonly IServiceController _serviceController;

        public TProfesoresController(TfgContext context, IServiceController sc)
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

        // GET: TProfesores/infoBasica/5
        public async Task<IActionResult> infoBasica(string id)
        {
            ProfesorView vistaProfesor = new ProfesorView();
            int sesionRol = giveSesionRol();
            UserNavigation actualUser = giveActualUser();
            string actualJson;

            if (id == null || _context.TProfesors == null || sesionRol == 0)
            {
                return NotFound();
            }

            var tProfesor = await _context.TProfesors
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tProfesor == null)
            {
                return NotFound();
            }

            if (!(actualUser.rol == constDefinidas.rolProfesor))
            {
                actualUser = new UserNavigation(id, constDefinidas.rolProfesor, actualUser);
                actualJson = JsonConvert.SerializeObject(actualUser);
                HttpContext.Session.SetString(constDefinidas.keyActualUser, actualJson);
            }

            vistaProfesor.Profesor = tProfesor;
            vistaProfesor.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenProfesor, sesionRol);
            vistaProfesor.LDocencias = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAsignaturas, sesionRol);
            vistaProfesor.LAlumnos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, sesionRol);
            vistaProfesor.LProfesores = await _serviceController
                .permisoPantalla(constDefinidas.screenListaProfesores, sesionRol);

            return View(vistaProfesor);
        }

        //GET: TProfesores/listaProfesores
        public async Task<IActionResult> listaProfesores(string volverPadre)
        {
            List<TProfesor> listaProfesores;
            ListaProfesoresView vistaListasProfesores = new ListaProfesoresView();
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();
            bool volverPadreValue;
            UserNavigation actualUser = giveActualUser();

            volverPadreValue = bool.Parse(volverPadre);
            if (volverPadreValue)
            {
                string userNavigationPadreJson = JsonConvert.SerializeObject(actualUser.padre);
                HttpContext.Session.SetString(constDefinidas.keyActualUser, userNavigationPadreJson);
                actualUser = giveActualUser();
            }

            listaProfesores = await _serviceController.listaProfesores();

            vistaListasProfesores.ListaProfesores = listaProfesores;
            vistaListasProfesores.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenListaProfesores, sesionRol);
            vistaListasProfesores.Profesor = await _serviceController
                .permisoPantalla(constDefinidas.screenProfesor, sesionRol);
            vistaListasProfesores.SesionNif = sesionNif;
            vistaListasProfesores.SesionRol = sesionRol;

            return View(vistaListasProfesores);
        }

        // GET: TProfesores/listaDocencias
        public async Task<IActionResult> listaDocencias()
        {
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();

            List<TProfesor> profesores = (await _serviceController.listaProfesores());
            ListaDocenciasView vistaDocencia = new ListaDocenciasView();
            vistaDocencia.Pantalla = await _serviceController
                .permisoPantalla(constDefinidas.screenListaDocencias, sesionRol);
            vistaDocencia.LProfesores = profesores;
            vistaDocencia.SesionRol = sesionRol;
            vistaDocencia.SesionNif = sesionNif;
            return View(vistaDocencia);
        }

        // GET: TProfesores/insertarDocencia
        public async Task<IActionResult> insertarDocencia()
        {
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();

            CrearDocenciaView crearDocenciaVista = new CrearDocenciaView();
            crearDocenciaVista.LProfesores = (await _serviceController.listaProfesores());
            crearDocenciaVista.LAsignaturas = (await _serviceController
                .listaAsignaturas(sesionNif, sesionRol));

            return View(crearDocenciaVista);
        }

        //POST: TProfesores/crearDocencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> crearDocencia(CrearDocenciaView model)
        {
            string sesionNif = giveSesionNif();
            int idAsignatura = int.Parse(model.IdAsignatura);
            var profesor = await _context.TProfesors
                .Include(p => p.IdAsignaturas)
                .FirstOrDefaultAsync(p => p.Nif == model.NifProfesor);
            var asignatura = await _context.TAsignaturas
                .FirstOrDefaultAsync(a => a.Id == idAsignatura);
            List<TAsignatura> listaAsiganturas = profesor!.IdAsignaturas.ToList();
            bool imparte = listaAsiganturas.Contains(asignatura!);
            if (ModelState.IsValid && !(imparte))
            {
                profesor!.IdAsignaturas.Add(asignatura!);
                await _context.SaveChangesAsync();
                await _serviceController
                    .guardarAuditoria(sesionNif, constDefinidas.screenListaDocencias, constDefinidas.accionCrear);

                return RedirectToAction("listaDocencias", "TProfesores");

            }
            TempData["ExisteDocencia"] = "El profesor ya está impartiendo esa asignatura";
            return RedirectToAction("insertarDocencia", "TProfesores");
        }

        //GET: TProfesores/modificarProfesor
        public async Task<IActionResult> modificarProfesor()
        {
            UserNavigation actualUser = giveActualUser();
            TProfesor profesor = (await _context.TProfesors
                .Include(p => p.NifNavigation)
                .FirstOrDefaultAsync(p => p.Nif == actualUser.nif))!;
            ModificarUsuarios modificarProfesorView = new ModificarUsuarios();
            modificarProfesorView.Nif = profesor.Nif;
            modificarProfesorView.Rol = constDefinidas.rolProfesor;
            modificarProfesorView.Nombre = profesor.Nombre;
            modificarProfesorView.Apellido1 = profesor.Apellido1;
            modificarProfesorView.Apellido2 = profesor.Apellido2;
            modificarProfesorView.Password = profesor.NifNavigation.Password;
            modificarProfesorView.ConfirmPassword = profesor.NifNavigation.Password;
            modificarProfesorView.Usuario = profesor.NifNavigation.Usuario;

            return View(modificarProfesorView);
        }

        //GET: TProfesores/insertarProfesor
        public IActionResult insertarProfesor()
        {
            CrearProfesorView vistaCrearProfesor = new CrearProfesorView();

            return View(vistaCrearProfesor);
        }

        //Función que crea al profesor: TProfesores/crearProfesor
        public async Task<IActionResult> crearProfesor(string nif, string nombre, string apellido1, string apellido2)
        {
            string sesionNif = giveSesionNif();
            var profesor = new TProfesor
            {
                Nif = nif,
                Nombre = nombre,
                Apellido1= apellido1,
                Apellido2= apellido2
            };

            _context.Add(profesor);
            await _context.SaveChangesAsync();
            await _serviceController
               .guardarCrearBorrarUsuarioAuditoria(sesionNif, constDefinidas.screenListaProfesores, constDefinidas.accionCrearUsuario, nif);

            return RedirectToAction("listaProfesores", "TProfesores", 
                new { volverPadre = "false" });
        }

        // GET: TProfesores/borrarDocencia/5
        public async Task<IActionResult> borrarDocencia(int idAsignatura, string nifProfesor)
        {
            BorrarDocenciaView vistaBorrarDocencia = new BorrarDocenciaView();
            TProfesor profesor = (await _context.TProfesors.FirstOrDefaultAsync(p => p.Nif == nifProfesor))!;
            TAsignatura asignatura = (await _context.TAsignaturas.FirstOrDefaultAsync(a => a.Id == idAsignatura))!;
            vistaBorrarDocencia.Asignatura = asignatura;
            vistaBorrarDocencia.Profesor = profesor;

            return View(vistaBorrarDocencia);
        }

        // POST: TAsignaturas/confirmarBorradoDocencia/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarBorradoDocencia(int idAsignatura, string nifProfesor)
        {
            string sesionNif = giveSesionNif();
            TProfesor profesor = (await _context.TProfesors
                .Include(p => p.IdAsignaturas)
                .FirstOrDefaultAsync(p => p.Nif == nifProfesor))!;
            TAsignatura asignatura = (await _context.TAsignaturas
                .Include(a => a.NifProfesors)
                .FirstOrDefaultAsync(a => a.Id == idAsignatura))!;
            profesor.IdAsignaturas.Remove(asignatura);
            asignatura.NifProfesors.Remove(profesor);

            await _context.SaveChangesAsync();
            await _serviceController
                .guardarAuditoria(sesionNif, constDefinidas.screenListaDocencias, constDefinidas.accionBorrar);
            return RedirectToAction("listaDocencias", "TProfesores");

        }

        // GET: TProfesores/borrarProfesor/5
        public async Task<IActionResult> borrarProfesor(string nifProfesor)
        {
            BorrarProfesorView vistaBorrarProfesor = new BorrarProfesorView();
            TProfesor profesor = (await _context.TProfesors
                    .FirstOrDefaultAsync(p => p.Nif == nifProfesor))!;
            vistaBorrarProfesor.Profesor = profesor;

            return View(vistaBorrarProfesor);
        }

        // POST: TProfesores/confirmarBorradoProfesor/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarBorradoProfesor(string nifProfesor)
        {
            string sesionNif = giveSesionNif();
            TProfesor profesor = (await _context.TProfesors
                    .FirstOrDefaultAsync(p => p.Nif == nifProfesor))!;
            _context.TProfesors.Remove(profesor);

            List<TAsignatura> asignaturas = (await _serviceController
                .listaAsignaturas(nifProfesor, constDefinidas.rolProfesor))!;
            foreach (var asignatura in asignaturas)
            {
                asignatura.NifProfesors.Remove(profesor);
            }
            TUsuario usuario = (await _context.TUsuarios
                .FirstOrDefaultAsync(u => u.Nif == nifProfesor))!;
            await _serviceController
                .guardarCrearBorrarUsuarioAuditoria(sesionNif, constDefinidas.screenListaProfesores, constDefinidas.accionBorrar, nifProfesor);

            _context.TUsuarios.Remove(usuario);

            await _context.SaveChangesAsync();


            return RedirectToAction("listaProfesores", "TProfesores", new
            {
                volverPadre = "false"
            });
        }
    }
}
