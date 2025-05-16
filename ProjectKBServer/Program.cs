using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings_secret.json");

MySqlConnection conn = new();

try
{
    conn.ConnectionString = builder.Configuration.GetConnectionString("ScoreDB");
}
catch (MySqlException ex)
{
    Console.Error.WriteLine("\x1b[31m" + ex.Message + "\x1b[0m");
}

// Add services to the container.

builder.Services.AddSingleton<MySqlConnection>(conn);

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
