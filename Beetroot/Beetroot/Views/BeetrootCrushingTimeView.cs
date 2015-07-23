using System;
using Beetroot.Events;
using d60.Cirqus.Extensions;
using d60.Cirqus.MsSql.Views;
using d60.Cirqus.Views.ViewManagers;
using d60.Cirqus.Views.ViewManagers.Locators;

namespace Beetroot.Views
{
    public class BeetrootCrushingTimeView : IViewInstance<InstancePerAggregateRootLocator>,
        ISubscribeTo<BeetrootCreated>, 
        ISubscribeTo<BeetrootCompletelyCrushed>
    {
        static readonly DateTime AbsentSqlDateTime = new DateTime(1754,1,1);
        public string Id { get; set; }

        public long LastGlobalSequenceNumber { get; set; }

        public BeetrootCrushingTimeView()
        {
            CreatedAt = AbsentSqlDateTime;
            CompletelyCrushedAt = AbsentSqlDateTime;
        }

        [NotNull]
        public DateTime CreatedAt { get; set; }

        [NotNull]
        public string Username { get; set; }

        public DateTime CompletelyCrushedAt { get; set; }

        public void Handle(IViewContext context, BeetrootCreated domainEvent)
        {
            Username = domainEvent.Meta[AggregateRoots.Beetroot.UsernameMetadataKey];

            CreatedAt = domainEvent.GetUtcTime();
        }

        public void Handle(IViewContext context, BeetrootCompletelyCrushed domainEvent)
        {
            CompletelyCrushedAt = domainEvent.GetUtcTime();
        }
    }
}