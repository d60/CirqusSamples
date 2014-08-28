using System;
using System.Collections.Generic;
using d60.Cirqus.Aggregates;
using d60.Cirqus.Events;
using ToDoomList.Events;

namespace ToDoomList.AggregateRoots
{
    public class ToDoomList : AggregateRoot,
        IEmit<ToDoomListCreated>,
        IEmit<TitleAssigned>,
        IEmit<BulletAdded>,
        IEmit<BulletMarkedAsComplete>
    {
        public ToDoomList()
        {
            Bullets = new List<Bullet>();
        }

        public string Title { get; set; }

        public List<Bullet> Bullets { get; set; }

        protected override void Created()
        {
            Emit(new ToDoomListCreated());
        }

        public void Apply(ToDoomListCreated e)
        {
        }

        public void SetTitle(string title)
        {
            Emit(new TitleAssigned { Title = title });
        }

        public void Apply(TitleAssigned e)
        {
            Title = e.Title;
        }

        public void AddBulletWithText(string text)
        {
            Emit(new BulletAdded { Text = text });
        }

        public void Apply(BulletAdded e)
        {
            Bullets.Add(new Bullet { Text = e.Text });
        }

        public void MarkBulletAsComplete(int bulletIndex)
        {
            if (bulletIndex < 0) throw new ArgumentException();
            if (bulletIndex >= Bullets.Count) throw new ArgumentException();

            if (Bullets[bulletIndex].Complete) return;

            Emit(new BulletMarkedAsComplete { BulletIndex = bulletIndex });
        }

        public void Apply(BulletMarkedAsComplete e)
        {
            Bullets[e.BulletIndex].MarkAsComplete();
        }
    }

    public class Bullet
    {
        public string Text { get; set; }

        public bool Complete { get; private set; }

        public void MarkAsComplete()
        {
            Complete = true;
        }
    }
}