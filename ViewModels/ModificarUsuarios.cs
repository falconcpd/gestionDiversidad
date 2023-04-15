using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels
{
    public class ModificarUsuarios
    {
        public string Nif { get; set; } = null!;
        public int Rol { get; set; }
        [Required(ErrorMessage = "El nombre no puede ser vacío.")]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "El primer apellido no puede ser vacío.")]
        public string Apellido1 { get; set; } = null!;
        [Required(ErrorMessage = "El segundo no puede ser vacío.")]
        public string Apellido2 { get; set; } = null!;
    }
}
