using Gimapi.Dto.RolDtos;

namespace Gimapi.Services
{
    public interface IRolService
    {
        Task<IEnumerable<RolDTO>> ObtenerTodos();
        Task<RolDTO?> ObtenerPorId(int id);
        Task<RolDTO> Crear(RolInput dto);
        Task<bool> Actualizar(int id, RolInput dto);
        Task<bool> BajaLogica(int id);
    }
}
