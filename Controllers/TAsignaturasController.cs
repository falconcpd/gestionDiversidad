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
    public class TAsignaturasController : Controller
    {
        private readonly TfgContext _context;
        private readonly IServiceController _serviceController;

        public TAsignaturasController(TfgContext context, IServiceController sc)
        {
            _context = context;
            _serviceController = sc;
        }

        // GET: TAsignaturas
        public async Task<IActionResult> Index()
        {
              return _context.TAsignaturas != null ? 
                          View(await _context.TAsignaturas.ToListAsync()) :
                          Problem("Entity set 'TfgContext.TAsignaturas'  is null.");
        }

        //GET: TAsignaturas/listaAsignaturas
        public async Task<IActionResult> listaAsignaturas(string nif, int rol)
        {
            //Cuidado, es perezoso, utilizar el include
            ListaAsignaturasView vistaListaAsignaturas= new ListaAsignaturasView();
            List<TAsignatura> asignaturas = await _serviceController.listaAsignaturas(nif, rol);
            int? rawRol = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            string rawNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            int sesionRol = rawRol ?? 0;           
            string sesionNif = rawNif;

            vistaListaAsignaturas.ListaAsignaturas = asignaturas;
            vistaListaAsignaturas.Permiso = await _serviceController.
                permisoPantalla(constDefinidas.screenListaAsignaturas, sesionRol);
            vistaListaAsignaturas.Rol = rol;
            vistaListaAsignaturas.Nif = nif;
            vistaListaAsignaturas.SesionRol = sesionRol;
            vistaListaAsignaturas.SesionNif = sesionNif;

            return View(vistaListaAsignaturas);

        }

        // GET: TAsignaturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TAsignaturas == null)
            {
                return NotFound();
            }

            var tAsignatura = await _context.TAsignaturas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tAsignatura == null)
            {
                return NotFound();
            }

            return View(tAsignatura);
        }

        // GET: TAsignaturas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TAsignaturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] TAsignatura tAsignatura)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tAsignatura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tAsignatura);
        }

        // GET: TAsignaturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TAsignaturas == null)
            {
                return NotFound();
            }

            var tAsignatura = await _context.TAsignaturas.FindAsync(id);
            if (tAsignatura == null)
            {
                return NotFound();
            }
            return View(tAsignatura);
        }

        // POST: TAsignaturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] TAsignatura tAsignatura)
        {
            if (id != tAsignatura.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tAsignatura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TAsignaturaExists(tAsignatura.Id))
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
            return View(tAsignatura);
        }

        // GET: TAsignaturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TAsignaturas == null)
            {
                return NotFound();
            }

            var tAsignatura = await _context.TAsignaturas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tAsignatura == null)
            {
                return NotFound();
            }

            return View(tAsignatura);
        }

        // POST: TAsignaturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TAsignaturas == null)
            {
                return Problem("Entity set 'TfgContext.TAsignaturas'  is null.");
            }
            var tAsignatura = await _context.TAsignaturas.FindAsync(id);
            if (tAsignatura != null)
            {
                _context.TAsignaturas.Remove(tAsignatura);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TAsignaturaExists(int id)
        {
          return (_context.TAsignaturas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
