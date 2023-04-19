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
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using gestionDiversidad.Constantes;
using System.Drawing.Text;
using Newtonsoft.Json;

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

        // GET: TAlumnos
        public async Task<IActionResult> Index()
        {
            var tfgContext = _context.TAlumnos.Include(t => t.NifNavigation);
            return View(await tfgContext.ToListAsync());
        }

        // GET: TAlumnos/infoBasica/5
        public async Task<IActionResult> infoBasica(string id)
        {
            AlumnoView vistaAlumno = new AlumnoView();
            int? rolRaw = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            string sesionNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            int rol = rolRaw ?? 0;
            string userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
            UserNavigation actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;
            //UserNavigation newUser = null;
            String actualJson;


            if (id == null || _context.TAlumnos == null || rol == 0)
            {
                return NotFound();
            }

            var tAlumno = await _context.TAlumnos
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tAlumno == null)
            {
                return NotFound();
            }

            //if(!(pastUser.padre == null)
            if (!(actualUser.rol == constDefinidas.rolAlumno))
            {
                actualUser = new UserNavigation(id, constDefinidas.rolAlumno, actualUser);
                actualJson = JsonConvert.SerializeObject(actualUser);
                HttpContext.Session.SetString(constDefinidas.keyActualUser, actualJson);
            }

            //string sessionActualUser = constDefinidas.keyActualUser;
            //string userNavigationJson = JsonConvert.SerializeObject(raiz);
            //HttpContext.Session.SetString(sessionActualUser, userNavigationJson);


            vistaAlumno.Alumno = tAlumno;
            vistaAlumno.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenAlumno, rol);
            vistaAlumno.LInformes = await _serviceController
                .permisoPantalla(constDefinidas.screenListalInformes , rol);
            vistaAlumno.LMatriculas = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAsignaturas, rol);
            vistaAlumno.LAlumnos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, rol);
            //vistaAlumno.Rol = constDefinidas.rolAlumno;
            vistaAlumno.SesionRol = rol;
            vistaAlumno.SesionNif = sesionNif;
            if (!(actualUser.padre == null))
            {
                vistaAlumno.PadreNif = actualUser.padre.nif;
                vistaAlumno.PadreRol = actualUser.padre.rol;
            }
            else
            {
                vistaAlumno.PadreNif = "dummy";
                vistaAlumno.PadreRol = 10;
            }
            /////////////////////////////
            return View(vistaAlumno);
        }

        //GET: TAlumnos/listaAlumnos
        public async Task<IActionResult> listaAlumnos(string nif, int rol, string volverPadre)
        {
            List<TAlumno> listaAlumnos;
            ListaAlumnosView vistaListasAlumno = new ListaAlumnosView();
            string sessionKeyRol = constDefinidas.keyRol;
            string sessionKeyNif = constDefinidas.keyNif;
            int? rawRol = HttpContext.Session.GetInt32(sessionKeyRol);
            string sesionNif = HttpContext.Session.GetString(sessionKeyNif)!;
            int sesionRol = rawRol ?? 0;
            bool volverPadreValue;
            string userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
            UserNavigation actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;

            //Si volvemos de un usuario importante, volvemos a ser el padre: 
            volverPadreValue = bool.Parse(volverPadre);
            if (volverPadreValue)
            {
                string userNavigationPadreJson = JsonConvert.SerializeObject(actualUser.padre);
                HttpContext.Session.SetString(constDefinidas.keyActualUser, userNavigationPadreJson);
                userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
                actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;
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

            //Añadir un boleano para ver si el usuario actual se baja una tier o no 

            return View(vistaListasAlumno);
        }

        //GET: TAlumnos/insertarAlumno
        public async Task<IActionResult> insertarAlumno()
        {
            int? rawRol = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            string rawNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            int sesionRol = rawRol ?? 0;
            string sesionNif = rawNif;

            CrearAlumnoView vistaCrearAlumno = new CrearAlumnoView();
            vistaCrearAlumno.ListaMedicos = (await _context.TMedicos.ToListAsync());

            return View(vistaCrearAlumno);
        }

        //Función que crea al alumno: TAlumnos/crearAlumno
        public async Task<IActionResult> crearAlumno(string nif, string nombre, string apellido1, string apellido2, string medico)
        {
            var alumno = new TAlumno
            {
                Nif = nif,
                Nombre = nombre,
                Apellido1 = apellido1,
                Apellido2 = apellido2
            };

            _context.Add(alumno);
            await _context.SaveChangesAsync(); 
            return RedirectToAction("crearInforme", "TInformes",
                new { nifMedico = medico, nifAlumno = nif});
        }

        // GET: TAlumnos/listaMatriculas
        public async Task<IActionResult> listaMatriculas()
        {
            string sessionKeyRol = constDefinidas.keyRol;
            string sessionKeyNif = constDefinidas.keyNif;
            int? rawRol = HttpContext.Session.GetInt32(sessionKeyRol);
            string sesionNif = HttpContext.Session.GetString(sessionKeyNif)!;
            int sesionRol = rawRol ?? 0;

            List<TAlumno> alumnos = await _serviceController.listaAlumnos(sesionNif, sesionRol);
            MatriculaView vistaMatricula = new MatriculaView();
            vistaMatricula.LAlumnos = alumnos;
            vistaMatricula.SesionRol = sesionRol;
            vistaMatricula.SesionNif = sesionNif;
            return View(vistaMatricula);
        }

        // GET: TAlumnos/insertarMatricula
        public async Task<IActionResult> insertarMatricula()
        {
            string sessionKeyRol = constDefinidas.keyRol;
            string sessionKeyNif = constDefinidas.keyNif;
            int? rawRol = HttpContext.Session.GetInt32(sessionKeyRol);
            string sesionNif = HttpContext.Session.GetString(sessionKeyNif)!;
            int sesionRol = rawRol ?? 0;

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
            int idAsignatura = Int32.Parse(model.IdAsignatura);
            var alumno = await _context.TAlumnos
                .Include(a => a.IdAsignaturas).FirstOrDefaultAsync(a => a.Nif == model.NifAlumno);
            var asignatura = await _context.TAsignaturas.FirstOrDefaultAsync(a => a.Id == idAsignatura);
            List<TAsignatura> listaAsiganturas = alumno!.IdAsignaturas.ToList();
            bool asiste = listaAsiganturas.Contains(asignatura!);
            if (ModelState.IsValid && !(asiste))
            {

                alumno!.IdAsignaturas.Add(asignatura!);
                await _context.SaveChangesAsync();

                return RedirectToAction("listaMatriculas", "TAlumnos");

            }
            TempData["ExisteMatricula"] = "El alumno ya está asistiendo a esta asignatura.";
            return RedirectToAction("insertarMatricula", "TAlumnos");

        }

        //GET: TAlumnos/modificarAlumno
        public async Task<IActionResult> modificarAlumno(string nif)
        {
            TAlumno alumno = (await _context.TAlumnos.FirstOrDefaultAsync(a => a.Nif == nif))!;
            ModificarUsuarios modificarAlumnoView = new ModificarUsuarios();
            modificarAlumnoView.Nif = nif;
            modificarAlumnoView.Rol = constDefinidas.rolAlumno;
            modificarAlumnoView.Nombre = alumno.Nombre;
            modificarAlumnoView.Apellido1 = alumno.Apellido1;
            modificarAlumnoView.Apellido2 = alumno.Apellido2;

            return View(modificarAlumnoView);
        }

        // GET: TAlumnos/borrarMatricula/5
        public IActionResult borrarMatricula(int idAsignatura, string nifAlumno)
        {
            BorrarMatriculaView vistaBorrarMatricula = new BorrarMatriculaView();
            vistaBorrarMatricula.NifAlumno = nifAlumno;
            vistaBorrarMatricula.IdAsignatura = idAsignatura;

            return View(vistaBorrarMatricula);
        }

        // POST: TAlumnos/confirmarBorradoMatricula/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarBorradoMatricula(BorrarMatriculaView model)
        {
            string nifAlumno = model.NifAlumno;
            int idAsigantura = model.IdAsignatura;

            if (ModelState.IsValid)
            {

                TAlumno alumno = (await _context.TAlumnos
                    .Include(a => a.IdAsignaturas)
                    .FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;
                TAsignatura asignatura = (await _context.TAsignaturas
                    .Include(a => a.NifProfesors)
                    .FirstOrDefaultAsync(asg => asg.Id == idAsigantura))!;
                alumno.IdAsignaturas.Remove(asignatura);
                asignatura.NifAlumnos.Remove(alumno);

                await _context.SaveChangesAsync();
                return RedirectToAction("listaMatriculas", "TAlumnos");
            }
            return RedirectToAction("borrarMatricula", "TAlumnos", new
            {
                idAsignatura = idAsigantura,
                nifAlumno = nifAlumno
            });
        }

        //GET: TAlumnos/borrarAlumno
        public async Task<IActionResult> borrarAlumno(string nifAlumno)
        {
            TAlumno alumno = (await _context.TAlumnos.FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;
            //Actual user
            string userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
            UserNavigation actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;
            string actualNif = actualUser.nif;
            int actualRol = actualUser.rol;
            //
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
            TAlumno alumno = (await _context.TAlumnos
                .Include(a => a.IdAsignaturas)
                .FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;

            //Quitar enlace Matriculas
            List<TAsignatura> asignaturas = await _serviceController.listaAsignaturas(nifAlumno, constDefinidas.rolAlumno);
            foreach(var asignatura in asignaturas)
            {
                asignatura.NifAlumnos.Remove(alumno);
            }

            //Quitar lo de los informes
            List<TInforme> informes = await _serviceController.listaInformes(nifAlumno, constDefinidas.rolAlumno);
            foreach(var informe in informes)
            {
                _context.TInformes.Remove(informe);
            }

            //Borrar alumno
            _context.TAlumnos.Remove(alumno);

            //Borrar usuario
            TUsuario usuario = (await _context.TUsuarios
                .FirstOrDefaultAsync(u => u.Nif == nifAlumno))!;
            _context.TUsuarios.Remove(usuario);

            await _context.SaveChangesAsync();

            return RedirectToAction("listaAlumnos", "TAlumnos", new
            {
                rol = actualRol,
                volverPadre = "false",
                nif = actualNif
            });
        }

        // GET: TAlumnos/Create
        public IActionResult Create()
        {
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif");
            return View();
        }

        // POST: TAlumnos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nif,Nombre,Apellido1,Apellido2")] TAlumno tAlumno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tAlumno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tAlumno.Nif);
            return View(tAlumno);
        }

        // GET: TAlumnos/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TAlumnos == null)
            {
                return NotFound();
            }

            var tAlumno = await _context.TAlumnos.FindAsync(id);
            if (tAlumno == null)
            {
                return NotFound();
            }
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tAlumno.Nif);
            return View(tAlumno);
        }

        // POST: TAlumnos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nif,Nombre,Apellido1,Apellido2")] TAlumno tAlumno)
        {
            if (id != tAlumno.Nif)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tAlumno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TAlumnoExists(tAlumno.Nif))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tAlumno.Nif);
            return View(tAlumno);
        }

        // GET: TAlumnos/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TAlumnos == null)
            {
                return NotFound();
            }

            var tAlumno = await _context.TAlumnos
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tAlumno == null)
            {
                return NotFound();
            }

            return View(tAlumno);
        }

        // POST: TAlumnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TAlumnos == null)
            {
                return Problem("Entity set 'TfgContext.TAlumnos'  is null.");
            }
            var tAlumno = await _context.TAlumnos.FindAsync(id);
            if (tAlumno != null)
            {
                _context.TAlumnos.Remove(tAlumno);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TAlumnoExists(string id)
        {
          return (_context.TAlumnos?.Any(e => e.Nif == id)).GetValueOrDefault();
        }
    }
}
