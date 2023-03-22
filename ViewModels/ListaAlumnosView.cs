using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class ListaAlumnosView
    {
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso Alumno { get; set; } = null!;
        public List<TAlumno> ListaAlumnos { get; set; } = null!;
    }
}
