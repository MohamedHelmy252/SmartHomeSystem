using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class AutomationSchedulerHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AutomationSchedulerHostedService(
            IServiceScopeFactory scopeFactory)
        {
            Console.WriteLine("Automation Scheduler Constructor Called");
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            Console.WriteLine("Automation Scheduler Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var automationService =
                        scope.ServiceProvider
                        .GetRequiredService<IAutomationService>();

                    await automationService.ExecuteScheduledRulesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Scheduler Error:");
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken);
            }
        }
    }
}
