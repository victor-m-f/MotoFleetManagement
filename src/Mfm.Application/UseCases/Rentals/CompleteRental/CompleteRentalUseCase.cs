using Mfm.Application.Helpers;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Exceptions;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.Rentals.CompleteRental;

internal sealed class CompleteRentalUseCase : UseCaseBase, ICompleteRentalUseCase
{
    private readonly IRentalRepository _rentalRepository;

    public CompleteRentalUseCase(
        ILogger<CompleteRentalUseCase> logger,
        IRentalRepository rentalRepository)
        : base(logger)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<CompleteRentalOutput> Handle(
        CompleteRentalInput request,
        CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

        var rental = await _rentalRepository.GetByIdAsync(
            request.RentalId,
            cancellationToken: cancellationToken);

        if (rental is null)
        {
            return CompleteRentalOutput.CreateRentalNotFoundError(request.RentalId);
        }

        var returnDate = request.ReturnDateString.ToDateTime()
            ?? throw new ValidationException("ReturnDate is invalid.");

        rental.CompleteRental(returnDate);
        await _rentalRepository.SaveChangesAsync(cancellationToken);

        return new CompleteRentalOutput();
    }
}
