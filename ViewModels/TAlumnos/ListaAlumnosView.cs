using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TAlumnos
{
    public class ListaAlumnosView
    {
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso Alumno { get; set; } = null!;
        public List<TAlumno> ListaAlumnos { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
        public string ActualName { get; set; } = null!;
    }
}

