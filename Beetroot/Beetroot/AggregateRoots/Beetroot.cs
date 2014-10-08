using System;
using System.Threading;
using Beetroot.Events;
using d60.Cirqus.Aggregates;
using d60.Cirqus.Events;

namespace Beetroot.AggregateRoots
{
    public class Beetroot : AggregateRoot,
        IEmit<BeetrootCreated>,
        IEmit<BeetrootSquashed>,
        IEmit<BeetrootCompletelyCrushed>
    {
        public const string UsernameMetadataKey = "x-username";

        decimal _squishiness = 1;

        public void Squeeze(decimal howMuch)
        {
            if (howMuch <= 0)
            {
                // this is your punishment for being so sloppy with the arguments
                Console.Write(".");
                Thread.Sleep(500);
                Console.Write(".");
                Thread.Sleep(500);
                Console.Write(".");
                Thread.Sleep(500);
                Console.Write(".");
                Thread.Sleep(500);

                throw new ArgumentException(string.Format("Attempted to squeeze by {0}, but it must be positive!", howMuch));
            }

            // nothing happens when we're already crushed
            if (_squishiness <= 0) return;

            // emit crushed when we go below zero
            if (_squishiness - howMuch <= 0)
            {
                Emit(new BeetrootCompletelyCrushed());
                return;
            }

            // otherwise, just subtract how much
            Emit(new BeetrootSquashed { HowMuch = howMuch });
        }

        protected override void Created()
        {
            Emit(new BeetrootCreated());
        }

        public void Apply(BeetrootCreated e)
        {
        }

        public void Apply(BeetrootSquashed e)
        {
            _squishiness -= e.HowMuch;
        }

        public void Apply(BeetrootCompletelyCrushed e)
        {
            _squishiness = 0;
        }
    }
}