using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TAuditorium
{
    public int Id { get; set; }

    public string NifUsuario { get; set; } = null!;

    public int Pantalla { get; set; }

    public string Accion { get; set; } = null!;

    public DateTime FechaHora { get; set; }

    public virtual TUsuario NifUsuarioNavigation { get; set; } = null!;

    public virtual TPantalla PantallaNavigation { get; set; } = null!;
}
