using gestionDiversidad.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels.TInformes
{
    public class CrearInformeView
    {
        [Required(ErrorMessage = "El alumno necesita un médico que lleve su informe")]
        [Remote(action: "verificarMedico", controller: "TUsuarios", ErrorMessage = "EL médico no ha sido encontrado: Por favor, elige un médico de la lista")]
        public string MedicoNif { get; set; } = null!;
        [Required(ErrorMessage = "Debes elegir un alumno")]
        [Remote(action: "verificarAlumno", controller: "TUsuarios", ErrorMessage = "EL/La alumno/a no ha sido encontrado/a: Por favor, elige un/a alumno/a de la lista")]
        public string NifAlumno { get; set; } = null!;
        [Required(ErrorMessage = "Un informe necesita un pdf como contenido")]
        public IFormFile PDF { get; set; } = null!;
        public List<TMedico>? ListaMedicos { get; set; }
        public List<TAlumno>? ListaAlumnos { get; set; }
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}

