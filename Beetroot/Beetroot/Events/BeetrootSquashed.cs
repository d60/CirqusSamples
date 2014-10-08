using d60.Cirqus.Events;

namespace Beetroot.Events
{
    public class BeetrootSquashed : DomainEvent<AggregateRoots.Beetroot>
    {
        public decimal HowMuch { get; set; } 
    }
}