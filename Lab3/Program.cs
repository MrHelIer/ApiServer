using Lab3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer              = true,
        ValidateAudience            = true,
        ValidateLifetime            = true,
        ValidateIssuerSigningKey    = true,

        ValidIssuer         = AuthOptions.ISSUER,
        ValidAudience       = AuthOptions.AUDIENCE,
        IssuerSigningKey    = AuthOptions.GetSymmetricSecurityKey()
    };
});

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
IServiceCollection serviceCollection = builder.Services.AddDbContext<ModelDB>(options => options.UseSqlServer(connection));
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapPost("/login", async (User loginData, ModelDB db) =>
{
    User? person = await db.Users!.FirstOrDefaultAsync(p => p.EMail == loginData.EMail &&
p.Password == loginData.Password);
    if (person is null) return Results.Unauthorized();
    var claims = new List<Claim> { new Claim(ClaimTypes.Email, person.EMail!) };
    var jwt = new JwtSecurityToken(issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        claims: claims,
        expires: DateTime.Now.Add(TimeSpan.FromMinutes(2)),
        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );
    var encoderJWT = new JwtSecurityTokenHandler().WriteToken(jwt);
    var response = new
    {
        access_token    = encoderJWT,
        username        = person.EMail
    };
    return Results.Json(response);
});

app.MapGet("/api/tariffs", [Authorize] async (ModelDB db) => await db.Tariffs!.ToListAsync());
app.MapGet("/api/tariffs/{name}", [Authorize] async (ModelDB db, string name) => await db.Tariffs!.Where(t => t.Name == name).ToListAsync());
app.MapGet("/api/tariffs/{id:int}", [Authorize] async (ModelDB db, int id) => await db.Tariffs!.Where(t => t.Id == id).FirstAsync());
app.MapPost("/api/tariffs", [Authorize] async (Tariff tariff, ModelDB db) =>
{
    await db.Tariffs!.AddAsync(tariff);
    await db.SaveChangesAsync();
    return tariff;
});
app.MapDelete("/api/tariffs/{id:int}", [Authorize] async (int id, ModelDB db) =>
{
    Tariff? tariff = await db.Tariffs!.FirstOrDefaultAsync(t => t.Id == id);
    if (tariff == null) return Results.NotFound(new { message = "Тариф не найден" });
    db.Tariffs!.Remove(tariff);
    await db.SaveChangesAsync();
    return Results.Json(tariff);
});
app.MapPut("/api/tariffs", [Authorize] async (Tariff tariff, ModelDB db) =>
{
    Tariff? a = await db.Tariffs!.FirstOrDefaultAsync(u => u.Id == tariff.Id);
    if (a == null) return Results.NotFound(new { message = "Ассортимент не найден" });

    tariff.Name             = a.Name;
    tariff.Cost             = a.Cost;
    tariff.DiagnosisCode    = a.DiagnosisCode;
    tariff.DiagnosisName    = a.DiagnosisName;

    await db.SaveChangesAsync();
    return Results.Json(a);
});

app.MapGet("/api/patients", [Authorize] async (ModelDB db) => await db.Patients!.ToListAsync());
app.MapPost("/api/patients", [Authorize] async (Patient patient, ModelDB db) =>
{
    await db.Patients!.AddAsync(patient);
    await db.SaveChangesAsync();
    return patient;
});
app.MapDelete("/api/patients/{id:int}", [Authorize] async (int id, ModelDB db) =>
{
    Patient? patient = await db.Patients!.FirstOrDefaultAsync(u => u.Id == id);
    if (patient == null) return Results.NotFound(new { message = "Регистрация не найдена" });
    db.Patients!.Remove(patient);
    await db.SaveChangesAsync();
    return Results.Json(patient);
});
app.MapPut("/api/patients", [Authorize] async ([FromBody] Patient patient, ModelDB db) =>
{
    Patient? pat = await db.Patients!.FirstOrDefaultAsync(u => u.Id == patient.Id);
    if (pat == null) return Results.NotFound(new { message = "Регистрация не найдена" });
    pat.Id              = patient.Id;
    pat.FullName        = patient.FullName;
    pat.Gender          = patient.Gender;
    pat.Age             = patient.Age;
    pat.PlaceOfResidence= patient.PlaceOfResidence;
    pat.DiagnosisCode   = patient.DiagnosisCode;
    pat.DiagnosisName   = patient.DiagnosisName;
    pat.Days            = patient.Days;
    pat.Cost            = patient.Cost;
    pat.TariffId        = patient.TariffId;
    await db.SaveChangesAsync();
    return Results.Json(pat);
});
app.Run();