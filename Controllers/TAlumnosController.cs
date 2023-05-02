using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using gestionDiversidad.ViewModels;
using gestionDiversidad.Interfaces;
using gestionDiversidad.Navigation;
using gestionDiversidad.Constantes;
using Newtonsoft.Json;
using gestionDiversidad.ViewModels.TAlumnos;

namespace gestionDiversidad.Controllers
{
    public class TAlumnosController : Controller
    {
        private readonly TfgContext _context;
        private readonly IServiceController _serviceController;

        public TAlumnosController(TfgContext context, IServiceController sc)
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

        // GET: TAlumnos/infoBasica/5
        public async Task<IActionResult> infoBasica(string id)
        {
            AlumnoView vistaAlumno = new AlumnoView();
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();
            UserNavigation actualUser = giveActualUser();
            int actualRol = actualUser.rol;
            string actualJson;

            if (id == null || _context.TAlumnos == null || sesionRol == 0)
            {
                return NotFound();
            }

            var alumno = await _context.TAlumnos
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (alumno == null)
            {
                return NotFound();
            }

            if (!(actualRol == constDefinidas.rolAlumno))
            {
                actualUser = new UserNavigation(id, constDefinidas.rolAlumno, actualUser);
                actualJson = JsonConvert.SerializeObject(actualUser);
                HttpContext.Session.SetString(constDefinidas.keyActualUser, actualJson);
            }

            vistaAlumno.Alumno = alumno;
            vistaAlumno.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenAlumno, sesionRol);
            vistaAlumno.LInformes = await _serviceController
                .permisoPantalla(constDefinidas.screenListalInformes, sesionRol);
            vistaAlumno.LMatriculas = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAsignaturas, sesionRol);
            vistaAlumno.LAlumnos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, sesionRol);
            vistaAlumno.SesionRol = sesionRol;
            vistaAlumno.SesionNif = sesionNif;
            vistaAlumno.PadreNif = actualUser.padre?.nif;               
            vistaAlumno.PadreRol = actualUser.padre?.rol;

            return View(vistaAlumno);
        }

        //GET: TAlumnos/listaAlumnos
        public async Task<IActionResult> listaAlumnos(string nif, int rol, string volverPadre)
        {
            List<TAlumno> listaAlumnos;
            ListaAlumnosView vistaListasAlumno = new ListaAlumnosView();
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

            listaAlumnos = await _serviceController.listaAlumnos(nif, rol);

            vistaListasAlumno.ListaAlumnos = listaAlumnos;
            vistaListasAlumno.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, sesionRol);
            vistaListasAlumno.Alumno = await _serviceController
                .permisoPantalla(constDefinidas.screenAlumno, sesionRol);
            vistaListasAlumno.Rol = rol;
            vistaListasAlumno.Nif = nif;
            vistaListasAlumno.ActualNif = actualUser.nif;
            vistaListasAlumno.ActualRol = actualUser.rol;
            vistaListasAlumno.SesionRol = sesionRol;
            vistaListasAlumno.SesionNif = sesionNif;

            return View(vistaListasAlumno);
        }

        //GET: TAlumnos/insertarAlumno
        public async Task<IActionResult> insertarAlumno()
        {
            UserNavigation actualUser = giveActualUser();
            TMedico medicoTemporal = (await _context.TMedicos
                .FirstOrDefaultAsync(m => m.Nif == constDefinidas.keyMedicoTemporal))!;

            CrearAlumnoView vistaCrearAlumno = new CrearAlumnoView();
            vistaCrearAlumno.ListaMedicos = (await _serviceController.listaMedicos());
            vistaCrearAlumno.ListaMedicos.Remove(medicoTemporal);
            vistaCrearAlumno.ActualRol = actualUser.rol;
            vistaCrearAlumno.ActualNif = actualUser.nif;

            return View(vistaCrearAlumno);
        }

        //Función que crea al alumno: TAlumnos/crearAlumno
        public async Task<IActionResult> crearAlumno(string nif, string nombre, string apellido1, string apellido2, string medico)
        {
            string sesionNif = giveSesionNif();

            var alumno = new TAlumno
            {
                Nif = nif,
                Nombre = nombre,
                Apellido1 = apellido1,
                Apellido2 = apellido2
            };

            _context.Add(alumno);
            await _context.SaveChangesAsync();
            await _serviceController
                .guardarAuditoria(sesionNif, constDefinidas.screenListaAlumnos, constDefinidas.accionCrear);

            return RedirectToAction("crearInforme", "TInformes", new {
                nifMedico = medico,
                nifAlumno = nif
            });
        }

        // GET: TAlumnos/listaMatriculas
        public async Task<IActionResult> listaMatriculas()
        {
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();

            List<TAlumno> alumnos = (await _serviceController.listaAlumnos(sesionNif, sesionRol));
            ListaMatriculasView vistaMatricula = new ListaMatriculasView();
            vistaMatricula.Pantalla = await _serviceController
                .permisoPantalla(constDefinidas.screenListaMatriculas, sesionRol);
            vistaMatricula.LAlumnos = alumnos;
            vistaMatricula.SesionRol = sesionRol;
            vistaMatricula.SesionNif = sesionNif;
            return View(vistaMatricula);
        }

        // GET: TAlumnos/insertarMatricula
        public async Task<IActionResult> insertarMatricula()
        {
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();

            CrearMatriculaView crearMatriculaVista = new CrearMatriculaView();
            crearMatriculaVista.LAlumnos = await _serviceController.listaAlumnos(sesionNif, sesionRol);
            crearMatriculaVista.LAsignaturas = await _serviceController.listaAsignaturas(sesionNif, sesionRol);

            return View(crearMatriculaVista);
        }

        //POST: TAlumnos/crearMatricula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> crearMatricula(CrearMatriculaView model)
        {
            string sesionNif = giveSesionNif();
            int idAsignatura = Int32.Parse(model.IdAsignatura);
            var alumno = await _context.TAlumnos
                .Include(a => a.IdAsignaturas)
                .FirstOrDefaultAsync(a => a.Nif == model.NifAlumno);
            var asignatura = await _context.TAsignaturas
                .FirstOrDefaultAsync(a => a.Id == idAsignatura);
            List<TAsignatura> listaAsiganturas = alumno!.IdAsignaturas.ToList();
            bool asiste = listaAsiganturas.Contains(asignatura!);
            if (ModelState.IsValid && !(asiste))
            {
                alumno!.IdAsignaturas.Add(asignatura!);
                await _context.SaveChangesAsync();
                await _serviceController
                    .guardarAuditoria(sesionNif, constDefinidas.screenListaMatriculas, constDefinidas.accionCrear);

                return RedirectToAction("listaMatriculas", "TAlumnos");

            }
            TempData["ExisteMatricula"] = "El alumno ya está asistiendo a esta asignatura.";
            return RedirectToAction("insertarMatricula", "TAlumnos");

        }

        //GET: TAlumnos/modificarAlumno
        public async Task<IActionResult> modificarAlumno(string nif)
        {
            TAlumno alumno = (await _context.TAlumnos
                .Include(a => a.NifNavigation)
                .FirstOrDefaultAsync(a => a.Nif == nif))!;
            ModificarUsuarios modificarAlumnoView = new ModificarUsuarios();
            modificarAlumnoView.Nif = nif;
            modificarAlumnoView.Rol = constDefinidas.rolAlumno;
            modificarAlumnoView.Nombre = alumno.Nombre;
            modificarAlumnoView.Apellido1 = alumno.Apellido1;
            modificarAlumnoView.Apellido2 = alumno.Apellido2;
            modificarAlumnoView.Password = alumno.NifNavigation.Password;
            modificarAlumnoView.ConfirmPassword = alumno.NifNavigation.Password;
            modificarAlumnoView.Usuario = alumno.NifNavigation.Usuario;

            return View(modificarAlumnoView);
        }

        // GET: TAlumnos/borrarMatricula/5
        public async Task<IActionResult> borrarMatricula(int idAsignatura, string nifAlumno)
        {
            BorrarMatriculaView vistaBorrarMatricula = new BorrarMatriculaView();
            TAlumno alumno = (await _context.TAlumnos.FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;
            TAsignatura asignatura = (await _context.TAsignaturas.FirstOrDefaultAsync(a => a.Id == idAsignatura))!;
            vistaBorrarMatricula.Alumno = alumno;
            vistaBorrarMatricula.Asignatura = asignatura;

            return View(vistaBorrarMatricula);
        }

        // POST: TAlumnos/confirmarBorradoMatricula/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarBorradoMatricula(string nifAlumno, int idAsignatura)
        {
            string sesionNif = giveSesionNif();
            TAlumno alumno = (await _context.TAlumnos
                .Include(a => a.IdAsignaturas)
                .FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;
            TAsignatura asignatura = (await _context.TAsignaturas
                .Include(a => a.NifProfesors)
                .FirstOrDefaultAsync(asg => asg.Id == idAsignatura))!;
            alumno.IdAsignaturas.Remove(asignatura);
             asignatura.NifAlumnos.Remove(alumno);

            await _context.SaveChangesAsync();
            await _serviceController
                .guardarAuditoria(sesionNif, constDefinidas.screenListaMatriculas, constDefinidas.accionBorrar);
            return RedirectToAction("listaMatriculas", "TAlumnos");
            
        }

        //GET: TAlumnos/borrarAlumno
        public async Task<IActionResult> borrarAlumno(string nifAlumno)
        {
            TAlumno alumno = (await _context.TAlumnos
                .FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;
            UserNavigation actualUser = giveActualUser();
            string actualNif = actualUser.nif;
            int actualRol = actualUser.rol;

            BorrarAlumnoView vistaBorrarAlumno = new BorrarAlumnoView();
            vistaBorrarAlumno.ActualNif = actualNif;
            vistaBorrarAlumno.ActualRol = actualRol;
            vistaBorrarAlumno.Alumno = alumno;

            return View(vistaBorrarAlumno);

        }

        // POST: TAlumnos/confirmarBorradoAlumno
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarBorradoAlumno(string nifAlumno, int actualRol, string actualNif)
        {
            string sesionNif = giveSesionNif();
            TAlumno alumno = (await _context.TAlumnos
                .Include(a => a.IdAsignaturas)
                .FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;

            List<TAsignatura> asignaturas = await _serviceController.listaAsignaturas(nifAlumno, constDefinidas.rolAlumno);
            foreach(var asignatura in asignaturas)
            {
                asignatura.NifAlumnos.Remove(alumno);
            }

            List<TInforme> informes = await _serviceController.listaInformes(nifAlumno, constDefinidas.rolAlumno);
            foreach(var informe in informes)
            {
                _context.TInformes.Remove(informe);
            }

            _context.TAlumnos.Remove(alumno);

            TUsuario usuario = (await _context.TUsuarios
                .FirstOrDefaultAsync(u => u.Nif == nifAlumno))!;
            _context.TUsuarios.Remove(usuario);

            await _context.SaveChangesAsync();
            await _serviceController
                .guardarAuditoria(sesionNif, constDefinidas.screenListaAlumnos, constDefinidas.accionBorrar);

            return RedirectToAction("listaAlumnos", "TAlumnos", new
            {
                rol = actualRol,
                volverPadre = "false",
                nif = actualNif
            });
        }
    }
}
