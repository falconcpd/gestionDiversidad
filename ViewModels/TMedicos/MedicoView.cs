using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TMedicos
{
    public class MedicoView
    {
        public TMedico Medico { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso LAlumnos { get; set; } = null!;
        public TPermiso LInformes { get; set; } = null!;
        public TPermiso LMedicos { get; set; } = null!;
    }
}
