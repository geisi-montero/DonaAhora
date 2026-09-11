using DonaAhora.Models;
using Microsoft.EntityFrameworkCore;

namespace DonaAhora.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Donante> Donantes { get; set; } = null!;
        public DbSet<Solicitud> Solicitudes { get; set; } = null!;
        public DbSet<InteresDonacion> InteresesDonacion { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Rol).HasConversion<string>().HasMaxLength(20);
            });

            modelBuilder.Entity<Donante>(entity =>
            {
                entity.ToTable("Donantes");
                entity.HasIndex(d => d.UsuarioId).IsUnique();
                entity.Property(d => d.TipoSangre).HasConversion<string>().HasMaxLength(10);
                entity.HasOne(d => d.Usuario)
                      .WithOne(u => u.Donante)
                      .HasForeignKey<Donante>(d => d.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Solicitud>(entity =>
            {
                entity.ToTable("Solicitudes");
                entity.Property(s => s.TipoSangre).HasConversion<string>().HasMaxLength(10);
                entity.Property(s => s.Urgencia).HasConversion<string>().HasMaxLength(15);
                entity.Property(s => s.Estado).HasConversion<string>().HasMaxLength(15);
                entity.HasOne(s => s.Solicitante)
                      .WithMany(u => u.Solicitudes)
                      .HasForeignKey(s => s.SolicitanteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InteresDonacion>(entity =>
            {
                entity.ToTable("InteresesDonacion");
                entity.Property(i => i.Estado).HasConversion<string>().HasMaxLength(15);
                entity.HasOne(i => i.Donante)
                      .WithMany(d => d.Intereses)
                      .HasForeignKey(i => i.DonanteId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(i => i.Solicitud)
                      .WithMany(s => s.Intereses)
                      .HasForeignKey(i => i.SolicitudId)
                      .OnDelete(DeleteBehavior.Cascade);
                // evita duplicar interés del mismo donante en una solicitud
                entity.HasIndex(i => new { i.DonanteId, i.SolicitudId }).IsUnique();
            });
        }
    }
}
