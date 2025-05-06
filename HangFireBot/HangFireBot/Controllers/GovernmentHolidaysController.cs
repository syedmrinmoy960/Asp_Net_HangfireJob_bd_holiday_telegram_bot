using HangFireBot.Managers.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HangFireBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernmentHolidaysController : ControllerBase
    {
        private readonly IGovernmentHolidaysManager _governmentHolidaysManager;

        public GovernmentHolidaysController(IGovernmentHolidaysManager governmentHolidaysManager)
        {
            _governmentHolidaysManager = governmentHolidaysManager;
        }

        [HttpGet("schedule-reminders")]
        public async Task<IActionResult> ScheduleReminders()
        {
            await _governmentHolidaysManager.ScheduleHolidayReminders();
            return Ok(new { message = "Holiday reminders scheduled successfully." });
        }
    }
}
