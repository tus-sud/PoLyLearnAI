using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Admin;
using PolyLearnAI.Api.Models.DTOs.Common;

namespace PolyLearnAI.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController(AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách người dùng trong hệ thống có phân trang và tìm kiếm
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminUserDto>>> GetUsers(
        [FromQuery] PaginationParams pagination,
        [FromQuery] string? search,
        CancellationToken ct)
    {
        var query = dbContext.Users
            .AsNoTracking()
            .Include(u => u.PreferredLanguage)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(u => u.Email.Contains(term) || u.DisplayName.Contains(term));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                PreferredLanguageCode = u.PreferredLanguage != null ? u.PreferredLanguage.Code : null,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<AdminUserDto>(items, totalCount, pagination.PageNumber, pagination.PageSize));
    }

    /// <summary>
    /// Khóa hoặc mở khóa tài khoản người dùng
    /// </summary>
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateUserStatusRequest request,
        CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null) throw new KeyNotFoundException($"Không tìm thấy người dùng ID {id}.");

        user.IsActive = request.IsActive;
        await dbContext.SaveChangesAsync(ct);

        var statusText = request.IsActive ? "kích hoạt" : "tạm khóa";
        return Ok(new { message = $"Đã {statusText} tài khoản {user.Email} thành công." });
    }

    /// <summary>
    /// Phân quyền người dùng (User / Admin)
    /// </summary>
    [HttpPut("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(
        int id,
        [FromBody] UpdateUserRoleRequest request,
        CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null) throw new KeyNotFoundException($"Không tìm thấy người dùng ID {id}.");

        user.Role = request.Role;
        await dbContext.SaveChangesAsync(ct);

        return Ok(new { message = $"Đã cập nhật vai trò tài khoản {user.Email} thành {request.Role}." });
    }
}
