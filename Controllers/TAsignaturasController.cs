﻿using System;
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
using gestionDiversidad.Navigation;
using Newtonsoft.Json;

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
            string userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
            UserNavigation actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;

            vistaListaAsignaturas.ListaAsignaturas = asignaturas;
            vistaListaAsignaturas.Permiso = await _serviceController.
                permisoPantalla(constDefinidas.screenListaAsignaturas, sesionRol);
            vistaListaAsignaturas.Rol = actualUser.rol;
            vistaListaAsignaturas.Nif = actualUser.nif;
            vistaListaAsignaturas.SesionRol = sesionRol;
            vistaListaAsignaturas.SesionNif = sesionNif;

            return View(vistaListaAsignaturas);

        }

        public async Task<List<string>> nombresAsignatura()
        {
            List<string> nombres = new List<string>();
            nombres = (await _context.TAsignaturas.Select(a => a.Nombre).ToListAsync());
            return nombres;

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

        //GET: TAsignaturas/insertarAsignatura
        public IActionResult insertarAsignatura(string nif, int rol)
        {
            int? rawRol = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            string rawNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            int sesionRol = rawRol ?? 0;
            string sesionNif = rawNif;

            CrearView vistaCrearAsignatura = new CrearView();
            vistaCrearAsignatura.Nif = nif;
            vistaCrearAsignatura.Rol= rol;

            return View(vistaCrearAsignatura);
        }

        // GET: TAsignaturas/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: TAsignaturas/crearAsignatura
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> crearAsignatura(int rol, string nif, CrearView model)
        {
            List<string> nombres = await nombresAsignatura();

            if (nombres.Contains(model.Asignatura.Nombre))
            {
                TempData["NombreRepetido"] = "El nombre ya está cogido.";
                return RedirectToAction("insertarAsignatura", "TAsignaturas", new { nif = nif, rol = rol });
            }

            if (ModelState.IsValid)
            {
                _context.Add(model.Asignatura);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("listaAsignaturas", "TAsignaturas", new { nif = nif, rol = rol});
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

        // GET: TAsignaturas/borrarAsignatura/5
        public async Task<IActionResult> borrarAsignatura(string nif, int rol,int? id)
        {
            AsignaturaView vistaAsignatura = new AsignaturaView();

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

            vistaAsignatura.Asignatura = tAsignatura;
            vistaAsignatura.Rol = rol;
            vistaAsignatura.Nif = nif;

            return View(vistaAsignatura);
        }

        // POST: TAsignaturas/confirmarBorrado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarBorrado(string nif, int rol, int id)
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
            return RedirectToAction("listaAsignaturas", "TAsignaturas", new { nif = nif, rol = rol });
        }

        private bool TAsignaturaExists(int id)
        {
          return (_context.TAsignaturas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
