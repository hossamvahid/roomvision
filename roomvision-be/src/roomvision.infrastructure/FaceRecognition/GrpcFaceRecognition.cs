using System;
using Grpc.Net.Client;
using roomvision.domain.Interfaces.FaceRecognition;
using FaceRecognitionClient = FaceRecognitionGrpc.FaceRecognition.FaceRecognitionClient;
namespace roomvision.infrastructure.FaceRecognition;

public class GrpcFaceRecognition : IFaceRecognition
{
    private readonly FaceRecognitionClient _client;

    public GrpcFaceRecognition(FaceRecognitionClient client)
    {
        _client = client;
    }

    public async Task<float[]> EncodeFaceAsync(byte[] imageData)
    {
        var image = Google.Protobuf.ByteString.CopyFrom(imageData);

        var request = new FaceRecognitionGrpc.Image
        {
            Image_ = image
        };

        var response = await _client.EncodeFaceAsync(request);
        return response.Encoding.ToArray();
    }

}


