using MediatR;
using Mfm.Application.Dtos.DeliveryPersons;
using Mfm.Application.UseCases.DeliveryPersons.CreateDeliveryPerson;
using Microsoft.AspNetCore.Mvc;

namespace Mfm.Api.Controllers.V1;

[Route("entregadores")]
public sealed class DeliveryPersonsController : ApiControllerBase<DeliveryPersonsController>
{
    public DeliveryPersonsController(ILogger<DeliveryPersonsController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateDeliveryPerson(
        [FromBody] DeliveryPersonDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateDeliveryPersonInput(request);
        var output = await Mediator.Send(input, cancellationToken);

        return Respond(output);
    }
}
