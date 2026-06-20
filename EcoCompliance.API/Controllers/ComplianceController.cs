using EcoCompliance.API.Services.Interfaces;
using EcoCompliance.API.ViewModels.Compliance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoCompliance.API.Controllers;

[ApiController]
[Route("api/compliance")]
public class ComplianceController : ControllerBase
{
    private readonly IComplianceService _service;

    public ComplianceController(IComplianceService service) => _service = service;

    /// <summary>Lista todos os registros de compliance com paginação.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>Lista registros de compliance de uma empresa com paginação.</summary>
    [HttpGet("company/{companyId:long}")]
    public async Task<IActionResult> GetByCompany(long companyId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetByCompanyAsync(companyId, page, pageSize);
        return Ok(result);
    }

    /// <summary>Cria um novo registro de compliance. Requer autenticação.</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ComplianceRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), result);
    }

    /// <summary>Atualiza um registro de compliance. Requer autenticação.</summary>
    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] ComplianceRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return Ok(result);
    }

    /// <summary>Remove um registro de compliance. Requer autenticação.</summary>
    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
