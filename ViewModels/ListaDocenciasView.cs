using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class ListaDocenciasView
    {
        public List<TProfesor> LProfesores { get; set; } = null!;
        public string SesionNif { get; set; } = null!;
        public int SesionRol { get; set;}
        public TPermiso Pantalla { get; set; } = null!;
    }
}
