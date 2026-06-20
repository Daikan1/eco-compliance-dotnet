using EcoCompliance.API.Controllers;
using EcoCompliance.API.Services.Interfaces;
using EcoCompliance.API.ViewModels;
using EcoCompliance.API.ViewModels.Company;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EcoCompliance.Tests.Controllers;

public class CompanyControllerTests
{
    private static (CompanyController Controller, Mock<ICompanyService> Service) Build()
    {
        var mock = new Mock<ICompanyService>();
        return (new CompanyController(mock.Object), mock);
    }

    [Fact]
    public async Task GetAll_Returns200WithPagedResult()
    {
        var (controller, service) = Build();
        service.Setup(s => s.GetAllAsync(1, 10))
               .ReturnsAsync(new PagedResult<CompanyResponse>
               {
                   Items = new[] { new CompanyResponse { Id = 1, Name = "ACME", Sector = "Tech" } },
                   TotalCount = 1,
                   Page = 1,
                   PageSize = 10
               });

        var result = await controller.GetAll(1, 10);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        var paged = Assert.IsType<PagedResult<CompanyResponse>>(ok.Value);
        Assert.Single(paged.Items);
    }

    [Fact]
    public async Task GetById_ExistingId_Returns200()
    {
        var (controller, service) = Build();
        service.Setup(s => s.GetByIdAsync(1))
               .ReturnsAsync(new CompanyResponse { Id = 1, Name = "ACME" });

        var result = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201()
    {
        var (controller, service) = Build();
        var request = new CompanyRequest { Name = "Nova Empresa", Sector = "ESG" };
        service.Setup(s => s.CreateAsync(request))
               .ReturnsAsync(new CompanyResponse { Id = 2, Name = "Nova Empresa" });

        var result = await controller.Create(request);

        Assert.IsType<CreatedAtActionResult>(result);
    }
}
