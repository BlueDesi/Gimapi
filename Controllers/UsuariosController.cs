using Gimapi.Dto.UsuarioDtos;

using Gimapi.Models;
using Gimapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gimapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioServicio;

        public UsuariosController(IUsuarioService usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _usuarioServicio.ObtenerTodos();
            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _usuarioServicio.ObtenerPorId(id);
            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });
            return Ok(usuario);
        }

        // POST: api/Usuarios (REGISTRO)
        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> PostUsuario(UsuarioInput dto)
        {
            try
            {
                var resultado = await _usuarioServicio.Crear(dto);
                return CreatedAtAction(nameof(GetUsuario), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                // En lugar de 500, devolvemos 400 con el mensaje que definimos en el Service
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // POST: api/Usuarios/login (AQUÍ ESTÁ EL MÉTODO QUE BUSCABAS)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var usuario = await _usuarioServicio.ValidarCredenciales(login.Email, login.Password);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Credenciales inválidas o cuenta inactiva" });

            // Por ahora devolvemos el DTO, luego aquí generarás el JWT
            return Ok(new
            {
                mensaje = "Login exitoso",
                datos = usuario
            });
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioInput dto)
        {
            var actualizado = await _usuarioServicio.Actualizar(id, dto);
            if (!actualizado) return NotFound();
            return NoContent();
        }

        // DELETE: api/Usuarios/5 (BAJA LÓGICA)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var exito = await _usuarioServicio.BajaLogica(id);
            if (!exito) return NotFound();
            return NoContent();
        }

        [HttpGet("socios")]
        public async Task<IActionResult> GetSocios([FromQuery] bool incluirInactivos = false)
        {
            var usuarios = await _usuarioServicio.ObtenerSocios(!incluirInactivos);
            return Ok(usuarios);
        }

        [HttpGet("empleados")]
        public async Task<IActionResult> GetEmpleados([FromQuery] bool incluirInactivos = false)
        {
            var usuarios = await _usuarioServicio.ObtenerEmpleados(!incluirInactivos);
            return Ok(usuarios);
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins([FromQuery] bool incluirInactivos = false)
        {
            var usuarios = await _usuarioServicio.ObtenerAdmins(!incluirInactivos);
            return Ok(usuarios);
        }
        [HttpGet("{id}/validar-acceso-id")]
        public async Task<IActionResult> ValidarAcceso(int id)
        {
            var tieneAcceso = await _usuarioServicio.ValidarPorUsuarioIdAsync(id);

            if (!tieneAcceso)
            {
                return Ok(new { status = "Denegado", mensaje = "El socio no tiene membresía vigente." });
            }

            return Ok(new { status = "Concedido", mensaje = "Acceso permitido." });
        }

    }
}