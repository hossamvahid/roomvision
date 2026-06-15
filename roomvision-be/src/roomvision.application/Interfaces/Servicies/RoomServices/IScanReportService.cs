using roomvision.domain.Common;

namespace roomvision.application.Interfaces.Servicies.RoomServices;

public interface IScanReportService
{
    Task<Result<byte[]>> Execute(string roomName);
}
