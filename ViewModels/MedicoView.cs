using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class MedicoView
    {
        public TMedico Medico { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso LAlumnos { get; set; } = null!;
        public TPermiso LInformes { get; set; } = null!;
        public int Rol { get; set; }
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
        public int PadreRol { get; set; }
        public string PadreNif { get; set; } = null!;
    }
}
