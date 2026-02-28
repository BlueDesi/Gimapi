using Gimapi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Gimapi.Data
{
    public class ApplicationDbContext:DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Definición de las tablas en la BD
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Membresia> Membresias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // 1. Seed de Roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, NombreRol = "Admin" },
                new Rol { Id = 2, NombreRol = "Empleado" },
                new Rol { Id = 3, NombreRol = "Socio" }
            );

            // 2. Seed de Usuarios (con contraseñas simples por ahora)
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Admin",
                    Apellido = "Gimnasio",
                    DNI = "12345678",
                    Email = "admin@gimapi.com",
                    Password = "admin", // Sin hash temporalmente
                    RolId = 1, // Vinculado al Id de Admin
                    Activo = true
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "Socio",
                    Apellido = "Prueba",
                    DNI = "99999999",
                    Email = "socio@gimapi.com",
                    Password = "socio",
                    RolId = 3, // Vinculado al Id de Socio
                    Activo = true
                }
            );




            // Aquí puedes configurar detalles adicionales, como 
            // precisión de fechas o valores por defecto si fuera necesario.
        }
    }
}

