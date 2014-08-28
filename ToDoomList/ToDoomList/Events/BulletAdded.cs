using d60.Cirqus.Events;

namespace ToDoomList.Events
{
    public class BulletAdded : DomainEvent<AggregateRoots.ToDoomList>
    {
        public string Text { get; set; }
    }
}