using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TProfesores
{
    public class BorrarDocenciaView
    {
        public TProfesor Profesor { get; set; } = null!;
        public TAsignatura Asignatura { get; set; } = null!;
    }
}
