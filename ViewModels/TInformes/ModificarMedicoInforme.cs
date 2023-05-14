using gestionDiversidad.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gestionDiversidad.ViewModels.TInformes
{
    public class ModificarMedicoInforme
    {
        public TAlumno Alumno { get; set; } = null!;
        public TMedico Medico { get; set; } = null!;
        public string Fecha { get; set; } = null!;
        public List<TMedico> ListaMedicos { get; set; } = null!;
        [Required(ErrorMessage = "El alumno necesita un médico que lleve su informe")]
        [Remote(action: "verificarMedico", controller: "TUsuarios", ErrorMessage = "EL médico no ha sido encontrado: Por favor, elige un médico de la lista")]
        public string MedicoNif { get; set; } = null!;

    }
}
