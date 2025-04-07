using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoService.Infrastructure.MessageBroker.Configurations
{
    public class RabbitMqSettings
    {
        public string Hostname { get; set; } = string.Empty;
        public string Queue { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;
    }
}