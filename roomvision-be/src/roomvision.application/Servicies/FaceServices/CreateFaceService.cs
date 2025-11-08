using System;
using roomvision.application.Common;
using roomvision.application.Interfaces.Servicies.FaceServices;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.FaceRecognition;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.FaceServices;

public class CreateFaceService : ICreateFaceService
{
    private readonly IFaceRepository _faceRepository;
    private readonly IFaceRecognition _faceRecognition;
    public CreateFaceService(IFaceRepository faceRepository, IFaceRecognition faceRecognition)
    {
        _faceRepository = faceRepository;
        _faceRecognition = faceRecognition;
    }

    public async Task<Result> Execute(string personName, byte[] imageFile)
    {
        var faceEncoded = await _faceRecognition.EncodeFaceAsync(imageFile);

        if (faceEncoded == null || faceEncoded.Length == 0)
        {
            return Result.Failure("No face detected in the image.", ErrorTypes.NotFound);
        }

        var face = new Face
        {
            Id = Guid.NewGuid().ToString(),
            PersonName = personName,
            Encoding = faceEncoded
        };
        await _faceRepository.AddAsync(face);
        return Result.Success();
    }
}
