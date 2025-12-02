using System;
using roomvision.domain.Common;
using roomvision.domain.ValueObjects;

namespace roomvision.domain.Interfaces.FaceRecognition;

public interface IFaceRecognition
{
    public Task<Result> EmbedFaceAsync(string personName,byte[] imageData);
    public Task<Result<RoomScanResult>> VerifyRoomAsync(byte[] imageData);
}
