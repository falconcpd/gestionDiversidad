using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TRol
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<TPermiso> TPermisos { get; set; } = new List<TPermiso>();

    public virtual ICollection<TUsuario> TUsuarios { get; set; } = new List<TUsuario>();
}
