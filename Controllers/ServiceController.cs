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

        //Guarda en la base de datos los cambios realizados
        public async Task guardarAuditoria(string nif, int pantalla, int accion)
        {
            DateTime fechaActual = fechaPresente();

            switch (accion)
            {
                case constDefinidas.accionCrear:
                    var auditoriaCrear = new TAuditorium
                    {
                        NifUsuario = nif,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = "Crear nuevo elemento"
                    };
                    _context.Add(auditoriaCrear);
                    await _context.SaveChangesAsync();

                    return;
                case constDefinidas.accionBorrar:
                    var auditoriaBorrar = new TAuditorium
                    {
                        NifUsuario = nif,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = "Borrar un elemento"
                    };
                    _context.Add(auditoriaBorrar);
                    await _context.SaveChangesAsync();

                    return;
                case constDefinidas.accionModificar:
                    var auditoriaModificar = new TAuditorium
                    {
                        NifUsuario = nif,
                        Pantalla = pantalla,
                        FechaHora = fechaActual,
                        Accion = "Modificar un elemento"
                    };
                    _context.Add(auditoriaModificar);
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
