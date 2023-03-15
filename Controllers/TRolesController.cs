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
    public class TRolesController : Controller
    {
        private readonly TfgContext _context;

        public TRolesController(TfgContext context)
        {
            _context = context;
        }

        // GET: TRoles
        public async Task<IActionResult> Index()
        {
              return _context.TRols != null ? 
                          View(await _context.TRols.ToListAsync()) :
                          Problem("Entity set 'TfgContext.TRols'  is null.");
        }

        // GET: TRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TRols == null)
            {
                return NotFound();
            }

            var tRol = await _context.TRols
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tRol == null)
            {
                return NotFound();
            }

            return View(tRol);
        }

        // GET: TRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] TRol tRol)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tRol);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tRol);
        }

        // GET: TRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TRols == null)
            {
                return NotFound();
            }

            var tRol = await _context.TRols.FindAsync(id);
            if (tRol == null)
            {
                return NotFound();
            }
            return View(tRol);
        }

        // POST: TRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] TRol tRol)
        {
            if (id != tRol.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tRol);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TRolExists(tRol.Id))
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
            return View(tRol);
        }

        // GET: TRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TRols == null)
            {
                return NotFound();
            }

            var tRol = await _context.TRols
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tRol == null)
            {
                return NotFound();
            }

            return View(tRol);
        }

        // POST: TRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TRols == null)
            {
                return Problem("Entity set 'TfgContext.TRols'  is null.");
            }
            var tRol = await _context.TRols.FindAsync(id);
            if (tRol != null)
            {
                _context.TRols.Remove(tRol);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TRolExists(int id)
        {
          return (_context.TRols?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
