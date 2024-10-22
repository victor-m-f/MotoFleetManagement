using MediatR;

namespace Mfm.Application.UseCases.Rentals.GetRentalById;

public sealed class GetRentalByIdInput : IRequest<GetRentalByIdOutput>
{
    public string Id { get; }

    public GetRentalByIdInput(string id)
    {
        Id = id;
    }
}