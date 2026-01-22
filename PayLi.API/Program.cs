using PayLi.API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// OpenAPI built-in
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IBridgeCardService, BridgeCardService>();
builder.Services.AddScoped<IBridgeCardService, BridgeCardService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // ????? OpenAPI 
    app.MapOpenApi();                       // /openapi/v1.json

    // ????? UI ??? API (???? Swagger UI)
    app.MapScalarApiReference(options =>
    {
        options.Title = "PayLi API";
        options.Servers = [new("https://localhost:5001")];
    });
    // Scalar UI ????? ????? ??? /scalar
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
