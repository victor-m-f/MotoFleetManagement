using Mfm.Application.Helpers;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Repositories;
using Mfm.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.DeliveryPersons.UpdateDeliveryPersonCnhImage;

internal sealed class UpdateDeliveryPersonCnhImageUseCase : UseCaseBase, IUpdateDeliveryPersonCnhImageUseCase
{
    private const string CnhStorageContainer = "upload-images";

    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly IStorageService _storageService;

    public UpdateDeliveryPersonCnhImageUseCase(
        ILogger<UpdateDeliveryPersonCnhImageUseCase> logger,
        IDeliveryPersonRepository deliveryPersonRepository,
        IStorageService storageService)
        : base(logger)
    {
        _deliveryPersonRepository = deliveryPersonRepository;
        _storageService = storageService;
    }

    public async Task<UpdateDeliveryPersonCnhImageOutput> Handle(
        UpdateDeliveryPersonCnhImageInput request,
        CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(request.Id, cancellationToken);

        if (deliveryPerson == null)
        {
            return UpdateDeliveryPersonCnhImageOutput.CreateNotFoundError(request.Id);
        }

        var (fileName, fileBytes) = ImageHelper.ConvertBase64ToBytes(
            request.CnhImage,
            request.Id);

        await _storageService.CreateBlobFileAsync(fileName, fileBytes, cancellationToken);

        return new UpdateDeliveryPersonCnhImageOutput();
    }
}
