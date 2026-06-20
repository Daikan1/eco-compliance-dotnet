using EcoCompliance.API.Controllers;
using EcoCompliance.API.Services.Interfaces;
using EcoCompliance.API.ViewModels;
using EcoCompliance.API.ViewModels.Compliance;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EcoCompliance.Tests.Controllers;

public class ComplianceControllerTests
{
    private static (ComplianceController Controller, Mock<IComplianceService> Service) Build()
    {
        var mock = new Mock<IComplianceService>();
        return (new ComplianceController(mock.Object), mock);
    }

    [Fact]
    public async Task GetAll_Returns200WithPagedResult()
    {
        var (controller, service) = Build();
        service.Setup(s => s.GetAllAsync(1, 10))
               .ReturnsAsync(new PagedResult<ComplianceResponse>
               {
                   Items = new[]
                   {
                       new ComplianceResponse
                       {
                           Id = 1, CompanyId = 1, CompanyName = "ACME",
                           ComplianceType = "ISO 14001", Status = "Aprovado",
                           ReportDate = DateTime.Today
                       }
                   },
                   TotalCount = 1, Page = 1, PageSize = 10
               });

        var result = await controller.GetAll(1, 10);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task GetByCompany_Returns200WithPagedResult()
    {
        var (controller, service) = Build();
        service.Setup(s => s.GetByCompanyAsync(1, 1, 10))
               .ReturnsAsync(new PagedResult<ComplianceResponse>
               {
                   Items = Enumerable.Empty<ComplianceResponse>(),
                   TotalCount = 0, Page = 1, PageSize = 10
               });

        var result = await controller.GetByCompany(1, 1, 10);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201()
    {
        var (controller, service) = Build();
        var request = new ComplianceRequest
        {
            CompanyId = 1, ComplianceType = "GRI", Status = "Pendente", ReportDate = DateTime.Today
        };
        service.Setup(s => s.CreateAsync(request))
               .ReturnsAsync(new ComplianceResponse { Id = 5, CompanyId = 1 });

        var result = await controller.Create(request);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Update_ExistingId_Returns200()
    {
        var (controller, service) = Build();
        var request = new ComplianceRequest
        {
            CompanyId = 1, ComplianceType = "GRI", Status = "Aprovado", ReportDate = DateTime.Today
        };
        service.Setup(s => s.UpdateAsync(1, request))
               .ReturnsAsync(new ComplianceResponse { Id = 1, Status = "Aprovado" });

        var result = await controller.Update(1, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingId_Returns204()
    {
        var (controller, service) = Build();
        service.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }
}
