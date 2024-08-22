using FinanceManager.Core.Services;
using FinanceManager.Core.Services.Abstractions;
using FinanceManager.Infrastructure.Data;
using FinanceManager.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/*
 * EF Core get started
 * ASP.NET DI
 * Controller
*/

#region Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
#endregion

var app = builder.Build();

using var scope = app.Services.CreateScope();
using var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await appDbContext.Database.MigrateAsync();

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
