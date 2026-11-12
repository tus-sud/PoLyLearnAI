using PolyLearnAI.Api.Models.DTOs.Auth;

namespace PolyLearnAI.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<UserProfileResponse> GetProfileAsync(int userId, CancellationToken ct = default);
    Task<UserProfileResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken ct = default);
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default);
}
