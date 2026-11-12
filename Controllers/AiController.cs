using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyLearnAI.Api.Models.DTOs.Ai;
using PolyLearnAI.Api.Services.Interfaces;

namespace PolyLearnAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController(IAiService aiService) : ControllerBase
{
    /// <summary>
    /// Kiểm tra ngữ pháp câu viết, sửa lỗi và gợi ý cách diễn đạt tự nhiên
    /// </summary>
    [HttpPost("check-grammar")]
    public async Task<ActionResult<GrammarCheckResponse>> CheckGrammar(
        [FromBody] GrammarCheckRequest request,
        CancellationToken ct)
    {
        var result = await aiService.CheckGrammarAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Giải thích nghĩa, phiên âm, ngữ cảnh sử dụng và câu ví dụ của từ/cụm từ
    /// </summary>
    [HttpPost("explain-word")]
    public async Task<ActionResult<WordExplanationResponse>> ExplainWord(
        [FromBody] WordExplanationRequest request,
        CancellationToken ct)
    {
        var result = await aiService.ExplainWordAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Chat nhập vai luyện phản xạ giao tiếp ngoại ngữ theo kịch bản (nhà hàng, sân bay, mua sắm...)
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<AiChatResponse>> Chat(
        [FromBody] AiChatRequest request,
        CancellationToken ct)
    {
        var result = await aiService.ChatAsync(request, ct);
        return Ok(result);
    }
}
