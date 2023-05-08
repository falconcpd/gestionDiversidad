﻿using gestionDiversidad.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels.TAlumnos
{
    public class CrearAlumnoView
    {
        [Required(ErrorMessage = "El Nif no puede estar vacío")]
        [Remote(action: "verificarNif", controller: "TUsuarios", ErrorMessage = "El NIF ya está en uso")]
        public string? Nif { get; set; }
        [Required(ErrorMessage = "El Usuario no puede estar vacío")]
        [Remote(action: "verificarCrearNombreUsuario", controller: "TUsuarios", ErrorMessage = "El nombre del usuario ya está en uso")]
        public string? Usuario { get; set; }
        [Required(ErrorMessage = "La contraseña no puede estar vacía")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "La confirmación de la contraseña no puede estar vacía")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }
        [Required(ErrorMessage = "El nombre no puede estar vacío")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "El primer apellido no puede estar vacío")]
        public string? Apellido1 { get; set; }
        [Required(ErrorMessage = "El segundo apellido no puede estar vacío")]
        public string? Apellido2 { get; set; }
        [Required(ErrorMessage = "El alumno necesita un medico que lleve su informe")]
        public string MedicoNif { get; set; } = null!;
        [Required(ErrorMessage = "Un alumno no se puede almacenar sin un informe")]
        public IFormFile PDF { get; set; } = null!;
        public List<TMedico>? ListaMedicos { get; set; }
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}
