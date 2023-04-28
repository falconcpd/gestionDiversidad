using gestionDiversidad.Models;
using System.ComponentModel.DataAnnotations;
namespace gestionDiversidad.ViewModels.TProfesores
{
    public class CrearDocenciaView
    {

        public List<TProfesor>? LProfesores { get; set; }
        public List<TAsignatura>? LAsignaturas { get; set; }
        [Required(ErrorMessage = "Debes elegir un profesor.")]
        public string IdAsignatura { get; set; } = null!;
        [Required(ErrorMessage = "Debes elegir un alumno.")]
        public string NifProfesor { get; set; } = null!;
    }
}
