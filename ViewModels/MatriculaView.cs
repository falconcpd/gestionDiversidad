using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class MatriculaView
    {
        public List<TAlumno> LAlumnos { get; set; } = null!;
        public string SesionNif { get; set; } = null!;
        public int SesionRol { get; set; }
    }
}
