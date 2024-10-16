using MediatR;
using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;
using Microsoft.AspNetCore.Mvc;

namespace Mfm.Api.Controllers.V1;

[Route("motos")]
public sealed class MotorcyclesController : ApiControllerBase<MotorcyclesController>
{
    public MotorcyclesController(ILogger<MotorcyclesController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateMotorcycle(
        [FromBody] MotorcycleDto motorcycle,
        CancellationToken cancellationToken)
    {
        using (StartUseCaseScope(nameof(CreateMotorcycle)))
        {
            var input = new CreateMotorcycleInput(motorcycle);
            var output = await Mediator.Send(input, cancellationToken);

            return output.IsValid
                ? RespondCreated(nameof(GetMotorcycleById), new { id = motorcycle.Id }, motorcycle)
                : RespondError(output);
        }
    }

    [HttpGet]
    public IActionResult GetMotorcycles()
    {
        return Ok();
    }

    [HttpPut("{id}/placa")]
    public IActionResult UpdateMotorcycleLicensePlate(
        string id,
        [FromBody] UpdateLicensePlateDto newPlate)
    {
        return NoContent();
    }

    [HttpGet("{id}", Name = nameof(GetMotorcycleById))]
    public IActionResult GetMotorcycleById(string id, CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMotorcycle(string id)
    {
        return NoContent();
    }
}
