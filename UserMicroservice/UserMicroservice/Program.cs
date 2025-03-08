using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserMicroservice.Modules.Users.Application.EditUser;
using UserMicroservice.Modules.Users.Application.GetOneUserById;
using UserMicroservice.Modules.Users.Application.Login;
using UserMicroservice.Modules.Users.Application.Register;
using UserMicroservice.Modules.Users.Domain;
using UserMicroservice.Modules.Users.Infrastructure.Services;
using UserMicroservice.Modules.Users.Infrastructure.SqlServer;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserMicroservice API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa 'Bearer' [espacio] y tu token JWT.\nEjemplo: Bearer eyJhbGciOiJIUzI1NiIsInR... ",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("SqlServerConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Register services
builder.Services.AddScoped<IUserRepository, SqlServerRepositoryImpl>();
builder.Services.AddScoped<RegisterUseCase>();
builder.Services.AddScoped<GetOneUserByIdUseCase>();
builder.Services.AddScoped<EditUserUseCase>();
builder.Services.AddScoped<LoginUseCase>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>(); // Add the new TokenService
builder.Services.AddSingleton<TokenBlacklistService>();

var jwtKey = builder.Configuration["Jwt:Key"];
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        NameClaimType = JwtRegisteredClaimNames.Sub,
        RoleClaimType = JwtRegisteredClaimNames.Jti,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            Console.WriteLine($"Authorization header: {context.Request.Headers["Authorization"]}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Get the token blacklist service
            var tokenBlacklistService = context.HttpContext.RequestServices
                .GetRequiredService<UserMicroservice.Modules.Users.Infrastructure.Services.TokenBlacklistService>();

            // Extract the token from the Authorization header
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Check if the token is blacklisted
                if (tokenBlacklistService.IsTokenBlacklisted(token))
                {
                    // If blacklisted, fail the authentication
                    context.Fail("Token has been revoked");
                    return Task.CompletedTask;
                }
            }

            var claims = context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
            Console.WriteLine($"Token validated with claims: {string.Join(", ", claims)}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            Console.WriteLine($"Authentication failed stack trace: {context.Exception.StackTrace}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"Challenge issued: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny", app =>
    {
        app.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Después de construir la aplicación pero antes de ejecutarla
var app = builder.Build();

// Aplicar migraciones automáticamente al iniciar
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
        app.Logger.LogError(ex, "An error occurred while migrating the database");
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
