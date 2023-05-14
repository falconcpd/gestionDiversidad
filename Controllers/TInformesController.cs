using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using gestionDiversidad.Models;
using System.Globalization;
using gestionDiversidad.Interfaces;
using gestionDiversidad.Constantes;
using gestionDiversidad.Navigation;
using Newtonsoft.Json;
using System.IO;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics.CodeAnalysis;
using gestionDiversidad.ViewModels.TInformes;

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

        //Función para recuperar el rol del usuario que ha iniciado sesión
        public int giveSesionRol()
        {
            int? rolRaw = HttpContext.Session.GetInt32(constDefinidas.keyRol);
            int rol = rolRaw ?? 0;
            return rol;
        }

        //Función para recuperar el nif del usuario que ha iniciado sesión 
        public string giveSesionNif()
        {
            string sesionNif = HttpContext.Session.GetString(constDefinidas.keyNif)!;
            return sesionNif;
        }

        //Función que devuelve el usuario en el que nos encontramos
        public UserNavigation giveActualUser()
        {
            string userNavigationJson = HttpContext.Session.GetString(constDefinidas.keyActualUser)!;
            UserNavigation actualUser = JsonConvert.DeserializeObject<UserNavigation>(userNavigationJson!)!;
            return actualUser;
        }

        // GET: TInformes/listaInformes
        public async Task<IActionResult> listaInformes()
        {
            List<TInforme> informes = new List<TInforme>();
            ListaInformesView vistaListaInformes = new ListaInformesView();
            int sesionRol = giveSesionRol();
            UserNavigation actualUser = giveActualUser();
            string actualName = (await _serviceController.giveActualNombre(actualUser.nif, actualUser.rol));

            informes = (await _serviceController.listaInformes(actualUser.nif, actualUser.rol));

             vistaListaInformes.Permiso = await _serviceController
                .permisoPantalla(constDefinidas.screenListalInformes, sesionRol);
            vistaListaInformes.Informe = await _serviceController
                .permisoPantalla(constDefinidas.screenInforme, sesionRol);
            vistaListaInformes.ListaInformes = informes;
            vistaListaInformes.ActualRol = actualUser.rol;
            vistaListaInformes.ActualNif = actualUser.nif;
            vistaListaInformes.ActualName = actualName;

            return View(vistaListaInformes);

        }

        // GET: TInformes/infoBasica
        public async Task<IActionResult> infoBasica(string nifAlumno, string nifMedico, string fecha)
        {
            InformeView informeView = new InformeView();
            TInforme informe;
            int sesionRol = giveSesionRol();
            string sesionNif = giveSesionNif();
            UserNavigation actualUser = giveActualUser();
            TAlumno alumno = (await _context.TAlumnos.FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;
            TMedico medico = (await _context.TMedicos.FirstOrDefaultAsync(m => m.Nif == nifMedico))!;

            informe = await _serviceController.buscarInforme(nifAlumno, nifMedico, fecha);

            informeView.Informe = informe;
            informeView.Permiso = await _serviceController.
                permisoPantalla(constDefinidas.screenInforme, sesionRol);
            informeView.Alumno = alumno;
            informeView.Medico = medico;

            return View(informeView);
        }

        // GET: TInformes/verInforme
        public async Task<IActionResult> verInforme(string nifAlumno, string nifMedico, string fecha)
        {
            TInforme informe = await _serviceController.buscarInforme(nifAlumno, nifMedico, fecha);
            MemoryStream stream = new MemoryStream(informe.Contenido);
            return new FileStreamResult(stream, "application/pdf");
            
        }

        // POST : TInformes/ActualizarPDF
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarPDF(string nifMedico, string nifAlumno, 
            string fecha, IFormFile PDF)
        {
            string sesionNif = giveSesionNif();
            var informe = await _serviceController.buscarInforme(nifAlumno, nifMedico, fecha);

            if (informe == null)
            {
                return NotFound();
            }

            if(PDF == null)
            {
                TempData["InformeVacio"] = "No se ha seleccionado ningún informe para sustituir";
                return RedirectToAction("infoBasica", "TInformes", new
                {
                    nifMedico = nifMedico,
                    nifAlumno = nifAlumno,
                    fecha = fecha
                });
            }

            using (var ms = new MemoryStream())
            {
                await PDF!.CopyToAsync(ms);
                informe.Contenido = ms.ToArray();
            }

            _context.TInformes.Update(informe);
            await _context.SaveChangesAsync();
            await _serviceController
                .guardarModificarInformeAuditoria(sesionNif, constDefinidas.screenListalInformes, informe); 

            return RedirectToAction("infoBasica", "TInformes", new { 
                nifMedico = nifMedico, 
                nifAlumno = nifAlumno, 
                fecha = fecha
            });
        }

        // POST : TInformes/crearInformeNuevo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> crearInformeNuevo(CrearInformeView model)
        {
            string sesionNif = giveSesionNif();

            if (ModelState.IsValid)
            {
                byte[] contenido;
                string nifMedico = model.MedicoNif;
                string nifAlumno = model.AlumnoNif;
                using (var ms = new MemoryStream())
                {
                    await model.PDF.CopyToAsync(ms);
                    contenido = ms.ToArray();
                }

                DateTime fechaActualFinal = _serviceController.fechaPresente();

                var informe = new TInforme
                {
                    NifMedico = nifMedico,
                    NifAlumno = nifAlumno,
                    Fecha = fechaActualFinal,
                    Contenido = contenido
                };
                _context.Add(informe);
                await _context.SaveChangesAsync();
                await _serviceController
                    .guardarCrearBorrarInformeAuditoria(sesionNif, constDefinidas.screenListalInformes, constDefinidas.accionCrearElemento, informe);

                return RedirectToAction("listaInformes", "TInformes");
            }

            return RedirectToAction("insertarInforme", "TInformes"); 
        }

        //Funcion que redirecciona para crear informe.
        public async Task<IActionResult> insertarInforme()
        {
            int sesionRol = giveSesionRol();
            string sesionNif = giveSesionNif();
            UserNavigation actualUser = giveActualUser();
            TMedico medicoTemporal = (await _context.TMedicos
                .FirstOrDefaultAsync(m => m.Nif == constDefinidas.keyMedicoTemporal))!;

            CrearInformeView vistaCrearInforme = new CrearInformeView();
            vistaCrearInforme.ListaMedicos = (await _serviceController.listaMedicos());
            vistaCrearInforme.ListaMedicos.Remove(medicoTemporal);
            vistaCrearInforme.ListaAlumnos = (await _serviceController.listaAlumnos(sesionNif, sesionRol));
            vistaCrearInforme.ActualNif = actualUser.nif;
            vistaCrearInforme.ActualRol = actualUser.rol;

            return View(vistaCrearInforme);
        }

        // GET: TInformes/borrarInforme
        public async Task<IActionResult> borrarInforme(string nifMedico, string nifAlumno, string fecha)
        {
            UserNavigation actualUser = giveActualUser();
            TAlumno alumno = (await _context.TAlumnos
                .Include(a => a.TInformes)
                .FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;

            int numInformes = alumno.TInformes.Count();
            if(numInformes < 2)
            {
                TempData["UnSoloInforme"] = "A ese alumno solo le queda un informe." +
                    "Como es obligatorio que tenga al menos uno, por favor, borre el alumno " +
                    "completamente o añada otro informe";

                return RedirectToAction("listaInformes", "TInformes");
                    
            }

            BorrarInformeView vistaBorrarInforme = new BorrarInformeView();
            vistaBorrarInforme.NifAlumno = nifAlumno;
            vistaBorrarInforme.NifMedico = nifMedico;
            vistaBorrarInforme.Fecha = fecha;

            return View(vistaBorrarInforme);

        }

        // POST: TInformes/confirmarBorradoInforme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarBorradoInforme(BorrarInformeView model)
        {
            if (ModelState.IsValid)
            {
                string sesionNif = giveSesionNif();
                TInforme informe = await _serviceController.buscarInforme(model.NifAlumno, model.NifMedico, model.Fecha);
                await _serviceController
                    .guardarCrearBorrarInformeAuditoria(sesionNif, constDefinidas.screenListalInformes, constDefinidas.accionBorrarElemento, informe);
                _context.TInformes.Remove(informe);
                await _context.SaveChangesAsync();

                return RedirectToAction("listaInformes", "TInformes");
            }

            return RedirectToAction("borrarInforme", "TInformes", new
            {
                nifMedico = model.NifMedico,
                nifAlumno = model.NifAlumno,
                fecha = model.Fecha
            });
        }

        // GET: TInformes/elegirMedicoInforme
        public async Task<IActionResult> elegirMedicoInforme(string nifMedico, string nifAlumno, string fecha)
        {
            UserNavigation actualUser = giveActualUser();

            List<TMedico> listaMedicos = (await _context.TMedicos.ToListAsync())!;
            if(listaMedicos.Count <= 1)
            {
                TempData["UnSoloMedicoParaCambiar"] = "No hay médicos, por lo que no tiene" +
                    "sentido cambiarlo";

                return RedirectToAction("listaInformes", "TInformes");
            }

            TAlumno alumno = (await _context.TAlumnos
                .FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;
            TMedico medico = (await _context.TMedicos
                .FirstOrDefaultAsync(m => m.Nif == nifMedico))!;

            listaMedicos.Remove(medico);

            ModificarMedicoInforme modificar = new ModificarMedicoInforme();
            modificar.Medico = medico;
            modificar.Alumno = alumno;
            modificar.Fecha = fecha;
            modificar.ListaMedicos = listaMedicos;

            return View(modificar);
        }
        // POST: TInformes/confirmarCambioMedicoInforme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarCambioMedicoInforme(string nifAnteriorMedico, string nifAlumno, string fecha, ModificarMedicoInforme model)
        {
            string sesionNif = giveSesionNif();
            string nifNuevoMedico = _serviceController.separarIdentificador(model.MedicoNif);
            TInforme informe = await _serviceController.buscarInforme(nifAlumno, nifAnteriorMedico, fecha);
            TMedico anteriorMedico = (await _context.TMedicos
                .FirstOrDefaultAsync(m => m.Nif == nifAnteriorMedico))!;
            TMedico nuevoMedico = (await _context.TMedicos
                .FirstOrDefaultAsync(m => m.Nif == nifNuevoMedico))!;
            TAlumno alumno = (await _context.TAlumnos
                .FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;
            await _serviceController
                .guardarModificarMedicoInformeAuditoria(sesionNif, constDefinidas.screenListalInformes, nifNuevoMedico, informe);

            anteriorMedico.TInformes.Remove(informe);
            alumno.TInformes.Remove(informe);

            byte[] contenido = informe.Contenido;
            _context.TInformes.Remove(informe);

            DateTime fechaTime = DateTime
                .ParseExact(fecha, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var nuevoInforme = new TInforme
            {
                NifMedico = nifNuevoMedico,
                NifAlumno = nifAlumno,
                Fecha = fechaTime,
                Contenido = contenido
            };
            _context.Add(nuevoInforme);
            await _context.SaveChangesAsync();

            return RedirectToAction("listaInformes", "TInformes");
        }
    }
}
