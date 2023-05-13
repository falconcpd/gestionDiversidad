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
using gestionDiversidad.Navigation;
using Newtonsoft.Json;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using gestionDiversidad.ViewModels.TAlumnos;
using gestionDiversidad.ViewModels.TProfesores;
using gestionDiversidad.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;

namespace gestionDiversidad.Controllers
{
    public class TUsuariosController : Controller
    {
        private readonly TfgContext _context;
        private readonly IServiceController _serviceController;

        public TUsuariosController(TfgContext context, IServiceController sc)
        {
            _context = context;
            _serviceController = sc;
        }

        // GET: TUsuarios/InicioSesion
        public IActionResult InicioSesion()
        {
            return View();
        }

        // GET: TUsuarios/logging
        public IActionResult logging(string usuario, string password)
        {
            var user = _context.TUsuarios
                .FirstOrDefault(u => u.Usuario == usuario && u.Password == password);
            string nif;
            int rol;

            if (usuario == null)
            {
                TempData["ErrorSesion"] = "El usuario está vacío";
                return View("InicioSesion");
            }
            else if (password == null)
            {
                TempData["ErrorSesion"] = "La contraseña está vacía";
                return View("InicioSesion");
            }
            else if (user == null)
            {
                TempData["ErrorSesion"] = "El usuario o contraseña son incorrectos";
                return View("InicioSesion");
            }
            else if(usuario == constDefinidas.keyMedicoTemporal)
            {
                TempData["ErrorSesion"] = "No se puede iniciar sesión como un médico temporal";
                return View("InicioSesion");
            }

            rol = user.IdRol;
            nif = user.Nif;
            UserNavigation raiz = new UserNavigation(nif, rol, null);

            string sessionKeyRol = constDefinidas.keyRol;
            string sessionKeyNif = constDefinidas.keyNif;
            string sessionKeyUser = constDefinidas.keyUser;
            string sessionActualUser = constDefinidas.keyActualUser;
            string userNavigationJson = JsonConvert.SerializeObject(raiz);
            HttpContext.Session.SetInt32(sessionKeyRol, rol);
            HttpContext.Session.SetString(sessionKeyNif, nif);
            HttpContext.Session.SetString(sessionKeyUser, usuario);
            HttpContext.Session.SetString(sessionActualUser, userNavigationJson);

            return RedirectToAction("volverPerfil", "TUsuarios", new
            {
                nif = nif,
                rol = rol
            });
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

        // Función para volver/iniciar un usuario
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
            var usuario = await _context.TUsuarios
                .AnyAsync(u => u.Nif == nif);
            return Json(!usuario);
        }

        //[Remote] para que no se repitan nombre de usuarios en un usuario
        //GET : TUsuarios/verificarModificarNombreUsuario
        public async Task<IActionResult> verificarModificarNombreUsuario(string usuario, string nif)
        {
            TUsuario user = (await _context.TUsuarios
                .FirstOrDefaultAsync(m => m.Nif == nif))!;

            var TUsuario = await _context.TUsuarios
                .AnyAsync(u => u.Usuario == usuario);

            if(user.Usuario == usuario)
            {
                return Json(TUsuario);
            }
            return Json(!TUsuario);
        }

        //[Remote] para que no se repitan nombre de usuarios en un usuario
        //GET : TUsuarios/verificarCrearNombreUsuario
        public async Task<IActionResult> verificarCrearNombreUsuario(string usuario)
        {
            var TUsuario = await _context.TUsuarios
                .AnyAsync(u => u.Usuario == usuario);

            return Json(!TUsuario);
        }

        //[Remote] para ver que la estructura está bien
        //GET : verificarAlumno
        public async Task<IActionResult> verificarAlumno(string nifAlumno)
        {
            if (_serviceController.confirmarEstructura(nifAlumno) && await _serviceController.existeDistintoUsuario(nifAlumno, constDefinidas.rolAlumno))
            {
                return Json(true);
            }

            return Json(false);

        }

        //POST: TUsuarios/crearUsuarioProfesor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> crearUsuarioProfesor(CrearProfesorView model)
        {
            if (ModelState.IsValid)
            {
                var user = new TUsuario
                {
                    Nif = model.Nif!,
                    Usuario = model.Usuario!,
                    Password = model.Password!,
                    IdRol = constDefinidas.rolProfesor
                };

                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("crearProfesor", "TProfesores", new
                {
                    nif = user.Nif,
                    nombre = model.Nombre,
                    apellido1 = model.Apellido1,
                    apellido2 = model.Apellido2
                });
            }
            return RedirectToAction("insertarProfesor", "TProfesores");
        }

        //POST: TUsuarios/crearUsuarioMedico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> crearUsuarioMedico(CrearProfesorView model)
        {
            if (ModelState.IsValid)
            {
                var user = new TUsuario
                {
                    Nif = model.Nif!,
                    Usuario = model.Usuario!,
                    Password = model.Password!,
                    IdRol = constDefinidas.rolMedico
                };

                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("crearMedico", "TMedicos",
                    new
                    {
                        nif = user.Nif,
                        nombre = model.Nombre,
                        apellido1 = model.Apellido1,
                        apellido2 = model.Apellido2
                    });

            }
            return RedirectToAction("insertarMedico", "TMedicos");
        }

        //POST: TUsuarios/crearUsuarioAlumno
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> crearUsuarioAlumno(CrearAlumnoView model)
        {
            if (ModelState.IsValid)
            {
                string sesionNif = giveSesionNif();
                byte[] file;
                using (var ms = new MemoryStream())
                {
                    await model.PDF.CopyToAsync(ms);
                    file = ms.ToArray();
                }

                var user = new TUsuario
                {
                    Nif = model.Nif!,
                    Usuario = model.Usuario!,
                    Password = model.Password!,
                    IdRol = constDefinidas.rolAlumno
                };

                _context.Add(user);
                await _context.SaveChangesAsync();
                /// Parte alumno
                var alumno = new TAlumno
                {
                    Nif = model.Nif!,
                    Nombre = model.Nombre!,
                    Apellido1 = model.Apellido1!,
                    Apellido2 = model.Apellido2!
                };

                _context.Add(alumno);
                await _context.SaveChangesAsync();
                await _serviceController
                    .guardarCrearBorrarUsuarioAuditoria(sesionNif, constDefinidas.screenListaAlumnos, constDefinidas.accionCrearElemento, model.Nif);
                ///Parte informe
                DateTime fechaActualFinal = _serviceController.fechaPresente();

                var informe = new TInforme
                {
                    NifMedico = model.MedicoNif,
                    NifAlumno = model.Nif,
                    Fecha = fechaActualFinal,
                    Contenido = file
                };

                _context.Add(informe);
                await _context.SaveChangesAsync();

                return RedirectToAction("listaAlumnos", "TAlumnos",new
                    {
                        volverPadre = "false"
                    });

            }
            return RedirectToAction("insertarAlumno", "TAlumnos");
        }

        // Función que verifica si de verdad hay cambios
        public async Task<bool> verficarCambios(ModificarUsuarios model)
        {
            TUsuario usuario = (await _context.TUsuarios
            .FirstOrDefaultAsync(a => a.Nif == model.Nif))!;

                switch (model.Rol)
                {
                    case constDefinidas.rolAlumno:
                        TAlumno alumno = (await _context.TAlumnos
                            .FirstOrDefaultAsync(a => a.Nif == model.Nif))!;
                        if(model.Nombre != alumno.Nombre || model.Apellido1 != alumno.Apellido1 || model.Apellido2 != alumno.Apellido2 || model.Usuario != usuario.Usuario || model.Password != usuario.Password)
                        {
                            return true;
                        }
                        break;
                    case constDefinidas.rolProfesor:
                        TProfesor profesor = (await _context.TProfesors
                            .FirstOrDefaultAsync(p => p.Nif == model.Nif))!;
                        if (model.Nombre != profesor.Nombre || model.Apellido1 != profesor.Apellido1 || model.Apellido2 != profesor.Apellido2 || model.Usuario != usuario.Usuario || model.Password != usuario.Password)
                        {
                            return true;
                        }
                        break;
                    case constDefinidas.rolMedico:
                        TMedico medico = (await _context.TMedicos
                            .FirstOrDefaultAsync(m => m.Nif == model.Nif))!;
                        if (model.Nombre != medico.Nombre || model.Apellido1 != medico.Apellido1 || model.Apellido2 != medico.Apellido2 || model.Usuario != usuario.Usuario || model.Password != usuario.Password)
                        {
                            return true;
                        }
                        break;
                }
            

            return false;
        }

        // POST: TUsuarios/confirmarCambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarCambios(ModificarUsuarios model)
        {
            int rol = model.Rol;
            string nif = model.Nif;

            if (ModelState.IsValid && (await verficarCambios(model)))
            {

                string sesionNif = giveSesionNif();
                TUsuario usuario = (await _context.TUsuarios
                            .FirstOrDefaultAsync(a => a.Nif == nif))!;
                var anteriorUsuario = new TUsuario
                {
                    Nif = usuario.Nif!,
                    Usuario = usuario.Usuario!,
                    Password = usuario.Password!,
                    IdRol = usuario.IdRol
                }; 

                usuario.Password = model.Password!;
                usuario.Usuario = model.Usuario!;

                switch (rol)
                {
                    case constDefinidas.rolAlumno:
                        await _serviceController
                            .guardarModificarUsuarioAuditoria(sesionNif, constDefinidas.screenAlumno, model, anteriorUsuario);
                        TAlumno alumno = (await _context.TAlumnos
                            .FirstOrDefaultAsync(a => a.Nif == nif))!;
                        alumno.Nombre = model.Nombre;
                        alumno.Apellido1 = model.Apellido1;
                        alumno.Apellido2 = model.Apellido2;
                        await _context.SaveChangesAsync();

                        return RedirectToAction("volverPerfil", "TUsuarios", new
                        {
                            nif = nif,
                            rol = rol
                        });
                    case constDefinidas.rolProfesor:
                        await _serviceController
                            .guardarModificarUsuarioAuditoria(sesionNif, constDefinidas.screenProfesor, model, anteriorUsuario);
                        TProfesor profesor = (await _context.TProfesors
                            .FirstOrDefaultAsync(p => p.Nif == nif))!;
                        profesor.Nombre = model.Nombre;
                        profesor.Apellido1 = model.Apellido1;
                        profesor.Apellido2 = model.Apellido2;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("volverPerfil", "TUsuarios", new
                        {
                            nif = nif,
                            rol = rol
                        });
                    case constDefinidas.rolMedico:
                        await _serviceController
                            .guardarModificarUsuarioAuditoria(sesionNif, constDefinidas.screenMedico, model, anteriorUsuario);
                        TMedico medico = (await _context.TMedicos
                            .FirstOrDefaultAsync(m => m.Nif == nif))!;
                        medico.Nombre = model.Nombre;
                        medico.Apellido1 = model.Apellido1;
                        medico.Apellido2 = model.Apellido2;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("volverPerfil", "TUsuarios", new
                        {
                            nif = nif,
                            rol = rol
                        });
                }
            }

            TempData["SinCambiosUsuario"] = "No se ha modificado ningún parámetro del usuario";
            switch (rol)
            {
                case constDefinidas.rolAlumno:
                    return RedirectToAction("modificarAlumno", "TAlumnos");
                case constDefinidas.rolProfesor:
                    TProfesor profesor = (await _context.TProfesors
                        .FirstOrDefaultAsync(p => p.Nif == nif))!;
                    return RedirectToAction("modificarProfesor", "TProfesores");
                case constDefinidas.rolMedico:
                    TAlumno medico = (await _context.TAlumnos
                        .FirstOrDefaultAsync(m => m.Nif == nif))!;
                    return RedirectToAction("modificarMedico", "TMedicos");
            }
            return RedirectToAction("volverPerfil", "TUsuarios", new
            {
                nif = nif,
                rol = rol
            });
        }

        //POST : TUsuarios/cerrarSesion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult cerrarSesion()
        {
            string sessionKeyRol = constDefinidas.keyRol;
            string sessionKeyNif = constDefinidas.keyNif;
            string sessionKeyUser = constDefinidas.keyUser;
            string sessionActualUser = constDefinidas.keyActualUser;

            HttpContext.Session.Remove(sessionKeyRol);
            HttpContext.Session.Remove(sessionKeyNif);
            HttpContext.Session.Remove(sessionKeyUser);
            HttpContext.Session.Remove(sessionActualUser);

            return RedirectToAction("InicioSesion", "TUsuarios");
        }

        //Función que redirecciona si algún usuario, informe cualquier otra cosa se ha borrado en medio de una sentencia y ya no existe. 
        public IActionResult errorVolverInicio()
        {
            string sessionKeyRol = constDefinidas.keyRol;
            string sessionKeyNif = constDefinidas.keyNif;
            string sessionKeyUser = constDefinidas.keyUser;
            string sessionActualUser = constDefinidas.keyActualUser;

            HttpContext.Session.Remove(sessionKeyRol);
            HttpContext.Session.Remove(sessionKeyNif);
            HttpContext.Session.Remove(sessionKeyUser);
            HttpContext.Session.Remove(sessionActualUser);

            return RedirectToAction("InicioSesion", "TUsuarios");
        }

        //POST : TUsuarios/volverPerfilPrincipal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult volverPerfilPrincipal(string nif, int rol)
        {
            string actualJson;
            UserNavigation actualUser = giveActualUser();

            if(actualUser.padre != null)
            {
                UserNavigation sesionUser = new UserNavigation(nif, rol, null);
                actualJson = JsonConvert.SerializeObject(sesionUser);
                HttpContext.Session.SetString(constDefinidas.keyActualUser, actualJson);
            }

            return RedirectToAction("volverPerfil", "TUsuarios", new
            {
                nif = nif,
                rol = rol
            });
        }

    }
}
