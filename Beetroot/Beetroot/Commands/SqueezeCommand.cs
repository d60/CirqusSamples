using System;
using d60.Cirqus.Commands;

namespace Beetroot.Commands
{
    public class SqueezeCommand : Command<AggregateRoots.Beetroot>
    {
        public SqueezeCommand(Guid aggregateRootId) : base(aggregateRootId.ToString())
        {
        }

        public decimal HowMuch { get; set; }

        public override void Execute(AggregateRoots.Beetroot aggregateRoot)
        {
            aggregateRoot.Squeeze(HowMuch);
        }
    }
}