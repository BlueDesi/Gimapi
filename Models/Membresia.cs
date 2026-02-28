namespace Gimapi.Models
{
    public class Membresia
    {
        public int Id { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }

        public bool EstaVigente => DateTime.Now < FechaVencimiento;
        public int DiasRestantes => EstaVigente ? (FechaVencimiento - DateTime.Now).Days : 0;
    }
}
