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

        var (fileName, fileBytes) = ConvertBase64ToBytes(
            request.DeliveryPerson.CnhImage,
            request.DeliveryPerson.Id);

        await _storageService.CreateBlobFileAsync(fileName, fileBytes, cancellationToken);

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
                fileName);

            _deliveryPersonRepository.Add(deliveryPerson);
            await _deliveryPersonRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _storageService.DeleteBlobFileAsync(
                fileName,
                cancellationToken);
            throw;
        }

        return new CreateDeliveryPersonOutput();
    }

    private static (string fileName, byte[] fileBytes) ConvertBase64ToBytes(string base64String, string id)
    {
        if (string.IsNullOrWhiteSpace(base64String))
        {
            throw new ValidationException("The image string cannot be empty.");
        }

        var prefixMap = new Dictionary<string, string>
        {
            { "data:image/png;base64,", "png" },
            { "data:image/bmp;base64,", "bmp" }
        };

        var prefix = prefixMap.Keys
            .FirstOrDefault(p => base64String.StartsWith(p, StringComparison.OrdinalIgnoreCase));

        var cleanBase64 = prefix == null ? base64String : base64String[prefix.Length..];

        try
        {
            var fileBytes = Convert.FromBase64String(cleanBase64);
            var extension = prefix == null ? "png" : prefixMap[prefix];
            var fileName = $"{id}.{extension}";
            return (fileName, fileBytes);
        }
        catch (FormatException ex)
        {
            throw new ValidationException("The provided image is not a valid base64 string.", ex);
        }
    }
}
