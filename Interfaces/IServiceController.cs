using gestionDiversidad.Models;
using System.Collections.Generic;

namespace gestionDiversidad.Interfaces
{
    public interface IServiceController
    {
        List<TPermiso> permisosRol(int rol);
        TPermiso permisoPantalla(int pantalla, int rol);
        List<TAsignatura> listaAsignaturas(string nif, int rol);
        List<TAlumno> listaAlumnos(string nif, int rol);
    }
}
