using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entity;
using minimal_api.Domain.Enum;
using minimal_api.Domain.Interface;
using minimal_api.Domain.ModelsViews;
using minimal_api.Domain.Service;
using minimal_api.Resources;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;


#region Builder 
var builder = WebApplication.CreateBuilder(args);


var key = builder.Configuration.GetValue<string>("Jwt:Key");
if (string.IsNullOrEmpty(key))
    throw new Exception("A chave JWT não está configurada corretamente.");

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Adm"));
    options.AddPolicy("EditorPolicy", policy => policy.RequireRole("Adm", "Editor"));
});

builder.Services.AddScoped<AdminInterfaceService, AdminService>();
builder.Services.AddScoped<VehicleInterfaceService, VehicleService>();
// Add services to the container.

builder.Services.AddDbContext<DbRepository>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
        );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});


var app = builder.Build();

#endregion




// Configure the HTTP request pipeline.
#region Default
app.MapGet("/", () => Results.Json(new Default())).WithTags("Default");
#endregion

#region Administradores
string GerarTokenJwt(AdminEntity administrador)
{
    if (string.IsNullOrEmpty(key))
        return string.Empty;
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}



app.MapPost("/admins/login", ( [FromBody] LoginDTO loginDto, AdminInterfaceService adminService) =>
{
    var adm = adminService.Login(loginDto);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdministradorLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();
} ).AllowAnonymous().WithTags("Admin");


app.MapPost("/admins/register",   ( [FromBody] AdminDTO adminDto, AdminInterfaceService adminService) =>
{var validacao = new ExceptionValidation{
                    Mensagens = new List<string>()
                };

                if(string.IsNullOrEmpty(adminDto.Email))
                    validacao.Mensagens.Add("Email não pode ser vazio");
                if(string.IsNullOrEmpty(adminDto.Password))
                    validacao.Mensagens.Add("Senha não pode ser vazia");
                if(adminDto.Perfil == null)
                    validacao.Mensagens.Add("Perfil não pode ser vazio");

                if(validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);
                
                var admin = adminService.AddAdmin(adminDto);

                if(admin == null)
                    return Results.BadRequest(new { Message = "Erro ao criar administrador" });

                return Results.Created($"/administrador/{admin.Id}", new AdminModelView{
                    Id = admin.Id,
                    Email = admin.Email,
                    Perfil = admin.Perfil
                });
            }).RequireAuthorization("AdminPolicy").WithTags("Admin");


app.MapGet("/admins", ([FromQuery] int? pagina, AdminInterfaceService adminService) => {
                var adms = new List<AdminModelView>();
                var administradores = adminService.GetAllAdmins();
                foreach(var adm in administradores)
                {
                    adms.Add(new AdminModelView{
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
                }
                return Results.Ok(adms);
            }).RequireAuthorization("AdminPolicy").WithTags("Admin");


 app.MapGet("/admins/{id}", ([FromRoute] int id, AdminInterfaceService adminService) => {
                var administrador = adminService.GetAdminById(id);
                if(administrador == null) return Results.NotFound();
                return Results.Ok(new AdminModelView{

                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil
                });
            }).RequireAuthorization("AdminPolicy").WithTags("Admin");

#endregion


#region Veiculos

ExceptionValidation validaDTO(VehicleDTO vehicleDTO)
{
    var validacao = new ExceptionValidation();

    if (string.IsNullOrEmpty(vehicleDTO.Nome))
        validacao.Mensagens.Add("Nome do veículo não pode ser vazio");
    
    if (string.IsNullOrEmpty(vehicleDTO.Marca))
        validacao.Mensagens.Add("Marca do veículo não pode ser vazia");
    
    if (vehicleDTO.Ano <= 0)
        validacao.Mensagens.Add("Ano do veículo deve ser maior que zero");

    return validacao;
}

app.MapPost("/veiculos", ( [FromBody] VehicleDTO vehicleDTO, VehicleInterfaceService vehicleService) =>
{

   


    var validar = validaDTO(vehicleDTO);

    if(validar.Mensagens.Count > 0)
    {
        return Results.BadRequest( validar);
    }


    var vehicle = new VehicleEntity{
        Nome = vehicleDTO.Nome,
        Marca = vehicleDTO.Marca,
        Ano = vehicleDTO.Ano,

    };

    vehicleService.AddVehicle(vehicle);


    return Results.Created($"/veiculo/{vehicle.Id}", vehicle);


} ).RequireAuthorization("EditorPolicy").WithTags("Veiculo");



app.MapGet("/veiculos", (int? pagina, string? nome, string? marca, int? ano, VehicleInterfaceService vehicleService) =>
{   var veiculos = vehicleService.GetAllVehicles(pagina ?? 1, nome, marca, ano);

    return Results.Ok(veiculos);
} ).WithTags("Veiculo");


app.MapGet("/veiculos/{id}", (int id, VehicleInterfaceService vehicleService) =>{
    var veiculo = vehicleService.GetVehicleById(id);

    if (veiculo == null)
    {
        return Results.NotFound(new { Message = "Veiculo não encontrado" });
    }
    return Results.Ok(veiculo);

} ).WithTags("Veiculo");    



app.MapPut("/veiculos/{id}", ( [FromBody] VehicleDTO vehicleDTO, int id, VehicleInterfaceService vehicleService) =>
{

    
    var validar = validaDTO(vehicleDTO);

    if(validar.Mensagens.Count > 0)
    {
        return Results.BadRequest( validar);
    }

    var veiculo = vehicleService.GetVehicleById(id);

    if (veiculo == null)

    {
        return Results.NotFound(new { Message = "Veiculo não encontrado" });
    }

    veiculo.Nome = vehicleDTO.Nome;
    veiculo.Marca = vehicleDTO.Marca;
    veiculo.Ano = vehicleDTO.Ano;

    vehicleService.UpdateVehicle(veiculo);


    return Results.Ok(new { Message = "Veiculo atualizado com sucesso" });
} ).WithTags("Veiculo");



app.MapDelete("/veiculos/{id}", (int id, VehicleInterfaceService vehicleService) =>
{
    var veiculo = vehicleService.GetVehicleById(id);

    if (veiculo == null)
    {
        return Results.NotFound(new { Message = "Veiculo não encontrado" });
    }

    vehicleService.DeleteVehicle(id);

    return Results.Ok(new { Message = "Veiculo deletado com sucesso" });
} ).WithTags("Veiculo");
#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();


app.Run();

#endregion