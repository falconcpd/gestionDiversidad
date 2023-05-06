using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TInformes
{
    public class ListaInformesView
    {
        public TPermiso Permiso { get; set; } = null!;
        public TPermiso Informe { get; set; } = null!;
        public List<TInforme> ListaInformes { get; set; } = null!;
        public string ActualName { get; set; } = null!;
        public int ActualRol;
        public string ActualNif { get; set; } = null!;

    }
}
