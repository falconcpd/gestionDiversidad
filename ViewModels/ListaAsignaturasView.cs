using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class ListaAsignaturasView
    {
        public TPermiso Permiso { get; set; } = null!;
        public List<TAsignatura> ListaAsignaturas { get; set; } = null!;
        public string Nif { get; set; } = null!;
        public int Rol { get; set; }
        public int SesionRol { get; set; }
        public string SesionNif { get;set; } = null!;
    }
}
