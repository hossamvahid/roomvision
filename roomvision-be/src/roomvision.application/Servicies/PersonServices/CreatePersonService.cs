using System;
using roomvision.domain.Common;
using roomvision.application.Interfaces.Servicies.PersonServices;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.FaceRecognition;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.PersonServices;

public class CreatePersonService : ICreatePersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IFaceRecognition _faceRecognition;
    public CreatePersonService(IPersonRepository personRepository, IFaceRecognition faceRecognition)
    {
        _personRepository = personRepository;
        _faceRecognition = faceRecognition;
    }

    public async Task<Result> Execute(string personName, byte[] imageFile)
    {

        var foundPerson = await _personRepository.GetByNameAsync(personName);
        if (foundPerson is not null)
        {
            return Result.Failure("Person with the same name already exists.", ErrorTypes.Conflict);
        }

        var result = await _faceRecognition.EmbedFaceAsync(personName, imageFile);
        
        if(result.IsFailure)
        {
            return Result.Failure(result.Error!, result.ErrorType!.Value);
        }

        var person = new Person
        {
            Id = Guid.NewGuid().ToString(),
            Name = personName,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),       
        };
        await _personRepository.AddAsync(person);
        return Result.Success();
    }
}
