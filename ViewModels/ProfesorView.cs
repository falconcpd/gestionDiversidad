﻿using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class ProfesorView
    {
        public TProfesor Profesor { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso LDocencias { get; set; } = null!;
        public TPermiso LAlumnos { get; set; } = null!;
    }
}
