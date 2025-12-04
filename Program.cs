using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepoDb;
using VoisinUp.Configuration;
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
    options.AddPolicy(
        "AllowUnity",
        policy => {
            policy
                .WithOrigins("http://localhost:80") // Autorise Unity
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// TODO : when deploying on prod, stop listening to port 80 and setup SSL with port 443

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
                var accessToken = context.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(accessToken)) {
                    context.Token = accessToken;
                }

                else {
                    Console.WriteLine("no token or empty");
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx => {
                Console.WriteLine("[AUTH FAILED] " + ctx.Exception.Message);
                return Task.CompletedTask;
            },
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

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);

//if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
//}

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

app.Run();