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
        var channel = GrpcChannel.ForAddress(Environment.GetEnvironmentVariable("FACE_RECOGNITION_GRPC_URL")!);
        _client = new FaceRecognitionClient(channel);
    }

    public async Task<float[]> EncodeFaceAsync(byte[] imageData)
    {
        var image = Google.Protobuf.ByteString.CopyFrom(imageData);

        var request = new FaceRecognitionGrpc.EncodeRequest
        {
            Image = image
        };

        var response = await _client.EncodeFaceAsync(request);
        return response.Encoding.ToArray();
    }

}


