using Gimapi.Data;
using Gimapi.Dto.MembresiaDtos;
using Gimapi.Models;
using Microsoft.EntityFrameworkCore;

namespace Gimapi.Services
{
    public class MembresiaService : IMembresiaService
    {
        private readonly ApplicationDbContext _context;

        public MembresiaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MembresiaDTO?> CrearAsync(CrearMembresiaInput dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == dto.UsuarioId && u.Activo);

            if (usuario == null)
                return null;

            var activa = await _context.Membresias
                .FirstOrDefaultAsync(m => m.UsuarioId == dto.UsuarioId && m.Activa);

            if (activa != null)
                activa.Activa = false;

            var inicio = DateTime.UtcNow;

            var nueva = new Membresia
            {
                UsuarioId = dto.UsuarioId,
                FechaInicio = inicio,
                FechaVencimiento = inicio.AddMonths(dto.Meses),
                Activa = true
            };

            _context.Membresias.Add(nueva);
            await _context.SaveChangesAsync();

            return MapToDto(nueva);
        }

        public async Task<MembresiaDTO?> ObtenerUltimaPorUsuarioIdAsync(int usuarioId)
        {
            var m = await _context.Membresias
                .Where(x => x.UsuarioId == usuarioId)
                .OrderByDescending(x => x.FechaInicio)
                .FirstOrDefaultAsync();

            return m == null ? null : MapToDto(m);
        }

        public async Task<MembresiaDTO?> ObtenerUltimaPorDniAsync(string dni)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.DNI == dni.Trim());

            if (usuario == null)
                return null;

            return await ObtenerUltimaPorUsuarioIdAsync(usuario.Id);
        }

        public async Task<IEnumerable<MembresiaDTO>> ObtenerPorUsuarioIdAsync(int usuarioId)
        {
            return await _context.Membresias
                .Where(m => m.UsuarioId == usuarioId)
                .OrderByDescending(m => m.FechaInicio)
                .Select(m => MapToDto(m))
                .ToListAsync();
        }

        public async Task<IEnumerable<MembresiaDTO>> ObtenerActivasAsync()
        {
            var ahora = DateTime.UtcNow;

            return await _context.Membresias
                .Where(m => m.Activa && m.FechaVencimiento > ahora)
                .Select(m => MapToDto(m))
                .ToListAsync();
        }

        public async Task<IEnumerable<MembresiaDTO>> ObtenerInactivasAsync()
        {
            var ahora = DateTime.UtcNow;

            return await _context.Membresias
                .Where(m => !m.Activa || m.FechaVencimiento <= ahora)
                .Select(m => MapToDto(m))
                .ToListAsync();
        }

        public async Task<bool> DarDeBajaAsync(int membresiaId)
        {
            var m = await _context.Membresias.FindAsync(membresiaId);

            if (m == null)
                return false;

            m.Activa = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ValidacionMembresiaDTO> ValidarPorDniAsync(string dni)
        {
            dni = dni.Trim();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.DNI == dni && u.Activo);

            if (usuario == null)
                return new ValidacionMembresiaDTO
                {
                    ExisteUsuario = false,
                    TieneMembresia = false,
                    EstaVigente = false,
                    DiasRestantes = 0,
                    AccesoPermitido = false,
                    Mensaje = "Usuario no registrado.",
                    Tipo = "error"
                };

            // Calcular edad
            var hoy = DateTime.Today;
            var edad = hoy.Year - usuario.FechaNacimiento.Year;
            if (usuario.FechaNacimiento.Date > hoy.AddYears(-edad))
                edad--;

            var activa = await _context.Membresias
                .Where(m => m.UsuarioId == usuario.Id && m.Activa)
                .OrderByDescending(m => m.FechaInicio)
                .FirstOrDefaultAsync();

            bool vigente = false;
            int dias = 0;

            if (activa != null)
            {
                if (activa.FechaVencimiento <= DateTime.UtcNow)
                {
                    activa.Activa = false;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    vigente = true;
                    dias = (activa.FechaVencimiento - DateTime.UtcNow).Days;
                    if (dias < 0) dias = 0;
                }
            }

            var ultima = await _context.Membresias
                .Where(m => m.UsuarioId == usuario.Id)
                .OrderByDescending(m => m.FechaInicio)
                .FirstOrDefaultAsync();

            return new ValidacionMembresiaDTO
            {
                ExisteUsuario = true,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Edad = edad,
                TieneMembresia = ultima != null,
                EstaVigente = vigente,
                DiasRestantes = dias,
                FechaUltimoPago = ultima?.FechaInicio,

                AccesoPermitido = vigente,
                Mensaje = vigente
        ? $"Acceso permitido. Restan {dias} días."
        : "Membresía vencida o inexistente.",
                Tipo = vigente ? "success" : "warning"
            };
        }

        private static MembresiaDTO MapToDto(Membresia m)
        {
            var dias = m.FechaVencimiento > DateTime.UtcNow
                ? (m.FechaVencimiento - DateTime.UtcNow).Days
                : 0;

            if (dias < 0) dias = 0;

            return new MembresiaDTO
            {
                Id = m.Id,
                UsuarioId = m.UsuarioId,
                FechaInicio = m.FechaInicio,
                FechaVencimiento = m.FechaVencimiento,
                Activa = m.Activa,
                DiasRestantes = dias
            };
        }
    }
}