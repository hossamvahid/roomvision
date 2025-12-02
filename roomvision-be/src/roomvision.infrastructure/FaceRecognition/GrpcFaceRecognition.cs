using System;
using Grpc.Net.Client;
using roomvision.domain.Common;
using roomvision.domain.Interfaces.FaceRecognition;
using roomvision.domain.ValueObjects;
using FaceRecognitionClient = FaceRecognitionGrpc.FaceRecognition.FaceRecognitionClient;
namespace roomvision.infrastructure.FaceRecognition;

public class GrpcFaceRecognition : IFaceRecognition
{
    private readonly FaceRecognitionClient _client;

    public GrpcFaceRecognition(FaceRecognitionClient client)
    {
        _client = client;
    }

    public async Task<Result> EmbedFaceAsync(string personName,byte[] imageData)
    {
        var image = Google.Protobuf.ByteString.CopyFrom(imageData);

        var request = new FaceRecognitionGrpc.Face
        {
            PersonName = personName,
            Image = image
        };

        var response = await _client.EmbedFaceAsync(request);

        if(response.Embedding.Count == 0)
        {
           return Result.Failure("No face detected in the image.", ErrorTypes.NotFound);
        }
        return Result.Success();
    }

    public async Task<Result<RoomScanResult>> VerifyRoomAsync(byte[] imageData)
    {
        var image = Google.Protobuf.ByteString.CopyFrom(imageData);

        var request = new FaceRecognitionGrpc.Image
        {
            Image_ = image
        };

        var response = await _client.VerifyRoomAsync(request);

        var identifiedFaces = response.Persons.ToList();
        var totalFaces = response.Total;

        var roomScanResult = new RoomScanResult(identifiedFaces, totalFaces);

        return Result<RoomScanResult>.Success(roomScanResult);
    }

}


