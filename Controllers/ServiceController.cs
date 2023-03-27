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
        public List<TPermiso> permisosRol(int rol)
        {
            List<TPermiso> permisos;
            permisos = _context.TPermisos.Where(p => p.IdRol == rol).ToList();
            return permisos;

        }

        //Filtra Listas de permisos por roles y permisos
        public TPermiso permisoPantalla(int pantalla, int rol)
        {
            List<TPermiso> r_permisos = permisosRol(rol);
            TPermiso permiso = r_permisos.FirstOrDefault(p => p.IdPantalla == pantalla && p.IdRol == rol);

            return permiso;
        }

        //Retorna una lista de Asignaturas según el rol 
        public List<TAsignatura> listaAsignaturas(string nif, int rol)
        {
            //Cuidado, es perezoso, utilizar el include
            TAlumno alumno;
            TProfesor profesor;
            TMedico medico;
            List<TAsignatura> asignaturas = new List<TAsignatura>();

            if (rol == 1)
            {
                alumno = _context.TAlumnos.Include(u => u.IdAsignaturas).FirstOrDefault(u => u.Nif == nif);
                asignaturas = alumno.IdAsignaturas.ToList();
            }
            if (rol == 2)
            {
                profesor = _context.TProfesors.Include(u => u.IdAsignaturas).FirstOrDefault(u => u.Nif == nif);
                asignaturas = profesor.IdAsignaturas.ToList();
            }

            return asignaturas;

        }

        //Retorna una lista de Alumnos según el rol 
        public List<TAlumno> listaAlumnos(string nif, int rol)
        {
            //Cuidado, es perezoso, utilizar el include
            TProfesor profesor;
            TAsignatura wakeAsignatura;
            TInforme wakeInforme;
            List<TAsignatura> asignaturas;
            List<TInforme> informes;
            List<TAlumno> alumnos = new List<TAlumno>();

            if (rol == 2)
            {
                asignaturas = listaAsignaturas(nif, rol);
                foreach(var asignatura in asignaturas)
                {
                    wakeAsignatura = _context.TAsignaturas.Include(a => a.NifAlumnos).FirstOrDefault(a => a.Id == asignatura.Id);
                    foreach(var alumno in wakeAsignatura.NifAlumnos)
                    {
                        if(!alumnos.Any(a => a.Nif == alumno.Nif))
                        {
                            alumnos.Add(alumno);
                        }
                    }
                }
            }else if (rol == 3)
            {
                informes = listaInformes(nif, rol);
                foreach(var informe in informes)
                {
                    wakeInforme = _context.TInformes.Include(i => i.NifAlumnoNavigation)
                        .FirstOrDefault(i => i.NifMedico.Equals(informe.NifMedico) && i.NifAlumno.Equals(informe.NifAlumno)
                        && i.Fecha.Equals(informe.Fecha)); 
                    if(!alumnos.Any(a => a.Nif == wakeInforme.NifAlumno))
                    {
                        alumnos.Add(wakeInforme.NifAlumnoNavigation);
                    }
                }
            }


            return alumnos;
        }
        //Retorna una lista de Informes según el rol
        public List<TInforme> listaInformes(string nif, int rol)
        {
            TAlumno alumno;
            TMedico medico;
            List<TInforme> informes = null;

            if (rol == 1)
            {
                alumno = _context.TAlumnos.Include(u => u.TInformes).FirstOrDefault(u => u.Nif == nif);
                informes = alumno.TInformes.ToList();
            }else if(rol == 3)
            {
                medico = _context.TMedicos.Include(u => u.TInformes).FirstOrDefault(u => u.Nif == nif);
                informes = medico.TInformes.ToList();
            }

            return informes;
        }
    }
}
