using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels
{
    public class CrearView
    {
        public int Rol { get; set; }
        public string Nif { get; set; } = null!;
        public TAsignatura? Asignatura { get; set; }
        public TInforme? Informe { get; set; }
        public TProfesor? Profesor { get; set; }
        public TUsuario? Usuario { get; set; }
    }
}
