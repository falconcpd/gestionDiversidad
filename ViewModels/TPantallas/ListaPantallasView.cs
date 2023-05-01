using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TPantallas
{
    public class ListaPantallasView
    {
        public List<TPantalla> ListaPantallas { get; set; } = null!;
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
    }
}
