using System;
using d60.Cirqus.Commands;

namespace ToDoomList.Commands
{
    public class CreateToDoomList : Command<AggregateRoots.ToDoomList>
    {
        public CreateToDoomList(Guid aggregateRootId, string title, string[] bulletTexts) : base(aggregateRootId)
        {
            Title = title;
            BulletTexts = bulletTexts;
        }

        public string Title { get; set; }

        public string[] BulletTexts { get; set; }
        
        public override void Execute(AggregateRoots.ToDoomList aggregateRoot)
        {
            aggregateRoot.SetTitle(Title);

            foreach (var text in BulletTexts)
            {
                aggregateRoot.AddBulletWithText(text);
            }
        }
    }
}