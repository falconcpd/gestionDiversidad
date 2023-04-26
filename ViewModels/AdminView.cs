using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class AdminView
    {
        public TAdministracion Admin { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso LAsignaturas { get; set; } = null!;
        public TPermiso LMatriculas { get; set; } = null!;
        public int Rol { get; set; }
        public string Nif { get; set; } = null!;
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
    }
}
