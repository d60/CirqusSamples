using System;
using d60.Cirqus.Commands;

namespace ToDoomList.Commands
{
    public class MarkAsComplete : Command<AggregateRoots.ToDoomList>
    {
        public MarkAsComplete(Guid aggregateRootId, int bulletIndex) : base(aggregateRootId.ToString())
        {
            BulletIndex = bulletIndex;
        }

        public int BulletIndex { get; set; }
        
        public override void Execute(AggregateRoots.ToDoomList aggregateRoot)
        {
            aggregateRoot.MarkBulletAsComplete(BulletIndex);
        }
    }
}