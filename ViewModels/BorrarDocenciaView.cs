using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class BorrarDocenciaView
    {
        public TProfesor Profesor { get; set; } = null!;
        public TAsignatura Asignatura { get; set; } = null!;
    }
}
