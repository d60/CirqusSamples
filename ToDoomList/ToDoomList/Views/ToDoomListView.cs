using System.Linq;
using d60.Cirqus.Extensions;
using d60.Cirqus.Views.ViewManagers;
using d60.Cirqus.Views.ViewManagers.Locators;
using ToDoomList.Events;

namespace ToDoomList.Views
{
    public class ToDoomListView : IViewInstance<InstancePerAggregateRootLocator>,
        ISubscribeTo<ToDoomListCreated>,
        ISubscribeTo<TitleAssigned>,
        ISubscribeTo<BulletAdded>,
        ISubscribeTo<BulletMarkedAsComplete>
    {
        public string Id { get; set; }
        public long LastGlobalSequenceNumber { get; set; }

        public string Title { get; set; }

        public string[] TextsOfCompletedBullets { get; set; }

        public string[] TextsOfBulletsWaitingToBeCompleted { get; set; }

        public void Handle(IViewContext context, ToDoomListCreated domainEvent)
        {
            Title = "";
            
            TextsOfBulletsWaitingToBeCompleted
                = TextsOfBulletsWaitingToBeCompleted
                    = new string[0];
        }

        public void Handle(IViewContext context, TitleAssigned domainEvent)
        {
            Title = domainEvent.Title;
        }

        public void Handle(IViewContext context, BulletAdded domainEvent)
        {
            var aggregateRootId = domainEvent.GetAggregateRootId();

            var toDoomList = context.Load<AggregateRoots.ToDoomList>(aggregateRootId);

            ExtractBulletsFrom(toDoomList);
        }

        public void Handle(IViewContext context, BulletMarkedAsComplete domainEvent)
        {
            var aggregateRootId = domainEvent.GetAggregateRootId();

            var toDoomList = context.Load<AggregateRoots.ToDoomList>(aggregateRootId);

            ExtractBulletsFrom(toDoomList);
        }

        void ExtractBulletsFrom(AggregateRoots.ToDoomList toDoomList)
        {
            TextsOfCompletedBullets = toDoomList.Bullets
                .Where(b => b.Complete)
                .Select(b => b.Text)
                .ToArray();

            TextsOfBulletsWaitingToBeCompleted = toDoomList.Bullets
                .Where(b => !b.Complete)
                .Select(b => b.Text)
                .ToArray();
        }
    }
}