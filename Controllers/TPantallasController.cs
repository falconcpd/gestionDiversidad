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
    public class TPantallasController : Controller
    {
        private readonly TfgContext _context;

        public TPantallasController(TfgContext context)
        {
            _context = context;
        }

        // GET: TPantallas
        public async Task<IActionResult> Index()
        {
              return _context.TPantallas != null ? 
                          View(await _context.TPantallas.ToListAsync()) :
                          Problem("Entity set 'TfgContext.TPantallas'  is null.");
        }

        // GET: TPantallas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TPantallas == null)
            {
                return NotFound();
            }

            var tPantalla = await _context.TPantallas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tPantalla == null)
            {
                return NotFound();
            }

            return View(tPantalla);
        }

        // GET: TPantallas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TPantallas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] TPantalla tPantalla)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tPantalla);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tPantalla);
        }

        // GET: TPantallas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TPantallas == null)
            {
                return NotFound();
            }

            var tPantalla = await _context.TPantallas.FindAsync(id);
            if (tPantalla == null)
            {
                return NotFound();
            }
            return View(tPantalla);
        }

        // POST: TPantallas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] TPantalla tPantalla)
        {
            if (id != tPantalla.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tPantalla);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TPantallaExists(tPantalla.Id))
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
            return View(tPantalla);
        }

        // GET: TPantallas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TPantallas == null)
            {
                return NotFound();
            }

            var tPantalla = await _context.TPantallas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tPantalla == null)
            {
                return NotFound();
            }

            return View(tPantalla);
        }

        // POST: TPantallas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TPantallas == null)
            {
                return Problem("Entity set 'TfgContext.TPantallas'  is null.");
            }
            var tPantalla = await _context.TPantallas.FindAsync(id);
            if (tPantalla != null)
            {
                _context.TPantallas.Remove(tPantalla);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TPantallaExists(int id)
        {
          return (_context.TPantallas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
