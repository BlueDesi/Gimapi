using Gimapi.Dto.UsuarioDtos;

namespace Gimapi.Services
{
    public interface ITokenService
    {
        string GenerarToken(UsuarioDTO usuario);
    }
}
