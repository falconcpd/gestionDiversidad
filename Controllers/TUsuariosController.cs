using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using gestionDiversidad.Constantes;
using gestionDiversidad.ViewModels;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
//using Microsoft.Identity.Client;
//using AspNetCore;

namespace gestionDiversidad.Controllers
{
    public class TUsuariosController : Controller
    {
        private readonly TfgContext _context;

        public TUsuariosController(TfgContext context)
        {
            _context = context;
        }

        // GET: TUsuarios
        public async Task<IActionResult> Index()
        {
            var tfgContext = _context.TUsuarios.Include(t => t.IdRolNavigation);
            return View(await tfgContext.ToListAsync());
        }

        //GET: TUsuarios/insertarUsuario

        // GET: TUsuarios/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TUsuarios == null)
            {
                return NotFound();
            }

            var tUsuario = await _context.TUsuarios
                .Include(t => t.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tUsuario == null)
            {
                return NotFound();
            }

            return View(tUsuario);
        }

        // GET: TUsuarios/InicioSesion
        public IActionResult InicioSesion()
        {
            return View();
        }

        /*
         public IActionResult InicioSesion()
        {
            return View();
        }
         */

        // POST: TUsuarios/logging
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult logging(string usuario, string password)
        {
            var user = _context.TUsuarios.FirstOrDefault(u => u.Usuario == usuario && u.Password == password);
            string nif;
            int rol;
            
            if (usuario == null || password == null || user == null)
            {
                TempData["ErrorSesion"] = "No has podido iniciar sesión";
                return View("InicioSesion");
            }
            rol = user.IdRol;
            nif = user.Nif;

            string sessionKeyRol = constDefinidas.keyRol;
            string sessionKeyNif = constDefinidas.keyNif;
            HttpContext.Session.SetInt32(sessionKeyRol, rol);
            HttpContext.Session.SetString(sessionKeyNif, nif);

            switch (rol)
            {
                case constDefinidas.rolAlumno:
                    return RedirectToAction("infoBasica", "TAlumnos", new { id = nif });
                case constDefinidas.rolProfesor:
                    return RedirectToAction("infoBasica", "TProfesores", new { id = nif });
                case constDefinidas.rolMedico:
                    return RedirectToAction("infoBasica", "TMedicos", new { id = nif });
                case constDefinidas.rolAdmin:
                    return RedirectToAction("infoBasica", "TAdministraciones", new { id = nif });
                default:
                    return View("InicioSesion");
            }

        }
        //Acción para volver 
        public IActionResult volverPerfil(string nif, int rol)
        {
            switch (rol)
            {
                case constDefinidas.rolAlumno:
                    return RedirectToAction("infoBasica", "TAlumnos", new { id = nif });
                case constDefinidas.rolProfesor:
                    return RedirectToAction("infoBasica", "TProfesores", new { id = nif });
                case constDefinidas.rolMedico:
                    return RedirectToAction("infoBasica", "TMedicos", new { id = nif });
                default:
                    return RedirectToAction("infoBasica", "TAdministraciones", new { id = nif });
            }

        }

        //[Remote] para que no se repitan NIF en un usuario
        //GET : TUsuarios/verificarNif
        public async Task<IActionResult> verificarNif(string nif)
        {
            var usuario = await _context.TUsuarios.AnyAsync(u => u.Nif == nif);
            return Json(!usuario);
        }

        //POST: TUsuarios/crearUsuarioProfesor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> crearUsuarioProfesor(CrearProfesorView model, int rolCreador, string nifCreador)
        {
            if (ModelState.IsValid)
            {
                var user = new TUsuario
                {
                    Nif = model.Nif,
                    Usuario = model.Usuario,
                    Password = model.Password,
                    IdRol = constDefinidas.rolProfesor
                };
                 _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("crearProfesor", "TProfesores", 
                    new { nif = user.Nif, nombre = model.Nombre, 
                        apellido1 = model.Apellido1, apellido2 = model.Apellido2, 
                    nifCreador = nifCreador, rolCreador = rolCreador });

            }
            return RedirectToAction("insertarProfesor", "TProfesores", new { nif = nifCreador, rol = rolCreador });
        }
        

        // GET: TUsuarios/Create
        public IActionResult Create()
        {
            ViewData["IdRol"] = new SelectList(_context.TRols, "Id", "Id");
            return View();
        }

        // POST: TUsuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nif,Usuario,Password,IdRol")] TUsuario tUsuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tUsuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRol"] = new SelectList(_context.TRols, "Id", "Id", tUsuario.IdRol);
            return View(tUsuario);
        }

        // GET: TUsuarios/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TUsuarios == null)
            {
                return NotFound();
            }

            var tUsuario = await _context.TUsuarios.FindAsync(id);
            if (tUsuario == null)
            {
                return NotFound();
            }
            ViewData["IdRol"] = new SelectList(_context.TRols, "Id", "Id", tUsuario.IdRol);
            return View(tUsuario);
        }

        // POST: TUsuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nif,Usuario,Password,IdRol")] TUsuario tUsuario)
        {
            if (id != tUsuario.Nif)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tUsuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TUsuarioExists(tUsuario.Nif))
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
            ViewData["IdRol"] = new SelectList(_context.TRols, "Id", "Id", tUsuario.IdRol);
            return View(tUsuario);
        }

        // GET: TUsuarios/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TUsuarios == null)
            {
                return NotFound();
            }

            var tUsuario = await _context.TUsuarios
                .Include(t => t.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.Nif == id);
            if (tUsuario == null)
            {
                return NotFound();
            }

            return View(tUsuario);
        }

        // POST: TUsuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TUsuarios == null)
            {
                return Problem("Entity set 'TfgContext.TUsuarios'  is null.");
            }
            var tUsuario = await _context.TUsuarios.FindAsync(id);
            if (tUsuario != null)
            {
                _context.TUsuarios.Remove(tUsuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TUsuarioExists(string id)
        {
          return (_context.TUsuarios?.Any(e => e.Nif == id)).GetValueOrDefault();
        }
    }
}
