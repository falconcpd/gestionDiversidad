using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace gestionDiversidad.Models;

public partial class TfgContext : DbContext
{
    public TfgContext()
    {
    }

    public TfgContext(DbContextOptions<TfgContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAdministracion> TAdministracions { get; set; }

    public virtual DbSet<TAlumno> TAlumnos { get; set; }

    public virtual DbSet<TAsignatura> TAsignaturas { get; set; }

    public virtual DbSet<TAuditorium> TAuditoria { get; set; }

    public virtual DbSet<TInforme> TInformes { get; set; }

    public virtual DbSet<TMedico> TMedicos { get; set; }

    public virtual DbSet<TPantalla> TPantallas { get; set; }

    public virtual DbSet<TPermiso> TPermisos { get; set; }

    public virtual DbSet<TProfesor> TProfesors { get; set; }

    public virtual DbSet<TRol> TRols { get; set; }

    public virtual DbSet<TUsuario> TUsuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=FATALIS_BLANCO\\SQLEXPRESS;Initial Catalog=tfg;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TAdministracion>(entity =>
        {
            entity.HasKey(e => e.Nif);

            entity.ToTable("tAdministracion");

            entity.Property(e => e.Nif)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NIF");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido2");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.NifNavigation).WithOne(p => p.TAdministracion)
                .HasForeignKey<TAdministracion>(d => d.Nif)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tAdministracion_tUsuario");
        });

        modelBuilder.Entity<TAlumno>(entity =>
        {
            entity.HasKey(e => e.Nif);

            entity.ToTable("tAlumno");

            entity.Property(e => e.Nif)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NIF");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido2");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.NifNavigation).WithOne(p => p.TAlumno)
                .HasForeignKey<TAlumno>(d => d.Nif)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tAlumno_tUsuario");

            entity.HasMany(d => d.IdAsignaturas).WithMany(p => p.NifAlumnos)
                .UsingEntity<Dictionary<string, object>>(
                    "TMatricula",
                    r => r.HasOne<TAsignatura>().WithMany()
                        .HasForeignKey("IdAsignatura")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_tMatricula_tAsignatura"),
                    l => l.HasOne<TAlumno>().WithMany()
                        .HasForeignKey("NifAlumno")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_tMatricula_tAlumno"),
                    j =>
                    {
                        j.HasKey("NifAlumno", "IdAsignatura");
                        j.ToTable("tMatricula");
                        j.IndexerProperty<string>("NifAlumno")
                            .HasMaxLength(50)
                            .IsUnicode(false)
                            .HasColumnName("NIF_alumno");
                        j.IndexerProperty<int>("IdAsignatura").HasColumnName("id_asignatura");
                    });
        });

        modelBuilder.Entity<TAsignatura>(entity =>
        {
            entity.ToTable("tAsignatura");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasMany(d => d.NifProfesors).WithMany(p => p.IdAsignaturas)
                .UsingEntity<Dictionary<string, object>>(
                    "TDocencium",
                    r => r.HasOne<TProfesor>().WithMany()
                        .HasForeignKey("NifProfesor")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_tDocencia_tProfesor"),
                    l => l.HasOne<TAsignatura>().WithMany()
                        .HasForeignKey("IdAsignatura")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_tDocencia_tAsignatura"),
                    j =>
                    {
                        j.HasKey("IdAsignatura", "NifProfesor");
                        j.ToTable("tDocencia");
                        j.IndexerProperty<int>("IdAsignatura").HasColumnName("id_asignatura");
                        j.IndexerProperty<string>("NifProfesor")
                            .HasMaxLength(50)
                            .IsUnicode(false)
                            .HasColumnName("NIF_profesor");
                    });
        });

        modelBuilder.Entity<TAuditorium>(entity =>
        {
            entity.ToTable("tAuditoria");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("accion");
            entity.Property(e => e.FechaHora)
                .HasColumnType("datetime")
                .HasColumnName("fecha_hora");
            entity.Property(e => e.NifUsuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NIF_usuario");
            entity.Property(e => e.Pantalla).HasColumnName("pantalla");

            entity.HasOne(d => d.PantallaNavigation).WithMany(p => p.TAuditoria)
                .HasForeignKey(d => d.Pantalla)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tAuditoria_tPantalla");
        });

        modelBuilder.Entity<TInforme>(entity =>
        {
            entity.HasKey(e => new { e.NifMedico, e.NifAlumno, e.Fecha });

            entity.ToTable("tInforme");

            entity.Property(e => e.NifMedico)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NIF_medico");
            entity.Property(e => e.NifAlumno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NIF_alumno");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Contenido).HasColumnName("contenido");

            entity.HasOne(d => d.NifAlumnoNavigation).WithMany(p => p.TInformes)
                .HasForeignKey(d => d.NifAlumno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tInforme_tAlumno");

            entity.HasOne(d => d.NifMedicoNavigation).WithMany(p => p.TInformes)
                .HasForeignKey(d => d.NifMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tInforme_tMedico");
        });

        modelBuilder.Entity<TMedico>(entity =>
        {
            entity.HasKey(e => e.Nif);

            entity.ToTable("tMedico");

            entity.Property(e => e.Nif)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NIF");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido2");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.NifNavigation).WithOne(p => p.TMedico)
                .HasForeignKey<TMedico>(d => d.Nif)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tMedico_tUsuario");
        });

        modelBuilder.Entity<TPantalla>(entity =>
        {
            entity.ToTable("tPantalla");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TPermiso>(entity =>
        {
            entity.HasKey(e => new { e.IdPantalla, e.IdRol }).HasName("PK_tPermiso_1");

            entity.ToTable("tPermiso");

            entity.Property(e => e.IdPantalla).HasColumnName("id_pantalla");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Acceder).HasColumnName("acceder");
            entity.Property(e => e.Borrar).HasColumnName("borrar");
            entity.Property(e => e.Insertar).HasColumnName("insertar");
            entity.Property(e => e.Modificar).HasColumnName("modificar");

            entity.HasOne(d => d.IdPantallaNavigation).WithMany(p => p.TPermisos)
                .HasForeignKey(d => d.IdPantalla)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tPermiso_tPantalla");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.TPermisos)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tPermiso_tRol");
        });

        modelBuilder.Entity<TProfesor>(entity =>
        {
            entity.HasKey(e => e.Nif);

            entity.ToTable("tProfesor");

            entity.Property(e => e.Nif)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NIF");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido2");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.NifNavigation).WithOne(p => p.TProfesor)
                .HasForeignKey<TProfesor>(d => d.Nif)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tProfesor_tUsuario");
        });

        modelBuilder.Entity<TRol>(entity =>
        {
            entity.ToTable("tRol");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TUsuario>(entity =>
        {
            entity.HasKey(e => e.Nif);

            entity.ToTable("tUsuario");

            entity.Property(e => e.Nif)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NIF");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Usuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usuario");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.TUsuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tUsuario_tRol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
