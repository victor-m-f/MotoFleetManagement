using MediatR;

namespace Mfm.Application.UseCases.Motorcycles.DeleteMotorcycle;

public sealed class DeleteMotorcycleInput : IRequest<DeleteMotorcycleOutput>
{
    public string Id { get; }

    public DeleteMotorcycleInput(string id)
    {
        Id = id;
    }
}