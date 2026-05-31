using ScadaServer.Domain.Entities;
using Opc.Ua;
using Opc.Ua.Client;

namespace ScadaServer.Infrastructure.Communication
{
    public class OpcUaDriver : IProtocolDriver
    {
        private Session _session;
        private Subscription _subscription;
        private readonly Dictionary<string, MonitoredItem> _monitoredItems = new();

        public async Task<bool> ConnectAsync(string connectionStr)
        {
            // Simplified connection logic for example
            var endpointDescription = CoreClientUtils.SelectEndpoint(connectionStr, false);
            var endpointConfiguration = EndpointConfiguration.Create();
            var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
            
            _session = await Session.Create(null, endpoint, false, false, "ScadaServer", 60000, null, null);
            _subscription = new Subscription(_session.DefaultSubscription) { PublishingEnabled = true, PublishingInterval = 1000 };
            _session.AddSubscription(_subscription);
            _subscription.Create();
            
            return _session.Connected;
        }

        public async Task<object> ReadAsync(ModelVariable variable)
        {
            var value = _session.ReadValue(variable.Address);
            return value.Value;
        }

        public async Task SubscribeAsync(ModelVariable variable, Action<object> onValueChanged)
        {
            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                StartNodeId = variable.Address,
                AttributeId = Attributes.Value,
                DisplayName = variable.Key
            };
            item.Notification += (sender, e) => {
                var notification = e.NotificationValue as MonitoredItemNotification;
                onValueChanged(notification?.Value.Value);
            };
            _subscription.AddItem(item);
            _subscription.ApplyChanges();
            _monitoredItems[variable.Key] = item;
        }

        public async Task UnsubscribeAsync(ModelVariable variable)
        {
            if (_monitoredItems.TryGetValue(variable.Key, out var item))
            {
                _subscription.RemoveItem(item);
                _subscription.ApplyChanges();
                _monitoredItems.Remove(variable.Key);
            }
        }

        public async Task DisconnectAsync()
        {
            _session?.Close();
        }
    }
}
