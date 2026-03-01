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
        Task<bool> ValidarPorUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<UsuarioDTO>> ObtenerSocios(bool soloActivos = true);
        Task<IEnumerable<UsuarioDTO>> ObtenerEmpleados(bool soloActivos = true);
        Task<IEnumerable<UsuarioDTO>> ObtenerAdmins(bool soloActivos = true);
    }
}
