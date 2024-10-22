using MediatR;
using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;
using Mfm.Application.UseCases.Motorcycles.DeleteMotorcycle;
using Mfm.Application.UseCases.Motorcycles.GetMotorcycleById;
using Mfm.Application.UseCases.Motorcycles.GetMotorcycles;
using Mfm.Application.UseCases.Motorcycles.UpdateMotorcycleLicensePlate;
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
        var input = new CreateMotorcycleInput(motorcycle);
        var output = await Mediator.Send(input, cancellationToken);

        return Respond(output, nameof(GetMotorcycleById), new { id = motorcycle.Id }, motorcycle);
    }

    [HttpGet]
    public async Task<IActionResult> GetMotorcycles(
        [FromQuery] string? licensePlate,
        CancellationToken cancellationToken)
    {
        var input = new GetMotorcyclesInput(licensePlate);
        var output = await Mediator.Send(input, cancellationToken);

        return Respond(output, output.Motorcycles);
    }

    [HttpPut("{id}/placa")]
    public async Task<IActionResult> UpdateMotorcycleLicensePlate(
        string id,
        [FromBody] UpdateLicensePlateDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateMotorcycleLicensePlateInput(id, request.LicensePlate);
        var output = await Mediator.Send(input, cancellationToken);
        return Respond(output);
    }

    [HttpGet("{id}", Name = nameof(GetMotorcycleById))]
    public async Task<IActionResult> GetMotorcycleById(string id, CancellationToken cancellationToken)
    {
        var input = new GetMotorcycleByIdInput(id);
        var output = await Mediator.Send(input, cancellationToken);
        return Respond(output, output.Motorcycle);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMotorcycle(string id, CancellationToken cancellationToken)
    {
        var input = new DeleteMotorcycleInput(id);
        var output = await Mediator.Send(input, cancellationToken);
        return Respond(output);
    }
}
