using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IMqttService
    {
        Task ConnectAsync();

        Task PublishAsync(string topic, string payload);

        Task SubscribeAsync(string topic);

        void SetMessageHandler(Func<string, string, Task> messageHandler);
    }
}
