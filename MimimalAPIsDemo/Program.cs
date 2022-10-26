using Microsoft.EntityFrameworkCore;
using MimimalAPIsDemo.Entities;
using MimimalAPIsDemo.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var migrationAssemblyName = typeof(ModelContext).GetTypeInfo().Assembly.GetName().Name;
builder.Services.AddDbContext<ModelContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection"), p =>
{
    p.UseOracleSQLCompatibility("11");
    p.MigrationsAssembly(migrationAssemblyName);
}));
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// get the list of tests
app.MapGet("/testall", async (ModelContext dbContext) =>
{
    var testList = await dbContext.Testtables.ToListAsync();
    if (testList == null)
    {
        return Results.NoContent();
    }
    return Results.Ok(testList);
})
.WithName("GetAllTests");

// get test by id
app.MapGet("/gettestbyid", async (int id, ModelContext dbContext) =>
{
    var result = await dbContext.Testtables.FindAsync(id);
    if (result == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(result);
})
.WithName("GetTestById");

// create a new test
app.MapPost("/createtest", async (Testtable testtable, ModelContext dbContext) =>
{
    var result = await dbContext.Testtables.AddAsync(testtable);
    await dbContext.SaveChangesAsync();
    return Results.Ok(result.Entity);
})
.WithName("CreateTest");

// update test
app.MapPut("/updateTest", async (Testtable testtable, ModelContext dbContext) =>
{
    var test = await dbContext.Testtables.FindAsync(testtable.Id);
    if (test == null)
    {
        return Results.NotFound();
    }
    test.Name = testtable.Name;
    test.Age = testtable.Age;

    await dbContext.SaveChangesAsync();
    return Results.Ok(test);
})
.WithName("UpdateTest");

// delete the test by id
app.MapDelete("/deletetest/{id}", async (int id, ModelContext dbContext) =>
{
    var test = await dbContext.Testtables.FindAsync(id);
    if (test == null)
    {
        return Results.NoContent();
    }
    dbContext.Testtables.Remove(test);
    await dbContext.SaveChangesAsync();
    return Results.Ok();
})
.WithName("DeleteTest");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}