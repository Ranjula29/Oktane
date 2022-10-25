using Oktane.Model;
using Oktane.Services;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.Services.Configure<DataBaseSetting>(
    builder.Configuration.GetSection("StationDatabase"));
}catch(Exception e){
    throw new Exception("database connection error " + e.Message);
}

builder.Services.AddSingleton<GasStationService>();
builder.Services.AddSingleton<StationQueService>();

builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
