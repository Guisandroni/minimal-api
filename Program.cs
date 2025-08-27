using minimal_api.Model.DTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.


app.MapGet("/", () => "ccc");

app.MapPost("/login", (LoginDTO loginDto) =>
{
    if (loginDto.Email == "example@teste.com" && loginDto.Password == "password123")
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

