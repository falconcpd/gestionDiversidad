using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TAlumnos
{
    public class ListaMatriculasView
    {
        public List<TAlumno> LAlumnos { get; set; } = null!;
        public string SesionNif { get; set; } = null!;
        public int SesionRol { get; set; }
        public TPermiso Pantalla { get; set; } = null!;
    }
}
