using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepoDb;
using VoisinUp.Configuration;
using VoisinUp.Hubs;
using VoisinUp.Repositories;
using VoisinUp.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔥 1. Ajouter le service CORS
builder.Services.AddCors(options => {
    options.AddPolicy(
        "AllowVue",
        policy => {
            policy.WithOrigins("http://localhost:5173") // Autorise Vue.js
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("missing jwtSettings")))
        };

        options.Events = new JwtBearerEvents {
            OnMessageReceived = context => {
                var origin = context.Request.Headers["Origin"].ToString();

                // 💥 Liste blanche des origines autorisées
                var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<String[]>();

                if (allowedOrigins == null) {
                    Console.WriteLine("appsettings.json is missing key AllowedOrigins");
                }

                if (allowedOrigins != null && !allowedOrigins.Contains(origin)) {
                    Console.WriteLine($"🚫❗ Origine refusée : {origin}");
                    context.NoResult(); // stoppe l’auth
                    return Task.CompletedTask;
                }

                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/tavernehub")) {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Active les Controllers pour les endpoints REST
builder.Services.AddControllers();

builder.Services.Configure<JsonOptions>(options => {
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Swagger to test and documente API 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    
    options.AddSecurityDefinition(
        "Bearer", new OpenApiSecurityScheme {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Entrez 'Bearer {token}' pour vous authentifier."
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement { {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );

});

// websocket / taverne
builder.Services.AddSignalR();


// Database
GlobalConfiguration.Setup().UsePostgreSql();

builder.Services.AddSingleton<DbConfig>();

builder.Services.AddScoped<QuestRepository>();
builder.Services.AddScoped<QuestService>();

builder.Services.AddScoped<VoisinageRepository>();
builder.Services.AddScoped<VoisinageService>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<QuestCategoryRepository>();
builder.Services.AddScoped<QuestCategoryService>();

builder.Services.AddScoped<AuthentificationService>();

builder.Services.AddScoped<TaverneRepository>();
builder.Services.AddScoped<TaverneService>();
builder.Services.AddHostedService<TaverneCleanupService>();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// 🔥 Activer CORS avant les Controllers
app.UseCors("AllowVue");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// Active les endpoints API
app.MapControllers();

// listen to the taverne
app.MapHub<TaverneHub>("/tavernehub").RequireCors("AllowVue");

app.Run();