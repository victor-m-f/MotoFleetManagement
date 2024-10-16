using MediatR;

namespace Mfm.Application.UseCases.Motorcycles.UpdateMotorcycleLicensePlate;

public interface IUpdateMotorcycleLicensePlateUseCase
    : IRequestHandler<UpdateMotorcycleLicensePlateInput, UpdateMotorcycleLicensePlateOutput>
{
}
