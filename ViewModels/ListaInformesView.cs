using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class ListaInformesView
    {
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso Informe { get; set; } = null!;
        public List<TInforme> ListaInformes { get; set; } = null!;
    }
}
