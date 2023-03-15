using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TInforme
{
    public string NifMedico { get; set; } = null!;

    public string NifAlumno { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public byte[] Contenido { get; set; } = null!;

    public virtual TAlumno NifAlumnoNavigation { get; set; } = null!;

    public virtual TMedico NifMedicoNavigation { get; set; } = null!;
}
