using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class AsignaturaView
    {
        public TAsignatura Asignatura { get; set; } = null!;
        public string Nif { get; set; } = null!;
        public int Rol;
        
    }
}
