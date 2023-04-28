using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TMedicos
{
    public class BorrarMedicoView
    {
        public TMedico Medico { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}
