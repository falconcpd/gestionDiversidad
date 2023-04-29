using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TInformes
{
    public class ListaInformesView
    {
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso Informe { get; set; } = null!;
        public List<TInforme> ListaInformes { get; set; } = null!;
        public int Rol;
        public string Nif { get; set; } = null!;
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
    }
}
