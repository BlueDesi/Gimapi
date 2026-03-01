namespace Gimapi.Dto.MembresiaDtos
{
    public class ValidacionMembresiaDTO
    {
        public bool ExisteUsuario { get; set; }

        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int? Edad { get; set; }

        public bool TieneMembresia { get; set; }
        public bool EstaVigente { get; set; }

        public int DiasRestantes { get; set; }

        public DateTime? FechaUltimoPago { get; set; }

        public bool AccesoPermitido { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;
    }
}
