using System.Reflection;
using RepoDb;
using VoisinUp.Configuration;
using VoisinUp.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔥 1. Ajouter le service CORS
builder.Services.AddCors(options => {
    options.AddPolicy(
        "AllowVue",
        policy => {
            policy.WithOrigins("http://localhost:5173") // Autorise Vue.js
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Active les Controllers pour les endpoints REST
builder.Services.AddControllers();

// Swagger to test and documente API 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Database
GlobalConfiguration.Setup().UsePostgreSql();

builder.Services.AddSingleton<DbConfig>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

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

app.UseStaticFiles();

// 🔥 Activer CORS avant les Controllers
app.UseCors("AllowVue");

// Active les endpoints API
app.MapControllers();

app.Run();