using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Base;
using System.Net;

namespace Mfm.Application.UseCases.Motorcycles.GetMotorcycles;

public sealed class GetMotorcyclesOutput : OutputBase
{
    public IEnumerable<MotorcycleDto> Motorcycles { get; }

    public GetMotorcyclesOutput(IEnumerable<MotorcycleDto> motorcycles)
        : base(HttpStatusCode.OK)
    {
        Motorcycles = motorcycles;
    }
}
