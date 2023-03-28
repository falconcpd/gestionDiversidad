using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class InformeView
    {
        public TInforme Informe { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public string Nif { get; set; } = null!;
        public int Rol;
        public int SesionRol { get; set; }
        public string SesionNif { get; set; } = null!;
        public IFormFile PDF { get; set; }
    }
}
