using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TAdministraciones
{
    public class AdminView
    {
        public TAdministracion Admin { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso LAsignaturas { get; set; } = null!;
        public TPermiso LMatriculas { get; set; } = null!;
        public TPermiso LMedicos { get; set; } = null!;
        public TPermiso LProfesores { get; set; } = null!;
        public TPermiso LAlumnos { get; set; } = null!;
        public TPermiso LInformes { get; set; } = null!;
        public TPermiso LDocencias { get; set; } = null!;
        public TPermiso LPantallas { get; set; } = null!;
        public int Rol { get; set; }
        public string Nif { get; set; } = null!;
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
    }
}
