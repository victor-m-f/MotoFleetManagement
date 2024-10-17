using Mfm.Application.UseCases.Base;

namespace Mfm.Application.UseCases.Motorcycles.GetMotorcycleById;

public sealed class GetMotorcycleByIdInput : InputBase<GetMotorcycleByIdOutput>
{
    public string Id { get; }

    public GetMotorcycleByIdInput(string id)
    {
        Id = id;
    }
}