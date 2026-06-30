using Application.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Text;

namespace Infrastructure.Services
{
    public class MqttService : IMqttService
    {
        private readonly MqttSettings _settings;
        private readonly IMqttClient _mqttClient;
        private readonly IServiceScopeFactory _scopeFactory;

        public MqttService(
            IOptions<MqttSettings> settings,
            IServiceScopeFactory scopeFactory)
        {
            _settings = settings.Value;
            _scopeFactory = scopeFactory;

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
        }

        public async Task ConnectAsync()
        {
            if (_mqttClient.IsConnected)
                return;

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(_settings.ClientId)
                .WithTcpServer(_settings.Broker, _settings.Port)
                .WithCredentials(_settings.Username, _settings.Password)
                .WithCleanSession();

            if (_settings.UseTls)
            {
                optionsBuilder.WithTlsOptions(o =>
                {
                    o.UseTls();
                });
            }

            var options = optionsBuilder.Build();

            await _mqttClient.ConnectAsync(options);
        }

        public async Task PublishAsync(string topic, string payload)
        {
            try
            {
                await ConnectAsync();

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(Encoding.UTF8.GetBytes(payload))
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await _mqttClient.PublishAsync(message);
            }
            catch (Exception ex)
            {
                using var scope = _scopeFactory.CreateScope();

                var logService =
                    scope.ServiceProvider.GetRequiredService<ILogService>();

                await logService.LogAsync(
                    eventType: "MqttPublishFailure",
                    severity: "Error",
                    riskScore: 15,
                    description: $"Failed to publish MQTT message. Topic: {topic}. Error: {ex.Message}",
                    actorRole: "System",
                    entityName: "MQTT",
                    statusCode: 500
                );

                throw;
            }
        }

        public async Task SubscribeAsync(string topic)
        {
            await ConnectAsync();

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();

            await _mqttClient.SubscribeAsync(subscribeOptions);
        }

        public void SetMessageHandler(Func<string, string, Task> messageHandler)
        {
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

                await messageHandler(topic, payload);
            };
        }
    }
}