using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels.TAsignaturas
{
    public class ModificarAsignatura
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre no puede estar vacío")]
        [Remote(action: "verificarModificarAsignatura", controller: "TAsignaturas", AdditionalFields = nameof(Id), ErrorMessage = "El nombre de esa asignatura ya está en uso")]
        public string Nombre { get; set; } = null!;
    }
}
