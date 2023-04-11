using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class DocenciaView
    {
        public List<TProfesor> LProfesores { get; set; } = null!;
        public string SesionNif { get; set; } = null!;
        public int SesionRol { get; set;} 
    }
}
