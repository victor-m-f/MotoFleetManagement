using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using System.Net;

namespace Mfm.Application.UseCases.DeliveryPersons.UpdateDeliveryPersonCnhImage;

public sealed class UpdateDeliveryPersonCnhImageOutput : OutputBase
{
    public UpdateDeliveryPersonCnhImageOutput()
        : base(HttpStatusCode.Created)
    {
    }

    private UpdateDeliveryPersonCnhImageOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }

    public static UpdateDeliveryPersonCnhImageOutput CreateNotFoundError(string deliveryPersonId)
    {
        var output = new UpdateDeliveryPersonCnhImageOutput(HttpStatusCode.NotFound);
        output.AddError(NotFoundMessage(nameof(DeliveryPerson), deliveryPersonId));
        return output;
    }
}
