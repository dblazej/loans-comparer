using dotNetAPI.Entity;
using dotNetAPI.Repository;
using dotNetAPI.Service;
using dotNetAPI.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICarStatusService, CarHistoryService>();
builder.Services.AddScoped<ICarStatusRepository, CarStatusRepository>();
builder.Services.AddScoped<IPriceCalculator, PriceCalculator>();
builder.Services.AddScoped<IVehicleQuoteRepository, VehicleQuoteRepository>();
builder.Services.AddScoped<ICarStatusRepository, CarStatusRepository>();

builder.Services.AddHttpClient();

var connectionString = builder.Configuration.GetConnectionString("AppDb");
builder.Services.AddDbContext<CarRentalDbContext>(x => x.UseSqlServer(connectionString));

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options => {
options.AddPolicy(name: MyAllowSpecificOrigins, builder => {
builder.WithOrigins("https://frontendminicarrental.azurewebsites.net").AllowAnyHeader(); }); });

var app = builder.Build();

builder.Services.AddControllers();

app.UseCors(MyAllowSpecificOrigins);


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }