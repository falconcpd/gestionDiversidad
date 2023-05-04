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
using System.Globalization;
using gestionDiversidad.ViewModels.TMedicos;

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

        // GET: TAlumnos/infoBasica/5
        public async Task<IActionResult> infoBasica(string id)
        {
            MedicoView vistaMedico = new MedicoView();
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();
            UserNavigation actualUser = giveActualUser();
            string actualJson;


            if (id == null || _context.TMedicos == null || sesionRol == 0)
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
                .permisoPantalla(constDefinidas.screenAlumno, sesionRol);
            vistaMedico.LInformes = await _serviceController
                .permisoPantalla(constDefinidas.screenListalInformes, sesionRol);
            vistaMedico.LAlumnos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaAlumnos, sesionRol);
            vistaMedico.LMedicos = await _serviceController
                .permisoPantalla(constDefinidas.screenListaMedicos, sesionRol);

            return View(vistaMedico);
        }

        //GET: TMedicos/listaMedicos
        public async Task<IActionResult> listaMedicos(string volverPadre)
        {
            List<TMedico> listaMedicos;
            ListaMedicosView vistaListasMedicos = new ListaMedicosView();
            bool volverPadreValue;
            string sesionNif = giveSesionNif();
            int sesionRol = giveSesionRol();
            UserNavigation actualUser = giveActualUser();

            volverPadreValue = bool.Parse(volverPadre);
            if (volverPadreValue)
            {
                string userNavigationPadreJson = JsonConvert.SerializeObject(actualUser.padre);
                HttpContext.Session.SetString(constDefinidas.keyActualUser, userNavigationPadreJson);
                actualUser = giveActualUser();
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

        //GET: TMedicos/insertarMedico
        public IActionResult insertarMedico()
        {
            CrearMedicoView vistaCrearMedico = new CrearMedicoView();

            return View(vistaCrearMedico);
        }

        //Función que crea al medico: TProfesores/crearMedico
        public async Task<IActionResult> crearMedico(string nif, string nombre, string apellido1, string apellido2)
        {
            string sesionNif = giveSesionNif();
            var medico = new TMedico
            {
                Nif = nif,
                Nombre = nombre,
                Apellido1 = apellido1,
                Apellido2 = apellido2
            };

            _context.Add(medico);
            await _context.SaveChangesAsync();
            await _serviceController
                .guardarCrearBorrarUsuarioAuditoria(sesionNif, constDefinidas.screenListaMedicos, constDefinidas.accionCrearElemento, nif);
            return RedirectToAction("listaMedicos", "TMedicos",
                new { volverPadre = "false" });
        }

        //GET: TMedicos/modificarMedico
        public async Task<IActionResult> modificarMedico()
        {
            UserNavigation actualUser = giveActualUser();
            TMedico medico = (await _context.TMedicos
                .Include(a => a.NifNavigation)
                .FirstOrDefaultAsync(m => m.Nif == actualUser.nif))!;
            ModificarUsuarios modificarMedicoView = new ModificarUsuarios();
            modificarMedicoView.Nif = medico.Nif;
            modificarMedicoView.Rol = constDefinidas.rolMedico;
            modificarMedicoView.Nombre = medico.Nombre;
            modificarMedicoView.Apellido1 = medico.Apellido1;
            modificarMedicoView.Apellido2 = medico.Apellido2;
            modificarMedicoView.Password = medico.NifNavigation.Password;
            modificarMedicoView.ConfirmPassword = medico.NifNavigation.Password;
            modificarMedicoView.Usuario = medico.NifNavigation.Usuario;

            return View(modificarMedicoView);
        }

        // GET: TMedicos/borrarMedico
        public async Task<IActionResult> borrarMedico(string nifMedico)
        {
            UserNavigation actualUser = giveActualUser();
            TMedico medico = (await _context.TMedicos
                .Include(m => m.TInformes)
                .FirstOrDefaultAsync(a => a.Nif == nifMedico))!;

            BorrarMedicoView vistaBorrarMedico = new BorrarMedicoView();
            vistaBorrarMedico.Medico = medico;

            return View(vistaBorrarMedico);

        }

        // POST: TMedicos/confirmarBorradoMedico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarBorradoMedico(string nifMedico)
        {
            string sesionNif = giveSesionNif();
            TMedico medico = (await _context.TMedicos
                .Include(m => m.TInformes)
                .FirstOrDefaultAsync(m => m.Nif == nifMedico))!;
            TMedico medicoTemporal = (await _context.TMedicos
                .FirstOrDefaultAsync(m => m.Nif == constDefinidas.keyMedicoTemporal))!;
            List<TInforme> informes = medico.TInformes.ToList();
            DateTime fechaTime;  
            string nifAlumno;
            TAlumno alumno;
            byte[] contenido;

            foreach (TInforme informe in informes)
            {
                fechaTime = informe.Fecha;
                nifAlumno = informe.NifAlumno;
                alumno = (await _context.TAlumnos
                .Include(a => a.TInformes)
                .FirstOrDefaultAsync(a => a.Nif == nifAlumno))!;

                medico.TInformes.Remove(informe);
                alumno.TInformes.Remove(informe);

                contenido = informe.Contenido; 
                _context.TInformes.Remove(informe);

                var nuevoInforme = new TInforme
                {
                    NifMedico = constDefinidas.keyMedicoTemporal,
                    NifAlumno = nifAlumno,
                    Fecha = fechaTime,
                    Contenido = contenido
                };
                _context.Add(nuevoInforme);
                await _context.SaveChangesAsync();
            }

            _context.TMedicos.Remove(medico);

            await _serviceController
                .guardarCrearBorrarUsuarioAuditoria(sesionNif, constDefinidas.screenListaMedicos, constDefinidas.accionBorrarElemento, nifMedico);

            TUsuario usuario = (await _context.TUsuarios
                .FirstOrDefaultAsync(u => u.Nif == nifMedico))!;
            _context.TUsuarios.Remove(usuario);
            await _context.SaveChangesAsync();


            return RedirectToAction("listaMedicos", "TMedicos", new
            {
                volverPadre = "false"
            });
        }

    }
}
