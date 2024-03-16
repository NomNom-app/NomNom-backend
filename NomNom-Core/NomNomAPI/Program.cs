#define USE_SWAGGER_UI

using NomNomAPI.Services.Users;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddScoped<IUserService, UserService>();
   
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie
        (option =>
        {
            option.LoginPath = "/Access/Login";
            option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        });


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
    app.UseHttpsRedirection();
    app.UseAuthentication();
    //app.MapControllers();
    app.MapControllerRoute(name: "default", pattern: "{controller=Access}/{action=Index}/{id?}");
    app.Run();
}