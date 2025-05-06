using HangFireBot.Managers.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public static class HangfireJobs
{
    // Schedule holiday reminders - async version
    public static async Task ScheduleHolidayReminders(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var holidaysManager = scope.ServiceProvider.GetRequiredService<IGovernmentHolidaysManager>();
            await holidaysManager.GetHolidaysAsync();  // Avoid Wait(), use async/await
        }
    }

    // Send daily reminder - async version
    public static async Task SendDailyReminder(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var holidaysManager = scope.ServiceProvider.GetRequiredService<IGovernmentHolidaysManager>();
            await holidaysManager.SendDailyReminder();  // Avoid Wait(), use async/await
        }
    }
}
