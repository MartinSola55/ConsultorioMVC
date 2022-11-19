using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ConsultorioMVC
{
    public partial class EFModel : DbContext
    {
        public EFModel()
            : base("name=EFModel")
        {
        }

        public virtual DbSet<DiaHorario> DiaHorario { get; set; }
        public virtual DbSet<HistoriasClinicas> HistoriasClinicas { get; set; }
        public virtual DbSet<Horarios> Horarios { get; set; }
        public virtual DbSet<ObrasSociales> ObrasSociales { get; set; }
        public virtual DbSet<Pacientes> Pacientes { get; set; }
        public virtual DbSet<Personas> Personas { get; set; }
        public virtual DbSet<Turnos> Turnos { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiaHorario>()
                .HasMany(e => e.Turnos)
                .WithRequired(e => e.DiaHorario)
                .HasForeignKey(e => e.dia_horario_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HistoriasClinicas>()
                .Property(e => e.descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<Horarios>()
                .HasMany(e => e.DiaHorario)
                .WithRequired(e => e.Horarios)
                .HasForeignKey(e => e.horario_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ObrasSociales>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<ObrasSociales>()
                .HasMany(e => e.Personas)
                .WithRequired(e => e.ObrasSociales)
                .HasForeignKey(e => e.obra_social_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pacientes>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Pacientes>()
                .Property(e => e.apellido)
                .IsUnicode(false);

            modelBuilder.Entity<Pacientes>()
                .Property(e => e.telefono)
                .IsUnicode(false);

            modelBuilder.Entity<Pacientes>()
                .Property(e => e.direccion)
                .IsUnicode(false);

            modelBuilder.Entity<Pacientes>()
                .Property(e => e.localidad)
                .IsUnicode(false);

            modelBuilder.Entity<Pacientes>()
                .HasMany(e => e.HistoriasClinicas)
                .WithRequired(e => e.Pacientes)
                .HasForeignKey(e => e.paciente_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Personas>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Personas>()
                .Property(e => e.apellido)
                .IsUnicode(false);

            modelBuilder.Entity<Personas>()
                .Property(e => e.telefono)
                .IsUnicode(false);

            modelBuilder.Entity<Personas>()
                .Property(e => e.correo)
                .IsUnicode(false);

            modelBuilder.Entity<Personas>()
                .HasMany(e => e.Turnos)
                .WithRequired(e => e.Personas)
                .HasForeignKey(e => e.persona_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuarios>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<Usuarios>()
                .Property(e => e.password)
                .IsUnicode(false);
        }
    }
}
