using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using roomvision.infrastructure.Contexts;
using roomvision.presentation;
using roomvision.presentation.Middleware;

XmlConfigurator.Configure(new FileInfo("Log/log.xaml"));
LogicalThreadContext.Properties["CorrelationId"] = Guid.NewGuid().ToString();
ILog log = LogManager.GetLogger("SERVER");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddConfig();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContextPgSQL = services.GetRequiredService<PgSqlContext>();
    dbContextPgSQL.Database.Migrate();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<StartRequestMiddleware>();
app.UseMiddleware<EndRequestMiddleware>();

log.Info("Starting RoomVision");

app.Run();


