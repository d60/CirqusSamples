using d60.Cirqus.Events;

namespace ToDoomList.Events
{
    public class TitleAssigned : DomainEvent<AggregateRoots.ToDoomList>
    {
        public string Title { get; set; } 
    }
}