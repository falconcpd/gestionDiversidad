namespace gestionDiversidad.ViewModels.TInformes
{
    public class BorrarInformeView
    {
        public string NifAlumno { get; set; } = null!;
        public string NifMedico { get; set; } = null!;
        public string Fecha { get; set; } = null!;
        public int ActualRol { get; set; }
        public string ActualNif { get; set; } = null!;
    }
}
