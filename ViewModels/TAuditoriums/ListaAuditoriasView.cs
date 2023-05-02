using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TAuditoriums
{
    public class ListaAuditoriasView
    {
        public TPermiso Permiso { get; set; } = null!;
        public List<TAuditorium> ListaAuditorias { get; set; } = null!;
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
    }
}
