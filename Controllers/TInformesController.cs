using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using System.Globalization;
using gestionDiversidad.Interfaces;
using gestionDiversidad.ViewModels;

namespace gestionDiversidad.Controllers
{
    public class TInformesController : Controller
    {
        private readonly TfgContext _context;
        private readonly IServiceController _serviceController;

        public TInformesController(TfgContext context, IServiceController sc)
        {
            _context = context;
            _serviceController = sc;
        }

        // GET: TInformes
        public async Task<IActionResult> Index()
        {
            var tfgContext = _context.TInformes.Include(t => t.NifAlumnoNavigation).Include(t => t.NifMedicoNavigation);
            return View(await tfgContext.ToListAsync());
        }

        // GET: TInformes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TInformes == null)
            {
                return NotFound();
            }

            var tInforme = await _context.TInformes
                .Include(t => t.NifAlumnoNavigation)
                .Include(t => t.NifMedicoNavigation)
                .FirstOrDefaultAsync(m => m.NifMedico == id);
            if (tInforme == null)
            {
                return NotFound();
            }

            return View(tInforme);
        }

        // GET: TInformes/listaInformes
        public async Task<IActionResult> listaInformes(string nif, int rol)
        {
            //Cuidado, es perezoso, utilizar el include
            TAlumno alumno;
            List<TInforme> informes = null;
            ListaInformesView vistaListaInformes = new ListaInformesView();
            int? lotad2 = HttpContext.Session.GetInt32("_rol");
            int sesionRol = lotad2 ?? 0;


            if (rol == 1)
            {
                alumno = _context.TAlumnos.Include(u => u.TInformes).FirstOrDefault(u => u.Nif == nif);
                informes = alumno.TInformes.ToList();
            }

            vistaListaInformes.Permiso = _serviceController.permisoPantalla(12, sesionRol);
            vistaListaInformes.Informe = _serviceController.permisoPantalla(5, sesionRol);
            vistaListaInformes.ListaInformes = informes;
            return View(vistaListaInformes);

        }

        //Función para buscar informes
        public TInforme buscarInforme(string nifAlumno, string nifMedico, string fecha)
        {
            DateTime fechaTime = DateTime.ParseExact(fecha, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            return _context.TInformes.FirstOrDefault(t => t.NifAlumno == nifAlumno && t.NifMedico == nifMedico && t.Fecha == fechaTime);
        }

        // GET: TInformes/infoBasica
        public async Task<IActionResult> infoBasica(string nifAlumno, string nifMedico, string fecha)
        {
            DateTime fechaTime;
            InformeView informeView = new InformeView();
            TInforme informe = null;
            int? lotad2 = HttpContext.Session.GetInt32("_rol");
            int sesionRol = lotad2 ?? 0;
            /* if (DateTime.TryParseExact(fecha, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaTime))
             {

             } */
            fechaTime = DateTime.ParseExact(fecha, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

            informe = _context.TInformes.FirstOrDefault(t => t.NifAlumno == nifAlumno && t.NifMedico == nifMedico && t.Fecha == fechaTime);

            informeView.Informe = informe;
            informeView.Permiso = _serviceController.permisoPantalla(5, sesionRol);
            if (lotad2 == 4)
            {
                informeView.Nif = nifMedico;
            }
            else 
            {
                informeView.Nif = nifAlumno;
            }
            return View(informeView);
        }

        // GET: TInformes/verInforme
        public async Task<IActionResult> verInforme(string nifAlumno, string nifMedico, string fecha)
        {
            TInforme informe = buscarInforme(nifAlumno, nifMedico, fecha);
            MemoryStream stream = new MemoryStream(informe.Contenido);
            return new FileStreamResult(stream, "application/pdf");
            

        }

        // GET: TInformes/Create
        public IActionResult Create()
        {
            ViewData["NifAlumno"] = new SelectList(_context.TAlumnos, "Nif", "Nif");
            ViewData["NifMedico"] = new SelectList(_context.TMedicos, "Nif", "Nif");
            return View();
        }

        // POST: TInformes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NifMedico,NifAlumno,Fecha,Contenido")] TInforme tInforme)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tInforme);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NifAlumno"] = new SelectList(_context.TAlumnos, "Nif", "Nif", tInforme.NifAlumno);
            ViewData["NifMedico"] = new SelectList(_context.TMedicos, "Nif", "Nif", tInforme.NifMedico);
            return View(tInforme);
        }

        // GET: TInformes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TInformes == null)
            {
                return NotFound();
            }

            var tInforme = await _context.TInformes.FindAsync(id);
            if (tInforme == null)
            {
                return NotFound();
            }
            ViewData["NifAlumno"] = new SelectList(_context.TAlumnos, "Nif", "Nif", tInforme.NifAlumno);
            ViewData["NifMedico"] = new SelectList(_context.TMedicos, "Nif", "Nif", tInforme.NifMedico);
            return View(tInforme);
        }

        // POST: TInformes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NifMedico,NifAlumno,Fecha,Contenido")] TInforme tInforme)
        {
            if (id != tInforme.NifMedico)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tInforme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TInformeExists(tInforme.NifMedico))
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
            ViewData["NifAlumno"] = new SelectList(_context.TAlumnos, "Nif", "Nif", tInforme.NifAlumno);
            ViewData["NifMedico"] = new SelectList(_context.TMedicos, "Nif", "Nif", tInforme.NifMedico);
            return View(tInforme);
        }

        // GET: TInformes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TInformes == null)
            {
                return NotFound();
            }

            var tInforme = await _context.TInformes
                .Include(t => t.NifAlumnoNavigation)
                .Include(t => t.NifMedicoNavigation)
                .FirstOrDefaultAsync(m => m.NifMedico == id);
            if (tInforme == null)
            {
                return NotFound();
            }

            return View(tInforme);
        }

        // POST: TInformes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TInformes == null)
            {
                return Problem("Entity set 'TfgContext.TInformes'  is null.");
            }
            var tInforme = await _context.TInformes.FindAsync(id);
            if (tInforme != null)
            {
                _context.TInformes.Remove(tInforme);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TInformeExists(string id)
        {
          return (_context.TInformes?.Any(e => e.NifMedico == id)).GetValueOrDefault();
        }
    }
}
