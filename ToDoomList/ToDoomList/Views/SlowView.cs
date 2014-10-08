using System.Threading;
using d60.Cirqus.Events;
using d60.Cirqus.Views.ViewManagers;
using d60.Cirqus.Views.ViewManagers.Locators;

namespace ToDoomList.Views
{
    public class SlowView : IViewInstance<GlobalInstanceLocator>, ISubscribeTo<DomainEvent>
    {
        public string Id { get; set; }
        public long LastGlobalSequenceNumber { get; set; }

        public int Events { get; set; }

        public void Handle(IViewContext context, DomainEvent domainEvent)
        {
            Thread.Sleep(1000);
            Events++;
        }
    }
}