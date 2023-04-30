using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class AsignaturaView
    {
        public TAsignatura Asignatura { get; set; } = null!;
        public string ActualNif { get; set; } = null!;
        public int ActualRol;
        
    }
}
