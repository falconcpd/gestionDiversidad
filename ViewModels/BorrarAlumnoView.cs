using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class BorrarAlumnoView
    {
        public TAlumno Alumno { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}
