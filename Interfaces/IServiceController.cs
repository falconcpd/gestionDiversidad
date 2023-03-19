using gestionDiversidad.Models;
using System.Collections.Generic;

namespace gestionDiversidad.Interfaces
{
    public interface IServiceController
    {
        List<TPermiso> permisosRol(int rol);
        TPermiso permisoPantalla(int pantalla, int rol);
    }
}
