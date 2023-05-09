using gestionDiversidad.Models;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels.TAsignaturas
{
    public class CrearAsignaturaView
    {
        [Required(ErrorMessage = "El nombre no puede estar vacío")]
        public string Nombre { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}
