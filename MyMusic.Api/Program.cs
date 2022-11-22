using MyMusic.Data;
using MyMusic.Core;
using Microsoft.EntityFrameworkCore;
using MyMusic.Core.Services;
using MyMusic.Services;
using MyMusic.Core.Models;
using Microsoft.AspNetCore.Identity;
using MyMusic.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
//using MyMusic.Data.DbInit;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "_myAllowSpecificOrigins",
            builder =>
            {
                builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins("http://localhost:3000/");
            });
        });

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddTransient<IMusicService, MusicService>();
        builder.Services.AddTransient<IArtistService, ArtistService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Jwt auth header",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddDbContext<MyMusicDbContext>(options => options.UseSqlServer("name=Default"));

        builder.Services.AddIdentityCore<User>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
        }).AddRoles<IdentityRole>().AddEntityFrameworkStores<MyMusicDbContext>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(builder.Configuration["JWTSettings:TokenKey"]))
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddScoped<TokenService>();

        var app = builder.Build();

        //biri admin biri member 2 kullanýcýnýn oluþturulmasý
        //using var scope = app.Services.CreateScope();
        //var context = scope.ServiceProvider.GetRequiredService<MyMusicDbContext>();
        //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        //try
        //{
        //    context.Database.Migrate();
        //    await DbInitializer.Initialize(userManager);
        //}
        //catch (Exception)
        //{
        //}

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("_myAllowSpecificOrigins");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}