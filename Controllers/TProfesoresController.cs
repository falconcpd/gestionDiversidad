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
using gestionDiversidad.Constantes;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.ComponentModel;
using gestionDiversidad.Navigation;
using Newtonsoft.Json;

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

        // GET: TProfesores
        public async Task<IActionResult> Index()
        {
            var tfgContext = _context.TProfesors.Include(t => t.NifNavigation);
            return View(await tfgContext.ToListAsync());
        }


        // GET: TProfesores/infoBasica/5
        public async Task<IActionResult> infoBasica(string id)
        {
            ProfesorView vistaProfesor = new ProfesorView();
            string sessionKeyRol = constDefinidas.keyRol;
            int? rawRol = HttpContext.Session.GetInt32(sessionKeyRol);
            int rol = rawRol ?? 0;
            string sesionNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            string userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
            UserNavigation actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;
            String actualJson;


            if (id == null || _context.TProfesors == null || rol == 0)
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
                .permisoPantalla(constDefinidas.screenProfesor, rol);
            vistaProfesor.LDocencias = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAsignaturas, rol);
            vistaProfesor.LAlumnos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, rol);
            vistaProfesor.LProfesores = await _serviceController
                .permisoPantalla(constDefinidas.screenListaProfesores, rol);
            vistaProfesor.Rol = constDefinidas.rolProfesor;
            vistaProfesor.SesionRol = rol;
            vistaProfesor.SesionNif = sesionNif;
            if (!(actualUser.padre == null))
            {
                vistaProfesor.PadreNif = actualUser.padre.nif;
                vistaProfesor.PadreRol = actualUser.padre.rol;
            }
            else
            {
                vistaProfesor.PadreNif = "dummy";
                vistaProfesor.PadreRol = 10;
            }

            /*
             var aux = _serviceController
                .permisoPantalla(constDefinidas.screenProfesor, rol);
            Task.WhenAll(aux);
            vistaProfesor.Permiso = aux.Result;
              */

            return View(vistaProfesor);
        }

        //GET: TProfesores/listaProfesores
        public async Task<IActionResult> listaProfesores(string volverPadre)
        {
            List<TProfesor> listaProfesores;
            ListaProfesoresView vistaListasProfesores = new ListaProfesoresView();
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

        // GET: TProfesores/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TProfesors == null)
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

            return View(tProfesor);
        }

        //GET: TProfesores/insertarProfesor
        public IActionResult insertarProfesor()
        {
            int? rawRol = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            string rawNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            int sesionRol = rawRol ?? 0;
            string sesionNif = rawNif;

            CrearProfesorView vistaCrearProfesor = new CrearProfesorView();

            return View(vistaCrearProfesor);
        }

        //Función que crea al profesor: TProfesores/crearProfesor
        public async Task<IActionResult> crearProfesor(string nif, string nombre, string apellido1, string apellido2)
        {
            var profesor = new TProfesor
            {
                Nif = nif,
                Nombre = nombre,
                Apellido1= apellido1,
                Apellido2= apellido2
            };

            _context.Add(profesor);
            await _context.SaveChangesAsync();
            return RedirectToAction("listaProfesores", "TProfesores", 
                new { volverPadre = "false" });
        }

        // GET: TProfesores/Create
        public IActionResult Create()
        {
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif");
            return View();
        }

        // POST: TProfesores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nif,Nombre,Apellido1,Apellido2")] TProfesor tProfesor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tProfesor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tProfesor.Nif);
            return View(tProfesor);
        }

        // GET: TProfesores/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TProfesors == null)
            {
                return NotFound();
            }

            var tProfesor = await _context.TProfesors.FindAsync(id);
            if (tProfesor == null)
            {
                return NotFound();
            }
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tProfesor.Nif);
            return View(tProfesor);
        }

        // POST: TProfesores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nif,Nombre,Apellido1,Apellido2")] TProfesor tProfesor)
        {
            if (id != tProfesor.Nif)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tProfesor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TProfesorExists(tProfesor.Nif))
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
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tProfesor.Nif);
            return View(tProfesor);
        }

        // GET: TProfesores/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TProfesors == null)
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

            return View(tProfesor);
        }

        // POST: TProfesores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TProfesors == null)
            {
                return Problem("Entity set 'TfgContext.TProfesors'  is null.");
            }
            var tProfesor = await _context.TProfesors.FindAsync(id);
            if (tProfesor != null)
            {
                _context.TProfesors.Remove(tProfesor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TProfesorExists(string id)
        {
          return (_context.TProfesors?.Any(e => e.Nif == id)).GetValueOrDefault();
        }
    }
}
