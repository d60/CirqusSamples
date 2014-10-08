using System;
using System.Timers;
using Beetroot.Commands;
using Beetroot.Dispatching;
using Beetroot.Events;
using Beetroot.Views;
using d60.Cirqus;
using d60.Cirqus.Config;
using d60.Cirqus.Logging;
using d60.Cirqus.MsSql.Config;
using d60.Cirqus.MsSql.Views;

namespace Beetroot
{
    class Program
    {
        const string ConnectionStringName = "mssql";

        static void Main()
        {
            var beetrootCrushingTimeView = new MsSqlViewManager<BeetrootCrushingTimeView>(ConnectionStringName, "BeetrootCrushingTimeView");

            var currentBeetrootId = Guid.NewGuid();

            // come up with a new beetroot ID when current beetroot is completely crushed
            var callbackEventDispatcher = new CallbackEventDispatcher<BeetrootCompletelyCrushed>(() =>
            {
                var newBeetrootId = Guid.NewGuid();
                Console.WriteLine("Beetroot completely crushed! New ID: {0}", newBeetrootId);
                currentBeetrootId = newBeetrootId;
            });

            var commandProcessor = CommandProcessor.With()
                .Logging(l => l.UseConsole(minLevel: Logger.Level.Warn))
                .EventStore(e => e.UseSqlServer(ConnectionStringName, "Events", automaticallyCreateSchema: true))
                .AggregateRootRepository(r => r.UseDefault())
                .EventDispatcher(e =>
                {
                    e.UseViewManagerEventDispatcher(beetrootCrushingTimeView);
                    e.UseEventDispatcher(callbackEventDispatcher);
                })
                .Options(o => o.PurgeExistingViews(true))
                .Create();

            ClearScreen();
            var username = Input("Please type your name");

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("You must enter your name so we can track your records!");
                return;
            }


            ClearScreen();

            while (true)
            {
                var howMuch = GetHowMuch();

                try
                {
                    Console.WriteLine("Squeezing {0} by {1:0.00}", currentBeetrootId, howMuch);
                    commandProcessor.ProcessCommand(new SqueezeCommand(currentBeetrootId)
                    {
                        HowMuch = howMuch,
                        Meta = { { AggregateRoots.Beetroot.UsernameMetadataKey, username } }
                    });
                }
                catch (Exception)
                {
                    Console.WriteLine("You failed!");
                }
            }
        }

        static readonly Random Randomizzle = new Random(DateTime.Now.GetHashCode());

        static decimal GetHowMuch()
        {
            var randomPhaseShift = Randomizzle.NextDouble() * Math.PI * 2;
            var sine = 0.0;
            var startTime = DateTime.UtcNow;

            using (var timer = new Timer())
            {
                timer.Interval = 10;
                timer.Elapsed += delegate
                {
                    var elapsedSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
                    sine = Math.Sin(elapsedSeconds * 5 + randomPhaseShift);
                    Console.WriteLine("{0:0.00}   ", sine);
                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                };
                timer.Start();

                Console.ReadLine();
                Console.WriteLine();
            }

            return (decimal)sine;
        }

        static void ClearScreen()
        {
            Console.Clear();
            Console.WriteLine(@"
-----------------------------------------------------------------------------
RULES: A number fluctuates in the interval [-1 ; 1] with a SINE-like pattern,
and you press ENTER when you want to use the number to squeeze the beetroot.

If the number is positive, the beetroot will lose the correpsonding amount of
squishiness, and by the time its squishiness becomes non-positive, you've
completed that particular beetroot.

Press CTRL+C anytime to quit.
-----------------------------------------------------------------------------
");
        }

        static string Input(string prompt)
        {
            Console.Write("{0} > ", prompt);
            return Console.ReadLine();
        }
    }
}
