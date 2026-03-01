namespace Gimapi.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public bool Activo { get; set; } = true;

        // Relación con Rol (1 a muchos)
        public int RolId { get; set; }
        public Rol? ObjetoRol { get; set; }

        // Relación con Membresias (1 a muchos)
        public ICollection<Membresia> Membresias { get; set; }
            = new List<Membresia>();

    }
}
