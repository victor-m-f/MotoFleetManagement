using MediatR;

namespace Mfm.Application.UseCases.DeliveryPersons.CreateDeliveryPerson;

public interface ICreateDeliveryPersonUseCase : IRequestHandler<CreateDeliveryPersonInput, CreateDeliveryPersonOutput>
{
}
