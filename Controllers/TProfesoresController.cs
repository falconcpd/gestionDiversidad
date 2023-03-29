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

            vistaProfesor.Profesor = tProfesor;
            vistaProfesor.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenProfesor, rol);
            vistaProfesor.LDocencias = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAsignaturas, rol);
            vistaProfesor.LAlumnos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, rol);
            vistaProfesor.Rol = constDefinidas.rolProfesor;

            return View(vistaProfesor);
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
