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
            var resultado = await _usuarioServicio.Crear(dto);
            return CreatedAtAction(nameof(GetUsuario), new { id = resultado.Id }, resultado);
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
    }
}