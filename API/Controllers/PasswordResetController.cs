using Shared.DTO.ForgotPassword;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/password")]
    public class PasswordResetController : ControllerBase
    {
        private readonly PasswordResetService _service;

        public PasswordResetController(PasswordResetService service)
        {
            _service = service;
        }

        /// <summary>
        /// Envoie un mail de réinitialisation du mot de passe.
        /// </summary>
        [HttpPost("forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            await _service.RequestReset(dto.Email);
            return Ok(); // Toujours OK
        }

        /// <summary>
        /// Réinitialise le mot de passe.
        /// </summary>
        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            await _service.ResetPassword(dto.Token, dto.NewPassword);
            return NoContent();
        }
    }
}
