using MediatR;
using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;
using Microsoft.AspNetCore.Mvc;

namespace Mfm.Api.Controllers.V1;

[ApiController]
[Route("motos")]
public class MotorcyclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MotorcyclesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMotorcycle(
        [FromBody] MotorcycleDto motorcycle,
        CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(new CreateMotorcycleInput(motorcycle), cancellationToken);

        return CreatedAtAction(nameof(GetMotorcycleById), new { id = motorcycle.Id }, motorcycle);
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

    [HttpGet("{id}")]
    public IActionResult GetMotorcycleById(string id)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMotorcycle(string id)
    {
        return NoContent();
    }
}
