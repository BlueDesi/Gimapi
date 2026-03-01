namespace Gimapi.Models
{
    public class Membresia
    {
        public int Id { get; set; }

        // FK
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public bool Activa { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }

        // Propiedades calculadas
        public bool EstaVigente => DateTime.UtcNow < FechaVencimiento;

        public int DiasRestantes =>
            EstaVigente
                ? (FechaVencimiento - DateTime.UtcNow).Days
                : 0;
    }
}
