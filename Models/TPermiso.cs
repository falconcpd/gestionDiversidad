using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TPermiso
{
    public int IdPantalla { get; set; }

    public int IdRol { get; set; }

    public bool Insertar { get; set; }

    public bool Modificar { get; set; }

    public bool Borrar { get; set; }

    public bool Acceder { get; set; }

    public virtual TPantalla IdPantallaNavigation { get; set; } = null!;

    public virtual TRol IdRolNavigation { get; set; } = null!;
}
