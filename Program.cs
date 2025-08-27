using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Model.DTO;
using minimal_api.Model.Interface;
using minimal_api.Model.Service;
using minimal_api.Resources;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<AdminInterfaceService, AdminService>();
// Add services to the container.

builder.Services.AddDbContext<DbRepository>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
        );
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/", () => "ccc");

app.MapPost("/login", ( [FromBody] LoginDTO loginDto, AdminInterfaceService adminService) =>
{
    if (adminService.Login(loginDto) != null)
    {
        return Results.Ok(new { Message = "Login successful" });
    }
    else
    {
        return Results.Unauthorized();
    }
} );

app.MapGet("/admins", () => { });

app.Run();

