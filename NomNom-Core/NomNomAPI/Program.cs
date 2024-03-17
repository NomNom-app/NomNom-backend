#define USE_SWAGGER_UI

using System.Text;

using NomNomAPI.Services.Users;
using NomNomAPI.Auth;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
{

    // Add services to the container.
    builder.Services.AddControllers();

    var jwtSettings = new JwtSettings();
    builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);
    builder.Services.AddSingleton(Options.Create(jwtSettings));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
        };
    });

    builder.Services.AddScoped<IUserService, UserService>();


#if USE_SWAGGER_UI
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
#endif

}

var app = builder.Build();
{

#if USE_SWAGGER_UI
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
#endif

    app.UseExceptionHandler("/error");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}