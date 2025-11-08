using System;
using roomvision.application.Common;

namespace roomvision.application.Interfaces.Servicies.FaceServices;

public interface ICreateFaceService
{
    public Task<Result> Execute(string personName, byte[] imageFile);
}
