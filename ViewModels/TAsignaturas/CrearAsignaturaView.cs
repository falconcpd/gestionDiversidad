using gestionDiversidad.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels.TAsignaturas
{
    public class CrearAsignaturaView
    {
        [Required(ErrorMessage = "El nombre no puede estar vacío")]
        [Remote(action: "verificarNombreAsignatura", controller: "TAsignaturas", ErrorMessage = "El nombre de esa asignatura ya está en uso")]
        public string Nombre { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}
