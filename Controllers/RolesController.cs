using Gimapi.Dto.RolDtos;
using Gimapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gimapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class RolesController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolesController(IRolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Listar() => Ok(await _rolService.ObtenerTodos());

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var resultado = await _rolService.ObtenerPorId(id);
            return resultado != null ? Ok(resultado) : NotFound("Rol no encontrado.");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Crear(RolInput dto)
        {
            var resultado = await _rolService.Crear(dto);
            return CreatedAtAction(nameof(Obtener), new { id = resultado.Id }, resultado);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Modificar(int id, RolInput dto)
        {
            var exito = await _rolService.Actualizar(id, dto);
            return exito ? NoContent() : NotFound("No se pudo modificar el rol.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Eliminar(int id)
        {
            var exito = await _rolService.BajaLogica(id);
            return exito ? NoContent() : NotFound("El rol no existe o ya fue dado de baja.");
        }
    }
}

