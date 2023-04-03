using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels
{
    
    public class CrearProfesorView
    {
        [Required(ErrorMessage ="El Nif no puede estar vacío.")]
        [Remote(action: "verificarNif", controller: "Tusuarios")]
        public string? Nif { get; set; }
        [Required(ErrorMessage = "El Usuario no puede estar vacío.")]
        public string? Usuario { get; set;}
        [Required(ErrorMessage = "La contraseña no puede estar vacía.")]
        public string? Password { get; set;}
        [Required(ErrorMessage = "El nombre no puede estar vacío.")]
        public string? Nombre { get; set;}
        [Required(ErrorMessage = "El primer apellido no puede estar vacío.")]
        public string? Apellido1 { get; set;}
        [Required(ErrorMessage = "El segundo apellido no puede estar vacío.")]
        public string? Apellido2 { get;set;}
        public string? NifCreador { get; set;}
        public int RolCreador { get; set;}
    }
}
