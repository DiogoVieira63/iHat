using iHat.Model;
using iHat.Model.iHatFacade;
using iHat.Model.Obras;
using iHat.Model.Capacetes;
using iHat.Model.Logs;

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

builder.Services.AddSingleton<IObrasService, ObrasService>();

builder.Services.AddSingleton<ICapacetesService, CapacetesService>();

builder.Services.AddSingleton<ILogsService, LogsService>();

builder.Services.AddSingleton<IiHatFacade, iHatFacade>();

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

app.MapControllers();

app.Run();
