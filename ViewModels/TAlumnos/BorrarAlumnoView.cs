using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TAlumnos
{
    public class BorrarAlumnoView
    {
        public TAlumno Alumno { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}
