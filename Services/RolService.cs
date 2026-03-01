using Gimapi.Data;
using Gimapi.Dto.RolDtos;
using Gimapi.Models;
using Microsoft.EntityFrameworkCore;

namespace Gimapi.Services
{
    public class RolService:IRolService
    {

        private readonly ApplicationDbContext _context;

        public RolService(ApplicationDbContext context) => _context = context;
        public async Task<IEnumerable<RolDTO>> ObtenerTodos()
        {
            return await _context.Roles
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<RolDTO?> ObtenerPorId(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            return rol != null ? MapToDto(rol) : null;
        }

        public async Task<RolDTO> Crear(RolInput dto)
        {
            var nuevoRol = new Rol { NombreRol = dto.NombreRol };
            _context.Roles.Add(nuevoRol);
            await _context.SaveChangesAsync();
            return MapToDto(nuevoRol);
        }

        public async Task<bool> Actualizar(int id, RolInput dto)
        {
            var rolExistente = await _context.Roles.FindAsync(id);
            if (rolExistente == null) return false;

            rolExistente.NombreRol = dto.NombreRol;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Eliminar(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return false;

            _context.Roles.Remove(rol);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BajaLogica(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return false;

            // Si la entidad Rol no tiene campo 'Activo', se simula o se extiende la clase Rol.
                        // rol.Activo = false; 

            // Si decides eliminarlo físicamente por ser una tabla maestra:
            _context.Roles.Remove(rol);

            return await _context.SaveChangesAsync() > 0;
        }

        private static RolDTO MapToDto(Rol r) => new RolDTO
        {
            Id = r.Id,
            NombreRol = r.NombreRol
        };



    }
}

