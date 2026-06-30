using Application.DTOs.MQTT;
using Application.DTOs.SmartDevice;
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class MqttSubscriberHostedService : BackgroundService
    {
        private readonly IMqttService _mqttService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MqttSettings _settings;
     
        public MqttSubscriberHostedService(
            IMqttService mqttService,
            IServiceScopeFactory scopeFactory,
            IOptions<MqttSettings> settings)
        {
            _mqttService = mqttService;
            _scopeFactory = scopeFactory;
            _settings = settings.Value;
       
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("MQTT Subscriber Hosted Service Started...");

            _mqttService.SetMessageHandler(HandleMessageAsync);

            await _mqttService.ConnectAsync();

            await _mqttService.SubscribeAsync(_settings.SensorReadingTopic);
            Console.WriteLine($"Subscribed to Sensor Reading Topic: {_settings.SensorReadingTopic}");

            await _mqttService.SubscribeAsync(_settings.DeviceStatusTopic);
            Console.WriteLine($"Subscribed to Device Status Topic: {_settings.DeviceStatusTopic}");
        }

        private async Task HandleMessageAsync(string topic, string payload)
        {
            Console.WriteLine("===== MQTT MESSAGE RECEIVED =====");
            Console.WriteLine($"Topic: {topic}");
            Console.WriteLine($"Payload: {payload}");

            if (topic.EndsWith("/reading"))
            {
                await HandleSensorReadingAsync(payload);
                return;
            }

            if (topic.EndsWith("/status"))
            {
                await HandleDeviceStatusAsync(payload);
                return;
            }

            Console.WriteLine("Unknown MQTT message type.");
        }

        private async Task HandleSensorReadingAsync(string payload)
        {
            try
            {
                var message = JsonSerializer.Deserialize<MqttSensorReadingMessage>(
                    payload,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (message == null)
                    return;

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var sensorExists = await db.Sensors
                    .AnyAsync(s => s.SensorId == message.SensorId);

                if (!sensorExists)
                {
                    Console.WriteLine($"SensorId {message.SensorId} does not exist.");
                    return;
                }

                db.SensorReadings.Add(new Domain.Entities.SensorReading
                {
                    SensorId = message.SensorId,
                    Value = message.Value,
                    Unit = message.Unit,
                    ReadAt = DateTime.Now
                });

                await db.SaveChangesAsync();
                //هنا جزء automation بعد متوصل قراءه السينسور بتتحفظ في الداتابيز وبعدها ندخل علي الجزء الجديد
                var automationService =
                 scope.ServiceProvider.GetRequiredService<IAutomationService>();

                await automationService.EvaluateSensorRulesAsync(
                    message.SensorId,
                    message.Value
                );
                //==========================================================================
                Console.WriteLine("Sensor reading saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sensor Reading Error:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
        }

        private async Task HandleDeviceStatusAsync(string payload)
        {
            try
            {
                var message = JsonSerializer.Deserialize<MqttDeviceStatusMessage>(
                    payload,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (message == null)
                    return;

                message.State = message.State.Trim().ToUpper();

                if (message.State != "ON" && message.State != "OFF")
                {
                    Console.WriteLine($"Invalid device state: {message.State}");
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var logService = scope.ServiceProvider.GetRequiredService<ILogService>();

                var device = await db.SmartDevices
                    .FirstOrDefaultAsync(d => d.SmartDeviceId == message.DeviceId);

                if (device == null)
                {
                    Console.WriteLine($"DeviceId {message.DeviceId} does not exist.");
                    return;
                }

                var oldState = device.CurrentState;

                if (oldState == "PENDING_ON" && message.State != "ON")
                {
                    await logService.LogAsync(
                        eventType: "DeviceStatusMismatch",
                        severity: "Warning",
                        riskScore: 10,
                        description: $"Device status mismatch. DeviceId: {message.DeviceId}. Expected: ON, Received: {message.State}",
                        actorRole: "System",
                        entityName: "SmartDevice",
                        entityId: message.DeviceId,
                        statusCode: 200
                    );
                }

                if (oldState == "PENDING_OFF" && message.State != "OFF")
                {
                    await logService.LogAsync(
                        eventType: "DeviceStatusMismatch",
                        severity: "Warning",
                        riskScore: 10,
                        description: $"Device status mismatch. DeviceId: {message.DeviceId}. Expected: OFF, Received: {message.State}",
                        actorRole: "System",
                        entityName: "SmartDevice",
                        entityId: message.DeviceId,
                        statusCode: 200
                    );
                }

                device.CurrentState = message.State;

                await db.SaveChangesAsync();

                Console.WriteLine($"Device {message.DeviceId} status updated to {message.State}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Device Status Error:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
        }
    }
}