using Hangfire;
using HangFireBot.Extensions;
using Microsoft.Extensions.DependencyInjection;
using HangFireBot.Managers.Interface;
using HangFireBot.Managers.Manager;
using Hangfire.MemoryStorage;
using HangFireBot.Managers.Manager;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add Hangfire with memory storage (or another storage like Redis or SQL Server for production)
builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();  // Use in-memory storage for jobs (for simplicity)
});

// Add your custom services (DI)
builder.Services.APIModuleServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.UseHangfireDashboard("/hangfire"); 


app.UseHangfireServer();



using (var scope = app.Services.CreateScope())
{
    
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<IGovernmentHolidaysManager>(
        "MyRecurringJob",                     // Unique job ID
        x => x.SendDailyReminder(),
      "*/30 * * * *");
    recurringJobManager.AddOrUpdate<IGovernmentHolidaysManager>(
        "wertwert",                     // Unique job ID
        x => x.ScheduleHolidayReminders(),
      "*/30 * * * *");
}



app.Run();
