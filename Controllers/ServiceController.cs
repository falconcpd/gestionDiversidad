using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Models;
using gestionDiversidad.ViewModels;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing.Text;
using gestionDiversidad.Interfaces;
using System.ComponentModel;
using gestionDiversidad.Constantes;
using System.Globalization;
using System.IO;
using System.Text;

namespace gestionDiversidad.Controllers
{
    public class ServiceController : Controller, IServiceController
    {
        private readonly TfgContext _context;

        public ServiceController(TfgContext context)
        {
            _context = context;
        }

        //Filtra Listas de permisos por roles
        public async Task<List<TPermiso>> permisosRol(int rol)
        {
            List<TPermiso> permisos;
            permisos = await _context.TPermisos.Where(p => p.IdRol == rol).ToListAsync();
            return permisos;

        }

        //Filtra Listas de permisos por roles y permisos
        public async Task<TPermiso> permisoPantalla(int pantalla, int rol)
        {
            List<TPermiso> r_permisos = await permisosRol(rol);
            TPermiso permiso = r_permisos.FirstOrDefault(p => p.IdPantalla == pantalla && p.IdRol == rol)!;
            
            return permiso;
        }

        //Función para buscar informes
        public async Task<TInforme> buscarInforme(string nifAlumno, string nifMedico, string fecha)
        {
            DateTime fechaTime = DateTime
                .ParseExact(fecha, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            return (await _context.TInformes
                .Include(t => t.NifAlumnoNavigation)
                .Include(t => t.NifMedicoNavigation)
                .FirstOrDefaultAsync(t => t.NifAlumno == nifAlumno && t.NifMedico == nifMedico && t.Fecha == fechaTime))!;
        }

        //Retorna una lista de Asignaturas según el rol y el nif 
        public async Task<List<TAsignatura>> listaAsignaturas(string nif, int rol)
        {
            //Cuidado, es perezoso, utilizar el include
            TAlumno alumno;
            TProfesor profesor;
            List<TAsignatura> asignaturas = new List<TAsignatura>();

            switch (rol)
            {
                case constDefinidas.rolAlumno:
                    alumno = (await _context.TAlumnos.Include(u => u.IdAsignaturas)
                        .FirstOrDefaultAsync(u => u.Nif == nif))!;
                    asignaturas = alumno.IdAsignaturas.ToList();
                    return asignaturas;
                case constDefinidas.rolProfesor:
                    profesor = (await _context.TProfesors.Include(u => u.IdAsignaturas)
                        .FirstOrDefaultAsync(u => u.Nif == nif))!;
                    asignaturas = profesor.IdAsignaturas.ToList();
                    return asignaturas;
                case constDefinidas.rolAdmin:
                    asignaturas = (await _context.TAsignaturas.ToListAsync());
                    return asignaturas;
                default: 
                    return asignaturas;

            }

        }

        //Retorna una lista de Alumnos según el rol 
        public async Task<List<TAlumno>> listaAlumnos(string nif, int rol)
        {
            //Cuidado, es perezoso, utilizar el include
            TAsignatura wakeAsignatura;
            TInforme wakeInforme;
            List<TAsignatura> asignaturas;
            List<TInforme> informes;
            List<TAlumno> alumnos = new List<TAlumno>();

            switch (rol)
            {
                case constDefinidas.rolProfesor:
                    asignaturas = await listaAsignaturas(nif, rol);
                    foreach (var asignatura in asignaturas)
                    {
                        wakeAsignatura = (await _context.TAsignaturas.Include(a => a.NifAlumnos)
                            .FirstOrDefaultAsync(a => a.Id == asignatura.Id))!;
                        foreach (var alumno in wakeAsignatura.NifAlumnos)
                        {
                            if (!alumnos.Any(a => a.Nif == alumno.Nif))
                            {
                                alumnos.Add(alumno);
                            }
                        }
                    }
                    return alumnos;
                case constDefinidas.rolMedico:
                    informes = await listaInformes(nif, rol);
                    foreach (var informe in informes)
                    {
                        wakeInforme = (await _context.TInformes.Include(i => i.NifAlumnoNavigation)
                            .FirstOrDefaultAsync(i => i.NifMedico.Equals(informe.NifMedico) && i.NifAlumno
                            .Equals(informe.NifAlumno)
                            && i.Fecha.Equals(informe.Fecha)))!;
                        if (!alumnos.Any(a => a.Nif == wakeInforme.NifAlumno))
                        {
                            alumnos.Add(wakeInforme.NifAlumnoNavigation);
                        }
                    }
                    return alumnos;
                case constDefinidas.rolAdmin:
                    alumnos = (await _context.TAlumnos
                        .Include(a => a.IdAsignaturas)
                        .ToListAsync());
                    return alumnos;
                default: 
                    return alumnos;
            }
        }
        //Retorna una lista de Informes según el rol
        public async Task<List<TInforme>> listaInformes(string nif, int rol)
        {
            TAlumno alumno;
            TMedico medico;
            List<TInforme> informes = new List<TInforme>();
            List<TInforme> wakeInformes = new List<TInforme>();

            switch (rol)
            {
                case constDefinidas.rolAlumno:
                    alumno = (await _context.TAlumnos
                        .Include(u => u.TInformes)
                        .FirstOrDefaultAsync(u => u.Nif == nif))!;
                    informes = alumno.TInformes.ToList();
                    foreach(var informe in informes)
                    {
                        string fechaFormateada = informe.Fecha
                        .ToString("yyyy-MM-ddTHH:mm:ss");
                        wakeInformes
                            .Add(await buscarInforme(informe.NifAlumno, informe.NifMedico, fechaFormateada));
                    }
                    return wakeInformes;
                case constDefinidas.rolMedico:
                    medico = (await _context.TMedicos
                        .Include(u => u.TInformes)
                        .FirstOrDefaultAsync(u => u.Nif == nif))!;
                    informes = medico.TInformes.ToList();
                    foreach (var informe in informes)
                    {
                        string fechaFormateada = informe.Fecha
                        .ToString("yyyy-MM-ddTHH:mm:ss");
                        wakeInformes
                            .Add(await buscarInforme(informe.NifAlumno, informe.NifMedico, fechaFormateada));
                    }
                    return wakeInformes;
                case constDefinidas.rolAdmin:
                    wakeInformes = (await _context.TInformes
                        .Include(i => i.NifMedicoNavigation)
                        .Include(i => i.NifAlumnoNavigation)
                        .OrderBy(i => i.NifAlumnoNavigation.Nombre).ToListAsync());
                    return wakeInformes;
                default:
                    return wakeInformes;
            }
        }

        //Retorna una lista de todos los profesores.
        //Esta función solo está disponible para un administrador.
        public async Task<List<TProfesor>> listaProfesores()
        {
            List<TProfesor> profesores = await _context.TProfesors
                .Include(p => p.NifNavigation)
                .Include(p => p.IdAsignaturas)
                .ToListAsync();

            return profesores;
        }

        //Retorna una lista de todos los médicos. 
        //Esta función solo está disponible para un administrador.
        public async Task<List<TMedico>> listaMedicos()
        {
            List<TMedico> medicos = await _context.TMedicos.ToListAsync();
            return medicos;
        }

        //Función que utilizo para la fecha de ahora
        public DateTime fechaPresente()
        {
            DateTime fechaActual = DateTime.Now;
            string fechaActualFormateada = fechaActual
                .ToString("yyyy-MM-ddTHH:mm:ss");
            DateTime fechaActualFinal = DateTime
                .ParseExact(fechaActualFormateada, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            return fechaActualFinal;
        }     

        //Guarda en la base de datos los cambios realizados que tengan que ver con la creacion y borrado de usuarios
        public async Task guardarCrearBorrarUsuarioAuditoria(string nifAutor, int pantalla, int accion, string nifUsuario)
        {
            DateTime fechaActual = fechaPresente();
            TUsuario usuario = (await _context.TUsuarios.FirstOrDefaultAsync(u => u.Nif == nifUsuario))!;

            switch (accion)
            {
                case constDefinidas.accionCrearElemento:
                    switch (usuario.IdRol)
                    {
                        case constDefinidas.rolAlumno:
                            var auditoriaCrearAlumno = new TAuditorium
                            {
                                NifUsuario = nifAutor,
                                Pantalla = pantalla,
                                FechaHora = fechaActual,
                                Accion = "Se ha creado un nuevo Usuario de tipo Alumno con NIF: " + nifUsuario
                            };
                            _context.Add(auditoriaCrearAlumno);
                            await _context.SaveChangesAsync();
                            break;
                        case constDefinidas.rolProfesor:
                            var auditoriaCrearProfesor = new TAuditorium
                            {
                                NifUsuario = nifAutor,
                                Pantalla = pantalla,
                                FechaHora = fechaActual,
                                Accion = "Se ha creado un nuevo Usuario de tipo Profesor con NIF: " + nifUsuario
                            };
                            _context.Add(auditoriaCrearProfesor);
                            await _context.SaveChangesAsync();
                            break;
                        case constDefinidas.rolMedico:
                            var auditoriaCrearMedico = new TAuditorium
                            {
                                NifUsuario = nifAutor,
                                Pantalla = pantalla,
                                FechaHora = fechaActual,
                                Accion = "Se ha creado un nuevo Usuario de tipo Medico con NIF: " + nifUsuario
                            };
                            _context.Add(auditoriaCrearMedico);
                            await _context.SaveChangesAsync();
                            break;
                    }
                    return;
                case constDefinidas.accionBorrarElemento:
                    switch (usuario.IdRol)
                    {
                        case constDefinidas.rolAlumno:
                            var auditoriaBorrarAlumno = new TAuditorium
                            {
                                NifUsuario = nifAutor,
                                Pantalla = pantalla,
                                FechaHora = fechaActual,
                                Accion = "Se ha borrado el Usuario de tipo Alumno con NIF: " + nifUsuario
                            };
                            _context.Add(auditoriaBorrarAlumno);
                            await _context.SaveChangesAsync();
                            break;
                        case constDefinidas.rolProfesor:
                            var auditoriaBorrarProfesor = new TAuditorium
                            {
                                NifUsuario = nifAutor,
                                Pantalla = pantalla,
                                FechaHora = fechaActual,
                                Accion = "Se ha borrado el Usuario de tipo Profesor con NIF: " + nifUsuario
                            };
                            _context.Add(auditoriaBorrarProfesor);
                            await _context.SaveChangesAsync();
                            break;
                        case constDefinidas.rolMedico:
                            var auditoriaBorrarMedico = new TAuditorium
                            {
                                NifUsuario = nifAutor,
                                Pantalla = pantalla,
                                FechaHora = fechaActual,
                                Accion = "Se ha borrado el Usuario de tipo Medico con NIF: " + nifUsuario
                            };
                            _context.Add(auditoriaBorrarMedico);
                            await _context.SaveChangesAsync();
                            break;
                    }
                    return;
            }
        }

        //Guarda en la base de datos los cambios realizados que tengan que ver con la modificación de los usuarios
        public async Task guardarModificarUsuarioAuditoria(string nifAutor, int pantalla, ModificarUsuarios model, TUsuario user)
        {
            DateTime fechaActual = fechaPresente();
            switch (user.IdRol)
            {
                case constDefinidas.rolAlumno:
                    string mensajeAlumno = "Los atributos del alumno con NIF: " + user.Nif + ", modificados son:" + "\n";
                    TAlumno alumno = (await _context.TAlumnos
                        .FirstOrDefaultAsync(a => a.Nif == user.Nif))!;
                    if (model.Usuario != user.Usuario)
                    {
                        mensajeAlumno += "Usuario anterior: " + user.Usuario + "; Usuario nuevo: " + model.Usuario + "\n";
                    }
                    if (model.Password != user.Password)
                    {
                        mensajeAlumno += "Password modificado" + "\n";
                    }
                    if (model.Nombre!= alumno.Nombre)
                    {
                        mensajeAlumno += "Nombre anterior: " + alumno.Nombre + "; Nombre nuevo: " + model.Nombre + "\n";

                    }
                    if (model.Apellido1 != alumno.Apellido1)
                    {
                        mensajeAlumno += "Primer apellido anterior: " + alumno.Apellido1 + "; Primer apellido nuevo: " + model.Apellido1 + "\n";
                    }
                    if (model.Apellido2 != alumno.Apellido2)
                    {
                        mensajeAlumno += "Segundo apellido anterior: " + alumno.Apellido2 + "; Segundo apellido nuevo: " + model.Apellido2;
                    }
                    mensajeAlumno = mensajeAlumno.Replace("\n", "<br/>");
                    var auditoriaModificarAlumno = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeAlumno
                    };
                    _context.Add(auditoriaModificarAlumno);
                    await _context.SaveChangesAsync();

                    return;

                case constDefinidas.rolProfesor:
                    string mensajeProfesor = "Los atributos del profesor con NIF: " + user.Nif + ", modificados son:" + "\n";
                    TProfesor profesor = (await _context.TProfesors
                        .FirstOrDefaultAsync(p => p.Nif == user.Nif))!;
                    if (model.Usuario != user.Usuario)
                    {
                        mensajeProfesor += "Usuario anterior: " + user.Usuario + "; Usuario nuevo: " + model.Usuario + "\n";
                    }
                    if (model.Password != user.Password)
                    {
                        mensajeProfesor += "Password modificado" + "\n";
                    }
                    if (model.Nombre != profesor.Nombre)
                    {
                        mensajeProfesor += "Nombre anterior: " + profesor.Nombre + "; Nombre nuevo: " + model.Nombre + "\n";

                    }
                    if (model.Apellido1 != profesor.Apellido1)
                    {
                        mensajeProfesor += "Primer apellido anterior: " + profesor.Apellido1 + "; Primer apellido nuevo: " + model.Apellido1 + "\n";
                    }
                    if (model.Apellido2 != profesor.Apellido2)
                    {
                        mensajeProfesor += "Segundo apellido anterior: " + profesor.Apellido2 + "; Segundo apellido nuevo: " + model.Apellido2;
                    }
                    mensajeProfesor = mensajeProfesor.Replace("\n", "<br/>");
                    var auditoriaModificarProfesor = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeProfesor
                    };
                    _context.Add(auditoriaModificarProfesor);
                    await _context.SaveChangesAsync();

                    return;

                case constDefinidas.rolMedico:
                    string mensajeMedico = "Los atributos del médico con NIF: " + user.Nif + ", modificados son:" + "\n";
                    TMedico medico = (await _context.TMedicos
                        .FirstOrDefaultAsync(a => a.Nif == user.Nif))!;
                    if (model.Usuario != user.Usuario)
                    {
                        mensajeMedico += "Usuario anterior: " + user.Usuario + "; Usuario nuevo: " + model.Usuario + "\n";
                    }
                    if (model.Password != user.Password)
                    {
                        mensajeMedico += "Password modificado" + "\n";
                    }
                    if (model.Nombre != medico.Nombre)
                    {
                        mensajeMedico += "Nombre anterior: " + medico.Nombre + "; Nombre nuevo: " + model.Nombre + "\n";

                    }
                    if (model.Apellido1 != medico.Apellido1)
                    {
                        mensajeMedico += "Primer apellido anterior: " + medico.Apellido1 + "; Primer apellido nuevo: " + model.Apellido1 + "\n";
                    }
                    if (model.Apellido2 != medico.Apellido2)
                    {
                        mensajeMedico += "Segundo apellido anterior: " + medico.Apellido2 + "; Segundo apellido nuevo: " + model.Apellido2;
                    }
                    mensajeMedico = mensajeMedico.Replace("\n", "<br/>");
                    var auditoriaModificarMedico = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeMedico
                    };
                    _context.Add(auditoriaModificarMedico);
                    await _context.SaveChangesAsync();

                    return;
            }
            
            return;

        }

        //Guarda en la base de datos los cambios realizados que tengan que ver con la creacion o borrado de los informes
        public async Task guardarCrearBorrarInformeAuditoria(string nifAutor, int pantalla, int accion, TInforme informe)
        {
            string fechaFormateada = informe.Fecha
                .ToString("dd/MM/yyyy HH:mm:ss");
            DateTime fechaActual = fechaPresente();

            switch (accion)
            {
                case constDefinidas.accionCrearElemento:
                    string mensajeCreacion = "Nuevo informe creado con los siguientes atributos: \n";
                    mensajeCreacion += "Nif Alumno: " + informe.NifAlumno + "\n";
                    mensajeCreacion += "Nif Médico: " + informe.NifMedico + "\n";
                    mensajeCreacion += "Fecha Creación: " + fechaFormateada + "\n";

                    mensajeCreacion = mensajeCreacion.Replace("\n", "<br/>");
                    var auditoriaCrearInforme = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeCreacion
                    };
                    _context.Add(auditoriaCrearInforme);
                    await _context.SaveChangesAsync();
                    return;
                case constDefinidas.accionBorrarElemento:
                    string mensajeBorrado = "Un informe con los siguientes atributos ha sido borrado : \n";
                    mensajeBorrado += "Nif Alumno: " + informe.NifAlumno + "\n";
                    mensajeBorrado += "Nif Médico: " + informe.NifMedico + "\n";
                    mensajeBorrado += "Fecha Creación: " + fechaFormateada + "\n";

                    mensajeBorrado = mensajeBorrado.Replace("\n", "<br/>");
                    var auditoriaBorrarInforme = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeBorrado
                    };
                    _context.Add(auditoriaBorrarInforme);
                    await _context.SaveChangesAsync();
                    return;
            }

        }

        //Guarda en la base de datos los cambios realizados que tengan que ver con la modificacion del médico de los informes
        public async Task guardarModificarMedicoInformeAuditoria(string nifAutor, int pantalla, string nifNuevoMedico, TInforme informe)
        {
            string fechaFormateada = informe.Fecha
                .ToString("dd/MM/yyyy HH:mm:ss");
            DateTime fechaActual = fechaPresente();

            string mensajeModificacion = "Cambio de médico en el informe con NIF alumno: " + informe.NifAlumno + ", y fecha: " + fechaFormateada + "\n";
            mensajeModificacion += "NIF del anterior médico: " + informe.NifMedico + "; NIF del médico actual: " + nifNuevoMedico;

            mensajeModificacion = mensajeModificacion.Replace("\n", "<br/>");
            var auditoriaModificarMedicoInforme = new TAuditorium
            {
                NifUsuario = nifAutor,
                Pantalla = pantalla,
                FechaHora = fechaActual,
                Accion = mensajeModificacion
            };
            _context.Add(auditoriaModificarMedicoInforme);
            await _context.SaveChangesAsync();
            return;

        }

        //Guarda en la base de datos los cambios realizados que tengan que ver con la modificacion del contenido de un informe
        public async Task guardarModificarInformeAuditoria(string nifAutor, int pantalla, TInforme informe)
        {
            string fechaFormateada = informe.Fecha
                .ToString("dd/MM/yyyy HH:mm:ss");
            DateTime fechaActual = fechaPresente();

            string mensajeModificacion = "Cambio del contenido del informe con NIF de alumno: " + informe.NifAlumno + ", NIF de médico: " + informe.NifMedico + "\n"; 
            mensajeModificacion += " y fecha: " + fechaFormateada + "\n";
            mensajeModificacion = mensajeModificacion.Replace("\n", "<br/>");
            var auditoriaModificarMedicoInforme = new TAuditorium
            {
                NifUsuario = nifAutor,
                Pantalla = pantalla,
                FechaHora = fechaActual,
                Accion = mensajeModificacion
            };
            _context.Add(auditoriaModificarMedicoInforme);
            await _context.SaveChangesAsync();
            return;

        }

        //Guarda en la base de datos los cambios realizados que tengan que ver con la creacion o borrado de las asignaturas
        public async Task guardarCrearBorrarAsignaturaAuditoria(string nifAutor, int pantalla, int accion, string nombreAsignatura)
        {
            DateTime fechaActual = fechaPresente();

            switch (accion)
            {
                case constDefinidas.accionCrearElemento:
                    string mensajeCreacion = "Nueva asignatura creada con nombre: " + nombreAsignatura;

                    var auditoriaCrearAsignatura = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeCreacion
                    };
                    _context.Add(auditoriaCrearAsignatura);
                    await _context.SaveChangesAsync();
                    return;
                case constDefinidas.accionBorrarElemento:
                    string mensajeBorrado = "Asignatura borrada con nombre: " + nombreAsignatura;

                    var auditoriaBorrarAsignatura = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeBorrado
                    };
                    _context.Add(auditoriaBorrarAsignatura);
                    await _context.SaveChangesAsync();
                    return;
            }

        }

        //Guarda en la base de datos los cambios realizados que tengan que ver con la creacion o borrado de las matrículas
        public async Task guardarCrearBorrarMatriculaAuditoria(string nifAutor, int pantalla, int accion, string nifAlumno, string nombreAsignatura)
        {
            DateTime fechaActual = fechaPresente();

            switch (accion)
            {
                case constDefinidas.accionCrearElemento:
                    string mensajeCreacion = "Nueva matrícula con NIF alumno: " + nifAlumno + "\n";
                    mensajeCreacion += "y asigantura: " + nombreAsignatura;

                    var auditoriaCrearMatricula = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeCreacion
                    };
                    _context.Add(auditoriaCrearMatricula);
                    await _context.SaveChangesAsync();
                    return;
                case constDefinidas.accionBorrarElemento:
                    string mensajeBorrado = "Matrícula borrada entre el alumno con NIF: " + nifAlumno + "\n";
                    mensajeBorrado += "y la asigantura: " + nombreAsignatura;

                    var auditoriaBorrarMatricula = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeBorrado
                    };
                    _context.Add(auditoriaBorrarMatricula);
                    await _context.SaveChangesAsync();
                    return;
            }

        }

        //Guarda en la base de datos los cambios realizados que tengan que ver con la creacion o borrado de las docencias
        public async Task guardarCrearBorrarDocenciaAuditoria(string nifAutor, int pantalla, int accion, string nifProfesor, string nombreAsignatura)
        {
            DateTime fechaActual = fechaPresente();

            switch (accion)
            {
                case constDefinidas.accionCrearElemento:
                    string mensajeCreacion = "Nueva docencia con NIF profesor: " + nifProfesor + "\n";
                    mensajeCreacion += "y asigantura: " + nombreAsignatura;

                    var auditoriaCrearDocencia = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeCreacion
                    };
                    _context.Add(auditoriaCrearDocencia);
                    await _context.SaveChangesAsync();
                    return;
                case constDefinidas.accionBorrarElemento:
                    string mensajeBorrado = "Docencia borrada entre el profesor con NIF: " + nifProfesor + "\n";
                    mensajeBorrado += "y la asigantura: " + nombreAsignatura;

                    var auditoriaBorrarDocencia = new TAuditorium
                    {
                        NifUsuario = nifAutor,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = mensajeBorrado
                    };
                    _context.Add(auditoriaBorrarDocencia);
                    await _context.SaveChangesAsync();
                    return;
            }

        }

        //Retorna una lista de todos las auditorias.
        //Esta función solo está disponible para un administrador.
        public async Task<List<TAuditorium>> listaAuditorias()
        {
            List<TAuditorium> auditorias = await _context.TAuditoria
                .Include(a => a.PantallaNavigation)
                .OrderByDescending(a => a.FechaHora)
                .ToListAsync();

            return auditorias;
        }
    }


}
