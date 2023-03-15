using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TAdministracion
{
    public string Nif { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido1 { get; set; } = null!;

    public string Apellido2 { get; set; } = null!;

    public virtual TUsuario NifNavigation { get; set; } = null!;
}
