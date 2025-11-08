using System;

namespace roomvision.domain.Interfaces.FaceRecognition;

public interface IFaceRecognition
{
    public Task<float[]> EncodeFaceAsync(byte[] imageData);
}
