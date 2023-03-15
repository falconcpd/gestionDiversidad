using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TPantalla
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<TPermiso> TPermisos { get; } = new List<TPermiso>();
}
