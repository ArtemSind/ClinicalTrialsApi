using Microsoft.EntityFrameworkCore;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Infrastructure.Data;
using ClinicalTrialsApi.Infrastructure.Repositories;
using ClinicalTrialsApi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IClinicalTrialRepository, ClinicalTrialRepository>();
builder.Services.AddScoped<IClinicalTrialService, ClinicalTrialService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
