using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyLearnAI.Api.Models.DTOs.Auth;
using PolyLearnAI.Api.Services.Interfaces;

namespace PolyLearnAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Đăng nhập nhận JWT Token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin tài khoản hiện tại
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserProfileResponse>> GetCurrentUser(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var profile = await authService.GetProfileAsync(userId, ct);
        return Ok(profile);
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân (tên hiển thị, ngôn ngữ ưu tiên)
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var profile = await authService.UpdateProfileAsync(userId, request, ct);
        return Ok(profile);
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        await authService.ChangePasswordAsync(userId, request, ct);
        return Ok(new { message = "Đổi mật khẩu thành công." });
    }

    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Không xác định được danh tính người dùng.");
        }
        return userId;
    }
}
