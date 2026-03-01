namespace Gimapi.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string NombreRol { get; set; } = string.Empty; // Ej: Admin, Empleado, Socio
        public bool Activo { get; set; } = true; // Requisito de baja lógica [cite: 38]

        // Propiedad de navegación: Un rol puede ser compartido por muchos usuarios
        // Aquí es donde usamos la Lista porque la flecha en el esquema tenía "pata de gallo"
        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
