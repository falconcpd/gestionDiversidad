using gestionDiversidad.Models;

namespace gestionDiversidad.ViewModels.TInformes
{
    public class InformeView
    {
        public TInforme Informe { get; set; } = null!;
        public TPermiso Permiso { get; set; } = null!;
        public TAlumno Alumno { get; set; } = null!;
        public TMedico Medico { get; set; } = null!;
        public IFormFile PDF { get; set; } = null!;
    }
}
