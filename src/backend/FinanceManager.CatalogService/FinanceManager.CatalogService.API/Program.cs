using FinanceManager.CatalogService.API.Extensions;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.EntityFramework.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.AddLogging(builder.Configuration);
builder.Services
    .AddDatabase(builder.Configuration, builder.Environment.IsDevelopment())
    .AddSeeding()
    .AddApplication();

builder.Services.AddWebApiConfiguration();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwagger(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.UseMigrationAsync();
await app.SeedDatabaseAsync();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();