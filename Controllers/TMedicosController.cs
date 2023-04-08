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
using gestionDiversidad.Navigation;
using Newtonsoft.Json;

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
            string sesionNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            int rol = rolRaw ?? 0;
            string userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
            UserNavigation actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;
            String actualJson;


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

            if (!(actualUser.rol == constDefinidas.rolMedico))
            {
                actualUser = new UserNavigation(id, constDefinidas.rolMedico, actualUser);
                actualJson = JsonConvert.SerializeObject(actualUser);
                HttpContext.Session.SetString(constDefinidas.keyActualUser, actualJson);
            }

            vistaMedico.Medico = tMedico;
            vistaMedico.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenAlumno, rol);
            vistaMedico.LInformes = await _serviceController
                .permisoPantalla(constDefinidas.screenListalInformes, rol);
            vistaMedico.LAlumnos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, rol);
            vistaMedico.Rol = constDefinidas.rolMedico;
            vistaMedico.SesionRol = rol;
            vistaMedico.SesionNif = sesionNif;
            if (!(actualUser.padre == null))
            {
                vistaMedico.PadreNif = actualUser.padre.nif;
                vistaMedico.PadreRol = actualUser.padre.rol;
            }
            else
            {
                vistaMedico.PadreNif = "dummy";
                vistaMedico.PadreRol = 10;
            }
            return View(vistaMedico);
        }

        //GET: TMedicos/listaMedicos
        public async Task<IActionResult> listaMedicos(string volverPadre)
        {
            List<TMedico> listaMedicos;
            ListaMedicosView vistaListasMedicos = new ListaMedicosView();
            string sessionKeyRol = constDefinidas.keyRol;
            string sessionKeyNif = constDefinidas.keyNif;

            int? rawRol = HttpContext.Session.GetInt32(sessionKeyRol);
            string sesionNif = HttpContext.Session.GetString(sessionKeyNif)!;
            int sesionRol = rawRol ?? 0;
            bool volverPadreValue;
            string userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
            UserNavigation actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;

            //Si volvemos de un usuario importante, volvemos a ser el padre: 
            volverPadreValue = bool.Parse(volverPadre);
            if (volverPadreValue)
            {
                string userNavigationPadreJson = JsonConvert.SerializeObject(actualUser.padre);
                HttpContext.Session.SetString(constDefinidas.keyActualUser, userNavigationPadreJson);
                userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
                actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;
            }

            listaMedicos = await _serviceController.listaMedicos();

            vistaListasMedicos.ListaMedicos = listaMedicos;
            vistaListasMedicos.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenListaMedicos, sesionRol);
            vistaListasMedicos.Medico = await _serviceController
                .permisoPantalla(constDefinidas.screenMedico, sesionRol);
            vistaListasMedicos.SesionNif = sesionNif;
            vistaListasMedicos.SesionRol = sesionRol;


            return View(vistaListasMedicos);

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

        //GET: TMedicos/insertarMedico
        public IActionResult insertarMedico()
        {
            int? rawRol = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            string rawNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            int sesionRol = rawRol ?? 0;
            string sesionNif = rawNif;

            CrearMedicoView vistaCrearMedico = new CrearMedicoView();

            return View(vistaCrearMedico);
        }

        //Función que crea al medico: TProfesores/crearMedico
        public async Task<IActionResult> crearMedico(string nif, string nombre, string apellido1, string apellido2)
        {
            var medico = new TMedico
            {
                Nif = nif,
                Nombre = nombre,
                Apellido1 = apellido1,
                Apellido2 = apellido2
            };

            _context.Add(medico);
            await _context.SaveChangesAsync();
            return RedirectToAction("listaMedicos", "TMedicos",
                new { volverPadre = "false" });
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
