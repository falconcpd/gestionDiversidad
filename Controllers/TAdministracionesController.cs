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

        // GET: TAdministraciones
        public async Task<IActionResult> Index()
        {
            var tfgContext = _context.TAdministracions.Include(t => t.NifNavigation);
            return View(await tfgContext.ToListAsync());
        }

        // GET: TAdministraciones/infoBasica/5
        public async Task<IActionResult> infoBasica(string id)
        {
            AdminView vistaAdmin = new AdminView();
            int? rolRaw = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            string sesionNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            int rol = rolRaw ?? 0;
            string nif = id;
            vistaAdmin = new AdminView();


            if (id == null || _context.TAlumnos == null || rol == 0)
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
                .permisoPantalla(constDefinidas.screenAdministracion, rol);
            vistaAdmin.LAsignaturas = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAsignaturas, rol);
            vistaAdmin.LMatriculas = await _serviceController
                .permisoPantalla(constDefinidas.screenListaMatriculas, rol);
            vistaAdmin.Rol = constDefinidas.rolAdmin;
            vistaAdmin.Nif = nif;
            vistaAdmin.SesionRol = rol;
            vistaAdmin.SesionNif = sesionNif;
            return View(vistaAdmin);
        }

        // GET: TAdministraciones/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TAdministracions == null)
            {
                return NotFound();
            }

            var tAdministracion = await _context.TAdministracions
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tAdministracion == null)
            {
                return NotFound();
            }

            return View(tAdministracion);
        }

        // GET: TAdministraciones/Create
        public IActionResult Create()
        {
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif");
            return View();
        }

        // POST: TAdministraciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nif,Nombre,Apellido1,Apellido2")] TAdministracion tAdministracion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tAdministracion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tAdministracion.Nif);
            return View(tAdministracion);
        }

        // GET: TAdministraciones/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TAdministracions == null)
            {
                return NotFound();
            }

            var tAdministracion = await _context.TAdministracions.FindAsync(id);
            if (tAdministracion == null)
            {
                return NotFound();
            }
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tAdministracion.Nif);
            return View(tAdministracion);
        }

        // POST: TAdministraciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nif,Nombre,Apellido1,Apellido2")] TAdministracion tAdministracion)
        {
            if (id != tAdministracion.Nif)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tAdministracion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TAdministracionExists(tAdministracion.Nif))
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
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tAdministracion.Nif);
            return View(tAdministracion);
        }

        // GET: TAdministraciones/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TAdministracions == null)
            {
                return NotFound();
            }

            var tAdministracion = await _context.TAdministracions
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tAdministracion == null)
            {
                return NotFound();
            }

            return View(tAdministracion);
        }

        // POST: TAdministraciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TAdministracions == null)
            {
                return Problem("Entity set 'TfgContext.TAdministracions'  is null.");
            }
            var tAdministracion = await _context.TAdministracions.FindAsync(id);
            if (tAdministracion != null)
            {
                _context.TAdministracions.Remove(tAdministracion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TAdministracionExists(string id)
        {
          return (_context.TAdministracions?.Any(e => e.Nif == id)).GetValueOrDefault();
        }
    }
}
