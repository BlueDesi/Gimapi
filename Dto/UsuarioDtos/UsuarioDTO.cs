namespace Gimapi.Dto.UsuarioDtos
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RolNombre { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }


        // Información de membresía procesada para el socio
        public bool TieneMembresia { get; set; }
        public bool MembresiaVigente { get; set; }
        public int DiasRestantes { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}
