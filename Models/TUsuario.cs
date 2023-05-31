using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TUsuario
{
    public string Nif { get; set; } = null!;

    public string Usuario { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdRol { get; set; }

    public virtual TRol IdRolNavigation { get; set; } = null!;

    public virtual TAdministracion? TAdministracion { get; set; }

    public virtual TAlumno? TAlumno { get; set; }

    public virtual TMedico? TMedico { get; set; }

    public virtual TProfesor? TProfesor { get; set; }
}
