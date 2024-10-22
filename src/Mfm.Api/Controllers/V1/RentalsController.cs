using MediatR;
using Mfm.Application.Dtos.Rentals;
using Mfm.Application.Helpers;
using Mfm.Application.UseCases.Rentals.CompleteRental;
using Mfm.Application.UseCases.Rentals.CreateRental;
using Mfm.Application.UseCases.Rentals.GetRentalById;
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

    [HttpGet("{id}", Name = nameof(GetRentalById))]
    public async Task<IActionResult> GetRentalById(string id, CancellationToken cancellationToken)
    {
        var input = new GetRentalByIdInput(id);
        var output = await Mediator.Send(input, cancellationToken);
        return Respond(output, output.Rental);
    }

    [HttpPut("{id}/devolucao")]
    public async Task<IActionResult> CompleteRental(
        string id,
        [FromBody] CompleteRentalDto dto,
        CancellationToken cancellationToken)
    {
        var input = new CompleteRentalInput(id, dto.ReturnDate);
        var output = await Mediator.Send(input, cancellationToken);
        return Respond(output, CompleteRentalOutput.SuccessMessage);
    }
}
