using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Base;
using System.Net;

namespace Mfm.Application.UseCases.Motorcycles.GetMotorcycleById;

public sealed class GetMotorcycleByIdOutput : OutputBase
{
    public MotorcycleDto? Motorcycle { get; }

    public GetMotorcycleByIdOutput(MotorcycleDto motorcycle)
        : base(HttpStatusCode.OK)
    {
        Motorcycle = motorcycle;
    }

    private GetMotorcycleByIdOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }

    public static GetMotorcycleByIdOutput CreateNotFoundError(string motorcycleId)
    {
        var output = new GetMotorcycleByIdOutput(HttpStatusCode.NotFound);
        output.AddError(NotFoundMessage(nameof(Motorcycle), motorcycleId));
        return output;
    }
}
