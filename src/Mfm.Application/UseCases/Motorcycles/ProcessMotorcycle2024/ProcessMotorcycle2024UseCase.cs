using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Repositories;

namespace Mfm.Application.UseCases.Motorcycles.ProcessMotorcycle2024;

public sealed class ProcessMotorcycle2024UseCase : UseCaseBase, IProcessMotorcycle2024UseCase
{
    private readonly IMotorcycle2024Repository _motorcycle2024Repository;
    private readonly TimeProvider _timeProvider;

    public ProcessMotorcycle2024UseCase(
        IMotorcycle2024Repository motorcycle2024Repository,
        TimeProvider timeProvider)
    {
        _motorcycle2024Repository = motorcycle2024Repository;
        _timeProvider = timeProvider;
    }

    public async Task<ProcessMotorcycle2024Output> Handle(
        ProcessMotorcycle2024Input request,
        CancellationToken cancellationToken)
    {
        var licensePlate = new LicensePlate(request.Motorcycle.LicensePlate);
        var motorcycle = new Motorcycle2024(
            request.Motorcycle.Id,
            request.Motorcycle.Year,
            licensePlate,
            request.Motorcycle.Model,
            _timeProvider);

        _motorcycle2024Repository.Add(motorcycle);
        await _motorcycle2024Repository.SaveChangesAsync(cancellationToken);

        return new ProcessMotorcycle2024Output();
    }
}
