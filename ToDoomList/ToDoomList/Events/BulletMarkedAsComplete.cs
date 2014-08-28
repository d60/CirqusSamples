using d60.Cirqus.Events;

namespace ToDoomList.Events
{
    public class BulletMarkedAsComplete : DomainEvent<AggregateRoots.ToDoomList>
    {
        public int BulletIndex { get; set; }
    }
}