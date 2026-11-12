using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Services.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}
