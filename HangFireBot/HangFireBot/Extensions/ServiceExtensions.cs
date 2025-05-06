using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Transactions;
using HangFireBot.Managers.Interface;
using HangFireBot.Managers.Manager;

namespace HangFireBot.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection APIModuleServices(this IServiceCollection services)
        {
            try
            {
                
                services.AddScoped<IGovernmentHolidaysManager, GovernmentHolidaysManager>();
                //services.AddScoped<IGovernmentHolidaysRepository, GovernmentHolidaysRepository>();
               
               
                return services;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
