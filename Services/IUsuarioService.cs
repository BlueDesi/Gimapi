using Gimapi.Dto.UsuarioDtos;

namespace Gimapi.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> ObtenerTodos();
        Task<UsuarioDTO?> ObtenerPorId(int id);
        Task<UsuarioDTO> Crear(UsuarioInput dto);
        Task<bool> Actualizar(int id, UsuarioInput dto);
        Task<bool> BajaLogica(int id);
        Task<UsuarioDTO?> ValidarCredenciales(string email, string password);
    }
}
