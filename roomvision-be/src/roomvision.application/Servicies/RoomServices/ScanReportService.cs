using log4net;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using roomvision.application.Interfaces.Servicies.RoomServices;
using roomvision.domain.Common;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.RoomServices;

public class ScanReportService : IScanReportService
{
    private readonly IScannedRoomRepository _scannedRoomRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ILog _log;

    public ScanReportService(IScannedRoomRepository scannedRoomRepository, IRoomRepository roomRepository, ILog log)
    {
        _scannedRoomRepository = scannedRoomRepository;
        _roomRepository = roomRepository;
        _log = log;
    }

    public async Task<Result<byte[]>> Execute(string roomName)
    {
        _log.Info($"Generating scan report PDF for room: {roomName}");

        var roomExists = await _roomRepository.GetByRoomNameAsync(roomName);
        if (roomExists is null)
        {
            _log.Warn($"Room with name {roomName} does not exist.");
            return Result<byte[]>.Failure($"Room with name {roomName} does not exist.", ErrorTypes.NotFound);
        }

        var scannedRoom = await _scannedRoomRepository.GetScannedRoomResult(roomName);
        if (scannedRoom is null)
        {
            _log.Warn($"No scanned result found for room {roomName}.");
            return Result<byte[]>.Failure($"No scanned result found for room {roomName}.", ErrorTypes.NotFound);
        }

        QuestPDF.Settings.License = LicenseType.Community;

        var identifiedFaces = scannedRoom.IdentifiedFaces ?? new List<string>();
        var knownFaces = identifiedFaces.Where(f => f != "Unknown").ToList();
        var unknownCount = identifiedFaces.Count(f => f == "Unknown");

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Column(col =>
                {
                    col.Item().BorderBottom(1).BorderColor("#e2e8f0").PaddingBottom(12).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("RoomVision").FontSize(20).Bold().FontColor("#0f172a");
                            c.Item().Text("Raport scanare cameră").FontSize(12).FontColor("#64748b");
                        });
                        row.ConstantItem(120).AlignRight().Column(c =>
                        {
                            c.Item().Text(DateTime.UtcNow.ToString("dd.MM.yyyy")).FontSize(10).FontColor("#64748b");
                            c.Item().Text(DateTime.UtcNow.ToString("HH:mm")).FontSize(10).FontColor("#64748b");
                        });
                    });
                });

                page.Content().PaddingTop(24).Column(col =>
                {

                    col.Item().Background("#f8fafc").Border(1).BorderColor("#e2e8f0").Padding(16).Column(info =>
                    {
                        info.Item().Text("Informații cameră").FontSize(13).Bold().FontColor("#0f172a");
                        info.Item().PaddingTop(8).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Cameră: {scannedRoom.RoomName}").FontColor("#334155");
                                c.Item().PaddingTop(4).Text($"Ultima scanare: {scannedRoom.ScannedAt:dd.MM.yyyy HH:mm}").FontColor("#334155");
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Total persoane: {scannedRoom.TotalFaces}").FontColor("#334155");
                                c.Item().PaddingTop(4).Text($"Necunoscuți: {unknownCount}").FontColor("#ef4444");
                            });
                        });
                    });


                    col.Item().PaddingTop(24).Column(section =>
                    {
                        section.Item().Text("Persoane identificate").FontSize(13).Bold().FontColor("#0f172a");
                        section.Item().PaddingTop(4).Text($"{knownFaces.Count} persoană/persoane recunoscute").FontSize(10).FontColor("#64748b");
                        section.Item().PaddingTop(12).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#0f172a").Padding(8).Text("#").FontColor("#ffffff").Bold();
                                header.Cell().Background("#0f172a").Padding(8).Text("Nume").FontColor("#ffffff").Bold();
                            });

                            if (knownFaces.Count == 0)
                            {
                                table.Cell().ColumnSpan(2).Padding(8).Background("#f8fafc")
                                    .Text("Nicio persoană identificată").FontColor("#94a3b8").Italic();
                            }
                            else
                            {
                                for (int i = 0; i < knownFaces.Count; i++)
                                {
                                    var bg = i % 2 == 0 ? "#ffffff" : "#f8fafc";
                                    table.Cell().Background(bg).Padding(8).Text($"{i + 1}").FontColor("#334155");
                                    table.Cell().Background(bg).Padding(8).Text(knownFaces[i]).FontColor("#334155");
                                }
                            }
                        });
                    });


                    if (unknownCount > 0)
                    {
                        col.Item().PaddingTop(24).Column(section =>
                        {
                            section.Item().Text("Persoane neidentificate").FontSize(13).Bold().FontColor("#0f172a");
                            section.Item().PaddingTop(4).Text($"{unknownCount} față/fețe nerecunoscute în sistem").FontSize(10).FontColor("#64748b");
                            section.Item().PaddingTop(12).Background("#fff7ed").Border(1).BorderColor("#fed7aa").Padding(12)
                                .Text($"⚠  {unknownCount} persoană/persoane prezente în cameră nu au fost recunoscute de sistem.")
                                .FontColor("#c2410c");
                        });
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("RoomVision  •  Generat automat  •  Pagina ").FontColor("#94a3b8").FontSize(9);
                    text.CurrentPageNumber().FontColor("#94a3b8").FontSize(9);
                    text.Span(" din ").FontColor("#94a3b8").FontSize(9);
                    text.TotalPages().FontColor("#94a3b8").FontSize(9);
                });
            });
        }).GeneratePdf();

        _log.Info($"PDF generated successfully for room: {roomName}");
        return Result<byte[]>.Success(pdfBytes);
    }
}
