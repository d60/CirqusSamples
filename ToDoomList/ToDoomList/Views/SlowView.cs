using System.Threading;
using d60.Cirqus.Views.ViewManagers;
using d60.Cirqus.Views.ViewManagers.Locators;
using ToDoomList.Events;

namespace ToDoomList.Views
{
    public class SlowView : IViewInstance<GlobalInstanceLocator>
        ,ISubscribeTo<BulletAdded>
        ,ISubscribeTo<BulletMarkedAsComplete>
        ,ISubscribeTo<ToDoomListCreated>
    {
        public string Id { get; set; }
        public long LastGlobalSequenceNumber { get; set; }

        public int Events { get; set; }

        public void Handle(IViewContext context, BulletAdded domainEvent)
        {
            Thread.Sleep(1000);
            Events++;
        }

        public void Handle(IViewContext context, BulletMarkedAsComplete domainEvent)
        {
            Thread.Sleep(1000);
            Events++;
        }

        public void Handle(IViewContext context, ToDoomListCreated domainEvent)
        {
            Thread.Sleep(1000);
            Events++;
        }
    }
}