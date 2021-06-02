using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Plugins;

namespace Nop.Plugin.SMS.Clickatell.Infrastructure.Cache
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        #region Fields

        private readonly IPluginService _pluginService;

        #endregion

        #region Ctor

        public OrderPlacedEventConsumer(IPluginService pluginService)
        {
            _pluginService = pluginService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            //check that plugin is installed
            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>("Mobile.SMS.Clickatell", LoadPluginsMode.All);
            var plugin = pluginDescriptor.Instance<ClickatellSmsProvider>();

            await plugin?.SendSms(string.Empty, eventMessage.Order.Id);
        }

        #endregion
    }
}