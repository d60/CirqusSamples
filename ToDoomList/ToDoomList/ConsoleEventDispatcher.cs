using System;
using System.Collections.Generic;
using d60.Cirqus.Events;

namespace ToDoomList
{
    internal class ConsoleEventDispatcher : IEventDispatcher
    {
        public void Initialize(IEventStore eventStore, bool purgeExistingViews = false)
        {
        }

        public void Dispatch(IEventStore eventStore, IEnumerable<DomainEvent> events)
        {
            Console.WriteLine("Dispatching");

            foreach (var e in events)
            {
                Console.WriteLine(" - {0}", e);
            }
        }
    }
}