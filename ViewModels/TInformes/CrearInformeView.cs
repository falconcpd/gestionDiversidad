using gestionDiversidad.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels.TInformes
{
    public class CrearInformeView
    {
        [Required(ErrorMessage = "El informe necesita un medico que lo lleve")]
        public string MedicoNif { get; set; } = null!;
        [Required(ErrorMessage = "El informe necesita un alumno al que pertenecer")]
        public string AlumnoNif { get; set; } = null!;
        [Required(ErrorMessage = "Un informe necesita un pdf como contenido")]
        public IFormFile PDF { get; set; } = null!;
        public List<TMedico>? ListaMedicos { get; set; }
        public List<TAlumno>? ListaAlumnos { get; set; }
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}

