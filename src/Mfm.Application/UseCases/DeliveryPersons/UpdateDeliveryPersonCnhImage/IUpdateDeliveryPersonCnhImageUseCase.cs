using MediatR;

namespace Mfm.Application.UseCases.DeliveryPersons.UpdateDeliveryPersonCnhImage;

public interface IUpdateDeliveryPersonCnhImageUseCase
    : IRequestHandler<UpdateDeliveryPersonCnhImageInput, UpdateDeliveryPersonCnhImageOutput>
{
}
