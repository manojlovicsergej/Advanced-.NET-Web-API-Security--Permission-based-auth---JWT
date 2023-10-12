using Application;
using Infrastructure;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIndentitySettings();
builder.Services.AddApplicationService();
builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(builder.Configuration));
builder.Services.AddIdentityServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterSwagger();

var app = builder.Build();
app.SeedDatabase();

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