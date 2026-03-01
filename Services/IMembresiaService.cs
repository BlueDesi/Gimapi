using Gimapi.Dto.MembresiaDtos;

namespace Gimapi.Services
{
    public interface IMembresiaService
    {
        Task<MembresiaDTO?> CrearAsync(CrearMembresiaInput dto);

        Task<MembresiaDTO?> ObtenerUltimaPorUsuarioIdAsync(int usuarioId);
        Task<MembresiaDTO?> ObtenerUltimaPorDniAsync(string dni);

        Task<ValidacionMembresiaDTO> ValidarPorDniAsync(string dni);

        Task<IEnumerable<MembresiaDTO>> ObtenerPorUsuarioIdAsync(int usuarioId);

        Task<IEnumerable<MembresiaDTO>> ObtenerActivasAsync();
        Task<IEnumerable<MembresiaDTO>> ObtenerInactivasAsync();
        Task<bool> DarDeBajaAsync(int membresiaId);
    }
}
