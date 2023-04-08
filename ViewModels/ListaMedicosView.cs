using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class ListaMedicosView
    {
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso Medico { get; set; } = null!;
        public List<TMedico> ListaMedicos { get; set; } = null!;
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
    }
}
