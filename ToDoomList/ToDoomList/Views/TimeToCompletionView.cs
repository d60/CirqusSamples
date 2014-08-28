using System;
using System.Collections.Generic;
using d60.Cirqus.Extensions;
using d60.Cirqus.Views.ViewManagers;
using d60.Cirqus.Views.ViewManagers.Locators;
using ToDoomList.Events;

namespace ToDoomList.Views
{
    public class TimeToCompletionView : IViewInstance<InstancePerAggregateRootLocator>,
        ISubscribeTo<BulletAdded>,
        ISubscribeTo<BulletMarkedAsComplete>
    {
        public TimeToCompletionView()
        {
            BulletAddedTime = new Dictionary<string, DateTime>();
            CompletionTimes = new Dictionary<string, TimeSpan>();
        }
        
        public string Id { get; set; }
        
        public long LastGlobalSequenceNumber { get; set; }

        public Dictionary<string, DateTime> BulletAddedTime { get; set; }

        public Dictionary<string, TimeSpan> CompletionTimes { get; set; }
        
        public void Handle(IViewContext context, BulletAdded domainEvent)
        {
            BulletAddedTime[domainEvent.Text] = domainEvent.GetUtcTime();
        }

        public void Handle(IViewContext context, BulletMarkedAsComplete domainEvent)
        {
            var toDoomList = context.Load<AggregateRoots.ToDoomList>(domainEvent.GetAggregateRootId());
            var text = toDoomList.Bullets[domainEvent.BulletIndex].Text;

            var timeWhenBulletWasAdded = BulletAddedTime[text];

            var timeWhenBulletWasClosed = domainEvent.GetUtcTime();

            var timeToCompletion = timeWhenBulletWasClosed.ToUniversalTime()
                
                -timeWhenBulletWasAdded.ToUniversalTime();

            CompletionTimes[text] = timeToCompletion;
        }
    }
}