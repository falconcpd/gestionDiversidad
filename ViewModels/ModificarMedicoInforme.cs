﻿using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class ModificarMedicoInforme
    {
        public TAlumno Alumno { get; set; } = null!;
        public TMedico Medico { get; set; } = null!;
        public string Fecha { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
        public List<TMedico> ListaMedicos { get; set; } = null!;
        public string NuevoMedicoNif { get; set; } = null!;

    }
}