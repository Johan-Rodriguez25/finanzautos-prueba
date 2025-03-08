using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PublicationMicroservice.Modules.Publications.CreatePublication;
using PublicationMicroservice.Modules.Publications.DeletePublication;
using PublicationMicroservice.Modules.Publications.Domain;
using PublicationMicroservice.Modules.Publications.GetPublicationById;
using PublicationMicroservice.Modules.Publications.GetPublicationsByUserId;
using PublicationMicroservice.Modules.Publications.Infrastructure.Services;
using PublicationMicroservice.Modules.Publications.Infrastructure.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PublicationMicroservice API", Version = "v1" });
});

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
    builder.Configuration.GetConnectionString("SqlServerConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IPublicationRepository, SqlServerRepositoryImpl>();
builder.Services.AddScoped<CreatePublicationUseCase>();
builder.Services.AddScoped<DeletePublicationUseCase>();
builder.Services.AddScoped<GetPublicationByIdUseCase>();
builder.Services.AddScoped<GetPublicationsByUserIdUseCase>();

builder.Services.AddScoped<PublicationService>();

builder.Services.AddHttpClient<TokenValidationService>();
builder.Services.AddScoped<TokenValidationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny", app =>
    {
        app.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        app.Logger.LogInformation("Database migrated successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAny");

app.UseAuthorization();

app.MapControllers();

app.Run();
