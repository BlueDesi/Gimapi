using Gimapi.Dto.MembresiaDtos;
using Gimapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gimapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembresiasController : ControllerBase
    {
        private readonly IMembresiaService _service;

        public MembresiasController(IMembresiaService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearMembresiaInput dto)
        {
            var result = await _service.CrearAsync(dto);

            if (result == null)
                return NotFound("No existe ese socio.");

            return Ok(result);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
            => Ok(await _service.ObtenerPorUsuarioIdAsync(usuarioId));

        [HttpGet("activas")]
        public async Task<IActionResult> Activas()
            => Ok(await _service.ObtenerActivasAsync());

        [HttpGet("inactivas")]
        public async Task<IActionResult> Inactivas()
            => Ok(await _service.ObtenerInactivasAsync());

        [HttpPut("baja/{id}")]
        public async Task<IActionResult> Baja(int id)
        {
            var ok = await _service.DarDeBajaAsync(id);
            if (!ok) return NotFound("No se encontró la membresía.");
            return Ok("Membresía dada de baja.");
        }

        [HttpGet("validar/{dni}")]
        public async Task<IActionResult> Validar(string dni)
        {
            var resultado = await _service.ValidarPorDniAsync(dni.Trim());
            return Ok(resultado);
        }
    }
}