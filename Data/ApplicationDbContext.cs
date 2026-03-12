using Gimapi.Models;
using Microsoft.EntityFrameworkCore;

namespace Gimapi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Membresia> Membresias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Seed de ROLES (Indispensable para que el ID 3 de Socio exista)
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, NombreRol = "Admin", Activo = true },
                new Rol { Id = 2, NombreRol = "Empleado", Activo = true },
                new Rol { Id = 3, NombreRol = "Socio", Activo = true }
            );

            // 2. Hash estático para el Admin (Password: Admin123!)
            string adminPasswordHash = "$2a$11$Xm7766/Wrc.0B0C6p7.SkeS.uM7Gz9.F8.7f88888888888888888";

            // 3. Seed de USUARIO ADMIN
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Admin",
                    Apellido = "Sistema",
                    DNI = "12345678",
                    Email = "admin@gimapi.com",
                    Password = adminPasswordHash,
                    RolId = 1, // Vinculado al Rol Admin (Id 1)
                    Activo = true,
                    FechaNacimiento = new DateTime(1990, 1, 1)
                }
            );
        }
    }
}