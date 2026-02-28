namespace Gimapi.Models
{
    public class Usuario
    { //comentarios
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty; // Para verificación de ingreso 
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Activo { get; set; } = true; // Implementación de baja lógica 

        // Relación con Rol (Muchos a Uno)
        // Eliminamos "public string Rol" porque ya tenemos la relación profesional aquí:
        public int RolId { get; set; }
        public Rol? ObjetoRol { get; set; } // Propiedad de navegación

        public int? MembresiaId { get; set; }
        public Membresia? Membresia { get; set; }

    }
}
