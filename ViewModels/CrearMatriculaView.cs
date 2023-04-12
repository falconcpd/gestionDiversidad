using gestionDiversidad.Models;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels
{
    public class CrearMatriculaView
    {
        public List<TAlumno>? LAlumnos { get; set; }
        public List<TAsignatura>? LAsignaturas { get; set; }
        [Required(ErrorMessage = "Debes elegir un alumno.")]
        public string IdAsignatura { get; set; } = null!;
        [Required(ErrorMessage = "Debes elegir un alumno.")]
        public string NifAlumno { get; set; } = null!;
    }
}
