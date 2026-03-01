namespace Gimapi.Dto.MembresiaDtos
{
    public class MembresiaDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Activa { get; set; }
        public bool EstaVigente { get; set; }
        public int DiasRestantes { get; set; }
    }
}
