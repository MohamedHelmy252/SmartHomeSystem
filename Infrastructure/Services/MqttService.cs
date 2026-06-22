using Application.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using MQTTnet.Client;
namespace Infrastructure.Services
{
    public class MqttService : IMqttService
    {
        private readonly MqttSettings _settings;
        private readonly IMqttClient _mqttClient;

        public MqttService(IOptions<MqttSettings> settings)
        {
            _settings = settings.Value;

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
            await ConnectAsync();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(message);
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
