using System;
using roomvision.application.Models;
using roomvision.domain.Common;

namespace roomvision.application.Interfaces.Servicies.RoomServices;

public interface IGetRooms
{
    public Task<Result<Paginated<RoomWithScan>>> Execute(int page, int pageSize = 6);
}
