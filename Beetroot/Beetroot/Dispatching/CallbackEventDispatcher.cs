using System;
using System.Collections.Generic;
using System.Linq;
using d60.Cirqus.Events;
using d60.Cirqus.Views;

namespace Beetroot.Dispatching
{
    /// <summary>
    /// Special event dispatcher that can invoke a callback when an event of a specific type is encountered
    /// </summary>
    public class CallbackEventDispatcher<TDomainEventToCallBackOn> : IEventDispatcher where TDomainEventToCallBackOn : DomainEvent
    {
        readonly Action _callback;

        public CallbackEventDispatcher(Action callback)
        {
            _callback = callback;
        }

        public void Initialize(IEventStore eventStore, bool purgeExistingViews = false)
        {
        }

        public void Dispatch(IEventStore eventStore, IEnumerable<DomainEvent> events)
        {
            if (events.OfType<TDomainEventToCallBackOn>().Any())
                _callback();
        }
    }
}