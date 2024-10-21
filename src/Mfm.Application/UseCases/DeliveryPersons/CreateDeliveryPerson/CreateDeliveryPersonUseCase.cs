using Mfm.Application.Helpers;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
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

        var (fileName, fileBytes) = ImageHelper.ConvertBase64ToBytes(
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
}
