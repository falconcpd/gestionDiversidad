using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;

namespace gestionDiversidad.Controllers
{
    public class TPermisosController : Controller
    {
        private readonly TfgContext _context;

        public TPermisosController(TfgContext context)
        {
            _context = context;
        }

        // GET: TPermisos
        public async Task<IActionResult> Index()
        {
            var tfgContext = _context.TPermisos.Include(t => t.IdPantallaNavigation).Include(t => t.IdRolNavigation);
            return View(await tfgContext.ToListAsync());
        }

        // GET: TPermisos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TPermisos == null)
            {
                return NotFound();
            }

            var tPermiso = await _context.TPermisos
                .Include(t => t.IdPantallaNavigation)
                .Include(t => t.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdPantalla == id);
            if (tPermiso == null)
            {
                return NotFound();
            }

            return View(tPermiso);
        }

        // GET: TPermisos/Create
        public IActionResult Create()
        {
            ViewData["IdPantalla"] = new SelectList(_context.TPantallas, "Id", "Id");
            ViewData["IdRol"] = new SelectList(_context.TRols, "Id", "Id");
            return View();
        }

        // POST: TPermisos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPantalla,IdRol,Insertar,Modificar,Borrar,Acceder")] TPermiso tPermiso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tPermiso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPantalla"] = new SelectList(_context.TPantallas, "Id", "Id", tPermiso.IdPantalla);
            ViewData["IdRol"] = new SelectList(_context.TRols, "Id", "Id", tPermiso.IdRol);
            return View(tPermiso);
        }

        // GET: TPermisos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TPermisos == null)
            {
                return NotFound();
            }

            var tPermiso = await _context.TPermisos.FindAsync(id);
            if (tPermiso == null)
            {
                return NotFound();
            }
            ViewData["IdPantalla"] = new SelectList(_context.TPantallas, "Id", "Id", tPermiso.IdPantalla);
            ViewData["IdRol"] = new SelectList(_context.TRols, "Id", "Id", tPermiso.IdRol);
            return View(tPermiso);
        }

        // POST: TPermisos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPantalla,IdRol,Insertar,Modificar,Borrar,Acceder")] TPermiso tPermiso)
        {
            if (id != tPermiso.IdPantalla)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tPermiso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TPermisoExists(tPermiso.IdPantalla))
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
            ViewData["IdPantalla"] = new SelectList(_context.TPantallas, "Id", "Id", tPermiso.IdPantalla);
            ViewData["IdRol"] = new SelectList(_context.TRols, "Id", "Id", tPermiso.IdRol);
            return View(tPermiso);
        }

        // GET: TPermisos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TPermisos == null)
            {
                return NotFound();
            }

            var tPermiso = await _context.TPermisos
                .Include(t => t.IdPantallaNavigation)
                .Include(t => t.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdPantalla == id);
            if (tPermiso == null)
            {
                return NotFound();
            }

            return View(tPermiso);
        }

        // POST: TPermisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TPermisos == null)
            {
                return Problem("Entity set 'TfgContext.TPermisos'  is null.");
            }
            var tPermiso = await _context.TPermisos.FindAsync(id);
            if (tPermiso != null)
            {
                _context.TPermisos.Remove(tPermiso);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TPermisoExists(int id)
        {
          return (_context.TPermisos?.Any(e => e.IdPantalla == id)).GetValueOrDefault();
        }
    }
}
