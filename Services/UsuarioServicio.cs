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
        //
        //
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
            // 1. Validar duplicados de DNI
            var existeDni = await _context.Usuarios
                .AnyAsync(u => u.DNI == dto.DNI);

            if (existeDni)
            {
                throw new Exception("El DNI ingresado ya se encuentra registrado.");
            }

            // 2. Validar duplicados de Email
            var existeEmail = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (existeEmail)
            {
                throw new Exception("El correo electrónico ya se encuentra registrado.");
            }

            // 3. Asignación de Rol por defecto (Socio = 3) si no se especifica
            if (dto.RolId == null || dto.RolId == 0)
            {
                dto.RolId = 3;
            }

            // 4. Validar que el Rol exista y esté activo
            var rolExiste = await _context.Roles
                .AnyAsync(r => r.Id == dto.RolId && r.Activo);

            if (!rolExiste)
            {
                throw new Exception($"El Rol con ID {dto.RolId} no existe en el sistema o está inactivo.");
            }

            // 5. Mapeo a la entidad y persistencia
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DNI = dto.DNI,
                Email = dto.Email,
                Password = dto.Password, // Nota: Se recomienda aplicar Hash aquí
                RolId = dto.RolId,
                Activo = true,
                FechaNacimiento = dto.FechaNacimiento
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // 6. Retornar el DTO (usando su método de mapeo existente)
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

        public async Task<IEnumerable<UsuarioDTO>> ObtenerSocios(bool soloActivos = true)
    => await ObtenerPorRol(3, soloActivos);

        public async Task<IEnumerable<UsuarioDTO>> ObtenerEmpleados(bool soloActivos = true)
            => await ObtenerPorRol(2, soloActivos);

        public async Task<IEnumerable<UsuarioDTO>> ObtenerAdmins(bool soloActivos = true)
            => await ObtenerPorRol(1, soloActivos);

        // Método privado reutilizable para evitar repetir código (DRY)
        private async Task<IEnumerable<UsuarioDTO>> ObtenerPorRol(int rolId, bool soloActivos)
        {
            var query = _context.Usuarios
                .Include(u => u.ObjetoRol)
                .Include(u => u.Membresias)
                .Where(u => u.RolId == rolId);

            if (soloActivos)
            {
                query = query.Where(u => u.Activo);
            }

            return await query
                .Select(u => MapToDto(u))
                .ToListAsync();
        }


        public async Task<bool> ValidarPorUsuarioIdAsync(int usuarioId)
        {
            // Buscamos el usuario incluyendo sus membresías para usar la lógica de vigencia
            var usuario = await _context.Usuarios
                .Include(u => u.Membresias)
                .FirstOrDefaultAsync(u => u.Id == usuarioId && u.Activo);

            if (usuario == null)
            {
                return false;
            }

            // Retorna true si tiene alguna membresía cuya propiedad EstaVigente sea true
            return usuario.Membresias.Any(m => m.EstaVigente);
        }



    }
}