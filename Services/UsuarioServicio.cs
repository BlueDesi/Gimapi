using Gimapi.Data;
using Gimapi.Dto.UsuarioDtos;
using Gimapi.Models;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Gimapi.Services
{
    public class UsuarioServicio : IUsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioServicio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioDTO>> ObtenerTodos()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.ObjetoRol)
                .Include(u => u.Membresias)   // ✅ corregido
                .Where(u => u.Activo)
                .ToListAsync();

            return usuarios.Select(u => MapToDto(u));
        }

        public async Task<UsuarioDTO?> ObtenerPorId(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.ObjetoRol)
                .Include(u => u.Membresias)   // ✅ corregido
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo);

            return usuario != null ? MapToDto(usuario) : null;
        }

        public async Task<UsuarioDTO> Crear(UsuarioInput dto)
        {
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DNI = dto.DNI,
                Email = dto.Email,
                Password = dto.Password,
                RolId = dto.RolId,
                Activo = true,
                      FechaNacimiento=dto.FechaNacimiento,

    };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return MapToDto(usuario);
        }

        public async Task<bool> BajaLogica(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            usuario.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        private static UsuarioDTO MapToDto(Usuario u)
        {
            var membresiaActiva = u.Membresias
                .OrderByDescending(m => m.FechaVencimiento)
                .FirstOrDefault(m => m.EstaVigente);

            return new UsuarioDTO
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                DNI = u.DNI,
                Email = u.Email,
                RolNombre = u.ObjetoRol?.NombreRol ?? "Sin Rol",

                TieneMembresia = u.Membresias.Any(),
                MembresiaVigente = membresiaActiva != null,
                DiasRestantes = membresiaActiva?.DiasRestantes ?? 0,
                FechaVencimiento = membresiaActiva?.FechaVencimiento,
                FechaNacimiento = u.FechaNacimiento,

            };
        }

        public async Task<bool> Actualizar(int id, UsuarioInput dto)
        {
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo);

            if (usuarioExistente == null)
                return false;

            usuarioExistente.Nombre = dto.Nombre;
            usuarioExistente.Apellido = dto.Apellido;
            usuarioExistente.DNI = dto.DNI;
            usuarioExistente.Email = dto.Email;
            usuarioExistente.RolId = dto.RolId;
            usuarioExistente.FechaNacimiento = dto.FechaNacimiento;


            if (!string.IsNullOrEmpty(dto.Password))
            {
                usuarioExistente.Password = dto.Password;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<UsuarioDTO?> ValidarCredenciales(string email, string password)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.ObjetoRol)
                .Include(u => u.Membresias)   // ✅ corregido
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    u.Password == password &&
                    u.Activo);

            return usuario != null ? MapToDto(usuario) : null;
        }
    }
}