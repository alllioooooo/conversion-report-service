using ConversionReportService.Application.Contracts.Services;
using ConversionReportService.Presentation.Http.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConversionReportService.Presentation.Http.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemInteractionController : ControllerBase
{
    private readonly IItemInteractionService _itemInteractionService;

    public ItemInteractionController(IItemInteractionService itemInteractionService)
    {
        _itemInteractionService = itemInteractionService;
    }

    [HttpPost("interactions")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddInteraction([FromBody] AddItemInteractionRequest request, CancellationToken cancellationToken)
    {
        var id = await _itemInteractionService.AddAsync(
            new Application.Contracts.Dtos.AddItemInteraction.Request(
                ItemId: request.ItemId,
                Type: request.Type,
                Timestamp: request.Timestamp
            ),
            cancellationToken
        );

        return CreatedAtAction(nameof(AddInteraction), new { id }, id);
    }
}