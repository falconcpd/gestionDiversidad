using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TProfesores
{
    public class BorrarProfesorView
    {
        public TProfesor Profesor { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}
