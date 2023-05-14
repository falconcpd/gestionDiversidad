using gestionDiversidad.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace gestionDiversidad.ViewModels.TProfesores
{
    public class CrearDocenciaView
    {

        public List<TProfesor>? LProfesores { get; set; }
        public List<TAsignatura>? LAsignaturas { get; set; }
        [Required(ErrorMessage = "Debes elegir una asignatura")]
        [Remote(action: "verificarAsignatura", controller: "TAsignaturas", ErrorMessage = "La asignatura no ha sido encontrada: Por favor, elige una asignatura de la lista")]
        public string IdAsignatura { get; set; } = null!;
        [Required(ErrorMessage = "Debes elegir un/a profesor/a")]
        [Remote(action: "verificarProfesor", controller: "TUsuarios", ErrorMessage = "EL/La profesor/a no ha sido encontrado/a: Por favor, elige un/a profesor/a de la lista")]
        public string NifProfesor { get; set; } = null!;
    }
}
