using Mfm.Application.Dtos.DeliveryPersons;
using Mfm.Application.UseCases.Base;

namespace Mfm.Application.UseCases.DeliveryPersons.CreateDeliveryPerson;

public sealed class CreateDeliveryPersonInput : InputBase<CreateDeliveryPersonOutput>
{
    public DeliveryPersonDto DeliveryPerson { get; }

    public CreateDeliveryPersonInput(DeliveryPersonDto deliveryPerson)
    {
        DeliveryPerson = deliveryPerson;
    }
}