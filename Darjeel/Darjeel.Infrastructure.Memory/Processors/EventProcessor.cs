using Darjeel.Infrastructure.EventSourcing;
using Darjeel.Infrastructure.Messaging;
using Darjeel.Infrastructure.Messaging.Handling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Darjeel.Infrastructure.Memory.Processors
{
    public class EventProcessor : MessageProcessor<IEvent>
    {
        private readonly IEventHandlerRegistry _registry;

        public EventProcessor(IEventHandlerRegistry registry, IProducerConsumerCollection<Envelope<IEvent>> commandCollection)
            : base(commandCollection)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            _registry = registry;
        }

        protected override async Task ProcessMessageAsync(IEvent message, string correlationId)
        {
            var versionedEvent = message as IVersionedEvent;
            message = versionedEvent?.Event ?? message;

            var eventType = message.GetType();
            IEnumerable<IEventHandler> handlers;
            var tasks = new List<Task>();

            if (_registry.TryGetHandlers(eventType, out handlers))
            {
                foreach (var handler in handlers)
                {
                    Trace.TraceInformation("Event '{0}' handled by '{1}.", eventType.FullName, handler.GetType().FullName);
                    var task = ((dynamic)handler).HandleAsync((dynamic)message);
                    tasks.Add(task);
                }
            }
            else if (_registry.TryGetHandlers(typeof(ICommand), out handlers))
            {
                foreach (var handler in handlers)
                {
                    Trace.TraceInformation("Event '{0}' handled by '{1}.", eventType.FullName, handler.GetType().FullName);
                    var task = ((dynamic)handler).Handle((dynamic)message);
                    tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}