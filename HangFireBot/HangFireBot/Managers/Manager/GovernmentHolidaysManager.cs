using HangFireBot.Managers.Interface;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static HangFireBot.Models.Entites.ResponseEntites.CommonResponse;
using Hangfire;
using System;
using HangFireBot.Models.Entites.ResponseEntites;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;

namespace HangFireBot.Managers.Manager
{
    public class GovernmentHolidaysManager : IGovernmentHolidaysManager
    {
        private readonly string _folderPath;
        private readonly string _fileName;
        private readonly string _telegramBotToken = "7829938516:AAFM9fVbo1HQMdnTKM4nYUGA6YWqIBPy2YQ";
        private readonly string _telegramChatId = "-4792870773"; // Replace with your target chat ID
        private readonly ILogger<GovernmentHolidaysManager> _logger;

        public GovernmentHolidaysManager(IConfiguration configuration, ILogger<GovernmentHolidaysManager> logger)
        {
            _folderPath = configuration.GetValue<string>("JsonFileSettings:FolderPath");
            _fileName = configuration.GetValue<string>("JsonFileSettings:FileName");
            _logger = logger;
        }
     

        public async Task<CommonResponse> GetHolidaysAsync()
        {
            try
            {
                string filePath = Path.Combine(_folderPath, _fileName);
                if (!File.Exists(filePath))
                {
                    return new CommonResponse
                    {
                        status_code = "404",
                        status_message = "File not found",
                        data = null,
                        error_messages = new List<CommonResponse.ErrorResponseData>
                {
                    new CommonResponse.ErrorResponseData { error_code = "404", error_message = "File not found" }
                }
                    };
                }

                var json = await File.ReadAllTextAsync(filePath);

                var holidayData = JsonConvert.DeserializeObject<GovernmentHolidayData>(json);

                return new CommonResponse
                {
                    status_code = "200",
                    status_message = "Success",
                    data = holidayData?.Holidays,
                    error_messages = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching holidays.");
                return new CommonResponse
                {
                    status_code = "500",
                    status_message = ex.Message,
                    data = null,
                    error_messages = new List<CommonResponse.ErrorResponseData>
            {
                new CommonResponse.ErrorResponseData { error_code = "500", error_message = "Internal server error" }
            }
                };
            }
        }
        public async Task ScheduleHolidayReminders()
        {
            var response = await GetHolidaysAsync();
            if (response.status_code == "200")
            {
                var holidayData = response.data as List<Holiday>;  // Deserialized to a list of holidays
                if (holidayData != null)
                {
                    foreach (var holiday in holidayData)
                    {
                        if (holiday.Date.Contains("to"))
                        {
                            var dateRange = holiday.Date.Split(" to ");
                            if (DateTime.TryParse(dateRange[0], out var startDate) && DateTime.TryParse(dateRange[1], out var endDate))
                            {
           
                                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                                {
                                    var reminderDate = date.AddDays(-1); // 1 day before the holiday
                                    if (reminderDate.Date > DateTime.Now.Date)
                                    {
                                        BackgroundJob.Schedule(() => SendTelegramReminder(holiday, date), reminderDate.AddHours(17));
                                    }
                                }
                            }
                            else
                            {
                                _logger.LogWarning($"Invalid date range format for holiday: {holiday.Name}");
                            }
                        }
                        else
                        {
                            // Handle single-day holiday
                            if (DateTime.TryParse(holiday.Date, out var holidayDate))
                            {
                                var reminderDate = holidayDate.AddDays(-1); // 1 day before the holiday
                                if (reminderDate.Date > DateTime.Now.Date)
                                {
                                    BackgroundJob.Schedule(() => SendTelegramReminder(holiday, holidayDate), reminderDate.AddHours(17));
                                }
                            }
                            else
                            {
                                _logger.LogWarning($"Invalid date format for holiday: {holiday.Name}");
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No holidays found in the response.");
                }
            }
        }




        public async Task SendTelegramReminder(Holiday holiday, DateTime holidayDate)
        {
            // Get weather data
          

            var formattedHolidayDate = holidayDate.ToString("dd MMM yyyy");

            var message = @$"
            🌸 **আসালামু আলাইকুম ভাই/বোন!** 🌸

            📅 **কালকে ছুটি মনে আছে তো?** 🏖️  
            আপনার কাজ ভালো করে করতে হবে, এবং এখন থেকেই প্রস্তুতি নিন! 💪

            ✨ **ছুটির দিন:** {holiday.Name}  
            📆 **তারিখ:** {formattedHolidayDate}

            🔔 **শুধু একটা কাজ বাকি — এক্সট্রা প্রোডাক্টিভ হতে শুরু করুন আজ!** 🚀

         

            🖼️ ![ZIRA Logo](https://yourdomain.com/logo.png)  

            🔗 [ZIRA Official](https://www.zirasites.com) 
            ";

            // Send the message via Telegram bot
            await SendTelegramMessage(message);
        }

        public async Task SendDailyReminder()
        {
            var today = DateTime.Now;
         
            // Skip sending on Friday and Saturday
            if (today.DayOfWeek == DayOfWeek.Friday || today.DayOfWeek == DayOfWeek.Saturday)
            {
                return;  // Skip sending reminder
            }

            // Format the current date to include in the message (e.g., "12 Dec 2024")
            var formattedDate = today.ToString("dd MMM yyyy");

            var message = @$"
            🚨 **আজকের ZIRA Logs!! দিয়েছেন তো?** 🚨

            👉 যদি কালকে সিঙ্গারা বা নাস্তা খাওয়ার ভয় থাকে, 
            🔥 এখনই **জিরা লগ ক্লিয়ার** করুন এবং 
            💪 প্রোডাক্টিভ কাজেই লেগে পড়ুন! 

            📅 **আজকের তারিখ:** {formattedDate}
          
            🌟 **Start now for a productive day!** 🌟

            🔗 [ZIRA Official]https://sslwireless.atlassian.net/jira/projects)  

"; 

            await SendTelegramMessage(message);
        }

        private async Task SendTelegramMessage(string message)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = $"https://api.telegram.org/bot{_telegramBotToken}/sendMessage";
                    var payload = new
                    {
                        chat_id = _telegramChatId,
                        text = message
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"Failed to send message to Telegram. Response: {errorContent}");
                        throw new Exception($"Failed to send message to Telegram. Response: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending Telegram message.");
                throw;
            }
        }
    }
}
