using API.DTO.Verification;
using API.Models.EntityFramework;
using API.Services.Verification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VerificationController : ControllerBase
{
    private readonly IVerificationService _verificationService;

    public VerificationController(IVerificationService verificationService)
    {
        _verificationService = verificationService;
    }

    [HttpPost("send")]
    public async Task<ActionResult<VerificationResponse>> SendVerificationCode(
        [FromBody] SendVerificationCodeRequest request)
    {
        var userId = GetConnectedUserId();
        if (userId == null)
            return Unauthorized();

        var (success, message, expiresAt) = await _verificationService
            .SendVerificationCodeAsync(userId.Value, request.Type);

        if (!success)
            return BadRequest(new VerificationResponse 
            { 
                Success = false, 
                Message = message 
            });

        return Ok(new VerificationResponse
        {
            Success = true,
            Message = message,
            ExpiresAt = expiresAt
        });
    }

    [HttpPost("verify")]
    public async Task<ActionResult<VerificationResponse>> VerifyCode(
        [FromBody] VerifyCodeRequest request)
    {
        var userId = GetConnectedUserId();
        if (userId == null)
            return Unauthorized();

        var (success, message) = await _verificationService
            .VerifyCodeAsync(userId.Value, request.Code, request.Type);

        if (!success)
            return BadRequest(new VerificationResponse 
            { 
                Success = false, 
                Message = message 
            });

        return Ok(new VerificationResponse
        {
            Success = true,
            Message = message
        });
    }

    private int? GetConnectedUserId()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int id))
            {
                return id;
            }
        }
        return null;
    }
}