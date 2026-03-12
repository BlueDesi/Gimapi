using Gimapi.Dto.MembresiaDtos;
using Gimapi.Services;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet]
        [Authorize(Roles = "Admin,Empleado")]

        public async Task<IActionResult> GetAll()
        {
            var lista = await _service.ObtenerTodasAsync();
            return Ok(lista);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Empleado")]

        public async Task<IActionResult> Crear(CrearMembresiaInput dto)
        {
            var result = await _service.CrearAsync(dto);

            if (result == null)
                return NotFound("No existe ese socio.");

            return Ok(result);
        }

        [HttpGet("usuario/{usuarioId}")]
        [Authorize]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
            => Ok(await _service.ObtenerPorUsuarioIdAsync(usuarioId));

        [HttpGet("activas")]
        [Authorize(Roles = "Admin,Empleado")]

        public async Task<IActionResult> Activas()
            => Ok(await _service.ObtenerActivasAsync());

        [HttpGet("inactivas")]
        [Authorize(Roles = "Admin,Empleado")]

        public async Task<IActionResult> Inactivas()
            => Ok(await _service.ObtenerInactivasAsync());

        [HttpPut("baja/{id}")]
        [Authorize(Roles = "Admin,Empleado")]

        public async Task<IActionResult> Baja(int id)
        {
            var ok = await _service.DarDeBajaAsync(id);
            if (!ok) return NotFound("No se encontró la membresía.");
            return Ok("Membresía dada de baja.");
        }

        [HttpGet("validar/{dni}")]
        [AllowAnonymous]
        public async Task<IActionResult> Validar(string dni)
        {
            var resultado = await _service.ValidarPorDniAsync(dni.Trim());
            return Ok(resultado);
        }


    }
}