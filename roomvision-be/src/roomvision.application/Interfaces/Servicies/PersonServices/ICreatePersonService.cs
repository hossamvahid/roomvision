using System;
using roomvision.domain.Common;

namespace roomvision.application.Interfaces.Servicies.PersonServices;

public interface ICreatePersonService
{
    public Task<Result> Execute(string personName, byte[] imageFile);
}
