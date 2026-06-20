using EcoCompliance.API.Services.Interfaces;
using EcoCompliance.API.ViewModels.EsgIndicator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoCompliance.API.Controllers;

[ApiController]
[Route("api/esg-indicators")]
public class EsgIndicatorController : ControllerBase
{
    private readonly IEsgIndicatorService _service;

    public EsgIndicatorController(IEsgIndicatorService service) => _service = service;

    /// <summary>Lista todos os indicadores ESG com paginação.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>Registra um novo indicador ESG. Requer autenticação.</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] EsgIndicatorRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), result);
    }
}
