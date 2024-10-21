using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Exceptions;
using Mfm.Domain.Repositories;
using Mfm.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.DeliveryPersons.CreateDeliveryPerson;

internal sealed class CreateDeliveryPersonUseCase : UseCaseBase, ICreateDeliveryPersonUseCase
{
    private const string CnhStorageContainer = "upload-images";

    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly IStorageService _storageService;

    public CreateDeliveryPersonUseCase(
        ILogger<CreateDeliveryPersonUseCase> logger,
        IDeliveryPersonRepository deliveryPersonRepository,
        IStorageService storageService)
        : base(logger)
    {
        _deliveryPersonRepository = deliveryPersonRepository;
        _storageService = storageService;
    }

    public async Task<CreateDeliveryPersonOutput> Handle(
        CreateDeliveryPersonInput request,
        CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

        var existsDeliveryPersonWithCnpj = await _deliveryPersonRepository
            .ExistsDeliveryPersonWithCnpjAsync(
            request.DeliveryPerson.Cnpj,
            cancellationToken);

        if (existsDeliveryPersonWithCnpj)
        {
            return CreateDeliveryPersonOutput.CreateSameCnpjError();
        }

        var existsDeliveryPersonWithCnhNumber = await _deliveryPersonRepository
            .ExistsDeliveryPersonWithCnhNumberAsync(
            request.DeliveryPerson.CnhNumber,
            cancellationToken);

        if (existsDeliveryPersonWithCnhNumber)
        {
            return CreateDeliveryPersonOutput.CreateSameCnhNumberError();
        }

        await _storageService.CreateBlobFileAsync(
            $"{request.DeliveryPerson.Id}.png",
            ConvertBase64ToBytes(request.DeliveryPerson.CnhImage),
            cancellationToken);

        try
        {
            var cnpj = new Cnpj(request.DeliveryPerson.Cnpj);
            var cnh = new Cnh(request.DeliveryPerson.CnhNumber, request.DeliveryPerson.CnhType);
            var deliveryPerson = new DeliveryPerson(
                request.DeliveryPerson.Id,
                request.DeliveryPerson.Name,
                cnpj,
                request.DeliveryPerson.DateOfBirth,
                cnh,
                $"{request.DeliveryPerson.Id}.png");

            _deliveryPersonRepository.Add(deliveryPerson);
            await _deliveryPersonRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _storageService.DeleteBlobFileAsync(
                $"{request.DeliveryPerson.Id}.png",
                cancellationToken);
            throw;
        }

        return new CreateDeliveryPersonOutput();
    }

    private static byte[] ConvertBase64ToBytes(string base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String))
        {
            throw new ValidationException("The image string cannot be empty.");
        }

        var prefix = "data:image/png;base64,";
        if (base64String.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            base64String = base64String[prefix.Length..];
        }

        try
        {
            return Convert.FromBase64String(base64String);
        }
        catch (FormatException ex)
        {
            throw new ValidationException("The provided image is not a valid base64 string.", ex);
        }
    }
}
