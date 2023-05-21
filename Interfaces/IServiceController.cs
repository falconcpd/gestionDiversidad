using gestionDiversidad.Models;
using gestionDiversidad.ViewModels;
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
        Task<List<TProfesor>> listaProfesores();
        Task<List<TMedico>> listaMedicos();
        Task<TInforme> buscarInforme(string nifAlumno, string nifMedico, string fecha);
        DateTime fechaPresente();
        Task<List<TAuditorium>> listaAuditorias();
        Task guardarCrearBorrarUsuarioAuditoria(string nifAutor, int pantalla, int accion, string nifUsuario);
        Task guardarModificarUsuarioAuditoria(string nifAutor, int pantalla, ModificarUsuarios model, TUsuario user);
        Task guardarCrearBorrarInformeAuditoria(string nifAutor, int pantalla, int accion, TInforme informe);
        Task guardarModificarMedicoInformeAuditoria(string nifAutor, int pantalla, string nifNuevoMedico, TInforme informe);
        Task guardarModificarInformeAuditoria(string nifAutor, int pantalla, TInforme informe);
        Task guardarCrearBorrarAsignaturaAuditoria(string nifAutor, int pantalla, int accion, string nombreAsignatura);
        Task guardarCrearBorrarMatriculaAuditoria(string nifAutor, int pantalla, int accion, string nifAlumno, string nombreAsignatura);
        Task guardarCrearBorrarDocenciaAuditoria(string nifAutor, int pantalla, int accion, string nifProfesor, string nombreAsignatura);
        Task<string> giveActualNombre(string nif, int rol);
        Task<bool> existeUsuario(string nif);
        Task<bool> existeDistintoUsuario(string nif, int rol);
        bool confirmarEstructura(string estructura);
        string separarIdentificador(string estructura);
        Task<bool> existeDistintaAsignatura(string estructura);
        string quitarEspacios(string palabra);
    }
}
