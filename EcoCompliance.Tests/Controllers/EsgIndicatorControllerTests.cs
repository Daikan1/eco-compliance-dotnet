using EcoCompliance.API.Controllers;
using EcoCompliance.API.Services.Interfaces;
using EcoCompliance.API.ViewModels;
using EcoCompliance.API.ViewModels.EsgIndicator;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EcoCompliance.Tests.Controllers;

public class EsgIndicatorControllerTests
{
    private static (EsgIndicatorController Controller, Mock<IEsgIndicatorService> Service) Build()
    {
        var mock = new Mock<IEsgIndicatorService>();
        return (new EsgIndicatorController(mock.Object), mock);
    }

    [Fact]
    public async Task GetAll_Returns200WithPagedResult()
    {
        var (controller, service) = Build();
        service.Setup(s => s.GetAllAsync(1, 10))
               .ReturnsAsync(new PagedResult<EsgIndicatorResponse>
               {
                   Items = new[]
                   {
                       new EsgIndicatorResponse
                       {
                           Id = 1, CompanyId = 1, CompanyName = "ACME",
                           IndicatorType = "CO2", Value = 120.5, Unit = "tCO2e",
                           MeasuredAt = DateTime.Today
                       }
                   },
                   TotalCount = 1, Page = 1, PageSize = 10
               });

        var result = await controller.GetAll(1, 10);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        var paged = Assert.IsType<PagedResult<EsgIndicatorResponse>>(ok.Value);
        Assert.Single(paged.Items);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201()
    {
        var (controller, service) = Build();
        var request = new EsgIndicatorRequest
        {
            CompanyId = 1, IndicatorType = "Água", Value = 500, Unit = "m³", MeasuredAt = DateTime.Today
        };
        service.Setup(s => s.CreateAsync(request))
               .ReturnsAsync(new EsgIndicatorResponse { Id = 3, CompanyId = 1, IndicatorType = "Água" });

        var result = await controller.Create(request);

        Assert.IsType<CreatedAtActionResult>(result);
    }
}
