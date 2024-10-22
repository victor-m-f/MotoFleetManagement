using FluentAssertions;
using Mfm.Application.Dtos.DeliveryPersons;
using Mfm.Application.UseCases.DeliveryPersons.CreateDeliveryPerson;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Repositories;
using Mfm.Domain.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Mfm.Application.UnitTests.UseCases.DeliveryPersons;
public sealed class CreateDeliveryPersonUseCaseTests
{
    [Fact]
    public async Task Handle_ShouldAddDeliveryPerson_WhenCalledWithValidData()
    {
        // Arrange
        var repository = Substitute.For<IDeliveryPersonRepository>();
        var storageService = Substitute.For<IStorageService>();

        var request = new CreateDeliveryPersonInput(
            new DeliveryPersonDto
            {
                Id = "deliveryperson-id",
                Name = "John Doe",
                Cnpj = "12345678000195",
                DateOfBirth = new DateTime(1990, 5, 20),
                CnhNumber = "12345678910",
                CnhType = CnhType.A,
                CnhImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/Kh0fQAAAABJRU5ErkJggg=="
            });

        var useCase = new CreateDeliveryPersonUseCase(
            Substitute.For<ILogger<CreateDeliveryPersonUseCase>>(),
            repository,
            storageService);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        repository.Received(1).Add(Arg.Is<DeliveryPerson>(x =>
            x.Id == request.DeliveryPerson.Id &&
            x.Name == request.DeliveryPerson.Name &&
            x.Cnpj.Value == request.DeliveryPerson.Cnpj &&
            x.DateOfBirth == request.DeliveryPerson.DateOfBirth &&
            x.Cnh.Number == request.DeliveryPerson.CnhNumber &&
            x.Cnh.Type == request.DeliveryPerson.CnhType &&
            x.CnhImageUrl == "deliveryperson-id.png"));

        await storageService
            .Received(1)
            .CreateBlobFileAsync(
            $"{request.DeliveryPerson.Id}.png",
            Arg.Any<byte[]>(),
            cancellationToken);

        await repository.Received(1)
            .SaveChangesAsync(cancellationToken);

        result.Should().BeOfType<CreateDeliveryPersonOutput>();
    }
}
