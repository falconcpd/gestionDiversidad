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

            switch (rol)
            {
                case constDefinidas.rolAlumno:
                    alumno = (await _context.TAlumnos.Include(u => u.TInformes).FirstOrDefaultAsync(u => u.Nif == nif))!;
                    informes = alumno.TInformes.ToList();
                    return informes;
                case constDefinidas.rolMedico:
                    medico = (await _context.TMedicos.Include(u => u.TInformes).FirstOrDefaultAsync(u => u.Nif == nif))!;
                    informes = medico.TInformes.ToList();
                    return informes;
                default:
                    return informes;
            }
        }
    }


}
