using gestionDiversidad.Models;
using System.Collections.Generic;

namespace gestionDiversidad.Interfaces
{
    public interface IServiceController
    {
        Task<List<TPermiso>> permisosRol(int rol);
        Task<TPermiso> permisoPantalla(int pantalla, int rol);
        Task<List<TAsignatura>> listaAsignaturas(string nif, int rol);
        Task<List<TAlumno>> listaAlumnos(string nif, int rol);
        Task<List<TInforme>> listaInformes(string nif, int rol);
    }
}
