using Mfm.Application.UseCases.Base;
using System.Net;

namespace Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;

public sealed class CreateMotorcycleOutput : OutputBase
{
    public CreateMotorcycleOutput()
        : base(HttpStatusCode.Created)
    {
    }

    public CreateMotorcycleOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }
}
