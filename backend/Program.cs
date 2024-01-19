using iHat.Model;
using iHat.Model.iHatFacade;
using iHat.Model.Obras;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using iHat.Model.MensagensCapacete;
using iHat.Model.MQTTService;
using Microsoft.AspNetCore.Http.Features;
using iHat.Model.Mapas;
using SignalR.Hubs;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "MyPolicy",
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
builder.Services.AddSingleton<IMensagemCapaceteService, MensagemCapaceteService>();
builder.Services.AddSingleton<IiHatFacade, iHatFacade>();
builder.Services.AddSingleton<ManageNotificationClients>();
builder.Services.AddSingleton<MQTTService>();
builder.Services.AddHostedService<MQTTBackgroundService>();
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo{
        Version = "v1",
        Title = "iHat Backend"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();
app.UseCors("MyPolicy");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapHub<ObrasHub>("obra");
app.MapHub<DadosCapaceteHub>("helmetdata");
app.Run();
