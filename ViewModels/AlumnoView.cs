using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public partial class AlumnoView
    {
        public TAlumno Alumno { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso LMatriculas { get; set; } = null!;
        public TPermiso LInformes { get; set; } = null!;
        public int Rol { get; set; }

    }
}
