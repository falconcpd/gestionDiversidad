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

namespace gestionDiversidad.Controllers
{
    public class TUsuariosController : Controller
    {
        private readonly TfgContext _context;

        public TUsuariosController(TfgContext context)
        {
            _context = context;
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
            
            if(usuario == null)
            {
                TempData["ErrorSesion"] = "El usuario está vacío.";
                return View("InicioSesion");
            }else if(password == null)
            {
                TempData["ErrorSesion"] = "La contraseña está vacía.";
                return View("InicioSesion");
            }
            else if(user == null)
            {
                TempData["ErrorSesion"] = "El usuario o contraseña son incorrectos.";
                return View("InicioSesion");
            }

            rol = user.IdRol;
            nif = user.Nif;
            UserNavigation raiz = new UserNavigation(nif, rol, null);

            string sessionKeyRol = constDefinidas.keyRol;
            string sessionKeyNif = constDefinidas.keyNif;
            string sessionActualUser = constDefinidas.keyActualUser;
            string userNavigationJson = JsonConvert.SerializeObject(raiz);
            HttpContext.Session.SetInt32(sessionKeyRol, rol);
            HttpContext.Session.SetString(sessionKeyNif, nif);
            HttpContext.Session.SetString(sessionActualUser, userNavigationJson);

            return RedirectToAction("volverPerfil", "TUsuarios", new
            {
                nif = nif,
                rol = rol
            });
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
        //GET : TUsuarios/verificarNombreUsuario
        public async Task<IActionResult> verificarNombreUsuario(string usuario)
        {
            var TUsuario = await _context.TUsuarios
                .AnyAsync(u => u.Usuario == usuario);
            return Json(!TUsuario);
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

                return RedirectToAction("crearProfesor", "TProfesores", new {
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
                using (var ms = new MemoryStream())
                {
                    await model.PDF.CopyToAsync(ms);
                    var file = ms.ToArray();
                    string base64file = Convert.ToBase64String(file);
                    TempData[constDefinidas.keyInformePDF] = base64file;
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

                return RedirectToAction("crearAlumno", "TAlumnos", new {
                        nif = model.Nif,
                        nombre = model.Nombre,
                        apellido1 = model.Apellido1,
                        apellido2 = model.Apellido2, 
                        medico = model.MedicoNif
                    });

            }
            return RedirectToAction("insertarAlumno", "TAlumnos");
        }

        // POST: TUsuarios/confirmarCambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmarCambios(ModificarUsuarios model)
        {
            int rol = model.Rol;
            string nif = model.Nif;

            if (ModelState.IsValid)
            {
                switch (rol)
                {
                    case constDefinidas.rolAlumno:
                        TAlumno alumno = (await _context.TAlumnos
                            .FirstOrDefaultAsync(a => a.Nif == nif))!;
                        alumno.Nombre = model.Nombre;
                        alumno.Apellido1 = model.Apellido1;
                        alumno.Apellido2 = model.Apellido2;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("volverPerfil", "TUsuarios", new { 
                            nif = nif, 
                            rol = rol 
                        });
                    case constDefinidas.rolProfesor:
                        TProfesor profesor = (await _context.TProfesors
                            .FirstOrDefaultAsync(p => p.Nif == nif))!;
                        profesor.Nombre = model.Nombre;
                        profesor.Apellido1 = model.Apellido1;
                        profesor.Apellido2 = model.Apellido2;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("volverPerfil", "TUsuarios", new { 
                            nif = nif,
                            rol = rol 
                        });
                    case constDefinidas.rolMedico:
                        TMedico medico = (await _context.TMedicos
                            .FirstOrDefaultAsync(m => m.Nif == nif))!;
                        medico.Nombre = model.Nombre;
                        medico.Apellido1 = model.Apellido1;
                        medico.Apellido2 = model.Apellido2;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("volverPerfil", "TUsuarios", new { 
                            nif = nif,
                            rol = rol 
                        });
                }
            }

            switch (rol)
            {
                case constDefinidas.rolProfesor:
                    TProfesor profesor = (await _context.TProfesors
                        .FirstOrDefaultAsync(p => p.Nif == nif))!;
                    return RedirectToAction("modificarAlumno", "TAlumnos", new {
                        nif = nif,
                        rol = rol 
                    });
                case constDefinidas.rolAlumno:
                    TAlumno alumno = (await _context.TAlumnos
                        .FirstOrDefaultAsync(a => a.Nif == nif))!;
                    return RedirectToAction("modificarProfesor", "TProfesores", new {
                        nif = nif,
                        rol = rol 
                    });
                case constDefinidas.rolMedico:
                    TAlumno medico = (await _context.TAlumnos
                        .FirstOrDefaultAsync(m => m.Nif == nif))!;
                    return RedirectToAction("modificarMedico", "TMedicos", new {
                        nif = nif, 
                        rol = rol 
                    });                
            }
            return RedirectToAction("volverPerfil", "TUsuarios", new {
                nif = nif,
                rol = rol
            });
        }
    }
}
