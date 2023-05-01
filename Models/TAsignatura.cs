using System;
using System.Collections.Generic;

namespace gestionDiversidad.Models;

public partial class TAsignatura
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<TAlumno> NifAlumnos { get; set; } = new List<TAlumno>();

    public virtual ICollection<TProfesor> NifProfesors { get; set; } = new List<TProfesor>();
}
