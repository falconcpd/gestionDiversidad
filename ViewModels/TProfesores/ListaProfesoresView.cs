using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TProfesores
{
    public class ListaProfesoresView
    {
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso Profesor { get; set; } = null!;
        public List<TProfesor> ListaProfesores { get; set; } = null!;
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
    }
}
