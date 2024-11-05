using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mongo.ServiceBus
{
    public interface IMessageBus
    {
        Task PublishMessage(string topic_queuq_name, string message);
    }
}
