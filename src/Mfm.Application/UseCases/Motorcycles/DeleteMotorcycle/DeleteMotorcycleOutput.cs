using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using System.Net;

namespace Mfm.Application.UseCases.Motorcycles.DeleteMotorcycle;

public sealed class DeleteMotorcycleOutput : OutputBase
{
    public const string HasRentalRecordsErrorMessage = "Cannot delete a motorcycle that has rental records.";

    public DeleteMotorcycleOutput()
        : base(HttpStatusCode.OK)
    {
    }

    private DeleteMotorcycleOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }

    public static DeleteMotorcycleOutput CreateNotFoundError(string motorcycleId)
    {
        var output = new DeleteMotorcycleOutput(HttpStatusCode.NotFound);
        output.AddError(NotFoundMessage(nameof(Motorcycle), motorcycleId));
        return output;
    }

    public static DeleteMotorcycleOutput CreateHasRentalRecordsError()
    {
        var output = new DeleteMotorcycleOutput(HttpStatusCode.Conflict);
        output.AddError(HasRentalRecordsErrorMessage);
        return output;
    }
}
