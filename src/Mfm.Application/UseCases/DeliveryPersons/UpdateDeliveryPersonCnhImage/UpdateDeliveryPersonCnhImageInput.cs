using Mfm.Application.UseCases.Base;

namespace Mfm.Application.UseCases.DeliveryPersons.UpdateDeliveryPersonCnhImage;

public sealed class UpdateDeliveryPersonCnhImageInput : InputBase<UpdateDeliveryPersonCnhImageOutput>
{
    public string Id { get; }
    public string CnhImage { get; }

    public UpdateDeliveryPersonCnhImageInput(string id, string cnhImage)
    {
        Id = id;
        CnhImage = cnhImage;
    }
}