using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TAsignaturas
{
    public class CrearAsignaturaView
    {
        public TAsignatura? Asignatura { get; set; }
        public TInforme? Informe { get; set; }
        public TProfesor? Profesor { get; set; }
        public TUsuario? Usuario { get; set; }
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}
