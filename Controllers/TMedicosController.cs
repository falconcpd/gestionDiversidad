using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using gestionDiversidad.Constantes;
using gestionDiversidad.Interfaces;
using gestionDiversidad.ViewModels;

namespace gestionDiversidad.Controllers
{
    public class TMedicosController : Controller
    {
        private readonly TfgContext _context;
        private readonly IServiceController _serviceController;

        public TMedicosController(TfgContext context, IServiceController sc)
        {
            _context = context;
            _serviceController = sc;
        }

        // GET: TMedicos
        public async Task<IActionResult> Index()
        {
            var tfgContext = _context.TMedicos.Include(t => t.NifNavigation);
            return View(await tfgContext.ToListAsync());
        }

        // GET: TAlumnos/infoBasica/5
        public async Task<IActionResult> infoBasica(string id)
        {
            MedicoView vistaMedico = new MedicoView();
            int? rolRaw = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            string? sesionNif = HttpContext.Session.GetString(constDefinidas.keyNif);
            int rol = rolRaw ?? 0;


            if (id == null || _context.TMedicos == null || rol == 0)
            {
                return NotFound();
            }

            var tMedico = await _context.TMedicos
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tMedico == null)
            {
                return NotFound();
            }

            vistaMedico.Medico = tMedico;
            vistaMedico.Permiso = _serviceController
                .permisoPantalla(constDefinidas.screenAlumno, rol);
            vistaMedico.LInformes = _serviceController
                .permisoPantalla(constDefinidas.screenListalInformes, rol);
            vistaMedico.LAlumnos = _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, rol);
            vistaMedico.Rol = constDefinidas.rolMedico;
            vistaMedico.SesionRol = rol;
            vistaMedico.SesionNif = sesionNif;
            return View(vistaMedico);
        }

        // GET: TMedicos/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TMedicos == null)
            {
                return NotFound();
            }

            var tMedico = await _context.TMedicos
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tMedico == null)
            {
                return NotFound();
            }

            return View(tMedico);
        }

        // GET: TMedicos/Create
        public IActionResult Create()
        {
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif");
            return View();
        }

        // POST: TMedicos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nif,Nombre,Apellido1,Apellido2")] TMedico tMedico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tMedico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tMedico.Nif);
            return View(tMedico);
        }

        // GET: TMedicos/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TMedicos == null)
            {
                return NotFound();
            }

            var tMedico = await _context.TMedicos.FindAsync(id);
            if (tMedico == null)
            {
                return NotFound();
            }
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tMedico.Nif);
            return View(tMedico);
        }

        // POST: TMedicos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nif,Nombre,Apellido1,Apellido2")] TMedico tMedico)
        {
            if (id != tMedico.Nif)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tMedico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TMedicoExists(tMedico.Nif))
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
            ViewData["Nif"] = new SelectList(_context.TUsuarios, "Nif", "Nif", tMedico.Nif);
            return View(tMedico);
        }

        // GET: TMedicos/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TMedicos == null)
            {
                return NotFound();
            }

            var tMedico = await _context.TMedicos
                .Include(t => t.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tMedico == null)
            {
                return NotFound();
            }

            return View(tMedico);
        }

        // POST: TMedicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TMedicos == null)
            {
                return Problem("Entity set 'TfgContext.TMedicos'  is null.");
            }
            var tMedico = await _context.TMedicos.FindAsync(id);
            if (tMedico != null)
            {
                _context.TMedicos.Remove(tMedico);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TMedicoExists(string id)
        {
          return (_context.TMedicos?.Any(e => e.Nif == id)).GetValueOrDefault();
        }
    }
}
