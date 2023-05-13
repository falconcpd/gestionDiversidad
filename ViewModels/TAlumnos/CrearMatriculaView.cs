using gestionDiversidad.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels.TAlumnos
{
    public class CrearMatriculaView
    {
        public List<TAlumno>? LAlumnos { get; set; }
        public List<TAsignatura>? LAsignaturas { get; set; }
        [Required(ErrorMessage = "Debes elegir una asignatura.")]
        [Remote(action: "verificarAsignatura", controller: "TAsignaturas", ErrorMessage = "La asignatura no ha sido encontrada: Por favor, elige una asignatura de la lista")]
        public string IdAsignatura { get; set; } = null!;
        [Required(ErrorMessage = "Debes elegir un alumno")]
        [Remote(action: "verificarAlumno", controller: "TUsuarios", ErrorMessage = "EL/La alumno/a no ha sido encontrado/a: Por favor, elige un/a alumno/a de la lista")]
        public string NifAlumno { get; set; } = null!;
    }
}
