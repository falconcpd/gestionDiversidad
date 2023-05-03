using Microsoft.AspNetCore.Mvc;
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
        [Required(ErrorMessage = "El segundo apellido no puede ser vacío.")]
        public string Apellido2 { get; set; } = null!;
        [Required(ErrorMessage = "La contraseña no puede estar vacía.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required(ErrorMessage = "La confirmación de la contraseña no puede estar vacía.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
        [Required(ErrorMessage = "El Usuario no puede estar vacío.")]
        [Remote(action: "verificarModificarNombreUsuario", controller: "TUsuarios", AdditionalFields = nameof(Nif), ErrorMessage = "El nombre del usuario ya está en uso")]
        public string? Usuario { get; set; }
    }
}
