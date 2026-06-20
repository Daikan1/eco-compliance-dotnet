namespace EcoCompliance.API.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string resource, object id)
        : base($"{resource} not found with id: {id}") { }
}
