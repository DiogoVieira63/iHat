using iHat.Model;
using iHat.Model.iHatFacade;
using iHat.Model.Obras;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using iHat.Model.MensagensCapacete;
using iHat.MQTTService;
using iHat.Model.Zonas;
using Microsoft.AspNetCore.Http.Features;
using iHat.Model.Mapas;
using SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
    builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


builder.Services.AddControllers();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database"));

builder.Services.Configure<FormOptions>(options => {
    options.ValueCountLimit = 100 * 1024 * 1024;
    options.ValueLengthLimit = 100 * 1024 * 1024;
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
    options.MultipartHeadersLengthLimit = 100 * 1024 * 1024;
});

builder.Services.AddSingleton<IObrasService, ObrasService>();

builder.Services.AddSingleton<ICapacetesService, CapacetesService>();

builder.Services.AddSingleton<ILogsService, LogsService>();

builder.Services.AddSingleton<IMapaService, MapaService>();

builder.Services.AddSingleton<MensagemCapaceteService>();

builder.Services.AddSingleton<IZonasService,ZonasService> ();

builder.Services.AddSingleton<IiHatFacade, iHatFacade>();

builder.Services.AddSingleton<ManageNotificationClients>();

builder.Services.AddSingleton<MQTTService>();
builder.Services.AddHostedService<MQTTBackgroundService>();

builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// No futuro é preciso tratar a questão do https (https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio-code#test-the-project)

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();*/
app.UseCors("MyPolicy");

app.MapHub<ObrasHub>("obra");
app.MapHub<DadosCapaceteHub>("helmetdata");
app.MapControllers();

app.Run();
