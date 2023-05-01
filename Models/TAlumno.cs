using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TAlumno
{
    public string Nif { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido1 { get; set; } = null!;

    public string Apellido2 { get; set; } = null!;

    public virtual TUsuario NifNavigation { get; set; } = null!;

    public virtual ICollection<TInforme> TInformes { get; set; } = new List<TInforme>();

    public virtual ICollection<TAsignatura> IdAsignaturas { get; set; } = new List<TAsignatura>();
}
