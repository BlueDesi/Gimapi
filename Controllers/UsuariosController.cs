using Gimapi.Dto.UsuarioDtos;

using Gimapi.Models;
using Gimapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gimapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly ITokenService _tokenService; // 1. Declarar el campo privado
        public UsuariosController(IUsuarioService usuarioServicio, ITokenService tokenService)
        {
            _usuarioServicio = usuarioServicio;
            _tokenService = tokenService; // Ahora sí 'tokenService' existe porque entra por el parámetro
        }

        // GET: api/Usuarios
        [HttpGet]
        [Authorize(Roles = "Admin,Empleado")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _usuarioServicio.ObtenerTodos();
            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Empleado")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _usuarioServicio.ObtenerPorId(id);
            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });
            return Ok(usuario);
        }

        // POST: api/Usuarios (REGISTRO)
        [HttpPost]
        [AllowAnonymous]

        //[Authorize(Roles = "Admin,Empleado")]
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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            // 1. Validar contra la DB (usando el servicio corregido arriba)
            var usuarioDto = await _usuarioServicio.ValidarCredenciales(login.Email, login.Password);

            if (usuarioDto == null)
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });

            // 2. Generar el Token JWT
            var tokenString = _tokenService.GenerarToken(usuarioDto);

            // 3. Retornar el DTO y el Token
            return Ok(new
            {
                Token = tokenString,
                Usuario = usuarioDto
            });
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Empleado")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioInput dto)
        {
            var actualizado = await _usuarioServicio.Actualizar(id, dto);
            if (!actualizado) return NotFound();
            return NoContent();
        }

        // DELETE: api/Usuarios/5 (BAJA LÓGICA)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Empleado")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var exito = await _usuarioServicio.BajaLogica(id);
            if (!exito) return NotFound();
            return NoContent();
        }

        [HttpGet("socios")]
        [Authorize(Roles = "Admin,Empleado")]
        public async Task<IActionResult> GetSocios([FromQuery] bool incluirInactivos = false)
        {
            var usuarios = await _usuarioServicio.ObtenerSocios(!incluirInactivos);
            return Ok(usuarios);
        }

        [HttpGet("empleados")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEmpleados([FromQuery] bool incluirInactivos = false)
        {
            var usuarios = await _usuarioServicio.ObtenerEmpleados(!incluirInactivos);
            return Ok(usuarios);
        }

        [HttpGet("admins")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdmins([FromQuery] bool incluirInactivos = false)
        {
            var usuarios = await _usuarioServicio.ObtenerAdmins(!incluirInactivos);
            return Ok(usuarios);
        }

        [HttpGet("{id}/validar-acceso-id")]
[AllowAnonymous]
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