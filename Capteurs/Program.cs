using System.Reflection;
using AspNetCoreRateLimit;
using Capteurs.Data;
using Capteurs.Filters;
using Capteurs.Services;
using Capteurs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Capteur API",
        Version = "v1",
        Description = "API pour la gestion des capteurs.",
        Contact = new OpenApiContact
        {
            Name = "Mohamed ROUIDI",
            Email = "rouidi.med@gmail.com"
        }
    });

    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "Capteur API",
        Description = "API pour la gestion des capteurs - Version 2 (améliorée)",
        Contact = new OpenApiContact
        {
            Name = "Mohamed ROUIDI",
            Email = "rouidi.med@gmail.com"
        }
    });

    // Include XML comments for better documentation (optional)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);


    // Configurer la sécurité API Key
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Clé API requise dans l'en-tête. Exemple: \"X-Api-Key: {votre_cle}\"",
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKey"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICapteurService, CapteurService>();
// Enregistrer le filtre personnalisé
builder.Services.AddScoped<ApiKeyAuthAttribute>();
//Forcer l'api a utiliser Https.
builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps());
});

// Enregistrer les services pour limiter les domaines autorisés (CORS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", builder =>
    {
        builder.WithOrigins("https://localhost:5001")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddOptions();
builder.Services.AddMemoryCache();
//Enregistrer les services pour limiter le nombre de requêtes (Rate Limiting) 
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();


var app = builder.Build();

app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestion des Capteurs - API - version1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Gestion des Capteurs - API - version2");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program() { }
