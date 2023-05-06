using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TAsignaturas
{
    public class ListaAsignaturasView
    {
        public TPermiso Permiso { get; set; } = null!;
        public List<TAsignatura> ListaAsignaturas { get; set; } = null!;
        public string ActualNif { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualName { get; set; } = null!;

    }
}
