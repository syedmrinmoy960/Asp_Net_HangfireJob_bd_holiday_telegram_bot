using HangFireBot.Managers.Manager;
using HangFireBot.Models.Entites.ResponseEntites;
using static HangFireBot.Models.Entites.ResponseEntites.CommonResponse;

namespace HangFireBot.Managers.Interface
{
    public interface IGovernmentHolidaysManager
    {
        Task<CommonResponse> GetHolidaysAsync();
        Task ScheduleHolidayReminders();
        Task SendTelegramReminder(Holiday holiday, DateTime holidayDate);
        Task SendDailyReminder();
    }
}
