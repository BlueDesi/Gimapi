using Gimapi.Data;
using Gimapi.Dto.UsuarioDtos;
using Gimapi.Models;
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
            return await _context.Usuarios
                .Include(u => u.ObjetoRol)
                .Include(u => u.Membresia)
                .Where(u => u.Activo)
                .Select(u => MapToDto(u))
                .ToListAsync();
        }

        public async Task<UsuarioDTO?> ObtenerPorId(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.ObjetoRol)
                .Include(u => u.Membresia)
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
                Password = dto.Password, // TODO: Hashear en etapa de seguridad
                RolId = dto.RolId,
                Activo = true
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

        // Método privado para no repetir lógica de mapeo
        private static UsuarioDTO MapToDto(Usuario u) => new UsuarioDTO
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            DNI = u.DNI,
            Email = u.Email,
            RolNombre = u.ObjetoRol?.NombreRol ?? "Sin Rol",
            TieneMembresia = u.Membresia != null,
            MembresiaVigente = u.Membresia?.EstaVigente ?? false,
            DiasRestantes = u.Membresia?.DiasRestantes ?? 0,
            FechaVencimiento = u.Membresia?.FechaVencimiento
        };

        public async Task<bool> Actualizar(int id, UsuarioInput dto)
        {
            // 1. Buscamos el usuario existente incluyendo sus relaciones
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo);

            if (usuarioExistente == null)
            {
                return false;
            }

            // 2. Actualizamos los campos permitidos
            usuarioExistente.Nombre = dto.Nombre;
            usuarioExistente.Apellido = dto.Apellido;
            usuarioExistente.DNI = dto.DNI;
            usuarioExistente.Email = dto.Email;
            usuarioExistente.RolId = dto.RolId;

            // Nota: La Password suele manejarse en un método aparte por seguridad, 
            // pero si el DTO la trae, la actualizamos aquí:
            if (!string.IsNullOrEmpty(dto.Password))
            {
                usuarioExistente.Password = dto.Password;
            }

            // 3. Persistimos los cambios de forma asincrónica
            // Al usar 'await', el warning desaparece.
            var filasAfectadas = await _context.SaveChangesAsync();

            return filasAfectadas > 0;
        }

        public async Task<UsuarioDTO?> ValidarCredenciales(string email, string password)
        {
            // Buscamos el usuario que coincida con email y password, y que esté activo
            var usuario = await _context.Usuarios
                .Include(u => u.ObjetoRol)
                .Include(u => u.Membresia)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.Activo);

            if (usuario == null)
            {
                return null;
            }

            // Retornamos el DTO usando tu método MapToDto
            return MapToDto(usuario);
        }
    }
}

