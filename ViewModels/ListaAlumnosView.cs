﻿using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class ListaAlumnosView
    {
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso Alumno { get; set; } = null!;
        public List<TAlumno> ListaAlumnos { get; set; } = null!;
        public string Nif { get; set; } = null!;
        public int Rol { get; set; }
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
    }
}

