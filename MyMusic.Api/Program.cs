using MyMusic.Data;
using MyMusic.Core;
using Microsoft.EntityFrameworkCore;
using MyMusic.Core.Services;
using MyMusic.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000/")
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IMusicService, MusicService>();
builder.Services.AddTransient<IArtistService, ArtistService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<MyMusicDbContext>(options => options.UseSqlServer("name=Default"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("_myAllowSpecificOrigins");

//
app.UseAuthorization();

app.MapControllers();

app.Run();
