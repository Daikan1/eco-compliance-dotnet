using EcoCompliance.API.Exceptions;

namespace EcoCompliance.API.Services;

// Regras de paginação compartilhadas pelas listagens
internal static class Pagination
{
    public const int MaxPageSize = 100;

    public static void Validate(int page, int pageSize)
    {
        if (page < 1)
            throw new AppValidationException("page must be greater than or equal to 1");

        if (pageSize < 1 || pageSize > MaxPageSize)
            throw new AppValidationException($"pageSize must be between 1 and {MaxPageSize}");
    }
}
