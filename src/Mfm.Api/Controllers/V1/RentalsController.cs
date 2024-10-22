using MediatR;
using Mfm.Application.Dtos.Rentals;
using Mfm.Application.UseCases.Rentals.CreateRental;
using Microsoft.AspNetCore.Mvc;

namespace Mfm.Api.Controllers.V1;

[Route("locacao")]
public sealed class RentalsController : ApiControllerBase<RentalsController>
{
    public RentalsController(ILogger<RentalsController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateRental(
        [FromBody] CreateRentalDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateRentalInput(request);
        var output = await Mediator.Send(input, cancellationToken);

        return Respond(output);
    }
}
