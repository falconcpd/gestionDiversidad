using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TPantalla
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<TAuditorium> TAuditoria { get; set; } = new List<TAuditorium>();

    public virtual ICollection<TPermiso> TPermisos { get; set; } = new List<TPermiso>();
}
