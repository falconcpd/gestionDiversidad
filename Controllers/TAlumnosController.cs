using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace gestionDiversidad.Controllers
{
    public class TAlumnosController : Controller
    {
        private readonly TfgContext _context;

        public TAlumnosController(TfgContext context)
        {
            _context = context;
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
            string sessionKeyRol = "_rol";
            int? lotad = HttpContext.Session.GetInt32(sessionKeyRol);


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
