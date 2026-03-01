using System.ComponentModel.DataAnnotations;

namespace Gimapi.Dto.UsuarioDtos
{
    public class UsuarioInput
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        public string DNI { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RolId
        {
            get; set;
        }

        public DateTime FechaNacimiento { get; set; }

    }
}
