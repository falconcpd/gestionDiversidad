using gestionDiversidad.Models;
using gestionDiversidad.Navigation;

namespace gestionDiversidad.ViewModels
{
    public partial class AlumnoView
    {
        public TAlumno Alumno { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso LMatriculas { get; set; } = null!;
        public TPermiso LInformes { get; set; } = null!;
        public TPermiso LAlumnos { get; set; } = null!;
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
        public int? PadreRol { get; set; }
        public string? PadreNif { get; set; }
    }
}
