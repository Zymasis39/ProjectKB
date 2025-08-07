using MySql.Data.MySqlClient;
using ProjectKBServer;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings_secret.json");

string connectionString = builder.Configuration.GetConnectionString("ScoreDB");

// Add services to the container.

builder.Services.AddScoped<DBWrapper>(sp => new DBWrapper(connectionString));

builder.Services.AddControllers();
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
