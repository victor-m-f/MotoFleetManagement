using Mfm.Application.Dtos.Motorcycles;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("motos")]
public class MotorcyclesController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateMotorcycle([FromBody] MotorcycleDto motorcycle)
    {
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
