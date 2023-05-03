using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TProfesores
{
    public class ProfesorView
    {
        public TProfesor Profesor { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso LDocencias { get; set; } = null!;
        public TPermiso LAlumnos { get; set; } = null!;
        public TPermiso LProfesores { get; set; } = null!;
        public int? PadreRol { get; set; }
        public string? PadreNif { get; set; }
    }
}
